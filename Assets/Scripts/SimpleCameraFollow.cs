using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SimpleCameraFollow : MonoBehaviour
{
    private float orbitRadius;
    public float scale = 1f;
    public Vector3 boomDirection = new Vector3(0, 0.25f, -1);

    public KatamariController katamari;

    public void SetInitialParameters()
    {
        orbitRadius = scale * katamari.Radius;
    }

    private void LateUpdate()
    {
        float targetDistance = katamari.Radius * scale;
        if (targetDistance > orbitRadius)
            orbitRadius = Mathf.Lerp(orbitRadius, targetDistance, Time.deltaTime);

        transform.position = katamari.transform.position + katamari.Forward() * boomDirection.normalized * orbitRadius;
        transform.LookAt(katamari.transform);
    }
}
