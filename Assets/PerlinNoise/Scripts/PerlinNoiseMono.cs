using System;
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(new Ray(prefab.transform.position, prefabDir));
    }
}