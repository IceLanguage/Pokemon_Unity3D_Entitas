using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TOUCH_Controller : SingletonMonobehavior<TOUCH_Controller>
{
	
	public const int ZONE_SCREEN = 0;
	public const int ZONE_PAUSE = 1;
	public const int STICK_WALK = 1;
	public const int STICK_ROTATE = 0;
	public TouchController ctrl;

	void Start()
	{
		InitTouchController();
	}
	private void Update()
	{
		TouchController();
	}
   
	private void OnGUI()
	{
		if (null != this.ctrl )
			this.ctrl.DrawControllerGUI();
	}

	private void InitTouchController()
	{
		TouchZone pauseZone = this.ctrl.GetZone(ZONE_PAUSE);
		TouchZone screenZone = this.ctrl.GetZone(ZONE_SCREEN);
		pauseZone.DisableGUI();
		screenZone.DisableGUI();
		
	}


	private bool IsClick()
	{
		if (Input.GetMouseButtonDown(0) || 
			Input.touchCount > 0 && TouchPhase.Began == Input.GetTouch(0).phase)
		{
#if IPHONE || ANDROID
			if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
			if (EventSystem.current.IsPointerOverGameObject())
#endif
				this.ctrl.DisableController();
			return true;


		}
		this.ctrl.EnableController();
		return false;
	}
	public void DisablewalkStick()
	{
		TouchStick walkStick = this.ctrl.GetStick(STICK_WALK);
		walkStick.disableGui = true;
		   
	}
	public void EnablewalkStick()
	{
		TouchStick walkStick = this.ctrl.GetStick(STICK_WALK);
		walkStick.disableGui = false;

	}
	public void DisableroatateStick()
	{
		TouchStick rotateStick = this.ctrl.GetStick(STICK_ROTATE);
		rotateStick.disableGui = true;

	}
	public void EnableroatateStick()
	{
		TouchStick rotateStick = this.ctrl.GetStick(STICK_ROTATE);
		rotateStick.disableGui = false;

	}
	private void TouchController()
	{
		if (this.ctrl)
		{
			TouchStick
					walkStick = this.ctrl.GetStick(STICK_WALK),
					rotateStick = this.ctrl.GetStick(STICK_ROTATE);
			if(IsClick()) return;

			if (walkStick.Pressed() && false == walkStick.disableGui)
			{
				
				var playerController = PlayerController.Instance;
				playerController.EnableMove();
				Vector3 PremoveXZ = walkStick.GetVec3d(true, 0);
				playerController.TouchMove(PremoveXZ.x, PremoveXZ.z);
			}

			else if (rotateStick.Pressed() && false == rotateStick.disableGui)
			{
				
				if (null == CameraController.Instance.cameraController) return;

				Vector3 PreroateXZ = rotateStick.GetVec3d(true, 0);
				CameraController.Instance.cameraController.RotateCamera(PreroateXZ.x, PreroateXZ.z);

			}
			else
			{

				PlayerController.Instance.DisableMove();
			}


		}
	}

	
}
