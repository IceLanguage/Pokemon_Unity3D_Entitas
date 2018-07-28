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
    public GrassLandSystemFeature(Contexts context)
    {
        //Add(new HideGrassSystem(context));
        //Add(new UpdateForcesOnGrassSystem(context));      
        Add(new AddGrassSystem(context));
        Add(new LoadGrassPosSystem(context));
        Add(new CheckPlayerInGrassSystem(context, PlayerController.Instance.transform));
        //Add(new WindForGlassSystem(context));

    }
}
