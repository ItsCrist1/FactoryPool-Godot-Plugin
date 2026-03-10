using System.Collections.Generic;

using Godot;

public partial class FactoryPoolNode<T> : Node
where T : Node2D {
	[Export] FactoryPoolConfig Config;
	
	[Signal] public delegate void OnExpandEventHandler(int amount);
	[Signal] public delegate void OnExtractEventHandler(int amount);
	
	FactoryPool<T> Pool;
	
	public override void _EnterTree() {
		Pool = new(Config);
		
		Pool.OnExpand += OnExpand;
		Pool.OnExtract += OnExtract;
	}
	
	public override void _ExitTree() {
		Pool.Dispose();
		Pool = null;
	}
	
	public List<T> ExtractPool(int amount=1)
	    => Pool.ExtractPool(amount);
		
	public void ContributePool(T node)
	    => Pool.ContributePool(node);
	
	void OnExpand(int amount) 
	    => EmitSignal(SignalName.OnExpand, amount);
		
	void OnExtract(int amount) 
	    => EmitSignal(SignalName.OnExtract, amount);
}