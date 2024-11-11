using System;

namespace Smart.Attributes
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class HideScriptFieldAttribute : Attribute { }
}