using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraController : SingletonMonobehavior<CameraController>
{
    public Shader shader;
    public float twistAngle = 0;
    public vThirdPersonCamera cameraController;
    private Material mat;
    void Start()
    {
        mat = new Material(shader);
        if (null == cameraController )
        {
            cameraController = GetComponent<vThirdPersonCamera>();
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        mat.SetFloat("_Distorted", twistAngle);

        Graphics.Blit(source, destination, mat);
    }



    
   public void SetBattleCamera()
    {
        cameraController.height = 5f;
        cameraController.defaultDistance = 1.5f;
    }

    public void SetSimpleCamera()
    {
        cameraController.height = 3f;
        cameraController.defaultDistance = 2.5f;
        
    }
}
