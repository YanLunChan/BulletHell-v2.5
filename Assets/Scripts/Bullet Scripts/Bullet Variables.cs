using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// settings used to 
/// </summary>
[Serializable]
public struct BulletVariables
{
    [Header("Sprite")]
    public Sprite sprite;
    //Shooting Variables
    [Header("How will the bullets shoot/look?")]
    public int shootNum;
    public float delayPerShot;
    public float spinSpeed;
    public float projSpeed;
    public float projSize;
    public Material projMat;

    //Where to shoot
    public bool target;
    [Header("How will the pattern look?")]
    //iteration small means the number per small angle
    [Min(1)] public int iterationSmall;
    [Range(1, 360)] public float angleSmall;
    //angle large refers to the gap
    [Min(0)] public float angleLarge;
    public float offsetAngle;
    //num angle refers to how many times it replicates thee small angle.
    public float numAngle;
    

    //point variable
    [Header("Point Variables")]
    public int pointNum;
    public float accelSpeed;

    [Header("Layer Mask")]
    public LayerMask layerMask;
}
