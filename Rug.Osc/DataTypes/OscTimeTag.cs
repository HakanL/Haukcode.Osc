using System;

namespace Rug.Osc
{
	/// <summary>
	/// 
	/// </summary>
	public struct OscTimeTag
	{
		public static readonly DateTime BaseDate = new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Ntp Timestamp value
		/// </summary>
		public ulong Value;

		public OscTimeTag(ulong value)
		{
			Value = value;
		}

		public override bool Equals(object obj)
		{
			if (obj is OscTimeTag)
			{
				return Value.Equals(((OscTimeTag)obj).Value);
			}
			else
			{
				return Value.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return ToDataTime().ToString("dd/MM/yyyy HH:mm:ss.fff");
		}

		public DateTime ToDataTime()
		{
			// http://stackoverflow.com/questions/5206857/convert-ntp-timestamp-to-utc

			uint seconds = (uint)((Value & 0xFFFFFFFF00000000) >> 32);

			uint fraction = (uint)(Value & 0xFFFFFFFF);

			double milliseconds = ((double)fraction / (double)UInt32.MaxValue) * 1000;
			
			DateTime datetime = BaseDate.AddSeconds(seconds).AddMilliseconds(milliseconds);

			return datetime;
		}

		public static OscTimeTag FromDataTime(DateTime datetime)
		{
			TimeSpan span = datetime.Subtract(BaseDate);

			double seconds = span.TotalSeconds;

			uint seconds_UInt = (uint)seconds;

			double milliseconds = span.TotalMilliseconds - ((double)seconds_UInt * 1000);

			double fraction = (milliseconds / 1000) * (double)UInt32.MaxValue;

			return new OscTimeTag(((ulong)(seconds_UInt & 0xFFFFFFFF) << 32) | ((ulong)fraction & 0xFFFFFFFF));
		}
	}
}
