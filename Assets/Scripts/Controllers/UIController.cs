using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyTeam.UI;
using UnityEngine;

class UIController:SingletonMonobehavior<UIController>
{
    private RectTransform canvas;
    public RectTransform Canvas
    {
        get
        {
            if (null == canvas)
            {
                canvas = GameObject.Find("UIRoot").GetComponent<RectTransform>();
            }
            return canvas;
        }
    }

    private Camera _camera;
    public Camera UICamera
    {
        get
        {
            if (null == _camera)
            {
                Transform cam = Canvas.transform.Find("UICamera");
                _camera = cam.GetComponent<Camera>();
            }
            return _camera;
        }
    }
    public Sprite border1, border2;
    private void OnEnable()
    {
        BeginBattleSystem.BeginBattleEvent += ShowBattleUI;
        EndBattleSystem.EndBattleEvent += CloseBattleUI;
        
    }

    private void Start()
    {
        //TTUIPage.ShowPage<UI_ShowINFO>();
        TTUIPage.ShowPage<UIToolTip>();
        TTUIPage.ShowPage<UIBattle_ShowPlaySkill>();
        TTUIPage.ShowPage<UIBattle_ShowEnemySkill>();
        TTUIPage.ShowPage<UICatchPokemon>();
        TTUIPage.ShowPage<UI_PokemonBagIcon>();
        TTUIPage.ShowPage<UI_BagIcon>();
        
    }

    private void ShowBattleUI()
    {
        TTUIPage.ShowPage<UIBattle_Buttons>();
        TTUIPage.ShowPage<UIBattle_EnemyInfo>();
        TTUIPage.ShowPage<UIBattle_PokemonInfos>();
        

        TTUIPage.ClosePage<UI_PokemonBagIcon>();
        TTUIPage.ClosePage<UI_BagIcon>();
    }

    private void CloseBattleUI()
    {

        TTUIPage.ClosePage<UIBattle_Buttons>();
        TTUIPage.ClosePage<UIBattle_EnemyInfo>();
        TTUIPage.ClosePage<UIBattle_PokemonInfos>();
        TTUIPage.ClosePage<UIBattle_Skills>();
        TTUIPage.ClosePage<UIBattle_Pokemons>();
        TTUIPage.ClosePage<UIKnapscakPanel>();

        TTUIPage.ShowPage<UI_PokemonBagIcon>();
        TTUIPage.ShowPage<UI_BagIcon>();
        

        
    }


}
