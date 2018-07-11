using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

public static class SearchGameObject
{
    public static List<T> SearchGameObjectList<T>(string path)
        where T:class
    {

        List<string> paths = SearchPathList<T>(path);

        //从路径获得该资源
        List<T> objs = new List<T>();
        paths.ForEach(p => objs.Add(AssetDatabase.LoadAssetAtPath(p,
            typeof(T)) as T));
        return objs;
    }

    public static List<string> SearchPathList<T>(string path)
    {
        //查找指定路径下指定类型的所有资源，返回的是资源GUID
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).ToString(), new string[] { path });
        //从GUID获得资源所在路径
        List<string> paths = new List<string>();
        guids.ToList().ForEach(m => paths.Add(AssetDatabase.GUIDToAssetPath(m)));
        return paths;
    }


}
