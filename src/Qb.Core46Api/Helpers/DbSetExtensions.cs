using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Qb.Core46Api.Helpers
{
    public static class DbSetExtensions
    {
        /// <summary>Adds new item or updates existing item.</summary>
        /// <typeparam name="T">Poco class.</typeparam>
        /// <param name="dbset">DbSet for Poco class in the DbContext.</param>
        /// <param name="item">The object add or update.</param>
        /// <param name="isMatch">A predicate that compares primary keys to check if the item exists.</param>
        /// <returns>void awaitable.</returns>
        public static async Task AddOrUpdate<T>(this DbSet<T> dbset, T item, Func<T, T, bool> isMatch) where T : class
        {
            var exists = await dbset.AnyAsync(entry => isMatch(item, entry));
            if (exists)
                dbset.Update(item);
            else
                dbset.Add(item);
        }

        /// <summary>Adds or updates each item depending on if it already exists.</summary>
        /// <typeparam name="T">The Poco type.</typeparam>
        /// <param name="dbset">The DBset in the DbContext.</param>
        /// <param name="items">The items to add or insert.</param>
        /// <param name="isMatch">A predicate to compare primary keys to see if the item already exists.</param>
        /// <returns>void awaitable.</returns>
        public static async Task AddOrUpdateRange<T>(this DbSet<T> dbset, IEnumerable<T> items, Func<T, T, bool> isMatch)
            where T : class
        {
            foreach (var item in items)
                await dbset.AddOrUpdate(item, isMatch);
        }
    }
}