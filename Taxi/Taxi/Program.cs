using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Taxi.BusinessLogic.Implementations;
using Taxi.BusinessLogic.Interfaces;
using Taxi.Common.Mapper;
using Taxi.Common.Models;
using Taxi.Model.Database;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


#region 
static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables();
}

var builder = new ConfigurationBuilder();
BuildConfig(builder);

builder.SetBasePath(Directory.GetCurrentDirectory());
builder.AddJsonFile("appsettings.json");
var config = builder.Build();
string connection = config.GetConnectionString("DefaultConnection");

var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MapperProfile()));
IMapper mapper = mappingConfig.CreateMapper();

var host = Host.CreateDefaultBuilder()
.ConfigureServices((context, services) =>
{
    services.AddTransient<IOrderService, OrderService>();
    services.AddDbContext<TaxiContext>(options => options.UseSqlServer(connection));
    services.AddSingleton(mapper);
})
.Build();

var orderService = ActivatorUtilities.CreateInstance<OrderService>(host.Services);
var categoryService = ActivatorUtilities.CreateInstance<CategoryService>(host.Services);
var carService = ActivatorUtilities.CreateInstance<CarService>(host.Services);

#endregion



#region Bot

string startAddress = "";
string destinationAddress = "";
string chooseReg = "";
string phone = "";

var botClient = new TelegramBotClient("Use your api");
using var cts = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};
botClient.StartReceiving(HandleUpdates, HandleError, receiverOptions, cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();
Console.WriteLine($"Имя: {me.Username}");
Console.ReadLine();

cts.Cancel();



async Task HandleUpdates(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message && update?.Message?.Text != null)
    {
        await HandleMessage(botClient, update.Message);
        return;
    }
    if (update.Type == UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(botClient, update.CallbackQuery);
        return;
    }
}

async Task HandleMessage(ITelegramBotClient botClient, Message message)
{
    ReplyKeyboardMarkup keyboardOrder = new(new[]
           {
            new KeyboardButton[] { "Заказать такси"}
            })
    {
        ResizeKeyboard = true
    };
    if (message.Text == "/start")
    {

        await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите действие", replyMarkup: keyboardOrder);
        return;
    }
    InlineKeyboardMarkup keyboardPosition = new(new[]
    {
            new[]{
            InlineKeyboardButton.WithCallbackData("Откуда поедете?", "starting"),
            InlineKeyboardButton.WithCallbackData("Куда поедете?", "destination"),
             InlineKeyboardButton.WithCallbackData("Телефон", "phone")
            },
              new[]{
            InlineKeyboardButton.WithCallbackData("Подтвердить", "select")
            }
        });
    if (message.Text == "Заказать такси")
    {

        await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите:", replyMarkup: keyboardPosition);
        return;
    }

    if (chooseReg == "starting")
    {
        startAddress = message.Text;
        await botClient.SendTextMessageAsync(message.Chat.Id, $"Откуда: {message.Text}", replyMarkup: keyboardPosition);
        return;
    }
    if (chooseReg == "destination")
    {
        destinationAddress = message.Text;
        await botClient.SendTextMessageAsync(message.Chat.Id, $"Куда: {message.Text} ", replyMarkup: keyboardPosition);
        return;
    }
    if (chooseReg == "phone")
    {
        phone = message.Text;
        await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш телефон: {message.Text} ", replyMarkup: keyboardPosition);
        return;
    }

}

async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    if (callbackQuery.Data.StartsWith("starting"))
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Введите адрес откуда Вас забрать");
        chooseReg = "starting";
    }
    if (callbackQuery.Data.StartsWith("destination"))
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Введите адрес куда Вас отвезти");
        chooseReg = "destination";
    }
    if (callbackQuery.Data.StartsWith("phone"))
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Введите свой номер телефона");
        chooseReg = "phone";
    }
    if (callbackQuery.Data.StartsWith("select"))
    {
        if (startAddress != "" && destinationAddress != "" && phone!="")
        {
            List<InlineKeyboardButton> listButton = new List<InlineKeyboardButton>();
            foreach (var item in categoryService.Get())
            {
                listButton.Add(InlineKeyboardButton.WithCallbackData(item.Name, item.Name));
            }
            InlineKeyboardMarkup keyboard = new(listButton.ToArray());
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Выберите класс поездки:", replyMarkup: keyboard);

           
            chooseReg = "";
        }
        else
        {
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Заполните все поля регистрации");
        }
    }
    if (categoryService.Get().Any(x=> callbackQuery.Data.StartsWith(x.Name)))
    {
    
        orderService.Create(new OrderModel {Phone = phone, DestinationAddress = destinationAddress, StartingAddress = startAddress });
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"Вы выбрали: {callbackQuery.Data}\nЗаказ оформлен");
    }


}
Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken cancellation)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
        => $"Ошибка телеграм АПИ:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
#endregion


