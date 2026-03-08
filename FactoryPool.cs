using System;
using System.Collections.Generic;
using Godot;

public class FactoryPool<T> : IDisposable
where T : Node {
    FactoryPoolConfig Config;
    Stack<T> pool;
    Timer timer;

    public FastEvent OnExpand, OnExtract;

    public FactoryPool(FactoryPoolConfig Config=null) {
        Config ??= new();
        this.Config = Config;

        OnExpand = new();
        OnExtract = new();

        pool = new();
        ExpandPool(Config.WarmupExpansionSize);

        timer = TimerManager.CreateLooping(new() {
            AutoStart = true,
            TickRate = Config.AddRate,
            TickFrequency = Config.AddFrequency
        });

        timer.OnTick += () => ExpandPool(Config.PerExpandBatch);
    }

    void ExpandPool(int amount=1) {
        if(pool.Count >= Config.MaxExpansionPoolSize) {
            timer.Pause();
            return;
        }

        int toAdd = Mathf.Min(amount, Config.MaxExpansionPoolSize - pool.Count);

        for(int i=0; i < toAdd; ++i) {
            pool.Push(Config.Object.Instantiate() as T);
            OnExpand.Invoke();
        }
    }

    public T ExtractPool() {
        if(pool.Count == 0)
            ExpandPool();

        timer.Resume();

        OnExtract.Invoke();
        return pool.Pop();
    }

    public void ContributePool(T node) {
        if(pool.Count >= Config.MaxPoolSize) {
            node.QueueFree();
            return;
        }

        pool.Push(node);
    }

    public void Dispose() {
        for(; pool.Count > 0; pool.Pop().QueueFree());
        pool = null;
        
        timer.Dispose();
        timer = null;

        OnExpand.Clear();
        OnExtract.Clear();

        GC.SuppressFinalize(this);
    }
}