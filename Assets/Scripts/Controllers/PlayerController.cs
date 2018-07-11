using Invector.CharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : SingletonMonobehavior<PlayerController> {

    public vThirdPersonController cc;
    public void TouchMove(float x, float y)
    {
        cc.input.x = x;
        cc.input.y = y;
    }
    public void Stop()
    {
        cc.input.x = cc.input.y = 0;

    }
    /// <summary>
    /// 不允许玩家移动和变更视角
    /// </summary>
    public void DisableMove(bool isLockCamera = true)
    {

        cc.isSprinting = false;
        Stop();
    }
    public void EnableMove(bool isloackCamera = false)
    {

        cc.input.x = cc.input.y = 0;
        cc.isSprinting = true;
    }
    private void Start()
    {
        if (null != cc )
            cc.Init();
    }
    protected virtual void FixedUpdate()
    {
        if (null == cc) return;
        cc.AirControl();
    }

    protected virtual void Update()
    {
        if (null == cc ) return;
        cc.UpdateMotor();                                
        cc.UpdateAnimator();               		               
    }
}
