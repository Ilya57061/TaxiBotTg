using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Taxi.BusinessLogic.Interfaces;
using Taxi.Common.Models;
using Taxi.Model.Database;
using Taxi.Model.Models;

namespace Taxi.BusinessLogic.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly TaxiContext _taxiContext;
        private readonly IMapper _mapper;
        public CategoryService(TaxiContext taxiContext, IMapper mapper)
        {
            _taxiContext = taxiContext;
            _mapper = mapper;
        }

        public IEnumerable<CategoryModel> Get()
        {
            
                var category = _taxiContext.Categories.AsNoTracking().ToList();
                var categoryModel = _mapper.Map<List<CategoryModel>>(category);
                return categoryModel;
            
        }


        public CategoryModel Get(string name)
        {
            var category = Find(name);
            var categoryModel = _mapper.Map<CategoryModel>(category);
            return categoryModel;
        }

        private Category Find(string name)
        {
            Category category = _taxiContext.Categories.FirstOrDefault(o => o.Name == name);
            if (category == null) throw new Exception("Object = null");
            return category;
        }
    }
}

