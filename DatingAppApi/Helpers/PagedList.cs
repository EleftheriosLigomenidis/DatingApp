using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Helpers
{
    public class PagedList<T> :List<T>
    {
        public PagedList(List<T> items , int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = PageSize;
            CurrentPage = pageNumber; 
            // have 13 users and i want to display 5 each play the i ll gave 3 pages!!
            TotalPage = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source,
            int pageNumber,int pageSize)
        {
            // how many items there are
            var count = await source.CountAsync();
            var items =  await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
