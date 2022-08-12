
using Taxi.Common.Models;

namespace Taxi.BusinessLogic.Interfaces
{
    public interface ICarService
    {
        IEnumerable<CarModel> Get();
        CarModel Get(string category);
    }
}
