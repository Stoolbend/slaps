using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace BCI.SLAPS.Server.Model.Media
{
    /// <summary>
    /// Object representing a slide to display as part of a slideshow on the client (if enabled).
    /// </summary>
    public class Slide
    {
        /// <summary>
        /// Unique identifier for this slide.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The title of this slide. Used for administration and not displayed to the client.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Automatically updated value displaying the last time the content was updated.
        /// </summary>
        public DateTimeOffset LastUpdated { get; set; }

        /// <summary>
        /// HTML content to be displayed by the client. Should be encoded using UTF8.
        /// </summary>
        public string Content
        {
            get => _Content;
            set
            {
                LastUpdated = DateTime.UtcNow;
                _Content = value;
            }
        }
        private string _Content;

        /// <summary>
        /// Automatically calculated MD5 hash of the Content property. Useful for comparing if the client has matching content on the server.
        /// </summary>
        [NotMapped]
        public string ContentHash
        {
            get
            {
                using (var md5 = MD5.Create())
                {
                    return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(Content)))
                                       .Replace("-", "", StringComparison.InvariantCultureIgnoreCase)
                                       .ToUpperInvariant();
                }
            }
        }

        /// <summary>
        /// The Guid of the parent SlideSet.
        /// </summary>
        public Guid SlideSetId { get; set; }

        /// <summary>
        /// Returns the parent SlideSet object.
        /// </summary>
        public virtual SlideSet SlideSet { get; }

        /// <summary>
        /// Sets the order of this slide inside it's Slide Set.
        /// </summary>
        public int Order { get; set; }
    }
}
