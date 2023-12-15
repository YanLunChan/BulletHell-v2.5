using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<U> : Singleton<ObjectPool<U>> where U : Component
{
    public GameObject[] parentRef;


    private const int INITIAL_AMOUNT = 2;

    private Dictionary<GameObject, Queue<U>> pool = new Dictionary<GameObject, Queue<U>>();

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < parentRef.Length; i++)
        {
            Queue<U> cache = new Queue<U>();

            for (int j = 0; j < INITIAL_AMOUNT; j++)
            {
                cache.Enqueue(InitializeObjectPool(parentRef[i]));
            }
            pool[parentRef[i]] = cache;
        }
    }
    protected abstract U InitializeObjectPool(GameObject parent);
    public U GetObject(GameObject parent) 
    {
        if (pool[parent].Count < 1)
        {
            for (int i = 0; i < parentRef.Length; i++)
            {
                if (parent == parentRef[i])
                {
                    pool[parent].Enqueue(InitializeObjectPool(parent));
                    break;
                }
            }
        }
        return pool[parent].Dequeue();
    }

    public void ReQueue(GameObject parent, U child) 
    {
        child.gameObject.transform.parent = parent.transform;
        pool[parent].Enqueue(child);
    }
}
