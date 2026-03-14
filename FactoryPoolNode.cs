using System;
using System.Collections.Generic;

using Godot;

[GlobalClass]
public partial class FactoryPoolNode : Node  {
	[Export] FactoryPoolConfig Config;
	
	[Signal] public delegate void OnExpandEventHandler(int amount);
	[Signal] public delegate void OnExtractEventHandler(int amount);
	
	FactoryPool Pool;
	
	public override void _EnterTree() {
		Pool = new(Config, this);
		
		Pool.OnExpand += _OnExpand;
		Pool.OnExtract += _OnExtract;
	}
	
	public override void _ExitTree() {
		Pool.Dispose();
		Pool = null;
	}
	
	public Node ExtractObject()
	    => Pool.ExtractObject();
	
	public List<Node> ExtractObjects(int amount=1)
	    => Pool.ExtractObjects(amount);
		
	public void ContributeObject(Node node)
	    => Pool.ContributeObject(node);

	public void FreeObject(Node node)
	    => Pool.FreeObject(node);
	
	void _OnExpand(int amount) 
	    => EmitSignal(SignalName.OnExpand, amount);
		
	void _OnExtract(int amount) 
	    => EmitSignal(SignalName.OnExtract, amount);
}