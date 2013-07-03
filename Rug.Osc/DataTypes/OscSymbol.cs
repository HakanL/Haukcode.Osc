
namespace Rug.Osc
{
	/// <summary>
	/// 
	/// </summary>
	public struct OscSymbol
	{
		public string Value;

		public OscSymbol(string value)
		{
			Value = value; 
		}

		public override string ToString()
		{
			return Value;
		}

		public override bool Equals(object obj)
		{
			if (obj is OscSymbol)
			{
				return Value.Equals(((OscSymbol)obj).Value);
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
	}
}
