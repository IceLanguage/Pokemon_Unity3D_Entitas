using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyTeam.UI;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using PokemonBattele;

public class UI_PokemonBag : TTUIPage
{
    public UI_PokemonBag() : base(UIType.Normal, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UI_PokemonBag";
    }
    private RectTransform thisrect;
    private readonly Image[] PokemonIcons = new Image[6];
    private readonly Button[] PokemonChoseButton = new Button[6];
    private readonly Image[] BorderImage = new Image[6];
    private Button CloseButton;
    private Text P_physicPower_text,P_physicDefence_text,P_energyPower_text,P_energyDefence_text,P_speed_text;
    private Text Race_physicPower_text,Race_physicDefence_text,Race_energyPower_text,Race_energyDefence_text,Race_speed_text;
    private Text IV_physicPower_text, IV_physicDefence_text, IV_energyPower_text, IV_energyDefence_text, IV_speed_text;
    private Text Basestats_physicPower_text, Basestats_physicDefence_text, Basestats_energyPower_text,
        Basestats_energyDefence_text, Basestats_speed_text;
    private Text Race_Name_text, Race_PokemonType_text, Race_Nature_text, Race_Ability_text;
    private Text HealthText;
    private InputField NameText;
    private readonly Text[] SkillPokemonType = new Text[4];
    private readonly Text[] SkillName = new Text[4];
    private readonly Text[] SkillPP = new Text[4];
    private readonly Text[] SkillPower = new Text[4];
    private readonly Text[] SkillHitRate = new Text[4];
    private readonly GameObject[] SkillsGo = new GameObject[4];
    private Button SeeRaceButton;
    private Button ChangeSkillsButton;
    private Button ChangePropButton;
    private Button SaveChangeButton;
    private Text propText;
    private Image propImage;
    private Button Basestats_physicPower_Add_button, Basestats_physicDefence_Add_button,
        Basestats_energyPower_Add_button, Basestats_energyDefence_Add_button, Basestats_speed_Add_button;
    private Button Basestats_physicPower_Clear_button, Basestats_physicDefence_Clear_button,
        Basestats_energyPower_Clear_button, Basestats_energyDefence_Clear_button, Basestats_speed_Clear_button;

    private int ChosePokemonIndex = 0;
    private Basestats basestats;
    private Trainer trainerData;
    private BattlePokemonData pokemon;
    public override void Awake(GameObject go)
    {
        InitUnityComponents();
        trainerData = Contexts.sharedInstance.game.playerData.scriptableObject;
    }
    public override void Active()
    {
        ClosePage<UIKnapscakPanel>();
        thisrect.DOAnchorPosX(0, 0.01f);
    }
    public override void Hide()
    {
        thisrect.DOAnchorPosX(-1000, 0.01f);
    }
    private void InitUnityComponents()
    {
        thisrect = gameObject.GetComponent<RectTransform>();

        for(int i=0;i<6;i++)
        {
            PokemonIcons[i] = transform.Find("IconsBg/PokemonIcon"+(i+1).ToString()+"/Icon").GetComponent<Image>();
            BorderImage[i] = transform.Find("IconsBg/PokemonIcon" + (i + 1).ToString()).GetComponent<Image>();
            PokemonChoseButton[i] = transform.Find("IconsBg/PokemonIcon" + (i + 1).ToString()).GetComponent<Button>();

            //lambda会新建一个类存储捕获的变量引用，这样做是为保证每次引用的i不同，没有这一行，i都为6
            var j = i;

            PokemonChoseButton[i].onClick.AddListener(() => ChoosePokemon(j));
        }

        CloseButton = transform.Find("Close").GetComponent<Button>();
        CloseButton.onClick.AddListener(Hide);

        P_physicPower_text = transform.Find("Power/PhysicPower/Text").GetComponent<Text>();
        P_physicDefence_text = transform.Find("Power/PhysicDefence/Text").GetComponent<Text>();
        P_energyPower_text = transform.Find("Power/EnergyPower/Text").GetComponent<Text>();
        P_energyDefence_text = transform.Find("Power/EnergyDefence/Text").GetComponent<Text>();
        P_speed_text = transform.Find("Power/Speed/Text").GetComponent<Text>();

        Race_physicPower_text = transform.Find("Race/PhysicPower").GetComponent<Text>();
        Race_physicDefence_text = transform.Find("Race/PhysicDefence").GetComponent<Text>();
        Race_energyPower_text = transform.Find("Race/EnergyPower").GetComponent<Text>();
        Race_energyDefence_text = transform.Find("Race/EnergyDefence").GetComponent<Text>();
        Race_speed_text = transform.Find("Race/Speed").GetComponent<Text>();

        IV_physicPower_text = transform.Find("IV/PhysicPower").GetComponent<Text>();
        IV_physicDefence_text = transform.Find("IV/PhysicDefence").GetComponent<Text>();
        IV_energyPower_text = transform.Find("IV/EnergyPower").GetComponent<Text>();
        IV_energyDefence_text = transform.Find("IV/EnergyDefence").GetComponent<Text>();
        IV_speed_text = transform.Find("IV/Speed").GetComponent<Text>();

        

        Basestats_physicPower_text = transform.Find("Basestats/PhysicPower").GetComponent<Text>();
        Basestats_physicDefence_text = transform.Find("Basestats/PhysicDefence").GetComponent<Text>();
        Basestats_energyPower_text = transform.Find("Basestats/EnergyPower").GetComponent<Text>();
        Basestats_energyDefence_text = transform.Find("Basestats/EnergyDefence").GetComponent<Text>();
        Basestats_speed_text = transform.Find("Basestats/Speed").GetComponent<Text>();

        Basestats_physicPower_Add_button = Basestats_physicPower_text.transform.Find("Button").GetComponent<Button>();
        Basestats_physicDefence_Add_button = Basestats_physicDefence_text.transform.Find("Button").GetComponent<Button>();
        Basestats_energyPower_Add_button = Basestats_energyPower_text.transform.Find("Button").GetComponent<Button>();
        Basestats_energyDefence_Add_button = Basestats_energyDefence_text.transform.Find("Button").GetComponent<Button>();
        Basestats_speed_Add_button = Basestats_speed_text.transform.Find("Button").GetComponent<Button>();

        Basestats_physicPower_Add_button.onClick.AddListener(Add_Basestats_physicPower);
        Basestats_physicDefence_Add_button.onClick.AddListener(Add_Basestats_physicDefence);
        Basestats_energyPower_Add_button.onClick.AddListener(Add_Basestats_energyPower);
        Basestats_energyDefence_Add_button.onClick.AddListener(Add_Basestats_energyDefence);
        Basestats_speed_Add_button.onClick.AddListener(Add_Basestats_speed);

        Basestats_physicPower_Clear_button = Basestats_physicPower_text.transform.Find("Button2").GetComponent<Button>();
        Basestats_physicDefence_Clear_button = Basestats_physicDefence_text.transform.Find("Button2").GetComponent<Button>();
        Basestats_energyPower_Clear_button = Basestats_energyPower_text.transform.Find("Button2").GetComponent<Button>();
        Basestats_energyDefence_Clear_button = Basestats_energyDefence_text.transform.Find("Button2").GetComponent<Button>();
        Basestats_speed_Clear_button = Basestats_speed_text.transform.Find("Button2").GetComponent<Button>();

        Basestats_physicPower_Clear_button.onClick.AddListener(Clear_Basestats_physicPower);
        Basestats_physicDefence_Clear_button.onClick.AddListener(Clear_Basestats_physicDefence);
        Basestats_energyPower_Clear_button.onClick.AddListener(Clear_Basestats_energyPower);
        Basestats_energyDefence_Clear_button.onClick.AddListener(Clear_Basestats_energyDefence);
        Basestats_speed_Clear_button.onClick.AddListener(Clear_Basestats_speed);

        Race_Name_text = transform.Find("RaceMessage/Name").GetComponent<Text>();
        Race_PokemonType_text = transform.Find("RaceMessage/Type").GetComponent<Text>();
        Race_Ability_text = transform.Find("RaceMessage/Ability").GetComponent<Text>();
        Race_Nature_text = transform.Find("RaceMessage/Nature").GetComponent<Text>();

        HealthText = transform.Find("Blood/Health").GetComponent<Text>();

        NameText = transform.Find("Name").GetComponent<InputField>();

        Func<string, string, string> path2Str = (x, y) =>
        {
            return new StringBuilder(30).AppendFormat("{0}{1}", x, y).ToString();
        };
        Func<string, string, string,string> path3Str = (x, y, z) =>
        {
            return new StringBuilder(30).AppendFormat("{0}{1}{2}", x, y,z).ToString();
        };
        for (int i=0;i<4;i++)
        {
            SkillsGo[i] = transform.Find(path2Str("Skills/Skill", (i + 1).ToString())).gameObject;

            SkillPokemonType[i] = transform.Find(
                path3Str("Skills/Skill", (i + 1).ToString(), "/Att")).GetComponent<Text>();
            SkillName[i] = transform.Find(path3Str("Skills/Skill", (i + 1).ToString(), "/Text")).GetComponent<Text>();
            SkillPP[i] = transform.Find(path3Str("Skills/Skill", (i + 1).ToString(), "/PP")).GetComponent<Text>();
            SkillPower[i] = transform.Find(path3Str("Skills/Skill", (i + 1).ToString(), "/Power")).GetComponent<Text>();
            SkillHitRate[i] = transform.Find(
                path3Str("Skills/Skill", (i + 1).ToString(), "/HitRate")).GetComponent<Text>();
        }

        SeeRaceButton = transform.Find("SeeRace").GetComponent<Button>();
        ChangeSkillsButton = transform.Find("ChangeSkill").GetComponent<Button>();
        ChangePropButton = transform.Find("ChangeProp").GetComponent<Button>();
        SaveChangeButton = transform.Find("SaveChange").GetComponent<Button>();

        SeeRaceButton.onClick.AddListener(SeeRace);
        ChangeSkillsButton.onClick.AddListener(ChangeSkill);
        ChangePropButton.onClick.AddListener(ChangeProp);
        SaveChangeButton.onClick.AddListener(SaveChanges);

        propText = transform.Find("Props/Name").GetComponent<Text>();
        propImage=transform.Find("Props/Image").GetComponent<Image>();
    }

    public override void Refresh()
    {
        pokemon = BattlePokemonData.Context
            [trainerData.pokemons[ChosePokemonIndex].GetInstanceID()];

        basestats = pokemon.basestats;
        Show_Icons();
        Show_Power();
        Show_Skills();
        Show_Race();
        Show_prop();
        Resources.UnloadUnusedAssets();
    }

    private void Show_Icons()
    {
        int i;
        int pokemonID = 1;
        for (i = 0; i < trainerData.pokemons.Count&&i<6; i++)
        {
            pokemonID = trainerData.pokemons[i].raceID;
            ShowUI(PokemonIcons[i].gameObject);
            if (i == ChosePokemonIndex)
            {
                BorderImage[i].sprite = UIController.Instance.border2;
            }
            else
            {
                BorderImage[i].sprite = UIController.Instance.border1;
            }
            StringBuilder sb = new StringBuilder(30);
            sb.AppendFormat("PokemonTexture/{0}", pokemonID);
            PokemonIcons[i].sprite = Resources.Load<Sprite>(sb.ToString());

        }
        for (; i < 6; i++)
        {
            HideUI(PokemonIcons[i].gameObject);
        }
    }
    /// <summary>
    /// 显示战力数值
    /// </summary>
    private void Show_Power()
    {
        P_physicPower_text.text = pokemon.PhysicPower.ToString();
        P_physicDefence_text.text = pokemon.PhysicDefence.ToString();
        P_energyPower_text.text = pokemon.EnergyPower.ToString();
        P_energyDefence_text.text = pokemon.EnergyDefence.ToString();
        P_speed_text.text = pokemon.Speed.ToString();

        var iv = pokemon.IV;
        IV_physicPower_text.text = iv.PhysicPower.ToString();
        IV_physicDefence_text.text = iv.PhysicDefence.ToString();
        IV_energyPower_text.text = iv.EnergyPower.ToString();
        IV_energyDefence_text.text = iv.EnergyDefence.ToString();
        IV_speed_text.text = iv.Speed.ToString();

        Race_physicPower_text.text = pokemon.race.phyPower.ToString();
        Race_physicDefence_text.text = pokemon.race.phyDefence.ToString();
        Race_energyPower_text.text = pokemon.race.energyPower.ToString();
        Race_energyDefence_text.text = pokemon.race.energyDefence.ToString();
        Race_speed_text.text = pokemon.race.speed.ToString();

        Basestats_physicPower_text.text = basestats.PhysicPower.ToString();
        Basestats_physicDefence_text.text = basestats.PhysicDefence.ToString();
        Basestats_energyPower_text.text = basestats.EnergyPower.ToString();
        Basestats_energyDefence_text.text = basestats.EnergyDefence.ToString();
        Basestats_speed_text.text = basestats.Speed.ToString();

        StringBuilder sb = new StringBuilder(40);
        sb.AppendFormat("{0}a{1}", pokemon.curHealth, pokemon.Health);
        HealthText.text = sb.ToString();
        NameText.text = pokemon.Ename;
    }

    
    private void Show_Skills()
    {
        List<int> skills = pokemon.skills;
        List<int> skillPps = pokemon.skillPPs;
        int i;
        if (null == skills)
        {
            Hide();
            return;
        }
        for(i=0;i<skills.Count&&i<4;i++)
        {
            Skill skill = ResourceController.Instance.allSkillDic[skills[i]];

            ShowUI(SkillsGo[i]);
            SkillPokemonType[i].text = skill.att.ToString();
            SkillName[i].text = skill.sname;

            StringBuilder sb = new StringBuilder(40);
            sb.AppendFormat("{0}a{1}", skillPps[i], skill.FullPP);
            SkillPP[i].text = sb.ToString();
                 
            SkillPower[i].text = skill.power.ToString();
            SkillHitRate[i].text = skill.hitRate.ToString();
        }
        for(;i<4;i++)
        {
            HideUI(SkillsGo[i]);
        }
    }

    private void Show_Race()
    {
        var Race = pokemon.race;
        Race_Name_text.text = Race.sname;

        StringBuilder sb = new StringBuilder(40);
        sb.Append(Race.pokemonMainType).Append(Race.pokemonSecondType);
        Race_PokemonType_text.text = sb.ToString();

        Race_Ability_text.text = pokemon.ShowAbility.ToString();
        Race_Nature_text.text = pokemon.nature.natureType.ToString();
    }

    private void Show_prop()
    {

    }
    /// <summary>
    /// 精灵选择事件
    /// </summary>
    /// <param name="i"></param>
    private void ChoosePokemon(int i)
    {
        if (i >= trainerData.pokemons.Count)
            return;
        ChosePokemonIndex = i;
        Refresh();
    }


    private void Add_Basestats_physicPower()
    {
        if (null == basestats ) return;
        basestats.PhysicPower++;
        Show_Power();
    }
    private void Add_Basestats_physicDefence()
    {
        if (null == basestats) return;
        basestats.PhysicDefence++;
        Show_Power();
    }
    private void Add_Basestats_energyPower()
    {
        if (null == basestats ) return;
        basestats.EnergyPower++;
        Show_Power();
    }
    private void Add_Basestats_energyDefence()
    {
        if (null == basestats ) return;
        basestats.EnergyDefence++;
        Show_Power();
    }
    private void Add_Basestats_speed()
    {
        if (null == basestats) return;
        basestats.Speed++;
        Show_Power();
    }
    private void Clear_Basestats_physicPower()
    {
        if (null == basestats) return;
        basestats.PhysicPower=0;
        Show_Power();
    }
    private void Clear_Basestats_physicDefence()
    {
        if (null == basestats ) return;
        basestats.PhysicDefence = 0;
        Show_Power();
    }
    private void Clear_Basestats_energyPower()
    {
        if (null == basestats) return;
        basestats.EnergyPower = 0;
        Show_Power();
    }
    private void Clear_Basestats_energyDefence()
    {
        if (null == basestats) return;
        basestats.EnergyDefence = 0;
        Show_Power();
    }
    private void Clear_Basestats_speed()
    {
        if (null == basestats) return;
        basestats.Speed = 0;
        Show_Power();
    }

    private void SeeRace()
    {

    }
    private void ChangeSkill()
    {

    }
    private void ChangeProp()
    {

    }
    private void SaveChanges()
    {
        Contexts.sharedInstance.game.ReplacePlayerData(trainerData);
    }
}
