
using LSTY.Sdtd.ServerAdmin.Shared.Constants;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    /// <summary>
    /// Represents the event when the sky changes.
    /// </summary>
    public class SkyChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Gets or sets the type of sky change event.
        /// </summary>
        public required SkyChangeEventType Type { get; set; }

        /// <summary>
        /// Gets or sets the hour of dawn.
        /// </summary>
        public required int DawnHour { get; set; }

        /// <summary>
        /// Gets or sets the hour of dusk.
        /// </summary>
        public required int DuskHour { get; set; }

        /// <summary>
        /// Gets or sets the remaining days until the blood moon.
        /// </summary>
        public required int BloodMoonDaysRemaining { get; set; }

        /// <summary>
        ///
        /// </summary>
        public required DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the current game time information for the simulation or game loop.
        /// </summary>
        public required GameTimeDto GameTime { get; set; }
    }
}