using System;

namespace Lego.Extensions
{

    public static class DateTimeExtensions
    {
        private static readonly long EpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
        
        /// <summary>
        /// Returns the Unix time for the given 
        /// </summary>
        /// <param name="when"></param>
        /// <returns></returns>
        /// <remarks>
        /// Unix time (aka POSIX time or Epoch time), is a system for describing instants in time, 
        /// defined as the number of seconds that have elapsed since 00:00:00 Coordinated Universal Time (UTC), 
        /// Thursday, 1 January 1970, not counting leap seconds. It is used widely in Unix-like and many 
        /// other operating systems and file formats. Due to its handling of leap seconds, 
        /// it is neither a linear representation of time nor a true representation of UTC. 
        /// Unix time may be checked on most Unix systems by typing date +%s on the command line.
        /// </remarks>
        public static long ToUnixTime(this DateTime when)
        {
            if (when.Kind != DateTimeKind.Utc)
            {
                when = when.ToUniversalTime();
            }

            var seconds = (when.Ticks - EpochTicks) / TimeSpan.TicksPerSecond;
            return seconds;
        }

    }
}