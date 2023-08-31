using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    new Camera camera;

    void Awake()
    {
        camera = Camera.main;
    }

    void LateUpdate()
    {
        camera.transform.position = new Vector3(
            transform.position.x,
            camera.transform.position.y,
            camera.transform.position.z
        );
    }
}
