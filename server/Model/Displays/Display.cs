using BCI.SLAPS.Server.Model.Media;
using System;

namespace BCI.SLAPS.Server.Model.Displays
{
    public class Display
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }

        #region SlideSet assignment
        public Guid SlideSetId { get; set; }
        public virtual SlideSet SlideSet { get; set; }
        #endregion
    }
}