using System;
using System.Collections.Generic;
using LevelEditorTools;
using UnityEditor;
using UnityEngine;

public class GameDemoLunch : MonoBehaviour
{
    public SceneContainer m_SceneContainer;

    public List<QuadRectangle> sceneNodeDatas = new List<QuadRectangle>();

    public List<QuadRectangle> sceneDoorDatas = new List<QuadRectangle>();

    private QuadCircle _circle = null;
    

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        _circle ??= new QuadCircle(Vector3.zero, 1);
        _circle.DrawGizmos();

        if (sceneNodeDatas != null && sceneNodeDatas.Count > 0)
        {
            foreach (QuadRectangle rectangle in sceneNodeDatas)
            {
                rectangle.DrawGizmos();
            }
        }

        if (sceneDoorDatas != null && sceneDoorDatas.Count > 0)
        {
            foreach (QuadRectangle rectangle in sceneDoorDatas)
            {
                rectangle.DrawGizmos();
            }
        }
    }
#endif
}