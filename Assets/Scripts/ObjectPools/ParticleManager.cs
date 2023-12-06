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

        //set target
        bulletRef.SetVariables(target);

        //get return to and put requeue into return to so when it dies it automaticallyl reque back if created by manager.
        bulletRef.ReturnTo = () => ReQueue(bulletRef);
        return bulletRef;
    }

    public override void ReQueue(PatternComponent reference)
    {
        reference.gameObject.transform.parent = this.gameObject.transform;
        pool.Enqueue(reference);
    }
}
