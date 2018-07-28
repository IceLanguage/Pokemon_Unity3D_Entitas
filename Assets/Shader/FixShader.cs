
using System.Collections.Generic;
using UnityEngine;

public class FixShader : MonoBehaviour
{

    private List<Material> thisMaterial;
    private List<string> shaders;

    void Start()
    {
        thisMaterial = new List<Material>(6);
        shaders = new List<string>(6);

        Renderer[] Renders = GetComponentsInChildren<Renderer>();
        int length = Renders.Length;

        for (int i = 0; i < length; i++)
        {
            int count = Renders[i].materials.Length;
            for (int j = 0; j < count; j++)
            {
                Material _mater = Renders[i].materials[j];
                thisMaterial.Add(_mater);
                shaders.Add(_mater.shader.name);
            }
        }


        for (int i = 0; i < thisMaterial.Count; i++)
        {
            thisMaterial[i].shader = Shader.Find(shaders[i]);
        }
    }
}