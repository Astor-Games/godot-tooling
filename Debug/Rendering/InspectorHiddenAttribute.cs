namespace GodotLib.Debug;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = true)]
internal class InspectorHiddenAttribute : Attribute;