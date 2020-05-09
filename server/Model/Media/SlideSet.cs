using System;
using System.Collections.Generic;

namespace BCI.SLAPS.Server.Model.Media
{
    /// <summary>
    /// Object representing a saved collection & order of Slide objects to display on clients.
    /// </summary>
    public class SlideSet
    {
        /// <summary>
        /// Unique identifier for this slide set.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The title of this slide set. Used for administration and not displayed to the client.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Returns a list of associated Slide objects.
        /// </summary>
        public List<Slide> Slides { get; }
    }
}
