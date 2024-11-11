using System;

namespace Smart.DataTypes
{
	[Serializable]
	public struct SizeInt
	{
		public int Width;
		public int Height;
        
		public SizeInt(int width = 0, int height = 0)
		{
			Width = width;
			Height = height;
		}
		
		public override bool Equals(object obj)
		{
			if (obj is SizeInt size)
			{
				return Width == size.Width && Height == size.Height;
			}
			
			return false;
		}

		public bool Equals(SizeInt other)
		{
			return Width == other.Width && Height == other.Height;
		}

		public override int GetHashCode()
		{
			return (Width, Height).GetHashCode();
		}

		public static bool operator == (SizeInt lhs, SizeInt rhs)
		{
			return lhs.Width == rhs.Width && lhs.Height == rhs.Height;
		}

		public static bool operator != (SizeInt lhs, SizeInt rhs)
		{
			return lhs.Width != rhs.Width || lhs.Height != rhs.Height;
		}

		public static SizeInt operator * (SizeInt size, float value)
		{
			size.Width = (int) (size.Width * value);
			size.Height = (int) (size.Height * value);
			return size;
		}

		public static SizeInt operator * (SizeInt size, int value)
		{
			size.Width *= value;
			size.Height *= value;
			return size;
		}

		public override string ToString()
		{
			return $"Size[{Width}, {Height}]";
		}
	}
}