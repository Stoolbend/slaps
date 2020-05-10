using System;
using System.ComponentModel.DataAnnotations;

namespace BCI.SLAPS.Server.Model.Chat
{
    public class PostDTO : PostDTOBase
    {
        public Guid Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    public class PostDTOBase
    {
        [Required]
        public string Author { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
