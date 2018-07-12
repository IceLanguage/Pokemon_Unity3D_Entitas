using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public struct GlassPosComponent : IComponent {
    [PrimaryEntityIndex]
    public Vector3 pos;
}

[Game]
public sealed class GlassMeshRenderComponent : IComponent
{
    public MeshRenderer meshRenderer;
}

[Game]
public sealed class GlassForcesComponent : IComponent
{
    public List<Force> forceList;
}
public class Force
{
    public float m_Time = 0;
    public Vector3 m_Force;

    public Force(Vector3 force,float duration = 0f)
    {
        m_Force = force;
        m_Time = duration;
    }
}

[UniquePrefix("Flag")]
[Game]
public sealed class FlagShowGlassComponent:IComponent
{
}