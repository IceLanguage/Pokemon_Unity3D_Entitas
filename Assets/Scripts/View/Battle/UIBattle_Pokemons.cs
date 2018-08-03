using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using MyUnityEventDispatcher;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using PokemonBattele;

public class UIBattle_Pokemons : TTUIPage
{
    public UIBattle_Pokemons() : base(UIType.Normal, UIMode.DoNothing, UICollider.WithBg)
    {
        uiPath = "UIPrefab/UIBattle_Pokemons";
    }

    private Button closeButton;
    private RectTransform thisrect;

    private List<GameObject> PokemonUIGameObjects = new List<GameObject>(6);
    private List<Image> PokemonIcon_list = new List<Image>(6);
    private List<Text> PokemonName_list = new List<Text>(6);
    private List<Slider> PokemonHealth_list = new List<Slider>(6);
    private List<Text> PokemonHealthText_list = new List<Text>(6);
    private List<Text> PhysicPowerText_list = new List<Text>(6);
    private List<Text> PhysicDefenceText_list = new List<Text>(6);
    private List<Text> EnergyPowerText_list = new List<Text>(6);
    private List<Text> EnergyDefenceText_list = new List<Text>(6);
    private List<Text> SpeedText_list = new List<Text>(6);
    private List<Text> RaceNameText_list = new List<Text>(6);
    private List<Text> PokemonTypeText_list = new List<Text>(6);
    private List<Text> NatureText_list = new List<Text>(6);
    private List<Text> AbilityText_list = new List<Text>(6);
    private List<Text> PropsText_list = new List<Text>(6);
    private List<Image> BattleIcon_list = new List<Image>(6);
    private List<Button> CallPokemonButton_list = new List<Button>(6);
    private List<Image> AbnormalStateEnum_list = new List<Image>(6);

    private readonly RectTransform[,] Skill1rect_list = new RectTransform[6, 4];
    private readonly Text[,] SkillPokemonTypeText_list = new Text[6, 4];
    private readonly Text[,] SkillNameText_list = new Text[6, 4];
    private readonly Text[,] SkillPPText_list = new Text[6, 4];

    private readonly List<BattlePokemonData> prePokemonDatas =
        new List<BattlePokemonData>() { null, null, null, null, null, null };

    private GameContext context;
    public override void Awake(GameObject go)
    {
        context = Contexts.sharedInstance.game;
        InitUnityComponents();
        Refresh();

    }

    public override void Hide()
    {

        thisrect.DOAnchorPosY(2000, 0.01f);
    }
    public override void Active()
    {
        thisrect.DOAnchorPosY(-2.1f, 0.01f);

    }

    public override void Refresh()
    {
        Show_UIBattle_Pokemons();
        Resources.UnloadUnusedAssets();
    }

    private void CallPokemon(int index)
    {
        Hide();
        NotificationCenter<int>.Get().DispatchEvent("ExchangePokemon", index);
    }
    private void InitUnityComponents()
    {
        if (null == closeButton)
        {
            closeButton = transform.Find("Close").GetComponent<Button>();
            if (closeButton.onClick.GetPersistentEventCount() > 0)
                closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePage<UIBattle_Pokemons>);
        }
        thisrect = transform.GetComponent<RectTransform>();
        PokemonIcon_list.Clear();
        PokemonName_list.Clear();
        PokemonHealth_list.Clear();
        PokemonHealthText_list.Clear();
        PhysicPowerText_list.Clear();
        PhysicDefenceText_list.Clear();
        EnergyPowerText_list.Clear();
        EnergyDefenceText_list.Clear();
        SpeedText_list.Clear();
        RaceNameText_list.Clear();
        PokemonTypeText_list.Clear();
        NatureText_list.Clear();
        AbilityText_list.Clear();
        PropsText_list.Clear();
        
        for (int i=1;i<=6;i++)
        {
            PokemonUIGameObjects.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i).ToString()).gameObject);
            PokemonIcon_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/Icon").ToString()).GetComponent<Image>());

            CallPokemonButton_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/Icon").ToString()).GetComponent<Button>());

            int k = i - 1;
            CallPokemonButton_list[i - 1].onClick.AddListener
            (
                () => CallPokemon(k)
            );

            PokemonName_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/Name").ToString()).GetComponent<Text>());
            PokemonHealth_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/Blood").ToString()).GetComponent<Slider>());
            PokemonHealthText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/Blood/Health").ToString()).GetComponent<Text>());
            PhysicPowerText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/PhysicPower/Text").ToString()).GetComponent<Text>());
            PhysicDefenceText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/PhysicDefence/Text ").ToString()).GetComponent<Text>());
            EnergyPowerText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/EnergyPower/Text ").ToString()).GetComponent<Text>());
            EnergyDefenceText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/EnergyDefence/Text").ToString()).GetComponent<Text>());
            SpeedText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/Speed/Text ").ToString()).GetComponent<Text>());
            RaceNameText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i).Append("/Race/Name").ToString()).GetComponent<Text>());
            PokemonTypeText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/Race/Type").ToString()).GetComponent<Text>());
            NatureText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/Race/Nature").ToString()).GetComponent<Text>());
            AbilityText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/Race/Ability").ToString()).GetComponent<Text>());
            PropsText_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/Props/Name").ToString()).GetComponent<Text>());

            BattleIcon_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/BattleIcon").ToString()).GetComponent<Image>());
            HideUI(BattleIcon_list[i-1].gameObject);

            AbnormalStateEnum_list.Add(transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}", i)
                .Append("/AbnormalState").ToString()).GetComponent<Image>());
            HideUI(AbnormalStateEnum_list[i - 1].gameObject);

            for (int j = 1; j <= 4; j++)
            {

                Skill1rect_list[i - 1, j - 1] = transform.Find(
                new StringBuilder(20)
                .AppendFormat("Pokemon{0}/Skill{1}", i, j).ToString()).GetComponent<RectTransform>();
                SkillPokemonTypeText_list[i - 1, j - 1] = transform.Find(
                    new StringBuilder(20)
                .AppendFormat("Pokemon{0}/Skill{1}", i, j)
                .Append("/Att").ToString()).GetComponent<Text>();
                SkillNameText_list[i - 1, j - 1] = transform.Find(
                    new StringBuilder(20)
                .AppendFormat("Pokemon{0}/Skill{1}", i, j)
                .Append("/Text").ToString()).GetComponent<Text>();
                SkillPPText_list[i - 1, j - 1] = transform.Find(
                    new StringBuilder(20)
                .AppendFormat("Pokemon{0}/Skill{1}", i, j)
                .Append("/PP").ToString()).GetComponent<Text>();
            }
        }
        
    }

    private void Show_UIBattle_Pokemons()
    {

        var pokemons = BattleController.Instance.playPokemons;
        int i;
        for (i = 0; i < pokemons.Count; i++)
        {
            BattlePokemonData pokemonData = pokemons[i];
            
            if(prePokemonDatas[i]!=pokemonData)
            {
                if (null != prePokemonDatas[i])
                {
                    var Preentity = prePokemonDatas[i].entity;
                    var Preaction = Preentity.pokemonDataChangeEvent.Event;
                    Preaction -= Refresh;
                    Preentity.ReplacePokemonDataChangeEvent(Preaction);
                }

                prePokemonDatas[i] = pokemonData;
                var entity = pokemonData.entity;
                var action = entity.pokemonDataChangeEvent.Event;
                action += Refresh;
                entity.ReplacePokemonDataChangeEvent(action);
            }

            var race = pokemonData.race;
            int pokemonID = race.raceid;
            PokemonIcon_list[i].sprite = Resources.Load<Sprite>("PokemonTexture/" + pokemonID.ToString());
            RaceNameText_list[i].text = race.sname;

            PokemonName_list[i].text = pokemonData.Ename;

            int curHealth = pokemonData.curHealth;
            int fullHealth = pokemonData.Health;
            PokemonHealthText_list[i].text =
                 new StringBuilder(20)
                    .AppendFormat("{0}a{1}", curHealth, fullHealth).ToString();
            PokemonHealth_list[i].value = curHealth / (float)fullHealth;


            PhysicPowerText_list[i].text = pokemonData.PhysicPower.ToString();
            PhysicDefenceText_list[i].text = pokemonData.PhysicDefence.ToString();
            EnergyPowerText_list[i].text = pokemonData.EnergyPower.ToString();
            EnergyDefenceText_list[i].text = pokemonData.EnergyDefence.ToString();
            SpeedText_list[i].text = pokemonData.Speed.ToString();

            var type1 = pokemonData.MainPokemonType;
            var type2 = pokemonData.SecondPokemonType;
            PokemonTypeText_list[i].text = type1.ToString()
                + type2.ToString();


            NatureText_list[i].text = pokemonData.nature.natureType.ToString();
            AbilityText_list[i].text = pokemonData.ShowAbility.ToString();


            List<int> skillIDs = pokemonData.skills;
            List<int> skillPPs = pokemonData.skillPPs;
            int j = 0;
            for (; j < skillIDs.Count; ++j)
            {
                id = skillIDs[j];
                Skill skill = ResourceController.Instance.allSkillDic[id];
                SkillPokemonTypeText_list[i, j].text = skill.att.ToString();
                SkillNameText_list[i, j].text = skill.sname;
                StringBuilder sb = new StringBuilder(20);
                sb.AppendFormat("{0}a{1}", skillPPs[j], skill.FullPP);
                SkillPPText_list[i, j].text = sb.ToString();
                ShowUI(Skill1rect_list[i, j].gameObject);
            }
            while (j < 4)
            {
                HideUI(Skill1rect_list[i, j].gameObject);
                ++j;
            }
            if (AbnormalStateEnum.Normal == pokemonData.Abnormal)
            {
                HideUI(AbnormalStateEnum_list[i].gameObject);
            }
            else
            {
                ShowUI(AbnormalStateEnum_list[i].gameObject);
                if(pokemonData.curHealth>0)
                    AbnormalStateEnum_list[i].sprite =
                    Resources.Load<Sprite>(
                        new StringBuilder(20)
                            .AppendFormat("UIPrefab/AbnormalStateEnum/{0}",
                            pokemonData.Abnormal).ToString());
                else
                    AbnormalStateEnum_list[i].sprite =
                    Resources.Load<Sprite>(
                        new StringBuilder(20)
                            .AppendFormat("UIPrefab/AbnormalStateEnum/{0}",
                            "death").ToString());
            }

            ShowUI(PokemonUIGameObjects[i]);


        }
        for (; i < 6; i++)
        {
            prePokemonDatas[i] = null;
            HideUI(PokemonUIGameObjects[i]);
        }
    }

    

}
