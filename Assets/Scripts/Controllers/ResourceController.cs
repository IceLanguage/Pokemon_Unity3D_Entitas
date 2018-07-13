using Newtonsoft.Json;
using PokemonBattelePokemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public partial class ResourceController : SingletonMonobehavior<ResourceController>
{
    public Material grassMaterial;
    public Material DefaultMaterial;
    public ParticleSystem PokemonShowParticle;

    public Dictionary<int, Race> allRaceDic = new Dictionary<int, Race>();
    public Dictionary<NatureType, Nature> allNatureDic = new Dictionary<NatureType, Nature>();
    public Dictionary<int, Ability> allAbilityDic = new Dictionary<int, Ability>();
    public Dictionary<int, int> allCtachRateDic = new Dictionary<int, int>();
    public Dictionary<int, Action> allBagItemEffectDic = new Dictionary<int, Action>();
    public Dictionary<int, Item> allItemDic = new Dictionary<int, Item>();
    public List<int> CanUsePokemonList = new List<int>();
    public float[,] TypeInf = new float[19, 19];
    public EncounterPokemon glassPokemons;
    private const string skillAssetPath = "SkillAsset/";
    private const string skillPoolPath = "SkillPoolConfig/";



    private const string Pokemondatapath = "Data/pokemon/";
    private const string Datapath = "Data/trainer/";
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
       
        LoadPokemonAbilitysData();
        LoadPokemonNaturesData();
        LoadPokemonRacesData();
        LoadPokemonTypeInf();
        LoadCatachRateData();
        LoadBagItemData();      
        LoadCanUsePokemonList();

    }

   

   

    public void LoadCanUsePokemonList()
    {
        try
        {

            TextAsset t = Resources.Load<TextAsset>("Config/AllPokemons");
            Resources.UnloadUnusedAssets();
            string json = t.text;

            CanUsePokemonList = JsonConvert.DeserializeObject<List<int>>(json);
        }
        catch (Exception e)
        {
            Debug.Log("可使用精灵列表数据读取异常" + e.Message);

        }
    }

    public void LoadBagItemData()
    {
        try
        {

            TextAsset t = Resources.Load<TextAsset>("ReadTxt/BagItems");
            Resources.UnloadUnusedAssets();
            string json = t.text;

            allItemDic = JsonConvert.DeserializeObject<Dictionary<int, Item>>(json);
        }
        catch (Exception e)
        {
            Debug.Log("道具数据读取异常" + e.Message);

        }

        allItemDic[124].Sprite = "BagItem/PokemonBall";
        Debug.Log("道具数据已加载");
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
