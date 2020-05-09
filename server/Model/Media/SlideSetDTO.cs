using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCI.SLAPS.Server.Model.Media
{
    public class SlideSetDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<SlideClientDTO> Slides { get; set; }
    }

    public class SlideSetAdminDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<SlideAdminDTO> Slides { get; set; }
    }

    public class SlideSetAdminCreateDTO
    {
        [Required]
        public string Title { get; set; }
    }
}
