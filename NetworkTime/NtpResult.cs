using System;

namespace NetworkTime
{
    /// <summary>
    /// Represents a detailed result returned by the NTP client.
    /// Includes timestamps and calculated time corrections.
    /// </summary>
    public class NtpResult
    {
        /// <summary>
        /// Gets or sets the hostname or IP address of the NTP server used.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the UTC time received from the NTP server.
        /// </summary>
        public DateTime UtcTime { get; set; }

        /// <summary>
        /// Gets or sets the local time converted from the UTC time.
        /// </summary>
        public DateTime LocalTime => UtcTime.ToLocalTime();

        /// <summary>
        /// Gets or sets the total round-trip delay measured during the NTP request.
        /// </summary>
        public TimeSpan RoundTripDelay { get; set; }

        /// <summary>
        /// Gets or sets the offset between the local system clock and the NTP server clock.
        /// </summary>
        public TimeSpan LocalOffset => DateTime.UtcNow - UtcTime;
    }
}