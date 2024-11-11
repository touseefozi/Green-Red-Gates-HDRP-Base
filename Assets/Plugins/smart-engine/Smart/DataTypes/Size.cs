using System;

namespace Smart.DataTypes
{
	[Serializable]
	public struct Size
	{
		public float Width;
		public float Height;
        
		public Size(float width, float height)
		{
			Width = width;
			Height = height;
		}
		
		public override bool Equals(object obj)
		{
			if (obj is Size size)
			{
				return Width == size.Width && Height == size.Height;
			}
			
			return false;
		}

		public bool Equals(Size other)
		{
			return Width == other.Width && Height == other.Height;
		}

		public override int GetHashCode()
		{
			return (Width, Height).GetHashCode();
		}

		public static bool operator == (Size lhs, Size rhs)
		{
			return lhs.Width == rhs.Width && lhs.Height == rhs.Height;
		}

		public static bool operator != (Size lhs, Size rhs)
		{
			return lhs.Width != rhs.Width || lhs.Height != rhs.Height;
		}

		public static Size operator * (Size size, float value)
		{
			size.Width *= value;
			size.Height *= value;
			return size;
		}

		public override string ToString()
		{
			return $"Size[{Width:0.0}, {Height:0.0}]";
		}
	}
}