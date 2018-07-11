using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Text;

public class GetCanUsePokemonJson 
{
    private const string path = "Assets/Resources/Config/AllPokemons.json";

    [MenuItem("Pokemon/获得加入的精灵列表")]
    public static void Get()
    {
        var str = JsonConvert.SerializeObject(
            SearchPokemon()
            .Where(x =>""!=x)
            .ToList()
            .ConvertAll<int>(x=>Convert.ToInt32(x)));
        File.WriteAllText(path, str, Encoding.UTF8);
    }

    private static List<int> PokemonsRaceIDList = new List<int>();
    private static List<string> SearchPokemon(string path = "Assets/Resources/PokemonPrefab")
    {
        //    string[] guids = AssetDatabase.FindAssets("t:GameObject", new string[] { path });
        //    //从GUID获得资源所在路径
        //    List<string> paths = new List<string>();
        //    guids.ToList().ForEach(m =>
        //    paths.Add(
        //         Regex.Replace(AssetDatabase.GUIDToAssetPath(m), @"[^\d]*", "")));

        var paths = SearchGameObject.SearchPathList<GameObject>(path);
        paths = paths.ConvertAll(x =>Regex.Replace(x, @"[^\d]*", ""));
        return paths;

    }
}