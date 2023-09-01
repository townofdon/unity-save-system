using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    new Camera camera;
    CameraBounds cameraBounds;

    void Awake()
    {
        camera = Camera.main;
        var cameraBoundsObj = GameObject.FindWithTag("CameraBounds");
        if (cameraBoundsObj != null)
        {
            cameraBounds = cameraBoundsObj.GetComponent<CameraBounds>();
        }
    }

    void LateUpdate()
    {
        var position = camera.transform.position;
        position.x = transform.position.x;
        position.y = transform.position.y;

        if (cameraBounds != null)
        {
            position.x = Mathf.Clamp(position.x, cameraBounds.bounds.min.x + camera.Size().x * 0.5f, cameraBounds.bounds.max.x - camera.Size().x * 0.5f);
            position.y = Mathf.Clamp(position.y, cameraBounds.bounds.min.y + camera.Size().y * 0.5f, cameraBounds.bounds.max.y - camera.Size().y * 0.5f);
        }

        camera.transform.position = position;
    }
}

public static class CameraExtensions
{
    public static Bounds OrthographicBounds(this Camera camera)
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    public static Vector2 Size(this Camera camera)
    {
        return camera.OrthographicBounds().size;
    }
}
