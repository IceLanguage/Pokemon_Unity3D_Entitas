using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using TinyTeam.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MyUnityEventDispatcher;
using PokemonBattele;
using LHCoroutine;

public class UICatchPokemon:TTUIPage
{
    public UICatchPokemon() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
    {
        uiPath = "UIPrefab/UICatchPokemon";
    }
    private RectTransform rectTransform;
    private Text health_text,physicPower_text,physicDefence_text,
        energyPower_text,energyDefence_text,speed_text;
    private Image pokemonIcon;

    private bool isInit = false;
    public override void Awake(GameObject go)
    {
        
        NotificationCenter<Pokemon>.Get().AddEventListener("CatchPokemonResult", Refresh);
        rectTransform = transform.GetComponent<RectTransform>();
        pokemonIcon = transform.Find("Icon").GetComponent<Image>();
        health_text = transform.Find("Text/health").GetComponent<Text>();
        physicPower_text = transform.Find("Text/physicPower").GetComponent<Text>();
        physicDefence_text = transform.Find("Text/physicDefence").GetComponent<Text>();
        energyDefence_text = transform.Find("Text/EnergyDefence").GetComponent<Text>();
        energyPower_text = transform.Find("Text/EnergyPower").GetComponent<Text>();
        speed_text = transform.Find("Text/speed").GetComponent<Text>();
        
        
    }

    public override void Active()
    {
        if(isInit)
        {
            rectTransform.DOScale(1, 0.2f);
            CoroutineManager.DoCoroutine(PreHide());
        }
        else
        {
            isInit = true;
            Hide();
        }
    }

    public override void Hide()
    {
        rectTransform.DOScale(0, 0.002f);
    }

    public void Refresh(Notification<Pokemon> notific)
    {
        Pokemon p = notific.param;

        StringBuilder sb = new StringBuilder(15);
        pokemonIcon.sprite = Resources.Load<Sprite>(sb.AppendFormat("PokemonTexture/{0}",p.raceID).ToString());

        var iv = p.IV;
        health_text.text = iv.Health.ToString();
        physicPower_text.text = iv.PhysicPower.ToString();
        physicDefence_text.text = iv.PhysicDefence.ToString();
        energyPower_text.text = iv.EnergyPower.ToString();
        energyDefence_text.text = iv.EnergyDefence.ToString();
        speed_text.text = iv.Speed.ToString();
        Show();
    }
    public override void Refresh()
    {
        Resources.UnloadUnusedAssets();
    }
    IEnumerator PreHide()
    {
        yield return new WaitForSeconds(3);
        Hide();
    }
}
