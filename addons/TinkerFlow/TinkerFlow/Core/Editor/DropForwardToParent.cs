#if UNITY_6000_0_OR_NEWER
using Godot;
using System;

[Tool]
public partial class DropForwardToParent : Button
{
	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		return GetParent<Control>()._CanDropData(atPosition, data);
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		GetParent<Control>()._DropData(atPosition, data);
	}
}

#endif
