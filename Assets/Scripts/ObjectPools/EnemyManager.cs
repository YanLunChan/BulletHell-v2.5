using System;
using UnityEngine;
/// <summary>
/// 
/// </summary>
/// 

public class EnemyManager : ObjectPool<GameObject>
{
    //prefab goes here
    [SerializeField] private GameObject enemyPrefab;

    protected override GameObject InitializeObjectPool()
    {
        //Instantiate Enemy
        GameObject enemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);

        //get enemy_mob component and attach this back to manager. 
        enemy.GetComponent<Enemy_Mob>().DeathReturn = () => ReQueue(enemy);
        return enemy;
    }
    public override void ReQueue(GameObject reference)
    {
        reference.gameObject.transform.parent = this.gameObject.transform;
        pool.Enqueue(reference);
    }
}
