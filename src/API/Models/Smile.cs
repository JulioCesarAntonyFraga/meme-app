using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Smile : Entity
    {
        [BsonRequired]
        [Required]
        public string UserId { get; set; }

        [BsonRequired]
        [Required]
        public string PostId { get; set; }
    }
}
