using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Health Manager", menuName = "Manager/Health Manager")]
public class HealthManager : ScriptableObject
{
    //health manager
    private const int INITIAL_LIFE = 3;
    public int lives;
    public Action loseLife;
    public Action Death;
    public void TakeDamage() 
    {
        if (--lives >= 0)
            loseLife?.Invoke();
        else
            Death?.Invoke();

    }
    private void OnEnable()
    {
        lives = INITIAL_LIFE;
    }
}
