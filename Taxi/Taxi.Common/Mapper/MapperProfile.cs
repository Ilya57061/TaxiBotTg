using AutoMapper;
using Taxi.Common.Models;
using Taxi.Model.Models;

namespace Taxi.Common.Mapper
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<Car, CarModel>().ReverseMap();
            CreateMap<Category, CategoryModel>().ReverseMap();
            CreateMap<Driver, DriverModel>().ReverseMap();
            CreateMap<Order, OrderModel>().ReverseMap();

        }
    }
}
