using System;
using System.Collections.Generic;
using Godot;

public class FactoryPool : IDisposable {
    public FactoryPoolConfig Config { get; set; }
    Stack<Node> Pool;
	HashSet<Node> potentialOrphans;
	Node Parent;

    public FastEvent<int> OnExpand, OnExtract;

    public FactoryPool(FactoryPoolConfig Config, Node Parent) {
        this.Config = Config;
		this.Parent = Parent;

        OnExpand = new();
        OnExtract = new();

        Pool = new();
        ExpandPool(Config.WarmupExpansionSize);
		
		potentialOrphans = new();
    }

    void ExpandPool(int amount=1) {
        for(int i=0; i < amount; ++i) {
            Node node = ToggleNode(Config.Object.Instantiate(),false);
			Pool.Push(node);
			Parent.AddChild(node);
		}
		
		OnExpand.Invoke(amount);
    }
	
	public Node ExtractObject() {
		if(Pool.Count < Config.MinPoolToExpand)
		    ExpandPool(Config.PerExpandBatch);
			
		OnExtract.Invoke(1);
		
		
		Node node = ToggleNode(Pool.Pop(), true);
		potentialOrphans.Add(node);
		return node;
	}
	
	public List<Node> ExtractObjects(int amount=1) {
		if(Pool.Count < Math.Max(amount,Config.MinPoolToExpand))
		    ExpandPool(amount - Pool.Count + Config.PerExpandBatch);
		
		List<Node> list = new(amount);
		for(int i=0; i < amount; ++i) {
			Node node = ToggleNode(Pool.Pop(), true);
			potentialOrphans.Add(node);
			list.Add(node);
		}
			
		OnExtract.Invoke(amount);
			
		return list;
	}

    public void ContributeObject(Node node) {
	    Node cnode = ToggleNode(node, false);
		potentialOrphans.Remove(cnode);
		Pool.Push(cnode);
	}

	public void FreeObject(Node node) {
		potentialOrphans.Remove(node);
		node.QueueFree();
	}

    public void Dispose() {
        while(Pool.Count > 0)
		    Pool.Pop().QueueFree();
		
        Pool = null;

        OnExpand.Clear();
        OnExtract.Clear();
		
		foreach(Node actualOrphan in potentialOrphans)
			if(GodotObject.IsInstanceValid(actualOrphan))
				// quietly murder the orphan
			    // what will they do, tell `Parent`?
			    actualOrphan.QueueFree();

        GC.SuppressFinalize(this);
    }
	
	Node ToggleNode(Node node, bool b) {
		node.ProcessMode = b ? Node.ProcessModeEnum.Inherit 
						     : Node.ProcessModeEnum.Disabled;
		
		if(node is Node2D node2D)
		    node2D.Visible = b;
		
		if(node is Node3D node3D)
		    node3D.Visible = b;
			
		if(node is Control nodeControl)
		    nodeControl.Visible = b;
		
		return node;
	}
}