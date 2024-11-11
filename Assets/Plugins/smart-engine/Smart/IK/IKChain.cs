using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Smart.IK
{
	[Serializable]
	public class IKChain
	{
		private enum ResolveMode
		{
			Default,
			Pole,
			Constraint,
		}
		
		private const int MinChainLength = 2;
		private const int MaxChainLenght = 10;
		
		[SerializeField] public bool ConstraintEnabled;
		[SerializeField] public Vector3 TargetPosition;
		[SerializeField] public Vector3 PolePosition;
		
		[SerializeField] private int _chainLength;
		[SerializeField] private IKCharacter _character;
		[SerializeField] private Transform _rootTransform;
		[SerializeField] private Transform _firstBone;
		[SerializeField] private Transform _middleBone;
		[SerializeField] private Transform _lastBone;
		[SerializeField] private bool _reversePole;

		[SerializeField] private float[] _bonesLength;
		[SerializeField] private float _completeLength;
		[SerializeField] private List<Transform> _bones;
		[SerializeField] private Vector3[] _startDirection;
		[SerializeField] private Quaternion[] _startRotationBone;
		
		public List<Transform> Bones => _bones;
		public Transform MiddleBone => _middleBone;
		public Transform RootTransform => _rootTransform;
		
		public bool IsValid => _rootTransform != null && _firstBone != null && _lastBone != null;
		
		private Vector3 _cachedTargetPosition;
		private Quaternion _cachedLastBoneRotation;

		public IKChain(IKCharacter character, Transform rootTransform, Transform firstBone, Transform lastBone, bool reversePole = false)
		{
			_character = character;
			_rootTransform = rootTransform;
			_firstBone = firstBone;
			_lastBone = lastBone;
			_reversePole = reversePole;

			Assert.IsNotNull(_rootTransform, "Root transform is null");
			Assert.IsNotNull(_firstBone, "First bone is null");
			Assert.IsNotNull(_lastBone, "Last bone is null");
			
			_bones = new List<Transform>();
			
			var transform = _lastBone;
			
			for (var i = 0; i < MaxChainLenght; i++)
			{
				_bones.Add(transform);
				transform = transform.parent;
				
				if (transform == _firstBone)
				{
					_bones.Add(transform);
					break;
				}
			}
			
			Assert.IsTrue(transform == _firstBone, "Chain lenght or bones parents is incorrect");
			
			_bones.Reverse();
			_completeLength = 0;
			_chainLength = _bones.Count - 1;
			
			Assert.IsTrue(_chainLength >= MinChainLength, $"Chain lenght must have length >= {MinChainLength}");
			
			_middleBone = _bones[_chainLength / 2];
			ResetPivots();
			
			_bonesLength = new float[_chainLength];
			_startDirection = new Vector3[_chainLength + 1];
			_startRotationBone = new Quaternion[_chainLength + 1];

			_completeLength = 0;
			
			for (var i = _bones.Count - 1; i >= 0; i--)
			{
				transform = _bones[i];
				_startRotationBone[i] = GetRotationRootSpace(transform.rotation);

				if (i == _bones.Count - 1)
				{
					_startDirection[i] = TargetPosition - GetPositionRootSpace(transform.position);
				}
				else
				{
					_startDirection[i] = GetPositionRootSpace(_bones[i + 1].position) - GetPositionRootSpace(transform.position);
					_bonesLength[i] = _startDirection[i].magnitude;
					_completeLength += _bonesLength[i];
				}
			}
		}
		
		public void ResetPivots()
		{
			ResetTargetPosition();
			ResetPolePosition();
		}

		public void ResetTargetPosition()
		{
			TargetPosition = _rootTransform.InverseTransformPoint(_lastBone.position);
		}

		public void ResetPolePosition()
		{
			var firstPosition = _firstBone.position;
			var lastPosition = _lastBone.position;
			var middlePosition = _middleBone.position;
			
			var projection = Vector3.Project(middlePosition - firstPosition, Vector3.Normalize(lastPosition - firstPosition));
			var normal = Vector3.Normalize(middlePosition - (firstPosition + projection));
			var distance = Vector3.Distance(firstPosition, middlePosition);

			if (normal == Vector3.zero)
			{
				normal = _reversePole ? Vector3.forward : Vector3.back;
				PolePosition = _rootTransform.InverseTransformPoint(_middleBone.TransformPoint(normal * distance));
			}
			else
			{
				PolePosition = _rootTransform.InverseTransformPoint(middlePosition + normal * distance);
			}
		}

		private void ResolveIK(ResolveMode mode)
		{
			var positions = new Vector3[_chainLength + 1];
			
			for (var i = 0; i < _bones.Count; i++)
			{
				positions[i] = GetPositionRootSpace(_bones[i].position);
			}
			
			var targetPosition = TargetPosition;
			var iterations = _character.Iterations;
			var delta = _character.Delta;
			var snapBackStrength = _character.SnapBackStrength;

			if ((targetPosition - GetPositionRootSpace(_bones[0].position)).sqrMagnitude >= _completeLength * _completeLength)
			{
				var direction = (targetPosition - positions[0]).normalized;
				
				for (int i = 1; i < positions.Length; i++)
				{
					positions[i] = positions[i - 1] + direction * _bonesLength[i - 1];
				}
			}
			else
			{
				for (int i = 0; i < positions.Length - 1; i++)
				{
					positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + _startDirection[i], snapBackStrength);
				}

				for (int iteration = 0; iteration < iterations; iteration++)
				{
					for (int i = positions.Length - 1; i > 0; i--)
					{
						if (i == positions.Length - 1)
						{
							positions[i] = targetPosition;
						}
						else
						{
							positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * _bonesLength[i];
						}
					}

					for (int i = 1; i < positions.Length; i++)
					{
						positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * _bonesLength[i - 1];
					}

					if ((positions[positions.Length - 1] - targetPosition).sqrMagnitude < delta * delta)
					{
						break;
					}
				}
			}
			
			for (int i = 1; i < positions.Length - 1; i++)
			{
				var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
				var projectedPole = plane.ClosestPointOnPlane(PolePosition);
				var projectedBone = plane.ClosestPointOnPlane(positions[i]);
				var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
				positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
			}

			for (int i = 0; i < positions.Length - 1; i++)
			{
				var boneTransform = _bones[i];
				var direction = positions[i + 1] - positions[i];
				
				SetRotationRootSpace(boneTransform,
					Quaternion.FromToRotation(_startDirection[i], direction) * Quaternion.Inverse(_startRotationBone[i]));
				
				//SetPositionRootSpace(boneTransform, positions[i]);
			}

			//if (mode != ResolveMode.Pole)
			{
				ResetPolePosition();
			}

			if (mode != ResolveMode.Constraint)
			{
				ResetTargetPosition(); 
			}
		}

		private void SetRotationRootSpace(Transform transform, Quaternion rotation)
		{
			transform.rotation = _rootTransform.rotation * rotation;
		}

		private void SetPositionRootSpace(Transform transform, Vector3 position)
		{
			transform.position = _rootTransform.rotation * position + _rootTransform.position;
		}

		private Quaternion GetRotationRootSpace(Quaternion rotation)
		{
			return Quaternion.Inverse(rotation) * _rootTransform.rotation;
		}

		public Vector3 GetPositionRootSpace(Vector3 position)
		{
			return Quaternion.Inverse(_rootTransform.rotation) * (position - _rootTransform.position);
		}

		public void CacheTargetPosition()
		{
			_cachedTargetPosition = _lastBone.InverseTransformPoint(_rootTransform.TransformPoint(TargetPosition));
		}

		public void RestoreTargetPosition()
		{
			TargetPosition = _rootTransform.InverseTransformPoint(_lastBone.TransformPoint(_cachedTargetPosition));
		}

		public void CacheConstraintTargetPosition()
		{
			_cachedTargetPosition = _rootTransform.TransformPoint(TargetPosition);
			_cachedLastBoneRotation = _lastBone.rotation;
		}

		public void ApplyConstraintTargetPosition()
		{
			TargetPosition = _rootTransform.InverseTransformPoint(_cachedTargetPosition);
			ResolveIK(ResolveMode.Constraint);
			_lastBone.rotation = _cachedLastBoneRotation;
		}
		
		public void SetTargetPosition(Vector3 value)
		{
			TargetPosition = value;
			ResolveIK(ResolveMode.Default);
		}
		
		public void SetPolePosition(Vector3 value)
		{
			PolePosition = value;
			ResolveIK(ResolveMode.Pole);
		}
	}
}