using System;
using Newtonsoft.Json;

namespace DevTracker.DAL.Models.Entities
{
    public class RefreshToken
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ReplacedBy { get; set; }
        public bool IsActive => this.RevokedOn == null && !this.IsExpired;
        public DateTime? RevokedOn { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= this.Expires;
    }
}
