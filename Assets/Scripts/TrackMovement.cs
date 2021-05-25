using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMovement : MonoBehaviour
{
    // public Transform katamari;
    private Transform _inputSpace;
    public KatamariController katamari;

    void Start()
    {
        _inputSpace = Camera.main.transform; 
    }

    void LateUpdate()
    {
        // need to change the scalar here
        // to the sphere's radius so no matter size it's always in front
        // Vector3 f = _inputSpace.transform.forward * katamari.radius * 1.25f;
        // Vector3 t = katamari.transform.position + f;
        // set x and z based off input space forward (camera rotation)
        // TODO: 
        // better check for y off the ground.
        // rotate so cube always orient correctly
        // float y = katamari.transform.position.y + katamari.radius;
        // Vector3 target = new Vector3(t.x, 0.0f, t.z);
        // if (target != transform.position)
        //     transform.position = Vector3.Lerp(transform.position, target, 1f);
    }


    private void Store()
    {
    }

    void OnGUI()
    {
        GUIStyle red = new GUIStyle();
        red.normal.textColor = Color.red;
        GUI.Label(new Rect(0, 80, 100, 100), "Player Foward " + _inputSpace.transform.forward.ToString(), red);        
    }
}
