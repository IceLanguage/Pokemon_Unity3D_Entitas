using UnityEngine;
using System.Collections.Generic;
using System;
[Serializable]
public class GrassPosConfig : ScriptableObject
{
    public List<float> GrassPosxList = new List<float>();
    public List<float> GrassPosyList = new List<float>();
    public List<float> GrassPoszList = new List<float>();

    public List<Vector3> GetGrassPos()
    {
       
        List<Vector3> GrassPosList = new List<Vector3>();
        int num = Mathf.Min(GrassPosxList.Count, GrassPosyList.Count, GrassPoszList.Count);
        for(int i=0;i<num;i++)
        {
            GrassPosList.Add(new Vector3(GrassPosxList[i], GrassPosyList[i], GrassPoszList[i]));
        }
        return GrassPosList;
    }
    private void SavePos(List<Vector3> GrassPosList)
    {
        GrassPosxList.Clear();
        GrassPosyList.Clear();
        GrassPoszList.Clear();
        var enumerator = GrassPosList.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                var v = enumerator.Current;
                GrassPosxList.Add(v.x);
                GrassPosyList.Add(v.y);
                GrassPoszList.Add(v.z);
            }
        }
        finally
        {
            enumerator.Dispose();
        }
        
    }
    public void AddGlassPos(List<Vector3> PosList)
    {
        List<Vector3> GrassPosList = GetGrassPos();
        for (int i = 0; i < PosList.Count; i++)
        {
            Vector3 pos = PosList[i];

            if (null == GrassPosList || 0 == GrassPosList.Count )
            {
                GrassPosList = new List<Vector3>();
                GrassPosList.Add(pos);
            }
            else if (GrassPosList.Count > 0 && !GrassPosList.Contains(pos))
            {
                GrassPosList.Add(pos);
            }
        }
        SavePos(GrassPosList);


    }
}