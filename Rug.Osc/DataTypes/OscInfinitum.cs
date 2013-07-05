
namespace Rug.Osc
{
	/// <summary>
	/// Osc Infinitum Singleton
	/// </summary>
	public sealed class OscInfinitum
	{
		public static readonly OscInfinitum Value = new OscInfinitum();

		private OscInfinitum() { }

		public override string ToString()
		{
			return "inf";
		}

		public static bool IsInfinitum(string str)
		{
			bool isTrue = false;

			isTrue |= "Infinitum".Equals(str, System.StringComparison.InvariantCultureIgnoreCase);

			isTrue |= "Inf".Equals(str, System.StringComparison.InvariantCultureIgnoreCase);

			return isTrue; 
		}
	}
}
