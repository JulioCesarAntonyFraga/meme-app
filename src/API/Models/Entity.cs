using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Models
{
    public abstract class Entity
    {
        public Entity()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreationDate = DateTime.Now;
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonRequired]
        public string Id { get; set; }

        [BsonRequired]
        public DateTime CreationDate { get; set; }

        [BsonRequired]
        public bool Enabled { get; set; }
    }
}
