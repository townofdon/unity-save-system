using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraBounds : MonoBehaviour
{
    new BoxCollider2D collider;

    public Bounds bounds;

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        bounds = collider.bounds;
        gameObject.SetActive(false);
    }
}
