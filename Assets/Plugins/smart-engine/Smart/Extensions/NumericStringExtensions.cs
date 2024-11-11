using Smart.Basics.Extensions;

namespace Utils
{
	public static class NumericStringExtensions
	{
		private const int BufferSize = 1000; 
		
		private static string[] _strings;
		
		public static string ToCachedString(this int value)
		{
			if (_strings.IsNullOrEmpty())
			{
				_strings = new string[BufferSize];

				for (int i = 0; i < BufferSize; i++)
				{
					_strings[i] = i.ToString();
				}
			}
			
			return value >= 0 && value < BufferSize ? _strings[value] : value.ToString();
		}
	}
}