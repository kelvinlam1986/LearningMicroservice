using MongoDB.Driver;
using Shared.SeedWork;

namespace Infrastructure.Common.Models
{
    public class PagedList<T> : List<T>
    {
        private MetaData _metaData;

        public MetaData MetaData => _metaData;

        public PagedList(IEnumerable<T> items, long totalItems, int pageIndex, int pageSize)
        {
            _metaData = new MetaData
            {
                TotalItems = totalItems,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                CurrentPage = pageIndex
            };

            AddRange(items);
        }

       public static async Task<PagedList<T>> ToPagedList(IMongoCollection<T> source,
           FilterDefinition<T> filter, int pageIndex, int pageSize)
        {
            var count = await source.Find(filter).CountDocumentsAsync();
            var items = await source.Find(filter)
                .Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
