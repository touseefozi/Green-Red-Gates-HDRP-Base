using System;
using System.Collections.Generic;
using UnityEngine;

namespace Smart.IK
{
	[Serializable]
	public class IKSkeleton
	{
		[Header("Spine:")]
		[SerializeField] private Transform _pelvis;
		[SerializeField] private Transform _stomach;
		[SerializeField] private Transform _chest;
		
		[Header("Head:")]
		[SerializeField] private Transform _neck;
		[SerializeField] private Transform _head;
		[SerializeField] private Transform _headEnd;
		
		[Header("Left Arm:")]
		[SerializeField] private Transform _clavicleLeft;
		[SerializeField] private Transform _armLeft;
		[SerializeField] private Transform _forearmLeft;
		[SerializeField] private Transform _handLeft;
		
		[Header("Right Arm:")]
		[SerializeField] private Transform _clavicleRight;
		[SerializeField] private Transform _armRight;
		[SerializeField] private Transform _forearmRight;
		[SerializeField] private Transform _handRight;
		
		[Header("Left Leg:")]
		[SerializeField] private Transform _thighLeft;
		[SerializeField] private Transform _calfLeft;
		[SerializeField] private Transform _footLeft;
		[SerializeField] private Transform _toeLeft;
		
		[Header("Right Leg:")]
		[SerializeField] private Transform _thighRight;
		[SerializeField] private Transform _calfRight;
		[SerializeField] private Transform _footRight;
		[SerializeField] private Transform _toeRight;
		
		[Header("Fingers Left:")]
		[SerializeField] private Transform _fingersLeft1;
		[SerializeField] private Transform _fingersLeft2;
		[SerializeField] private Transform _fingersLeft3;
		[SerializeField] private Transform _thumbLeft1;
		[SerializeField] private Transform _thumbLeft2;
		
		[Header("Fingers Right:")]
		[SerializeField] private Transform _fingersRight1;
		[SerializeField] private Transform _fingersRight2;
		[SerializeField] private Transform _fingersRight3;
		[SerializeField] private Transform _thumbRight1;
		[SerializeField] private Transform _thumbRight2;
		
		public Transform Pelvis => _pelvis;
		public Transform Stomach => _stomach;
		public Transform Chest => _chest;
		public Transform Neck => _neck;
		public Transform Head => _head;
		public Transform HeadEnd => _headEnd;
		public Transform ClavicleLeft => _clavicleLeft;
		public Transform ArmLeft => _armLeft;
		public Transform ForearmLeft => _forearmLeft;
		public Transform HandLeft => _handLeft;
		public Transform ClavicleRight => _clavicleRight;
		public Transform ArmRight => _armRight;
		public Transform ForearmRight => _forearmRight;
		public Transform HandRight => _handRight;
		public Transform ThighLeft => _thighLeft;
		public Transform CalfLeft => _calfLeft;
		public Transform FootLeft => _footLeft;
		public Transform ToeLeft => _toeLeft;
		public Transform ThighRight => _thighRight;
		public Transform CalfRight => _calfRight;
		public Transform FootRight => _footRight;
		public Transform ToeRight => _toeRight;
		public Transform FingersLeft1 => _fingersLeft1;
		public Transform FingersLeft2 => _fingersLeft2;
		public Transform FingersLeft3 => _fingersLeft3;
		public Transform ThumbLeft1 => _thumbLeft1;
		public Transform ThumbLeft2 => _thumbLeft2;
		public Transform FingersRight1 => _fingersRight1;
		public Transform FingersRight2 => _fingersRight2;
		public Transform FingersRight3 => _fingersRight3;
		public Transform ThumbRight1 => _thumbRight1;
		public Transform ThumbRight2 => _thumbRight2;

		public bool IsValid()
		{
			return _pelvis != null &&
			       _stomach != null &&
			       _chest != null &&
			       _neck != null &&
			       _head != null &&
			       _headEnd != null &&
			       _clavicleLeft != null &&
			       _armLeft != null &&
			       _forearmLeft != null &&
			       _handLeft != null &&
			       _clavicleRight != null &&
			       _armRight != null &&
			       _forearmRight != null &&
			       _handRight != null &&
			       _thighLeft != null &&
			       _calfLeft != null &&
			       _footLeft != null &&
			       _thighRight != null &&
			       _calfRight != null &&
			       _footRight != null;
		}

		public List<Transform> GetAllBones()
		{
			return new List<Transform>
			{
				_pelvis,
				_stomach,
				_chest,
				_neck,
				_head,
				_headEnd,
				_clavicleLeft,
				_armLeft,
				_forearmLeft,
				_handLeft,
				_clavicleRight,
				_armRight,
				_forearmRight,
				_handRight,
				_thighLeft,
				_calfLeft,
				_footLeft,
				_toeLeft,
				_thighRight,
				_calfRight,
				_footRight,
				_toeRight,
				_fingersLeft1,
				_fingersLeft2,
				_fingersLeft3,
				_thumbLeft1,
				_thumbLeft2,
				_fingersRight1,
				_fingersRight2,
				_fingersRight3,
				_thumbRight1,
				_thumbRight2,
			};
		}

		public Quaternion[] GetRotations()
		{
			var bones = GetAllBones();
			var rotations = new Quaternion[bones.Count];

			for (int i = 0; i < bones.Count; i++)
			{
				var bone = bones[i];
				rotations[i] = bone != null ? bone.localRotation : Quaternion.identity;
			}
			
			return rotations;
		}

		public Transform GetBoneTransform(IKPivot pivot)
		{
			switch (pivot)
			{
				case IKPivot.ChestPole:		 return _chest;
				case IKPivot.HeadPole:		 return _neck;
				case IKPivot.ArmPoleLeft:    return _forearmLeft;
				case IKPivot.ArmPoleRight:	 return _forearmRight;
				case IKPivot.LegPoleLeft:	 return _calfLeft;
				case IKPivot.LegPoleRight:	 return _calfRight;
				
				case IKPivot.Pelvis:		 return _pelvis;
				case IKPivot.Stomach:		 return _stomach;
				case IKPivot.Chest:			 return _chest;
				
				case IKPivot.Neck:           return _neck;
				case IKPivot.Head:			 return _head;
				case IKPivot.HeadEnd:        return _headEnd;
				
				case IKPivot.ClavicleLeft:	 return _clavicleLeft;
				case IKPivot.ArmLeft:		 return _armLeft;
				case IKPivot.ForearmLeft:	 return _forearmLeft;
				case IKPivot.HandLeft:       return _handLeft;
				
				case IKPivot.ClavicleRight:	 return _clavicleRight;
				case IKPivot.ArmRight:		 return _armRight;
				case IKPivot.ForearmRight:	 return _forearmRight;
				case IKPivot.HandRight:      return _handRight;
				
				case IKPivot.ThighLeft:	     return _thighLeft;
				case IKPivot.CalfLeft:	     return _calfLeft;
				case IKPivot.FootLeft:       return _footLeft;
				case IKPivot.ToeLeft:	     return _toeLeft;
				
				case IKPivot.ThighRight:	 return _thighRight;
				case IKPivot.CalfRight:	     return _calfRight;
				case IKPivot.FootRight:      return _footRight;
				case IKPivot.ToeRight:	     return _toeRight;
				
				case IKPivot.FingersLeft_1:  return _fingersLeft1;
				case IKPivot.FingersLeft_2:  return _fingersLeft2;
				case IKPivot.FingersLeft_3:  return _fingersLeft3;
				case IKPivot.ThumbLeft_1:    return _thumbLeft1;
				case IKPivot.ThumbLeft_2:    return _thumbLeft2;
				
				case IKPivot.FingersRight_1: return _fingersRight1;
				case IKPivot.FingersRight_2: return _fingersRight2;
				case IKPivot.FingersRight_3: return _fingersRight3;
				case IKPivot.ThumbRight_1:   return _thumbRight1;
				case IKPivot.ThumbRight_2:   return _thumbRight2;
				
				default: return null;
			}
		}
	}
}