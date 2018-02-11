using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HTLLBB.Models;

namespace HTLLBB.Repository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategories(bool includeForums);
        Task<Category> GetCategoryById(int id);
        Task<Boolean> CategoryExists(int id);

		Task AddCategory(Category category);
        Task UpdCategory(Category category);
        Task DelCategory(int id);
    }
}
