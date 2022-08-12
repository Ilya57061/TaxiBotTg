using Taxi.Common.Models;

namespace Taxi.BusinessLogic.Interfaces
{
    public interface IMailService
    {
        void Send(OrderModel order);
    }
}
