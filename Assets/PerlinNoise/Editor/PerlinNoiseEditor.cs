using Unity.Mathematics;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PerlinNoiseMono))]
public class PerlinNoiseEditor : Editor
{
    private PerlinNoiseMono _mono;

    private void OnEnable()
    {
        if (Selection.activeObject is GameObject go)
        {
            _mono = go.GetComponent<PerlinNoiseMono>();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create"))
        {
            Create();
        }

        if (GUILayout.Button("销毁"))
        {
            DestoryGo();
        }
    }

    private void DestoryGo()
    {
        if (_mono == null) return;
        for (int i = _mono.parent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_mono.parent.GetChild(i).gameObject);
        }
    }

    private void Create()
    {
        if (_mono == null) return;

       
        
        for (int i = 0; i < _mono.mapSize.x; i++)
        {
            for (int j = 0; j < _mono.mapSize.y; j++)
            {
                Vector3 pos = new Vector3(i, 0, j);
                float3 noisePos = new float3(pos.x, 0, pos.z);
                float noiseValue = math.abs(noise.cnoise(noisePos / 10f));
                float scaleValue = noiseValue * 10;
                Vector3 scale = new Vector3(1, noiseValue * 10 + 1, 1);
                pos.y += 1 + scaleValue / 2;
                GameObject go = Instantiate(_mono.prefab, pos, Quaternion.identity, _mono.parent);
                go.transform.localScale = scale;
            }
        }
    }
}