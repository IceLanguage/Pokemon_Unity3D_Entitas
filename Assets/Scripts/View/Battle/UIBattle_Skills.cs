using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyTeam.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using MyUnityEventDispatcher;
using PokemonBattele;

public class UIBattle_Skills:TTUIPage
{
    public UIBattle_Skills() : base(UIType.Normal, UIMode.DoNothing, UICollider.WithBg)
    {
        uiPath = "UIPrefab/UIBattle_Skills";
    }
    private List<Button> button_list=new List<Button>();
    private List<RectTransform> skill1rect_list=new List<RectTransform>();
    private List<Text> SkillPokemonTypeText_list=new List<Text>();
    private List<Text> SkillNameText_list=new List<Text>();
    private List<Text> SkillPPText_list=new List<Text>();
    private RectTransform thisrect;
    private BattlePokemonData PrePokemonData;
    private GameContext context;
    public override void Awake(GameObject go)
    {
        
        context = Contexts.sharedInstance.game;
        InitUnityComponents();
        NotificationCenter<int>.Get().AddEventListener("DisableSkillButton", DisableSkillButtonEvent);
        //NotificationCenter<int>.Get().AddEventListener("EnableSkillButton", EnableSkillButtonEvent);
        BattleStateForPlayer.InitEvent += PlayerRound;
        BattleStateForBattle.InitEvent += BattleRound;
        Refresh();
        
    }

    private void PlayerRound()
    {
        var PlayerCurPokemonData = BattleController.Instance.PlayerCurPokemonData;
        int i = 0;
        for (i = 0; i < PlayerCurPokemonData.skills.Count; i++)
        {
            if (PlayerCurPokemonData.skillPPs[i] > 0)
                button_list[i].interactable = true;
        }
        while (i < 4)
        {
            button_list[i].interactable = true;
            i++;
        }
    }

    private void BattleRound()
    {
        //隐藏技能
        for (int i = 0; i < 4; i++)
        {
            button_list[i].interactable = false;
        }
    }


    public override void Hide()
    {
        thisrect.DOAnchorPosY(-400, 0.01f);

    }

    public override void Active()
    {
        thisrect.DOAnchorPosY(25, 0.1f);
    }

    public override void Refresh()
    {
        Show_UIBattle_Skills();
    }
    private void InitUnityComponents()
    {
        if (null == thisrect )
            thisrect = gameObject.GetComponent<RectTransform>();
        button_list.Clear();
        skill1rect_list.Clear();
        SkillPokemonTypeText_list.Clear();
        SkillNameText_list.Clear();
        SkillPPText_list.Clear();
        for (int i = 1; i <= 4; i++)
        {
            var sb = new StringBuilder(10)
                .AppendFormat("Skill{0}", i);
            button_list.Add(transform.Find(
               sb.ToString()).GetComponent<Button>());
            skill1rect_list.Add(transform.Find(sb.ToString()).GetComponent<RectTransform>());

            SkillPokemonTypeText_list.Add(transform.Find(
                new StringBuilder(10)
                .AppendFormat("Skill{0}", i)
                .Append("/Att").ToString()).GetComponent<Text>());
            SkillNameText_list.Add(transform.Find(
                new StringBuilder(10)
                .AppendFormat("Skill{0}", i)
                .Append("/Text").ToString()).GetComponent<Text>());
            SkillPPText_list.Add(transform.Find(
                new StringBuilder(10)
                .AppendFormat("Skill{0}", i)
                .Append("/PP").ToString()).GetComponent<Text>());

            int j = i - 1;
            button_list[i-1].onClick.AddListener(() =>
            {
                NotificationCenter<int>.Get().DispatchEvent("UseSkill", j);
            });
        }

    }


    private void Show_UIBattle_Skills()
    {

        BattlePokemonData pokemonData =
            BattleController.Instance.PlayerCurPokemonData;
        if(null == pokemonData)
        {
            Hide();
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

            var entity = pokemonData.entity;
            var action = entity.pokemonDataChangeEvent.Event;
            action += Refresh;
            entity.ReplacePokemonDataChangeEvent(action);
        }


        List<int> skillIDs = pokemonData.skills;
        List<int> skillPPs = pokemonData.skillPPs;
        int i = 0;
        for(;i<skillIDs.Count;++i)
        {
            id = skillIDs[i];
            Skill skill = ResourceController.Instance.allSkillDic[id];
            SkillPokemonTypeText_list[i].text = skill.att.ToString();
            SkillNameText_list[i].text = skill.sname;
            StringBuilder sb = new StringBuilder(20);
            sb.AppendFormat("{0}a{1}", skillPPs[i], skill.FullPP);
            SkillPPText_list[i].text = sb.ToString();
            ShowSkill(skill1rect_list[i]);
        }
        while(i<4)
        {
            HideSkill(skill1rect_list[i]);
            ++i;
        }
            

    }

    

    private void HideSkill(RectTransform rectTransform)
    {
        rectTransform.DOAnchorPosY(-383.8f, 1);
    }

    public void ShowSkill(RectTransform rectTransform)
    {
        rectTransform.DOAnchorPosY(-273.8f, 1);
    }

    /// <summary>
    /// 禁止技能的使用
    /// </summary>
    /// <param name="notific"></param>
    private void DisableSkillButtonEvent(Notification<int> notific)
    {
        button_list[notific.param].interactable = false;
    }

    //private void EnableSkillButtonEvent(Notification<int> notific)
    //{
    //    button_list[notific.param].interactable = true;
    //}
}
