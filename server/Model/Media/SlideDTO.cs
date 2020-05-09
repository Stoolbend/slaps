using System;
using System.ComponentModel.DataAnnotations;

namespace BCI.SLAPS.Server.Model.Media
{
    public class SlideAdminDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ContentHash { get; set; }
    }

    public class SlideAdminCreateDTO
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }

    public class SlideClientDTO
    {
        public Guid Id { get; set; }
        public string ContentHash { get; set; }
    }

    public class SlideClientContentDTO : SlideClientDTO
    {
        public string Content { get; set; }
    }
}
