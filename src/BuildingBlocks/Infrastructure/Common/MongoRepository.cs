using Contracts.Common.Interfaces;
using Contracts.Domains;
using Infrastructure.Extensions;
using MongoDB.Driver;
using Shared.Configurations;
using System.Linq.Expressions;

namespace Inventory.Product.API.Repositories.Abstraction
{
    public class MongoRepository<T> : IMongoRepositoryBase<T> where T : MongoEntity
    {
        private readonly IMongoDatabase _database;

        public MongoRepository(IMongoClient client, MongoDbSettings settings)
        {
            _database = client.GetDatabase(settings.DatabaseName);

        }

        protected virtual IMongoCollection<T> Collection =>
            _database.GetCollection<T>(GetCollectionName());

        public Task CreateAsync(T entity)
        {
            return Collection.InsertOneAsync(entity);
        }

        public Task DeleteAsync(string id)
        {
            return Collection.DeleteOneAsync(x => x.Id.Equals(id));
        }

        public IMongoCollection<T> FindAll(ReadPreference? readPreference = null)
        {
            return _database.WithReadPreference(readPreference ?? ReadPreference.Primary)
                .GetCollection<T>(GetCollectionName());
        }

        public Task UpdateAsync(T entity)
        {
            Expression<Func<T, string>> func = f => f.Id;
            var value = (string)entity.GetType()
                .GetProperty(func.Body.ToString()
                    .Split(".")[1])?.GetValue(entity, null);

            var filter = Builders<T>.Filter.Eq(func, value);
            return Collection.ReplaceOneAsync(filter, entity);
        }

        private static string GetCollectionName()
        {
            return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true)
                .FirstOrDefault() as BsonCollectionAttribute)?.CollectionName;

        }
    }
}
