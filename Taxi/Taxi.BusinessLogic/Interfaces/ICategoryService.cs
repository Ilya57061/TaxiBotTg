using Taxi.Common.Models;

namespace Taxi.BusinessLogic.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<CategoryModel> Get();
        CategoryModel Get(int id);
    }
}
