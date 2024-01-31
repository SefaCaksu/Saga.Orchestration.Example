using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Stock.API.Models
{
    public class Stock
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement(Order = 0)]
        public ObjectId Id { get; set; }

        [BsonElement(Order = 1)]
        [BsonRepresentation(MongoDB.Bson.BsonType.Int64)]
        public int ProductId { get; set; }

        [BsonElement(Order = 2)]
        [BsonRepresentation(MongoDB.Bson.BsonType.Int64)]
        public int Count { get; set; }
    }
}

