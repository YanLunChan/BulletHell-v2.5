using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : ObjectPool<BulletPattern>
{
    protected override BulletPattern InitializeObjectPool(GameObject parent)
    {
        //create new gameobject
        GameObject cache = new GameObject();
        cache.name = "Particle System";
        cache.transform.parent = parent.transform;
        //game script's gameobject
        BulletPattern bulletRef = cache.AddComponent<BulletPattern>();
        bulletRef.parentRef = parent;
        cache.transform.eulerAngles = new Vector3(bulletRef.angle, 90f, 0);

        //add IbulletSetup function component or something...

        return bulletRef;
    }
}
