#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Smart.Attributes.Internal
{
	[InitializeOnLoad]
	public class MustBeAssignedConditionalFieldExclude
	{
		static MustBeAssignedConditionalFieldExclude()
		{
			MustBeAssignedAttributeChecker.ExcludeFieldFilter += ExcludeCheckIfConditionalFieldHidden;
		}
		
		private static readonly Type _conditionallyVisibleType = typeof(ConditionalFieldAttribute);
		
		private static bool ExcludeCheckIfConditionalFieldHidden(FieldInfo field, Object obj)
		{
			if (_conditionallyVisibleType == null) return false;
			if (!field.IsDefined(_conditionallyVisibleType, false)) return false;

			var conditionalFieldAttribute = field.GetCustomAttributes(_conditionallyVisibleType, false)
				.Select(a => a as ConditionalFieldAttribute)
				.SingleOrDefault();

			return conditionalFieldAttribute != null &&
			       !ConditionalFieldUtility.BehaviourPropertyIsVisible(obj, field.Name, conditionalFieldAttribute);
		}
	}
}
#endif