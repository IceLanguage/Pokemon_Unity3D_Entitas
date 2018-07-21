using DG.Tweening;
using PokemonBattele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIBattle_PokemonInfos : TTUIPage
{
    public UIBattle_PokemonInfos() : base(UIType.Normal, UIMode.DoNothing, UICollider.Normal)
    { 
        uiPath = "UIPrefab/UIBattle_PokemonInfo";
    }
    private RectTransform thisrect;
    
    private Image PokemonIcon;
    private Text PokemonName;
    private Slider PokemonHealth;
    private Text PokemonHealthText;
    private Text PhysicPowerText;
    private Text PhysicDefenceText;
    private Text EnergyPowerText;
    private Text EnergyDefenceText;
    private Text SpeedText;
    private Text RaceNameText;
    private Text PokemonTypeText;
    private Text NatureText;
    private Text AbilityText;
    private Text PropsText;
    private Image AbnormalStateEnumImage;
    private BattlePokemonData PrePokemonData;
    private GameContext context;
    public override void Awake(GameObject go)
    {
        context = Contexts.sharedInstance.game;

        InitUnityComponents();
        
       
        Refresh();
    }
    public override void Active()
    {
        thisrect.DOAnchorPosX(0, 1);
    }
    public override void Hide()
    {
        

        thisrect.DOAnchorPosX(-160, 1);
    }
    public override void Refresh()
    {
        Show_UIBattle_PokemonInfo();
        Resources.UnloadUnusedAssets();

    }
    private void InitUnityComponents()
    {
        thisrect = transform.GetComponent<RectTransform>();
        PokemonIcon = transform.Find("Icon").GetComponent<Image>();
        PokemonName = transform.Find("Name").GetComponent<Text>();
        PokemonHealth = transform.Find("Blood").GetComponent<Slider>();
        PokemonHealthText = transform.Find("Blood/Health").GetComponent<Text>();
        PhysicPowerText = transform.Find("PhysicPower/Text").GetComponent<Text>();
        PhysicDefenceText = transform.Find("PhysicDefence/Text ").GetComponent<Text>();
        EnergyPowerText = transform.Find("EnergyPower/Text ").GetComponent<Text>();
        EnergyDefenceText = transform.Find("EnergyDefence/Text").GetComponent<Text>();
        SpeedText = transform.Find("Speed/Text ").GetComponent<Text>();
        RaceNameText = transform.Find("Race/Name").GetComponent<Text>();
        PokemonTypeText = transform.Find("Race/Type").GetComponent<Text>();
        NatureText = transform.Find("Race/Nature").GetComponent<Text>();
        AbilityText = transform.Find("Race/Ability").GetComponent<Text>();
        PropsText = transform.Find("Props/Name").GetComponent<Text>();
        AbnormalStateEnumImage = transform.Find("AbnormalState").GetComponent<Image>();
    }


    private void Show_UIBattle_PokemonInfo()
    {

        BattlePokemonData pokemonData =
           BattleController.Instance.PlayerCurPokemonData;

        if(null == pokemonData)
        {
            var entity = PrePokemonData.entity;
            var action = entity.pokemonDataChangeEvent.Event;
            action -= Refresh;
            entity.ReplacePokemonDataChangeEvent(action);
            PrePokemonData = null;
            return;
        }

        if (PrePokemonData != pokemonData)
        {
            if (null != PrePokemonData)
            {
                var Preentity = PrePokemonData.entity;
                var Preaction = Preentity.pokemonDataChangeEvent.Event;
                Preaction -= Refresh;
                Preentity.ReplacePokemonDataChangeEvent(Preaction);
            }

            PrePokemonData = pokemonData;
            var entity= pokemonData.entity;
            var action = entity.pokemonDataChangeEvent.Event;
            action += Refresh;
            entity.ReplacePokemonDataChangeEvent(action);
        }

        int pokemonID = pokemonData.race.raceid;
        PokemonIcon.sprite = Resources.Load<Sprite>("PokemonTexture/" + pokemonID.ToString());
        RaceNameText.text = pokemonData.race.sname;
        PokemonName.text = pokemonData.Ename;

        var type1 = pokemonData.MainPokemonType;
        var type2 = pokemonData.SecondPokemonType;
        PokemonTypeText.text = type1.ToString()
            + type2.ToString();

        int curHealth = pokemonData.curHealth;
        int fullHealth = pokemonData.Health;
        PokemonHealthText.text =
             new StringBuilder(20)
                .AppendFormat("{0}a{1}", curHealth, fullHealth).ToString();
        PokemonHealth.value = curHealth / (float)fullHealth;

        PhysicPowerText.text = pokemonData.PhysicPower.ToString();
        PhysicDefenceText.text = pokemonData.PhysicDefence.ToString();
        EnergyPowerText.text = pokemonData.EnergyPower.ToString();
        EnergyDefenceText.text = pokemonData.EnergyDefence.ToString();
        SpeedText.text = pokemonData.Speed.ToString();

        NatureText.text = pokemonData.nature.natureType.ToString();
        AbilityText.text = pokemonData.ShowAbility.ToString();

        if(AbnormalStateEnum.Normal == pokemonData.Abnormal)
        {
            HideUI(AbnormalStateEnumImage.gameObject);
        }
        else
        {
            ShowUI(AbnormalStateEnumImage.gameObject);
            AbnormalStateEnumImage.sprite =
                Resources.Load<Sprite>(
                    new StringBuilder(20)
                        .AppendFormat("UIPrefab/AbnormalState/{0}",
                        pokemonData.Abnormal).ToString());
        }
    }

}
