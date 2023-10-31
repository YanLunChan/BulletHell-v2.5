using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : ObjectPool<PatternComponent>
{
    //Variables Goes here
    public GameObject target;
    protected override PatternComponent InitializeObjectPool()
    {
        //create new gameobject
        GameObject cache = new GameObject();
        cache.name = "Particle Object";
        cache.transform.parent = gameObject.transform;
        //game script's gameobject
        PatternComponent bulletRef = cache.AddComponent<PatternComponent>();

        //set property
        bulletRef.SetVariables(this.gameObject, target);
        return bulletRef;
    }

    public override void ReQueue(PatternComponent reference)
    {
        reference.gameObject.transform.parent = this.gameObject.transform;
        pool.Enqueue(reference);
    }
}
