using Taxi.Common.Models;

namespace Taxi.BusinessLogic.Interfaces
{
    public interface IOrderService
    {
        void Create(OrderModel order);
        void Delete(int id);
    }
}
