using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class GrassPosConfigEditor:MonoBehaviour
{
    [HideInInspector]
    public static List<Vector3> pointList = new List<Vector3>();

    public LayerMask ground_layerMask = 0;
    public static Material GrassMaterial;
    public const string GrassPosConfigPath = "Assets/Resources/Config/GrassesPos.json";
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "grass.png");

        int count = pointList.Count;


        Handles.color = Color.red;
        //GUIStyle style = new GUIStyle();

        for (int i = 0; i < count; i++)
        {
            if (i == count - 1)
            {
                Handles.Label(pointList[i], "End");
            }
            else
                Handles.Label(pointList[i], "N");
            if (count < 2) return;
            if (i == count - 1)
            {
                Handles.color = Color.blue;
            }

            Handles.DrawLine(pointList[i], pointList[(i + 1) % count]);

        }
    }

    public void AddControlPoint(Vector3 pos)
    {

        int count = pointList.Count;
        if (pointList.Count != 0 && Vector3.Distance(pointList[count - 1], pos) < 1)
        {
            return;
        }
        RevisePoint(ref pos);
        pointList.Insert(count, pos);

    }

    /// <summary>
    /// x,z坐标取整
    /// </summary>
    /// <param name="pos"></param>
    private void RevisePoint(ref Vector3 pos)
    {

        pos.x = Mathf.Round(pos.x);
        pos.z = Mathf.Round(pos.z);
    }

    [MenuItem("CONTEXT/GrassPosConfigEditor/Save Grass Config")]
    public static void BuildGrassLand()
    {

#if UNITY_EDITOR
        if (null == GrassMaterial)
            GrassMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaders/Grasses/Mat/GrassMat.mat");

        if (pointList.Count < 2)
        {

            EditorUtility.DisplayDialog("提示", "你还没有画好草地的范围", "ok");

        }
        List<Vector3> allPoint = AllPointIn(pointList);
        TextAsset t = Resources.Load<TextAsset>("Config/GrassesPos");
        string json = "";
        if (null != t )
        {
            json = t.text;
        }
       
        GrassPosConfig grassPosConfig=  JsonConvert.DeserializeObject<GrassPosConfig>(json);
        
        if(null == grassPosConfig )
        {
            grassPosConfig = ScriptableObject.CreateInstance<GrassPosConfig>();
        }
        grassPosConfig.AddGrassPos(allPoint);


        json =JsonConvert.SerializeObject(grassPosConfig);
        File.WriteAllText(GrassPosConfigPath, json, Encoding.UTF8);

        pointList.Clear();

#endif
    }


    /// <summary>
    /// 获取多边形内符合草地生成条件的点
    /// </summary>
    /// <param name="curlist"></param>
    /// <returns></returns>
    private static List<Vector3> AllPointIn(List<Vector3> curlist)
    {
        List<Vector3> res = new List<Vector3>();
        float minz = float.MaxValue, minx = float.MaxValue;
        float maxz = float.MinValue, maxx = float.MinValue;
        foreach (Vector3 v in curlist)
        {
            if (v.x < minx) minx = v.x;
            if (v.x > maxx) maxx = v.x;
            if (v.z < minz) minz = v.z;
            if (v.z > maxz) maxz = v.z;
        }
        if (minx == float.MaxValue || minz == float.MaxValue) return res;
        if (maxx == float.MinValue || maxz == float.MinValue) return res;
        for (int i = (int)minx - 10; i < maxx + 10; i++)
        {
            Vector3 min = new Vector3(i, 0, minz - 10);
            Vector3 max = new Vector3(i, 0, maxz + 10);
            List<Vector3> Intersection = GetIntersection(curlist, min, max);
            Intersection.Sort((p1, p2) => p1.z.CompareTo(p2.z));
            bool isIn = false;
            int IntersectionC = Intersection.Count;
            for (int j = 0; j < IntersectionC; j++)
            {
                isIn = !isIn;
                if (isIn && j + 1 < IntersectionC)
                {
                    for (int k = (int)Intersection[j].z; k < Intersection[j + 1].z; k++)
                    {
                        res.Add(new Vector3(i, 0, k));
                    }
                }
            }
        }
        return res;
    }

    /// <summary>
    /// 获取线段和多边形的交点
    /// </summary>
    /// <param name="curlist"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private static List<Vector3> GetIntersection(List<Vector3> curlist, Vector3 min, Vector3 max)
    {
        List<Vector3> res = new List<Vector3>();
        int cout = curlist.Count;
        for (int i = 0; i < cout; i++)
        {
            Vector3 a = curlist[i];
            Vector3 b = curlist[(i + 1) % cout];
            Vector3 point;
            if (GeometryDetection.GetIntersection(a, b, min, max, out point) == 1)
            {
                res.Add(point);
            }

        }
        return res;
    }

    public void ChangePointPos(Vector3 pos, int index)
    {
        if (index != -1)
        {
            RevisePoint(ref pos);
            pointList[index] = pos;
        }

    }

    public int GetIndex(Vector3 pos)
    {
        int index = -1;
        float minDis = float.MaxValue;
        for (int i = 0; i < pointList.Count; i++)
        {
            float dis = Vector3.Distance(pos, pointList[i]);
            if (dis < minDis)
            {
                minDis = dis;
                index = i;
            }
        }
        return index;
    }
    public void DeleteActiveControlPoint(Vector3 pos)
    {
        int index = GetIndex(pos);
        if (index != -1)
        {
            pointList.RemoveAt(index);
        }

    }

    /// <summary>
    /// 修正不正确的画线
    /// </summary>
    public void UpdateLine()
    {
        bool isIntersection = false;//是否相交
        int cout = pointList.Count;
        for (int i = cout - 1; i >= 0; i--)
        {
            if (isIntersection)
                break;
            for (int j = i - 2; j >= 1; j--)
            {
                Vector3 a = pointList[i];
                Vector3 b = pointList[(i + 1) % cout];
                Vector3 c = pointList[j];
                Vector3 d = pointList[(j + 1) % cout];
                if (GeometryDetection.IsIntersection(a, b, c, d))
                {
                    isIntersection = true;
                    break;
                }
            }

        }
        if (isIntersection)
        {
            pointList.RemoveAt(cout - 1);
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("提示", "你画的线不能构成多边形，请再画一遍好吗", "ok");
#endif
        }

    }
#endif   
}
#if UNITY_EDITOR
[CustomEditor(typeof(GrassPosConfigEditor))]
public class GrassPosConfigCustomEditor : Editor
{
    private Vector3 GetWorldPointFromMouse(LayerMask layerMask)
    {
        float planeLevel = 0;
        var groundPlane = new Plane(Vector3.up, new Vector3(0, planeLevel, 0));

        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit rayHit;
        Vector3 hit = new Vector3(0, 0, 0);
        float dist;

        if (Physics.Raycast(ray, out rayHit, Mathf.Infinity, 1 << layerMask.value))
            hit = rayHit.point;
        else if (groundPlane.Raycast(ray, out dist))
            hit = ray.origin + ray.direction.normalized * dist;

        return hit;
    }
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
#endif

    private void OnSceneGUI()
    {
        Event current = Event.current;
        GrassPosConfigEditor _target = (GrassPosConfigEditor)target;
        if (current.shift && current.type == EventType.MouseDown)
        {
            Undo.RecordObject(_target, "grassedit");
            _target.AddControlPoint(GetWorldPointFromMouse(_target.ground_layerMask));

        }
        if (current.alt && current.type == EventType.MouseDown)
        {
            Undo.RecordObject(_target, "grassedit");
            _target.DeleteActiveControlPoint(GetWorldPointFromMouse(_target.ground_layerMask));

        }

        if (current.alt && current.shift && current.type == EventType.MouseDrag)
        {
            Undo.RecordObject(_target, "grassedit");
            int index = _target.GetIndex(GetWorldPointFromMouse(_target.ground_layerMask));
            _target.ChangePointPos(GetWorldPointFromMouse(_target.ground_layerMask), index);

        }

        if (!current.alt && !current.shift)
        {
            _target.UpdateLine();

        }
    }


}
#endif