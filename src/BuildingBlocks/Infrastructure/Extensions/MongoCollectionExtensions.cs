using Infrastructure.Common.Models;
using MongoDB.Driver;

namespace Infrastructure.Extensions
{
    public static class MongoCollectionExtensions
    {
        public static Task<PagedList<TDestination>> 
            PaginatedListAsync<TDestination>(this IMongoCollection<TDestination> collections,
                FilterDefinition<TDestination> filterDefinition,
                int pageIndex, int pageSize)
            where TDestination : class
        {
            return PagedList<TDestination>.ToPagedList(collections, filterDefinition, pageIndex, pageSize);
        }
    }
}
