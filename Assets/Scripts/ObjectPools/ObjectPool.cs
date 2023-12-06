using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Need object pool for the following. Number of enemies and the particle bullet pattern that is needed to create re-usable enemies in the world.
/// </summary>
/// <typeparam name="U"></typeparam>
public abstract class ObjectPool<U> : Singleton<ObjectPool<U>>
{
    private const int INITIAL_AMOUNT = 2;

    protected Queue<U> pool = new Queue<U>();

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < INITIAL_AMOUNT; i++)
        {
            pool.Enqueue(InitializeObjectPool());
        }
    }
    protected abstract U InitializeObjectPool();
    public U GetObject() 
    {
        if (pool.Count < 1)
        {
            pool.Enqueue(InitializeObjectPool());
        }
        return pool.Dequeue();
    }

    public abstract void ReQueue(U reference);
}
