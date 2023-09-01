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

        Vector2 heading = (player.transform.position - transform.position).normalized;
        body.velocity = new Vector2(-heading.x * moveSpeed, body.velocity.y);
    }

    void FindPlayer()
    {
        if (player != null)
        {
            hasSeenPlayer = false;
            return;
        }
        player = GameObject.FindWithTag("Player");
    }
}