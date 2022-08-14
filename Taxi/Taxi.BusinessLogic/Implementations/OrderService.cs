using AutoMapper;
using Taxi.BusinessLogic.Interfaces;
using Taxi.Common.Models;
using Taxi.Model.Database;
using Taxi.Model.Models;

namespace Taxi.BusinessLogic.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly TaxiContext _taxiContext;
        private readonly IMapper _mapper;
        public OrderService(TaxiContext taxiContext, IMapper mapper)
        {
            _taxiContext = taxiContext;
            _mapper = mapper;
        }

        public void Create(OrderModel model)
        {
            var order = _mapper.Map<Order>(model);
            _taxiContext.Orders.Add(order);
            _taxiContext.SaveChanges();
        }

        public void Delete(int id)
        {
            Order order = Find(id);
            _taxiContext.Orders.Remove(order);
            _taxiContext.SaveChanges();
        }
        private Order Find(int id)
        {
            Order order = _taxiContext.Orders.FirstOrDefault(o=>o.Id==id);
            if (order==null) throw new Exception("Object = null");
            return order;
        }
    }
}
