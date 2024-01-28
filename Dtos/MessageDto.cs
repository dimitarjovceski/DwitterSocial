using DwitterSocial.Entities;
using Newtonsoft.Json;

namespace DwitterSocial.Dtos
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;
        public string SenderPhotoUrl { get; set; } = string.Empty;
        public int ReceiverId { get; set; }
        public string ReceiverUsername { get; set; } = string.Empty;
        public string ReceiverPhotoUrl { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        [JsonIgnore]
        public bool ReceiverDeleted { get; set; }
        [JsonIgnore]
        public bool SenderDeleted { get; set; }
    }
}
