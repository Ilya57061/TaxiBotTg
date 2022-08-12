using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Taxi.BusinessLogic.Interfaces;
using Taxi.Common.Models;
using Taxi.Model.Database;
using Taxi.Model.Models;

namespace Taxi.BusinessLogic.Implementations
{
    public class CarService : ICarService
    {
        private readonly TaxiContext _taxiContext;
        private readonly IMapper _mapper;
        public CarService(TaxiContext taxiContext, IMapper mapper)
        {
            _taxiContext = taxiContext;
            _mapper = mapper;
        }

        public IEnumerable<CarModel> Get()
        {
            var car = _taxiContext.Cars.AsNoTracking().ToList();
            var carModel = _mapper.Map<List<CarModel>>(car);
            return carModel;
        }

        public CarModel Get(string category)
        {
            var car = Find(category);
            var carModel = _mapper.Map<CarModel>(car);
            return carModel;
        }
        private Car Find(string category)
        {
            Car car= _taxiContext.Cars.FirstOrDefault(o => o.Category.Name == category);
            if (car == null) throw new Exception("Object = null");
            return car;
        }
    }

}
