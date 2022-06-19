using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class User : Entity
    {
        [BsonRequired]
        [Required]
        public string UserName { get; set; }

        [BsonRequired]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BsonRequired]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [BsonRequired]
        [Required]
        public string Role { get; set; }

        public List<string>? FollowersIds { get; set; }

        public List<string>? PostsIds { get; set; }

        public List<string>? FollowingIds { get; set; }

        public string? RefreshToken { get; set; }
    }
}
