using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PerlinNoiseMono : MonoBehaviour
{
    public GameObject prefab;

    public Transform parent;
    public Vector2 mapSize;

    private Unity.Mathematics.Random randomValue = Unity.Mathematics.Random.CreateFromIndex(31);

    private float3 maxPos = float3.zero;

    [Range(0f, 100.0f)] public float noiceParam = 0.5f;

    private Vector3 prefabDir;
    
    private void Start()
    {
        maxPos = new float3(1, 0, 1) * 100;
        
        values.Clear();
        for (int i = -20; i < 20; i++)
        {
            for (int j = -20; j < 20; j++)
            {
                var value = AscIIHash($"{i}{j}");
                
                if (!values.Add(value))
                {
                    Debug.LogError($"Error: {i},{j}, Value:{value}");
                }
            }
        }
        Debug.LogError("");
    }

    private void Update()
    {
        var pos = prefab.transform.position;
        float value = randomValue.NextFloat(0.1f, 0.5f);
        var angle = (noiceParam + noise.cnoise(pos / 10f)) * 4.0f * math.PI;
        
        Debug.Log(noise.snoise(pos / noiceParam));
        
        var dir = float3.zero;
        math.sincos(angle, out dir.x, out dir.z);
        prefabDir = new Vector3(dir.x, 0, dir.z);
        
        
        
        prefab.transform.Translate(prefabDir * Time.deltaTime * math.PI);
        prefab.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }

    private HashSet<int> values = new HashSet<int>();

    public static unsafe int AscIIHash(string source)
    {
        int h = 0;
        fixed (char* c = source)
        {
            byte* p = (byte*)c;
            int len = source.Length * 2;
            for (int i = 0; i < len; i += 2) h = 31 * h + p[i];
        }

        return h;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(new Ray(prefab.transform.position, prefabDir));
        if(values.Count > 0) return;
        values.Clear();
        for (int i = -20; i <= 20; i++)
        {
            for (int j = -20; j <= 20; j++)
            {
                var value = AscIIHash($"{i}{j}");
                           }
        }
        
    }
}