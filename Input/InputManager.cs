using System.Collections.Generic;
using Godot;
using GodotLib.Util;

namespace GodotLib.Input;

public partial class InputManager : Node
{
	public static InputManager Instance => autoload ??= NodeUtils.GetAutoload<InputManager>();
	private static InputManager autoload;
	
	private readonly List<InputDevice> devices = new();
	
	public InputDevice CreateDevice()
	{
		var deviceIndex = devices.Count;
		var device = new InputDevice(deviceIndex);
		AddChild(device);
		devices.Add(device);
		return device;
	}
	
	
}
