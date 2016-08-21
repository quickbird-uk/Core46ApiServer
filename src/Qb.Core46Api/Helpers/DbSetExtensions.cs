using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Qb.Core46Api.Helpers
{
    public static class DbSetExtensions
    {
        public static async Task AddOrUpdate<T>(this DbSet<T> dbset, T item, Func<T, T, bool> isMatch) where T : class
        {
            var exists = await dbset.AnyAsync(entry => isMatch(item, entry));
            if (exists)
                dbset.Update(item);
            else
                dbset.Add(item);
        }

        public static async Task AddOrUpdateRange<T>(this DbSet<T> dbset, IEnumerable<T> items, Func<T, T, bool> isMatch)
            where T : class
        {
            foreach (var item in items)
                await dbset.AddOrUpdate(item, isMatch);
        }
    }
}