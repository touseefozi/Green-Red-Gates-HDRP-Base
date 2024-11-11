namespace Smart.IK
{
	public static class IKPivotConstants
	{
		private static readonly IKPivot[] _pelvis =
		{
			IKPivot.ThighLeft, IKPivot.CalfLeft, IKPivot.FootLeft, IKPivot.ToeLeft,
			IKPivot.ThighRight, IKPivot.CalfRight, IKPivot.FootRight, IKPivot.ToeRight,
		};
		
		private static readonly IKPivot[] _stomach =
		{
			IKPivot.Chest, IKPivot.Neck, IKPivot.Head, IKPivot.HeadEnd,
			IKPivot.ClavicleLeft, IKPivot.ArmLeft, IKPivot.ForearmLeft, IKPivot.HandLeft,
			IKPivot.ClavicleRight, IKPivot.ArmRight, IKPivot.ForearmRight, IKPivot.HandRight,
			IKPivot.FingersLeft_1, IKPivot.FingersLeft_2, IKPivot.FingersLeft_3, IKPivot.ThumbLeft_1, IKPivot.ThumbLeft_2,
			IKPivot.FingersRight_1, IKPivot.FingersRight_2, IKPivot.FingersRight_3, IKPivot.ThumbRight_1, IKPivot.ThumbRight_2,
		};
		
		private static readonly IKPivot[] _chest =
		{
			IKPivot.Neck, IKPivot.Head, IKPivot.HeadEnd,
			IKPivot.ClavicleLeft, IKPivot.ArmLeft, IKPivot.ForearmLeft, IKPivot.HandLeft,
			IKPivot.ClavicleRight, IKPivot.ArmRight, IKPivot.ForearmRight, IKPivot.HandRight,
			IKPivot.FingersLeft_1, IKPivot.FingersLeft_2, IKPivot.FingersLeft_3, IKPivot.ThumbLeft_1, IKPivot.ThumbLeft_2,
			IKPivot.FingersRight_1, IKPivot.FingersRight_2, IKPivot.FingersRight_3, IKPivot.ThumbRight_1, IKPivot.ThumbRight_2,
		};
		
		private static readonly IKPivot[] _neck = { IKPivot.Head, IKPivot.HeadEnd };
		private static readonly IKPivot[] _head = { IKPivot.HeadEnd };
		
		private static readonly IKPivot[] _clavicleLeft =
		{
			IKPivot.ArmLeft, IKPivot.ForearmLeft, IKPivot.HandLeft, 
			IKPivot.FingersLeft_1, IKPivot.FingersLeft_2, IKPivot.FingersLeft_3, IKPivot.ThumbLeft_1, IKPivot.ThumbLeft_2,
		};
		private static readonly IKPivot[] _armLeft = 
		{
			IKPivot.ForearmLeft, IKPivot.HandLeft, 
			IKPivot.FingersLeft_1, IKPivot.FingersLeft_2, IKPivot.FingersLeft_3, IKPivot.ThumbLeft_1, IKPivot.ThumbLeft_2,
		};
		private static readonly IKPivot[] _forearmLeft =
		{
			IKPivot.HandLeft,
			IKPivot.FingersLeft_1, IKPivot.FingersLeft_2, IKPivot.FingersLeft_3, IKPivot.ThumbLeft_1, IKPivot.ThumbLeft_2,
		};
		private static readonly IKPivot[] _handLeft =
		{
			IKPivot.FingersLeft_1, IKPivot.FingersLeft_2, IKPivot.FingersLeft_3, IKPivot.ThumbLeft_1, IKPivot.ThumbLeft_2,
		};
		private static readonly IKPivot[] _fingersLeft_1 = { IKPivot.FingersLeft_2, IKPivot.FingersLeft_3 };
		private static readonly IKPivot[] _thumbLeft_1 = { IKPivot.ThumbLeft_2 };
		
		private static readonly IKPivot[] _clavicleRight =
		{
			IKPivot.ArmRight, IKPivot.ForearmRight, IKPivot.HandRight,
			IKPivot.FingersRight_1, IKPivot.FingersRight_2, IKPivot.FingersRight_3, IKPivot.ThumbRight_1, IKPivot.ThumbRight_2,
		};
		private static readonly IKPivot[] _armRight =
		{
			IKPivot.ForearmRight, IKPivot.HandRight,
			IKPivot.FingersRight_1, IKPivot.FingersRight_2, IKPivot.FingersRight_3, IKPivot.ThumbRight_1, IKPivot.ThumbRight_2,
		};
		private static readonly IKPivot[] _forearmRight =
		{
			IKPivot.HandRight,
			IKPivot.FingersRight_1, IKPivot.FingersRight_2, IKPivot.FingersRight_3, IKPivot.ThumbRight_1, IKPivot.ThumbRight_2,
		};
		private static readonly IKPivot[] _handRight =
		{
			IKPivot.FingersRight_1, IKPivot.FingersRight_2, IKPivot.FingersRight_3, IKPivot.ThumbRight_1, IKPivot.ThumbRight_2,
		};
		private static readonly IKPivot[] _fingersRight_1 = { IKPivot.FingersRight_2, IKPivot.FingersRight_3 };
		private static readonly IKPivot[] _thumbRight_1 =  { IKPivot.ThumbRight_2 };
		
		private static readonly IKPivot[] _thighLeft = { IKPivot.CalfLeft, IKPivot.FootLeft, IKPivot.ToeLeft};
		private static readonly IKPivot[] _calfLeft = { IKPivot.FootLeft, IKPivot.ToeLeft};
		private static readonly IKPivot[] _footLeft = { IKPivot.ToeLeft};
		
		private static readonly IKPivot[] _thighRight = { IKPivot.CalfRight, IKPivot.FootRight, IKPivot.ToeRight};
		private static readonly IKPivot[] _calfRight = { IKPivot.FootRight, IKPivot.ToeRight};
		private static readonly IKPivot[] _footRight = { IKPivot.ToeRight};

		public static IKPivot[] GetPivotChildren(IKPivot pivot)
		{
			switch (pivot)
			{
				case IKPivot.Pelvis:         return _pelvis;
				case IKPivot.Stomach:        return _stomach;
				case IKPivot.Chest:          return _chest;
				case IKPivot.Neck:           return _neck;
				case IKPivot.Head:           return _head;
				case IKPivot.ClavicleLeft:   return _clavicleLeft;
				case IKPivot.ArmLeft:        return _armLeft;
				case IKPivot.ForearmLeft:    return _forearmLeft;
				case IKPivot.HandLeft:       return _handLeft;
				case IKPivot.FingersLeft_1:  return _fingersLeft_1;
				case IKPivot.ThumbLeft_1:    return _thumbLeft_1;
				case IKPivot.ClavicleRight:  return _clavicleRight;
				case IKPivot.ArmRight:       return _armRight;
				case IKPivot.ForearmRight:   return _forearmRight;
				case IKPivot.HandRight:      return _handRight;
				case IKPivot.FingersRight_1: return _fingersRight_1;
				case IKPivot.ThumbRight_1:   return _thumbRight_1;
				case IKPivot.ThighLeft:      return _thighLeft;
				case IKPivot.CalfLeft:       return _calfLeft;
				case IKPivot.FootLeft:       return _footLeft;
				case IKPivot.ThighRight:     return _thighRight;
				case IKPivot.CalfRight:      return _calfRight;
				case IKPivot.FootRight:      return _footRight;
			}
			
			return null;
		}
		
		private static readonly IKPivot[] _armLeftChildren = { IKPivot.ArmLeft, IKPivot.ForearmLeft, IKPivot.HandLeft };
		private static readonly IKPivot[] _armRightChildren = { IKPivot.ArmRight, IKPivot.ForearmRight, IKPivot.HandRight };
		private static readonly IKPivot[] _legLeftChildren = { IKPivot.ThighLeft, IKPivot.CalfLeft, IKPivot.FootLeft };
		private static readonly IKPivot[] _legRightChildren = { IKPivot.ThighRight, IKPivot.CalfRight, IKPivot.FootRight };
			
		public static IKPivot[] GetLimbChildren(IKPivot pivot)
		{
			switch (pivot)
			{
				case IKPivot.ArmPoleLeft:
				case IKPivot.HandLeft:  return _armLeftChildren;
				
				case IKPivot.ArmPoleRight:
				case IKPivot.HandRight: return _armRightChildren;
				
				case IKPivot.LegPoleLeft:
				case IKPivot.FootLeft:  return _legLeftChildren;
				
				case IKPivot.LegPoleRight:
				case IKPivot.FootRight: return _legRightChildren;
					
				default: return null;
			}
		}
		
		public static IKPivot GetMirroredPivot(IKPivot pivot)
		{
			switch (pivot)
			{
				case IKPivot.ClavicleLeft: return IKPivot.ClavicleRight;
				case IKPivot.ArmLeft:      return IKPivot.ArmRight;
				case IKPivot.ForearmLeft:  return IKPivot.ForearmRight;
				case IKPivot.HandLeft:     return IKPivot.HandRight;
				
				case IKPivot.ClavicleRight: return IKPivot.ClavicleLeft;
				case IKPivot.ArmRight:      return IKPivot.ArmLeft;
				case IKPivot.ForearmRight:  return IKPivot.ForearmLeft;
				case IKPivot.HandRight:     return IKPivot.HandLeft;
				
				case IKPivot.ThighLeft: return IKPivot.ThighRight;
				case IKPivot.CalfLeft:  return IKPivot.CalfRight;
				case IKPivot.FootLeft:  return IKPivot.FootRight;
				case IKPivot.ToeLeft:   return IKPivot.ToeRight;
				
				case IKPivot.ThighRight: return IKPivot.ThighLeft;
				case IKPivot.CalfRight:  return IKPivot.CalfLeft;
				case IKPivot.FootRight:  return IKPivot.FootLeft;
				case IKPivot.ToeRight:   return IKPivot.ToeLeft;
					
				default: return IKPivot.Unknown;
			}
		}
	}
}