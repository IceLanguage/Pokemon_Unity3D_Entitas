using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Text;
using PokemonBattele;

public class GetPokemonJson 
{

    [MenuItem("Pokemon/获得加入的精灵列表")]
    public static void GetCanUsePokemon()
    {
        const string path = "Assets/Resources/Config/AllPokemons.json";
        var list = SearchPokemon()
            .Where(x => "" != x)
            .Distinct()
            .ToList()
            .ConvertAll(x => Convert.ToInt32(x));
        var str = JsonConvert.SerializeObject(list);
        var SkillPools = SearchGameObject.SearchGameObjectList<SkillPool>("Assets/Skill/SkillPoolAsset")
            .ConvertAll(x=>x.PokemonID);
        foreach(int id in list)
        {
            if (SkillPools.Contains(id)) continue;
            SkillPool skillPool = ScriptableObject.CreateInstance<SkillPool>();
            skillPool.PokemonID = id;
            AssetDatabase.CreateAsset(skillPool, "Assets/Skill/SkillPoolAsset/" + id + ".asset");
        }
        File.WriteAllText(path, str, Encoding.UTF8);
    }

    [MenuItem("Pokemon/获得道具列表")]
    public static void GetBagItem()
    {
        const string path = "Assets/Resources/ReadTxt/BagItem.json";
        var list = SearchGameObject.SearchGameObjectList<Item>("Assets/BagItem");
        List<string> nameList = list.Select(x => x.Name).ToList();
        var list2 = SearchGameObject.SearchGameObjectList<Item>("Assets/Resources/BagItem/BagItemAsset");
        int i = 0;
        foreach (Item e in list)
        {
            EditorUtility.DisplayCancelableProgressBar("复制道具数据", i + "/" + list.Count, (float)i / list.Count);
            Item other = list2.Find(x => x.Name == e.Name);
            if (e.Equals(other)) continue;
            AssetDatabase.CreateAsset(e.Clone(), "Assets/Resources/BagItem/BagItemAsset/" + e.Name+ ".asset");
        }
        EditorUtility.ClearProgressBar();
        var str = JsonConvert.SerializeObject(nameList);
        File.WriteAllText(path, str, Encoding.UTF8);
    }

    private static List<string> SearchPokemon(string path = "Assets/Resources/PokemonPrefab")
    {
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new string[] { path });
        //从GUID获得资源所在路径
        List<string> paths = new List<string>();
        guids.ToList().ForEach(m =>
        paths.Add(
             Regex.Replace(AssetDatabase.GUIDToAssetPath(m), @"[^\d]*", "")));

        return paths;

    }
}