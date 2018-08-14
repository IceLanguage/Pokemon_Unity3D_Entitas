using Newtonsoft.Json;
using PokemonBattele;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public partial class ResourceController : SingletonMonobehavior<ResourceController>
{
    public ParticleSystem PokemonShowParticle;
    public GameObject glassPrefab;

    public Dictionary<int, Skill> allSkillDic = new Dictionary<int, Skill>();
    public Dictionary<int, Race> allRaceDic = new Dictionary<int, Race>();
    public Dictionary<NatureType, Nature> allNatureDic = new Dictionary<NatureType, Nature>();
    public Dictionary<int, Ability> allAbilityDic = new Dictionary<int, Ability>();
    public Dictionary<int, int> allCtachRateDic = new Dictionary<int, int>();
  
    public List<int> CanUsePokemonList = new List<int>(100);
    public SortedDictionary<int, SkillPool> PokemonSkillPoolDic = new SortedDictionary<int, SkillPool>();
    public Dictionary<string, Item> ItemDic = new Dictionary<string, Item>(300);
    public float[,] TypeInf = new float[19, 19];
    public EncounterPokemon glassPokemons;
    public Dictionary<string, UseBagItem> UseBagUItemDict = new Dictionary<string, UseBagItem>(300);
    private const string skillAssetPath = "SkillAsset/";
    private const string skillPoolPath = "SkillPoolConfig/";

    public static string WWW_STREAM_ASSET_PATH;



    private const string Pokemondatapath = "Data/pokemon/";
    private const string Datapath = "Data/trainer/";

    public bool LOADITEM, LOADSKILL, LOADSKILLPOOL;
    public string UserName
    {
        get
        {
            return PlayerPrefs.GetString("AccountName");
        }
    }
    public StringBuilder TrainerDataPath
    {
        get
        {
            if (null == UserName) return null;
            StringBuilder sb = new StringBuilder(100);
            return sb.AppendFormat("{0}/{1}Play.json",
                Application.persistentDataPath, Datapath);
        }
    }
    


    private void Start()
    {
        WWW_STREAM_ASSET_PATH =
#if UNITY_ANDROID&&!UNITY_EDITOR
            Application.streamingAssetsPath;      // 路径与上面不同，安卓直接用这个
#else
            "file://" + Application.streamingAssetsPath;  // 反而其他平台加file://
#endif
        StartCoroutine(LoadPokemonSkillPools());
        StartCoroutine(LoadItemData());
        StartCoroutine(LoadPokemonSkillsData());

        LoadPokemonAbilitysData();
        LoadPokemonNaturesData();
        LoadPokemonRacesData();
        LoadPokemonTypeInf();
        LoadCatachRateData();    
        LoadCanUsePokemonList();
    }


    IEnumerator LoadItemData()
    {
        using (WWW www = new WWW(WWW_STREAM_ASSET_PATH + "/bagitems"))

        {
            yield return www;
            string[] names = www.assetBundle.GetAllAssetNames();
            foreach (string name in names)
            {
                AssetBundleRequest request = www.assetBundle.LoadAssetAsync<Item>(name);
                yield return request;
                Item item = request.asset as Item;
                if (null != item)
                    ItemDic[item.Name] = item; 

            }
            DebugHelper.Log("背包物品数据已加载");
            LOADITEM = true;
            AssetBundle.UnloadAllAssetBundles(false);
        }
    }

    IEnumerator LoadPokemonSkillsData()
    {
        using (WWW www = new WWW(WWW_STREAM_ASSET_PATH + "/skills"))

        {
            yield return www;
            string[] names = www.assetBundle.GetAllAssetNames();

            foreach (string name in names)
            {
                AssetBundleRequest request = www.assetBundle.LoadAssetAsync<Skill>(name);
                yield return request;
                Skill skill = request.asset as Skill;
                if (null != skill)
                    allSkillDic[skill.SKillID] = skill;
                

            }
            LOADSKILL = true;
            DebugHelper.Log("技能数据已加载");
            AssetBundle.UnloadAllAssetBundles(false);
        }
    }
    IEnumerator LoadPokemonSkillPools()
    {
        using (WWW www = new WWW(WWW_STREAM_ASSET_PATH + "/pokemonskillpools"))

        {
            yield return www;
            string[] names = www.assetBundle.GetAllAssetNames();
            foreach (string name in names)
            {
                AssetBundleRequest request = www.assetBundle.LoadAssetAsync<SkillPool>(name);
                yield return request;
                SkillPool skillPool = request.asset as SkillPool;
                if (null != skillPool)
                    PokemonSkillPoolDic[skillPool.PokemonID] = skillPool;

            }
            DebugHelper.Log("技能池数据已加载");
            LOADSKILLPOOL = true;
            AssetBundle.UnloadAllAssetBundles(false);
           
        }
    }


    public void LoadCanUsePokemonList()
    {
        try
        {

            TextAsset t = Resources.Load<TextAsset>("Config/AllPokemons");
            Resources.UnloadUnusedAssets();
            string json = t.text;

            CanUsePokemonList = JsonConvert.DeserializeObject<List<int>>(json);
            CanUsePokemonList.TrimExcess();
        }
        catch (Exception e)
        {
            Debug.Log("可使用精灵列表数据读取异常" + e.Message);

        }
    }


    public void LoadCatachRateData()
    {
        try
        {

            TextAsset t = Resources.Load<TextAsset>("ReadTxt/CatchRates");
            Resources.UnloadUnusedAssets();
            string json = t.text;

            allCtachRateDic = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);
          
        }
        catch (Exception e)
        {
            Debug.Log("精灵捕获率数据读取异常" + e.Message);

        }


        Debug.Log("精灵捕获率数据已加载");
    }

    public void LoadPokemonRacesData()
    {
        try
        {

            TextAsset t = Resources.Load<TextAsset>("ReadTxt/PokemonRaces");
            Resources.UnloadUnusedAssets();
            string json = t.text;

            allRaceDic = JsonConvert.DeserializeObject<Dictionary<int, Race>>(json);
        }
        catch (Exception e)
        {
            Debug.Log("精灵种族数据读取异常" + e.Message);

        }


        Debug.Log("精灵种族数据已加载");

    }




    public void LoadPokemonNaturesData()
    {
        try
        {

            TextAsset t = Resources.Load<TextAsset>("ReadTxt/PokemonNatures");
            Resources.UnloadUnusedAssets();
            string json = t.text;

            allNatureDic = JsonConvert.DeserializeObject<Dictionary<NatureType, Nature>>(json);
        }
        catch (Exception e)
        {
            Debug.Log("精灵性格数据读取异常" + e.Message);
        }

        Debug.Log("精灵性格数据已加载");
    }

    public void LoadPokemonAbilitysData()
    {
        try
        {

            TextAsset t = Resources.Load<TextAsset>("ReadTxt/PokemonAbilitys");
            Resources.UnloadUnusedAssets();
            string json = t.text;

            allAbilityDic = JsonConvert.DeserializeObject<Dictionary<int, Ability>>(json);
        }
        catch (Exception e)
        {
            Debug.Log("精灵特性数据读取异常" + e.Message);
        }


        Debug.Log("精灵特性数据已加载");
    }



    public void LoadPokemonTypeInf()
    {
        try
        {

            TextAsset t = Resources.Load<TextAsset>("ReadTxt/PokemonTypeInfos");
            Resources.UnloadUnusedAssets();
            string json = t.text;

            TypeInf = JsonConvert.DeserializeObject<float[,]>(json);
        }
        catch (Exception e)
        {
            Debug.Log("属性克制数据读取异常" + e.Message);
        }
        Debug.Log("属性克制数据已加载");
    }

    

    /// <summary>
    /// 获得属性克制影响
    /// </summary>
    /// <param name="attack">攻击方</param>
    /// <param name="defence">防御方</param>
    /// <returns></returns>
    public float GetTypeInfo(int attack, int defence)
    {
        if (attack == 0 || defence == 0) return 1;
        return TypeInf[attack, defence];
    }
}
