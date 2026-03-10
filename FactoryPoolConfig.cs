using Godot;

public partial class FactoryPoolConfig : Resource {
    [Export] public PackedScene Object;
    [Export] public int WarmupExpansionSize = 32;
	[Export] public int MinPoolToExpand = 8;
    [Export] public int PerExpandBatch = 16;
}