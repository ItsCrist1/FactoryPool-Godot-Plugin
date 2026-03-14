using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class FactoryPoolManager : Node {
	[Export] Godot.Collections.Array<FactoryPoolConfig> Configs;
	
	public static FactoryPoolManager Instance { get; private set; }
	
	Dictionary<Type, FactoryPool> Pools;
	
	public override void _EnterTree() {
		Instance = this;
		
		Pools = new();
		
		foreach(FactoryPoolConfig config in Configs)
		    Pools[config.ObjectType] = new(config, this);
	}
	
	public T ExtractObject<T>() where T : Node
	    => (T)Pools[typeof(T)].ExtractObject();
	
	public IEnumerable<T> ExtractObjects<T>(int amount=1) where T : Node
	    => Pools[typeof(T)].ExtractObjects(amount).Cast<T>();
		
	public void ContributeObject(Node node)
	    => Pools[node.GetType()].ContributeObject(node);

	public void FreeObject(Node node)
	    => Pools[node.GetType()].FreeObject(node);

	public void ResetPool<T>(FactoryPoolConfig Config) {
		Pools[typeof(T)].Dispose();
		Pools[typeof(T)] = new(Config, this);
	}
	
	public override void _ExitTree() {
		Instance = null;
		
		foreach(FactoryPoolConfig config in Configs)
		    Pools[config.ObjectType].Dispose();
	}
}