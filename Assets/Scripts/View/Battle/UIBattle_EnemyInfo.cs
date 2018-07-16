using DG.Tweening;
using Entitas;
using PokemonBattelePokemon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIBattle_EnemyInfo:TTUIPage
{
    public UIBattle_EnemyInfo() : base(UIType.Normal, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UIBattle_EnemyInfo";
    }
    private RectTransform thisrect;
    
    private Image PokemonIcon;
    private Text PokemonName;
    private Slider PokemonHealth;
    private Text PokemonHealthText;
    private Image AbnormalStateImage;
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
        thisrect.DOAnchorPosX(879, 1);
    }

    public override void Refresh()
    {
        Show_UIBattle_EnemyInfo();
        Resources.UnloadUnusedAssets();
    }

    public override void Hide()
    {
        //var entity = context.GetEntityWithBattlePokemonData(PrePokemonData);
        //var action = entity.pokemonDataChangeEvent.Event;
        //action -= Refresh;
        //entity.ReplacePokemonDataChangeEvent(action);

        thisrect.DOAnchorPosX(1050, 1);
    }
    private void InitUnityComponents()
    {
        thisrect = transform.GetComponent<RectTransform>();
        PokemonIcon = transform.Find("Icon").GetComponent<Image>();
        PokemonName = transform.Find("Name").GetComponent<Text>();
        PokemonHealth = transform.Find("Blood").GetComponent<Slider>();
        PokemonHealthText = transform.Find("Blood/Health").GetComponent<Text>();
        AbnormalStateImage = transform.Find("AbnormalState").GetComponent<Image>();
    }
   

    private void Show_UIBattle_EnemyInfo()
    {
       
        BattlePokemonData pokemonData = BattleController.Instance.EnemyCurPokemonData;
        if (PrePokemonData != pokemonData)
        {
            PrePokemonData = pokemonData;
            var entity = context.GetEntityWithBattlePokemonData(pokemonData);
            var action = entity.pokemonDataChangeEvent.Event;
            action += Refresh;
            entity.ReplacePokemonDataChangeEvent(action);
        }

        int pokemonID = pokemonData.race.raceid;
        PokemonIcon.sprite = Resources.Load<Sprite>(
                     new StringBuilder(20)
                     .AppendFormat("PokemonTexture/{0}", pokemonID).ToString());

        PokemonName.text = pokemonData.Ename;

        int curHealth = pokemonData.curHealth;
        int fullHealth = pokemonData.Health;
        PokemonHealthText.text =
             new StringBuilder(20)
                .AppendFormat("{0}a{1}", curHealth, fullHealth).ToString();
        PokemonHealth.value = curHealth / (float)fullHealth;
    }


}
