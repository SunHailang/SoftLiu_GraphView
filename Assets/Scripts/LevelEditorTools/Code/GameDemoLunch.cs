using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDemoLunch : MonoBehaviour
{
    public List<Rectangle> sceneNodeDatas = new List<Rectangle>();

    public List<Rectangle> sceneDoorDatas = new List<Rectangle>();

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (sceneNodeDatas != null && sceneNodeDatas.Count > 0)
        {
            foreach (Rectangle rectangle in sceneNodeDatas)
            {
                rectangle.DrawGizmos();
            }
        }
        if (sceneDoorDatas != null && sceneDoorDatas.Count > 0)
        {
            foreach (Rectangle rectangle in sceneDoorDatas)
            {
                rectangle.DrawGizmos();
            }
        }
    }
#endif
}
