using UnityEngine;
public static class Bezier
{
    public static Vector3 GetPoint(Vector3 point0, Vector3 point1, Vector3 point2, float time)
    {
        time = Mathf.Clamp01(time);
        float oneMinusT = 1f - time;
        return oneMinusT * oneMinusT * point0 + 2f * oneMinusT * time * point1 + time * time * point2;
    }
    public static Vector3 GetFirstDerivative(Vector3 point0, Vector3 point1, Vector3 point2, float time)
    {
        return 2f * (1f - time) * (point1 - point0) + 2f * time * (point2 - point1);
    }
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }
}