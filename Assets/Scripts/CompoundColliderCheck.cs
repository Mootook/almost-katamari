using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CompoundColliderCheck : MonoBehaviour
{
    static readonly int COMPOUND_COLLIDER_LAYER = 7;

    /// <summary>
    /// Execute update() call in editor mode
    /// for sanity check on game object's child layers.
    /// 
    /// https://docs.unity3d.com/ScriptReference/ExecuteAlways.html 
    /// </summary>
    private void Start()
    {
        if (!Application.IsPlaying(gameObject))
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.layer != COMPOUND_COLLIDER_LAYER)
                {
                    child.gameObject.layer = COMPOUND_COLLIDER_LAYER;
                    Debug.Log("Corrected child layer");
                }
            }
        }
    }
}

