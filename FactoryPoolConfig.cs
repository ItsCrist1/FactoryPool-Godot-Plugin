using Godot;
using System;

[GlobalClass]
public partial class FactoryPoolConfig : Resource {
    [Export] public PackedScene Object;
    [Export] public int WarmupExpansionSize = 32;
	[Export] public int MinPoolToExpand = 8;
    [Export] public int PerExpandBatch = 16;
	
	public Type ObjectType {
		get {
			if(field == null) {
				Node instance = Object.Instantiate();
				field = instance.GetType();
				instance.QueueFree();
			}
			
			return field;
		}
	} = null;
}