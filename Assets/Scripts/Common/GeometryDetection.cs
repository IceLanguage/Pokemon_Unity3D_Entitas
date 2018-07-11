using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct GeometryDetection 
{

    public struct AABB
    {
        public Vector3 min;
        public Vector3 max;
        public Vector3 center;
       
        public AABB(Vector3 vmin, Vector3 vmax)
        {
            min = vmin;
            max = vmax;
            center = (min + max) / 2;
        }
       
        
    }

    // 点和AABB的相交  
    public static bool Overlap_Point_AABB(Vector3 p, AABB aabb)
    {
        return ((p.x >= aabb.min.x && p.x <= aabb.max.x)
            && (p.y >= aabb.min.y && p.y <= aabb.max.y)
            && (p.z >= aabb.min.z && p.z <= aabb.max.z));


    }

    public struct Sphere
    {
        public Vector3 center;
        public float r;

        public Sphere(Vector3 c,float r)
        {
            center = c;
            this.r = r;
        }
    }


    /// <summary>
    /// AABB和球体相交
    /// </summary>
    /// <param name="aabb"></param>
    /// <param name="sphere"></param>
    /// <returns></returns>
    public static bool Overlap_AABB_Sphere(AABB aabb,Sphere sphere)
    {

        Vector3 h = aabb.max - aabb.center;
        Vector3 c = aabb.center;
        Vector3 p = sphere.center;
        float r = sphere.r;
        Vector3 v = new Vector3(Mathf.Abs(p.x - c.x), Mathf.Abs(p.y - c.y), Mathf.Abs(p.z - c.z));
        Vector3 u = new Vector3(Mathf.Max(v.x - h.x, 0), Mathf.Max(v.y - h.y, 0), Mathf.Max(v.z - h.z, 0));
        return Vector3.Dot(u, v) <= r * r;
    }

    
    /// <summary>
    /// 矩形和圆相交
    /// </summary>
    /// <param name="c"></param>
    /// <param name="h"></param>
    /// <param name="p"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    private static bool OverLap_Box_Circle(Vector2 c,Vector2 h,Vector2 p,float r)
    {
        Vector2 v = new Vector2(Mathf.Abs(p.x - c.x), Mathf.Abs(p.y - c.y));
        Vector2 u = new Vector2(Mathf.Max(v.x - h.x, 0), Mathf.Max(v.y- h.y, 0));
        return Vector2.Dot(u,v) <= r * r;
    }

    /// <summary>
    /// 线段是否相交
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool IsIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        var crossA = Mathf.Sign(Vector3.Cross(d - c, a - c).y);
        var crossB = Mathf.Sign(Vector3.Cross(d - c, b - c).y);

        if (Mathf.Approximately(crossA, crossB)) return false;

        var crossC = Mathf.Sign(Vector3.Cross(b - a, c - a).y);
        var crossD = Mathf.Sign(Vector3.Cross(b - a, d - a).y);

        if (Mathf.Approximately(crossC, crossD)) return false;

        return true;
    }

    public static int GetIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 contractPoint)
    {
        contractPoint =Vector3.zero;

        if (Mathf.Abs(b.z - a.z) + Mathf.Abs(b.x - a.x) + Mathf.Abs(d.z - c.z)
                + Mathf.Abs(d.x - c.x) == 0)
        {
            if ((c.x - a.x) + (c.z - a.z) == 0)
            {
                //Debug.Log("ABCD是同一个点！");
            }
            else
            {
                //Debug.Log("AB是一个点，CD是一个点，且AC不同！");
            }
            return 0;
        }

        if (Mathf.Abs(b.z - a.z) + Mathf.Abs(b.x - a.x) == 0)
        {
            if ((a.x - d.x) * (c.z - d.z) - (a.z - d.z) * (c.x - d.x) == 0)
            {
                //Debug.Log("A、B是一个点，且在CD线段上！");
            }
            else
            {
                //Debug.Log("A、B是一个点，且不在CD线段上！");
            }
            return 0;
        }
        if (Mathf.Abs(d.z - c.z) + Mathf.Abs(d.x - c.x) == 0)
        {
            if ((d.x - b.x) * (a.z - b.z) - (d.z - b.z) * (a.x - b.x) == 0)
            {
                //Debug.Log("C、D是一个点，且在AB线段上！");
            }
            else
            {
                //Debug.Log("C、D是一个点，且不在AB线段上！");
            }
            return 0;
        }

        if ((b.z - a.z) * (c.x - d.x) - (b.x - a.x) * (c.z - d.z) == 0)
        {
            //Debug.Log("线段平行，无交点！");
            return 0;
        }

        contractPoint.x = ((b.x - a.x) * (c.x - d.x) * (c.z - a.z) -
                c.x * (b.x - a.x) * (c.z - d.z) + a.x * (b.z - a.z) * (c.x - d.x)) /
                ((b.z - a.z) * (c.x - d.x) - (b.x - a.x) * (c.z - d.z));
        contractPoint.z = ((b.z - a.z) * (c.z - d.z) * (c.x - a.x) - c.z
                * (b.z - a.z) * (c.x - d.x) + a.z * (b.x - a.x) * (c.z - d.z))
                / ((b.x - a.x) * (c.z - d.z) - (b.z - a.z) * (c.x - d.x));

        if ((contractPoint.x - a.x) * (contractPoint.x - b.x) <= 0
                && (contractPoint.x - c.x) * (contractPoint.x - d.x) <= 0
                && (contractPoint.z - a.z) * (contractPoint.z - b.z) <= 0
                && (contractPoint.z - c.z) * (contractPoint.z - d.z) <= 0)
        {

            //Debug.Log("线段相交于点(" + contractPoint.x + "," + contractPoint.z + ")！");
            return 1; // '相交  
        }
        else
        {
            //Debug.Log("线段相交于虚交点(" + contractPoint.x + "," + contractPoint.z + ")！");
            return -1; // '相交但不在线段上  
        }
    }
    
}
