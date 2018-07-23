using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game]
public struct GrassPosComponent : IComponent {
    [PrimaryEntityIndex]
    public Vector3 pos;
}

[Game]
public sealed class GrassMeshRenderComponent : IComponent
{
    public MeshRenderer meshRenderer;
}

[Game]
public sealed class GrassForcesComponent : IComponent
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

