#if UNITY_EDITOR
namespace Smart.Attributes.Internal
{
	using UnityEditor;
	using UnityEngine;

	[InitializeOnLoad]
	public class RequireLayerOtRagAttributeHandler
	{
		static RequireLayerOtRagAttributeHandler()
		{
			EditorApplication.playModeStateChanged += AutoSaveWhenPlaymodeStarts;
		}

		private static void AutoSaveWhenPlaymodeStarts(PlayModeStateChange obj)
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
			{
				var components = Object.FindObjectsOfType<Component>();
				foreach (var component in components)
				{
					foreach (var attribute in component.GetType().GetCustomAttributes(true))
					{
						if (attribute is RequireLayerAttribute layerAttribute)
						{
							var requiredLayer = layerAttribute.LayerName != null ?
								LayerMask.NameToLayer(layerAttribute.LayerName) :
								layerAttribute.LayerIndex;
							if (component.gameObject.layer == requiredLayer) continue;

							Debug.LogWarning("Layer of " + component.name + " changed by RequireLayerAttribute to " +
							                 layerAttribute.LayerName);
							component.gameObject.layer = requiredLayer;
							EditorUtility.SetDirty(component);

							continue;
						}

						if (attribute is RequireTagAttribute tagAttribute)
						{
							if (component.CompareTag(tagAttribute.Tag)) continue;

							Debug.LogWarning("Tag of " + component.name + " changed by RequireTagAttribute to " + tagAttribute.Tag);
							component.gameObject.tag = tagAttribute.Tag;
							EditorUtility.SetDirty(component);
						}
					}
				}
			}
		}
	}
}
#endif