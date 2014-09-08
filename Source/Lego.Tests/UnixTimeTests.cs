using System;
using Lego.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lego.Tests
{
    [TestClass]
    public class UnixTimeTests
    {
        [TestMethod]
        public void utc_epoch_is_zero()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            var expected = 0;
            var actual = DateTimeExtensions.ToUnixTime(epoch);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void local_datetime_kind_epoch_is_adjusted_for_current_time_zone()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

            TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(epoch);

            // date time offset is how to adjust the current time
            // from UTC, so we negate it to calculate how much to 
            // adjust UTC from the current time.
            var expected = -offset.TotalSeconds;
            var actual = DateTimeExtensions.ToUnixTime(epoch);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void unspecified_datetime_kind_epoch_is_adjusted_for_current_time_zone()
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);

            TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(epoch);

            // date time offset is how to adjust the current time
            // from UTC, so we negate it to calculate how much to 
            // adjust UTC from the current time.
            var expected = -offset.TotalSeconds;
            var actual = DateTimeExtensions.ToUnixTime(epoch);

            Assert.AreEqual(expected, actual);
        }
    }
}