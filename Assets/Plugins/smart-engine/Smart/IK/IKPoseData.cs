using System;
using UnityEngine;

namespace Smart.IK
{
	[Serializable]
	public class IKPoseData
	{
		[SerializeField] private Vector3 _pelvisLocalPosition;
		[SerializeField] private Quaternion[] _rotations;

		public Vector3 PelvisLocalPosition => _pelvisLocalPosition;
		public Quaternion[] Rotations => _rotations;

		public IKPoseData(Vector3 pelvisLocalPosition, Quaternion[] rotations)
		{
			_pelvisLocalPosition = pelvisLocalPosition;
			_rotations = rotations;
		}
	}
}