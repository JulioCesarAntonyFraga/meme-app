using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Post : Entity
    {
        [BsonRequired]
        [Required]
        public string FileUrl { get; set; }

        [BsonRequired]
        [Required]
        public string UserId { get; set; }

        public List<string>? SmilesIds { get; set; }

        public List<string>? CommentsIds { get; set; }
    }
}
