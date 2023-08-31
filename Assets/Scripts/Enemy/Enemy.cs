using System;
using UnityEngine;

[RequireComponent(typeof(SaveIdentifier))]
public class Enemy : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;

    Rigidbody2D body;
    SaveIdentifier id;

    GameObject player;
    bool hasSeenPlayer;
    float hp = 10;
    bool isAlive = true;

    public void Die()
    {
        State.game.AddEnemyKilled(id.uuid);
        Destroy(gameObject);
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        id = GetComponent<SaveIdentifier>();
    }

    void Start()
    {
        if (State.game.GetIsEnemyKilled(id.uuid))
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider) return;
        HandleCollision(other.collider);
    }

    void HandleCollision(Collider2D other)
    {
        if (!isAlive) return;
        if (other == null) return;

        float damage = 0;
        if (other.CompareTag("Hazard"))
        {
            damage = 1000f;
        }
        hp -= damage;

        if (hp <= 0) Die();
    }

    void Update()
    {
        FindPlayer();
        if (player == null) return;

        if (!hasSeenPlayer && Vector2.Distance(transform.position, player.transform.position) < 2f)
        {
            hasSeenPlayer = true;
        }
    }

    void FixedUpdate()
    {
        if (!hasSeenPlayer) return;
        if (player == null) return;

        Vector2 heading = Vector2.zero;
        heading.x = player.transform.position.x - transform.position.x;
        // normalize
        if (heading.x != 0) heading.x /= heading.x;
        body.velocity = heading * moveSpeed;
    }

    void FindPlayer()
    {
        if (player != null) return;
        hasSeenPlayer = false;
        player = GameObject.FindWithTag("Player");
    }
}