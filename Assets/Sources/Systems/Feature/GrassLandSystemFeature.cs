using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using UnityEngine;

/// <summary>
/// 草地系统
/// </summary>
internal class GrassLandSystemFeature:Feature
{
    public GrassLandSystemFeature(Contexts context,Material grassMaterial)
    {
        Add(new UpdateForcesOnGlassSystem(context));      
        Add(new AddGlassSystem(context, grassMaterial));
        Add(new LoadGlassPosSystem(context));
        Add(new CheckPlayerInGlassSystem(context, PlayerController.Instance.transform));

    }
}
