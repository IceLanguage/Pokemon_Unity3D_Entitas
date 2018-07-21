using System;
using DG.Tweening;
using Entitas;
using UnityEngine;
/// <summary>
/// 屏幕扭曲
/// </summary>
public static partial class SpecialEffect
{
    private static Action ScreeDistortedStopEvent;
    public static void ScreenDistorted(GameContext contexts,Action action) 
    {
        TOUCH_Controller.Instance.DisablewalkStick();
        TOUCH_Controller.Instance.DisableroatateStick();


        ScreeDistortedStopEvent = action;
        if (null == CameraController.Instance ) return;
        DOTween.To(() => CameraController.Instance.twistAngle,
                    x => CameraController.Instance.twistAngle = x, 5, 3)
                    .OnComplete(StopDistorted);
        
    }


    private static void StopDistorted()
    {
        
        DOTween.To(() => CameraController.Instance.twistAngle,
                    x => CameraController.Instance.twistAngle = x, 0, 3)
                    .OnComplete(ScreenDistortedEnd);
        

    }
    private static void ScreenDistortedEnd()
    {
        ScreeDistortedStopEvent();
        
        
    }

  




}

