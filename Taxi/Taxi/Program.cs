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
    services.AddTransient<IMailService, MailService>();
    services.AddDbContext<TaxiContext>(options => options.UseSqlServer(connection));
    services.AddSingleton(mapper);
})
.Build();

var orderService = ActivatorUtilities.CreateInstance<OrderService>(host.Services);
var categoryService = ActivatorUtilities.CreateInstance<CategoryService>(host.Services);
var carService = ActivatorUtilities.CreateInstance<CarService>(host.Services);
var mailService = ActivatorUtilities.CreateInstance<MailService>(host.Services);

#endregion



#region Bot

string startAddress = "";
string destinationAddress = "";
string chooseReg = "";
string phone = "";

var botClient = new TelegramBotClient("5502195701:AAF3SAib84_N_zLFeBX-cDuHEkn44QfHb_4");
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
    if (update.Type == UpdateType.Message && update?.Message != null)
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

    if (message.Text == "/start")
    {
        ReplyKeyboardMarkup keyboardOrder = new(new[]
          {
            new KeyboardButton[] { "Заказать такси"}
            })
        {
            ResizeKeyboard = true
        };
        await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите действие", replyMarkup: keyboardOrder);
        return;
    }
<<<<<<< HEAD
=======
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
>>>>>>> 56e1489a26dc8f78642a2d8d5ba56357a1946d3c
    if (message.Text == "Заказать такси")
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
        {
            new []
            {
        KeyboardButton.WithRequestLocation("Откуда Вас забрать"),
        //KeyboardButton.WithRequestLocation("Куда Вас отвезти"),
        KeyboardButton.WithRequestContact("Поделитесь номером телефона"),
            },
            new KeyboardButton[] {"Подтвердить" }
        })
        {
            ResizeKeyboard = true
        };

        await botClient.SendTextMessageAsync(
              message.Chat.Id,
              text: $"Укажите дополнительную информацию, пользуясь предложенными кнопками.",
              replyMarkup: replyKeyboardMarkup);
        return;
    }
    if (message.Text=="Подтвердить")
    {
        if (phone != "" && startAddress!="")
        {
            List<InlineKeyboardButton> listButton = new List<InlineKeyboardButton>();
            foreach (var item in categoryService.Get())
            {
                listButton.Add(InlineKeyboardButton.WithCallbackData(item.Name, item.Name));
            }
            InlineKeyboardMarkup keyboard = new(listButton.ToArray());
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Выберите класс поездки:", replyMarkup: keyboard);
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Не все данные записаны");
        }
    
    }
    if (message.Contact != null)
    {
        phone = message.Contact.PhoneNumber;
    }
    if (message.Location!=null)
    {
        startAddress = $"Широта: {message.Location.Latitude}\nДолгота: {message.Location.Longitude}";
    }
   

}

async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{

   
    if (categoryService.Get().Any(x => callbackQuery.Data.StartsWith(x.Name)))
    {

        OrderModel order = new OrderModel { Car = carService.Get(callbackQuery.Data), Category = categoryService.Get(callbackQuery.Data), Phone = phone, DestinationAddress = destinationAddress, StartingAddress = startAddress };
        orderService.Create(order);

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


