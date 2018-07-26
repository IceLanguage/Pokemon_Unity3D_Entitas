using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GlassMesh:MonoBehaviour
{
    private Mesh mesh;
    MeshFilter filter;
   
    float width;
    float height;
    float t = 0;
    float timer = 0;
    public float delte = 0.1f;
    public float MIN,MAX = 50;
    private Vector3[] points = new Vector3[25];
    
    private void Start()
    {
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                points[i * 5 + j] = new Vector3(-0.5f + 0.2f * i, 0, -0.5f + 0.2f * j);
            }
        }
        Generate();
    }
    private void Generate()
    {
        filter = GetComponent<MeshFilter>();
        mesh = filter.mesh = new Mesh();
        mesh.name = "GlassMesh";

        
        Vector3[] vertices = new Vector3[13 * 25];
        Vector2[] uvs = new Vector2[13 * 25];
        Vector3[] normals = new Vector3[13 * 25];
        int[] triangles = new int[66 * 25];

        float offsetV = 1f / 6f;
        
        Vector3 root =Vector3.zero;
        int[] show = new int[100];
        for(int i=0;i<100;i++)
        {
            show[i] = RandomService.game.Int(0, 3);
        }
        for(int k = 0; k < 25; k++)
        {
           // if (show[k] == 1) continue;
            width = 0.04f * RandomService.game.Float(0.5f, 1.5f);
            height = 1f * RandomService.game.Float(0.5f, 1.5f);
            float currentV = 0f;
            float currentVertexHeight = 0;
            root = points[k];
            for (int i = 0; i < 12; i++)
            {
                normals[13 * k + i] = Vector3.forward;
                if (i % 2 == 0)
                {
                    vertices[13 * k + i] = new Vector3(root.x - width, root.y + currentVertexHeight, root.z);
                    uvs[13 * k + i] = new Vector2(0, currentV);
                }
                else
                {
                    vertices[13 * k + i] = new Vector3(root.x + width, root.y + currentVertexHeight, root.z);
                    uvs[13 * k + i] = new Vector2(1, currentV);

                    currentV += offsetV;
                    currentVertexHeight = currentV * height;
                }
            }
            float randomB = RandomService.game.Float(-1f, 1f);
            vertices[13 * k + 12] = new Vector3
                (root.x + randomB * width,
                root.y + currentVertexHeight + RandomService.game.Float(0.01f, offsetV / 3f),
                root.z);
            uvs[13 * k + 12] = new Vector2(0, 1);
            normals[13 * k + 12] = Vector3.forward;


            int j = 0;
            for (int p = 0; p < 11; p++)
            {
                triangles[66 * k + j++] = 13 * k + p;
                triangles[66 * k + j++] = 13 * k + p + 2;
                triangles[66 * k + j++] = 13 * k + p + 1;

            }
            for (int p = 0; p < 11; p++)
            {
                triangles[66 * k + j++] = 13 * k + p;
                triangles[66 * k + j++] = 13 * k + p + 1;
                triangles[66 * k + j++] = 13 * k + p + 2;

            }
            float angle = RandomService.game.Float(0, 90);
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.up);
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, q , Vector3.one);
            for (int i = 0; i < 13; ++i)
            {
                vertices[13 * k + i] = matrix.MultiplyPoint3x4(vertices[13 * k + i]);
            }

        }
        

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.normals = normals;

    }

    //private void Update()
    //{
    //    timer += Time.deltaTime;
    //    if (timer < 0.2f)
    //        return;
    //    timer = 0;
    //    float offsetV = 1f / 6f;
    //    float currentV = 0f;
    //    float currentVertexHeight = 0;
    //    Vector3 root = transform.position;
    //    float windCoEff = 0;

    //    for (int i = 0; i < 13; i++)
    //    {
    //        normals[i] = Vector3.forward;
    //        if (i % 2 == 0)
    //        {
    //            vertices[i] = new Vector3(root.x - width, root.y + currentVertexHeight, root.z);
    //            uvs[i] = new Vector2(0, currentV);
    //        }
    //        else
    //        {
    //            vertices[i] = new Vector3(root.x + width, root.y + currentVertexHeight, root.z);
    //            uvs[i] = new Vector2(1, currentV);

    //            currentV += offsetV;
    //            currentVertexHeight = currentV * height;
    //            windCoEff += offsetV;
    //        }

    //        float oscillateDelta = 0.05f;
    //        float oscillationStrength = 2.5f;
    //        t+= delte;
    //        if (t < MIN) t = MAX;
    //        if (t > MAX) t = MIN;
    //        Vector2 wind = new Vector2(Mathf.Sin(t * Mathf.PI), Mathf.Sin(t * Mathf.PI));
    //        wind.x += (Mathf.Sin(t + root.x / 25f) + Mathf.Sin((t + root.x / 15) + 50)) * 0.5f;
    //        wind.y += Mathf.Cos(t + root.z / 80);

    //        float lerpCoeff = (Mathf.Sin(oscillationStrength * t) + 1.0f) / 2;
    //        Vector2 leftWindBound = wind * (1.0f - oscillateDelta);
    //        Vector2 rightWindBound = wind * (1.0f+ oscillateDelta);

    //        wind = Vector3.Lerp(leftWindBound, rightWindBound, lerpCoeff);
    //        float windForce = wind.magnitude;

    //        vertices[i].x += wind.x * windCoEff;
    //        vertices[i].z += wind.y * windCoEff;
    //        vertices[i].y -= windForce * windCoEff * 0.08f;
    //    }
    //    mesh = filter.mesh = new Mesh();
    //    mesh.vertices = vertices;
    //    mesh.uv = uvs;
    //    mesh.triangles = triangles;
    //    mesh.normals = normals;
    //}
}
