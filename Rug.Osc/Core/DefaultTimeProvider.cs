using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc
{
	public sealed class DefaultTimeProvider : IOscTimeProvider
	{
		public static readonly DefaultTimeProvider Instance = new DefaultTimeProvider(); 

		public double FrameSizeInSeconds { get; set; }

		/// <summary>
		/// Get the current time 
		/// </summary>
		public OscTimeTag Now { get { return OscTimeTag.FromDataTime(DateTime.UtcNow); } }

		public DefaultTimeProvider() { FrameSizeInSeconds = 1; }

		/// <summary>
		/// Is the supplied time within the current frame according to this time provider
		/// </summary>
		/// <param name="time">the time to check</param>
		/// <returns>true if within the frame else false</returns>
		public bool IsWithinTimeFrame(OscTimeTag time)
		{
			double offset = DifferenceInSeconds(time); 

			if (offset > -FrameSizeInSeconds &&
				offset < FrameSizeInSeconds)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Get the difference in seconds between the current time and the suppied time
		/// </summary>
		/// <param name="time">the time to compair</param>
		/// <returns>the difference in seconds between the current time and the suppied time</returns>
		public double DifferenceInSeconds(OscTimeTag time)
		{
			TimeSpan span = DateTime.UtcNow.Subtract(time.ToDataTime());

			return span.TotalSeconds;
		}
	}
}
