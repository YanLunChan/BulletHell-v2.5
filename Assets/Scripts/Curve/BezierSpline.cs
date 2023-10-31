using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.UIElements;
/// <summary>
/// This section is used to initialize the settings and location the mob should spawn in.
/// Enemy setting is used to put all settings for enemy in one struct to initalize per wave.
/// </summary>
[Serializable]
public struct Enemy_Settings 
{
    //shared default settings
    public EnemySO sharedvalue;
    //number of volley
    public int numVolley;
    //Reference to enemy_manager (if doesn't exist then won't run due to nullable)
    public EnemyManager eManager;
    //Enemy Variables (HP, wait time before leaving from spawn, speed etc...)
    public int health;
    //Enemy Start Movement settings.
    public float baseWait;
    public float increWait;
    public float speed;

    //Bullet Variables (Pattern, shotspeed, Shotnum, delay)
    [SerializeField] public BulletVariables[] bulletPatterns;

}
/// <summary>
/// This Struct is incharge of keeping all information on where to stop on the curve and when the enemy needs to shoot.
/// </summary>
[Serializable]
public struct TupleInspector<T>
{
    [Range(0f,1f)] public T destination;
    public T incrDes;
    [Min(0f)] public T time;
    public T incrTime;

    public TupleInspector(T left, T right, T incrDes, T incrTime)
    {
        this.destination = left;
        this.time = right;
        this.incrDes = incrDes;
        this.incrTime = incrTime;
    }
}
public class BezierSpline : MonoBehaviour
{
    //Reference for Singleton here

    //Varaible for Spline
    [SerializeField] private Vector3[] points;
    [SerializeField] private BezierControlPointMode[] modes;
    [SerializeField] private bool loop;

    //Variables for enemy
    [SerializeField] private Enemy_Settings enemySettings;
    //Enemy Shooting/Pauing duration
    [SerializeField] private TupleInspector<float>[] shootDurations;

    public bool Loop
    {
        get
        {
            return loop;
        }
        set
        {
            loop = value;
            if (loop)
            {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }
    public int ControlPointCount
    {
        get
        {
            return points.Length;
        }
    }
    public Enemy_Settings EnemySettings 
    { 
        get 
        { 
            return enemySettings; 
        } 
    }
    public Vector3 GetControlPoint(int index)
    {
        return points[index];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = point - points[index];
            if (loop)
            {
                if (index == 0)
                {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1)
                {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else
                {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length)
                {
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        EnforceMode(index);
    }
    public int CurveCount
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }
    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(
            points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(
            points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }
    public void AddCurve()
    {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop)
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }
    public BezierControlPointMode GetControlPointMode(int index)
    {
        return modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;
        if (loop)
        {
            if (modeIndex == 0)
            {
                modes[modes.Length - 1] = mode;
            }
            else if (modeIndex == modes.Length - 1)
            {
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }
    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0)
            {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= points.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = points.Length - 2;
            }
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }
    public void Reset()
    {
        points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    //Initialize enemy class
    public void InitializeEnemy(int index) 
    {
        GameObject reference = enemySettings.eManager.GetObject();
        //check if emanager is valid
        if (enemySettings.eManager)
        {
            reference.gameObject.transform.parent = gameObject.transform;

            reference.transform.position = GetPoint(0);
            enemySettings.sharedvalue?.Initialization(reference);

            Enemy_Mob cache = reference.GetComponent<Enemy_Mob>();
            cache.SetSplinetoWalker(this);

            //Start Couroutine (Initial spawn timer, incrementing wait, index, and shooting locaiton and duration)
            cache.Walker.StartCoroutine(cache.Walker.Cycle(enemySettings, index, shootDurations));

            ApplyPattern(reference);
        }
    }
    public void ApplyPattern(GameObject attachTo)
    {
        //Attach number of Particle Component here.
        ObjectPool<PatternComponent> reference = ParticleManager.GetInstance();
        if (reference == null)
            return;

        // foreach bullet variable inside the array
        foreach (BulletVariables value in enemySettings.bulletPatterns)
        {
            float step = 0f;
            if (value.iterationSmall > 1f)
                step = value.angleSmall / (value.iterationSmall - 1f);
            //number of times
            for (int i = 0; i < value.numAngle; i++)
            {
                for (int j = 0; j < value.iterationSmall; j++)
                {
                    //Get Instance
                    PatternComponent pattern = reference.GetObject();

                    //attach pattern to parent of parameter and set position
                    pattern.gameObject.transform.parent = attachTo.transform;
                    pattern.transform.position = attachTo.transform.position;

                    //formula for where angle is aligned.
                    float angle = (step * j) + (value.angleLarge * i) + (value.angleSmall * i) + value.offsetAngle;

                    //Set up particle system.
                    pattern.PatternConfig(value, angle);
                }

            }
        }
    }

    //debug
    private void Start()
    {
        InitializeEnemy(0);
    }
}