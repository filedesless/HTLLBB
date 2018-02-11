﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HTLLBB.Data;
using HTLLBB.Models;
using Microsoft.EntityFrameworkCore;

namespace HTLLBB.Repository
{
    public class EFCategoryRepository : ICategoryRepository
    {
        readonly ApplicationDbContext _ctx;

        public EFCategoryRepository(ApplicationDbContext ctx) 
            => _ctx = ctx;

        public async Task AddCategory(Category category)
        {
            await _ctx.AddAsync(category);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Boolean> CategoryExists(int id)
            => await GetCategoryById(id) != null;

        public async Task DelCategory(int id)
        {
            _ctx.Remove(await GetCategoryById(id));
            await _ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _ctx.Categories
                             .Include(cat => cat.Forums)
                             .ToListAsync();
        }

        public async Task<Category> GetCategoryById(int id)
            => await _ctx.FindAsync<Category>(id);

        public async Task UpdCategory(Category category)
        {
            _ctx.Update(category);
            await _ctx.SaveChangesAsync();
        }
    }
}
