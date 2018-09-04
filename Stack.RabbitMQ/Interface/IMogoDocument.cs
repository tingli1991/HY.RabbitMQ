using MongoDB.Bson;

namespace Stack.RabbitMQ.Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IMogoDocument<TId>
    {
        /// <summary>
        /// 
        /// </summary>
        TId Id { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMogoDocument : IMogoDocument<ObjectId>
    {

    }
}