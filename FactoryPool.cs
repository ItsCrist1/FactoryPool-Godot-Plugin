using System;
using System.Collections.Generic;
using Godot;

public class FactoryPool<T> : IDisposable
where T : Node2D {
    public FactoryPoolConfig Config { get; set; }
    Stack<T> Pool;

    public FastEvent<int> OnExpand, OnExtract;

    public FactoryPool(FactoryPoolConfig Config) {
        this.Config = Config;

        OnExpand = new();
        OnExtract = new();

        Pool = new();
        ExpandPool(Config.WarmupExpansionSize);
    }

    void ExpandPool(int amount=1) {
        for(int i=0; i < amount; ++i) {
            Pool.Push(ToggleNode(
				(T)Config.Object.Instantiate(), 
				false
			));
		}
		
		OnExpand.Invoke(amount);
    }
	
	public List<T> ExtractPool(int amount=1) {
		if(Pool.Count < Math.Max(amount,Config.MinPoolToExpand))
		    ExpandPool(amount - Pool.Count + Config.PerExpandBatch);
		
		List<T> list = new(amount);
		for(int i=0; i < amount; ++i)
		    list.Add(ToggleNode(
				Pool.Pop(), 
				true
			));
			
		OnExtract.Invoke(amount);
			
		return list;
	}

    public void ContributePool(T node)
        => Pool.Push(ToggleNode(
			node, 
			false
		));

    public void Dispose() {
        while(Pool.Count > 0)
		    Pool.Pop().QueueFree();
		
        Pool = null;

        OnExpand.Clear();
        OnExtract.Clear();

        GC.SuppressFinalize(this);
    }
	
	T ToggleNode(T node, bool b) {
		node.ProcessMode = b ? Node.ProcessModeEnum.Inherit 
						     : Node.ProcessModeEnum.Disabled;
		node.Visible = b;
		
		return node;
	}
}