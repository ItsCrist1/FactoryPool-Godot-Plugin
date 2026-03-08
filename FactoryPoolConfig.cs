using Godot;

public partial class FactoryPoolConfig : Resource {
    [Export] public PackedScene Object;
    [Export] public int MaxPoolSize = 256;
    [Export] public int MaxExpansionPoolSize = 128;
    [Export] public int WarmupExpansionSize = 16;
    [Export] public int PerExpandBatch = 4;
    [Export] public TickRate AddRate = TickRate.PhysicsProcess;
    [Export] public float AddFrequency = .1f;
}