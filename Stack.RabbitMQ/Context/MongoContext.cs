using MongoDB.Driver;
using Stack.RabbitMQ.Interface;
using Stack.RabbitMQ.Options;

namespace Stack.RabbitMQ.Context
{
    /// <summary>
    /// MongoDb上下文
    /// </summary>
    sealed class MongoContext
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IMongoDatabase _db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        public MongoContext(MongoConnectionOptions option)
        {
            var client = new MongoClient(option.ConnectionString);
            _db = client.GetDatabase(option.DatabaseName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <returns></returns>
        public IMongoCollection<T> Collection<T, TId>() where T : IMogoDocument<TId>
        {
            var collectionName = InferCollectionNameFrom<T>();
            return _db.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IMongoCollection<T> Collection<T>() where T : IMogoDocument
        {
            var collectionName = InferCollectionNameFrom<T>();
            return _db.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static string InferCollectionNameFrom<T>()
        {
            var type = typeof(T);
            return type.Name;
        }
    }
}