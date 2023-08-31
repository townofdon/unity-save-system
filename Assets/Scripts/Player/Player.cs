using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

struct Controls
{
    public Vector2 move;
    public bool jump;
    public bool isJumpReleaseNeeded;
    public float timeSinceJumpLastPressed;
}

public class Player : MonoBehaviour
{
    [SerializeField] bool debug = false;
    [SerializeField] float initialHealth = 100;

    [Space]
    [Space]

    [SerializeField] float moveSpeed = 5;
    [SerializeField] float jumpForce = 5;
    [SerializeField] float earlyJump = 0.2f;
    [SerializeField] float coyoteTime = 0.3f;
    [SerializeField] float gravity = 2f;
    [SerializeField] float fallGravityMod = 1.5f;

    [Space]
    [Space]

    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    Controls controls;
    Rigidbody2D body;

    float hp = 100;
    bool isAlive = true;
    bool isGrounded;
    float timeSinceLastGrounded = float.MaxValue;
    RaycastHit2D[] groundHitResults = new RaycastHit2D[1];

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        hp = initialHealth;
        // transform.position = State.game.GetPlayerSpawnPosition(transform.position);
        // State.game.SetPlayerSpawnPosition(transform.position);
        controls.timeSinceJumpLastPressed = float.MaxValue;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        HandleCollision(other.collider);
    }

    void HandleCollision(Collider2D other)
    {
        if (!isAlive) return;
        if (other == null) return;

        float damage = 0;
        if (other.CompareTag("Enemy") && other.TryGetComponent<Enemy>(out var enemy))
        {
            float angle = Vector2.Angle(Vector2.up, (transform.position - enemy.transform.position).normalized);

            Debug.Log(angle);

            if (angle < 30)
            {
                enemy.Die();
            }
            else
            {
                damage = 10f;
            }
        }
        else if (other.CompareTag("Hazard"))
        {
            damage = 1000f;
        }
        else if (other.TryGetComponent<ICollectible>(out var collectible))
        {
            collectible.Pickup();
        }
        hp -= damage;

        if (hp <= 0) Die();
    }

    void Update()
    {
        if (!isAlive) return;
        controls.move.y = Input.GetAxisRaw("Vertical");
        controls.move.x = Input.GetAxisRaw("Horizontal");
        controls.jump = Input.GetButton("Jump");
        if (!controls.jump) controls.isJumpReleaseNeeded = false;
        if (controls.jump && !controls.isJumpReleaseNeeded) controls.timeSinceJumpLastPressed = 0;
        TickTimers();
    }

    private void TickTimers()
    {
        timeSinceLastGrounded += Time.deltaTime;
        controls.timeSinceJumpLastPressed += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!isAlive) return;

        GroundCheck();

        body.velocity = new Vector2(controls.move.x * moveSpeed, body.velocity.y);

        HandleJump();
        UpdateGravity();
    }

    void GroundCheck()
    {
        int numHits = Physics2D.CircleCastNonAlloc(groundCheck.position, 0.1f, Vector2.down, groundHitResults, 0.1f, groundLayer);
        isGrounded = numHits > 0;
        if (isGrounded) timeSinceLastGrounded = 0;
    }

    void UpdateGravity()
    {
        if (controls.jump && (Mathf.Approximately(body.velocity.y, 0) || body.velocity.y > 0))
        {
            body.gravityScale = gravity;
        }
        else
        {
            body.gravityScale = gravity * fallGravityMod;
        }
    }

    void HandleJump()
    {
        bool isGroundedFinal = body.velocity.y <= 0.2f && (isGrounded || timeSinceLastGrounded < coyoteTime);
        bool isJumpPressedFinal = controls.jump || controls.timeSinceJumpLastPressed < earlyJump;
        bool canJump = isGroundedFinal && isJumpPressedFinal && !controls.isJumpReleaseNeeded;
        if (!canJump) return;

        body.velocity = new Vector2(body.velocity.x, 0);
        body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        controls.isJumpReleaseNeeded = true;
    }

    void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        State.game.LosePlayerLife();
        GlobalEvents.OnPlayerDeath?.Invoke();
        Destroy(gameObject);
    }

    void OnGUI()
    {
        if (!debug) return;
        GUILayout.Label($"isGrounded={isGrounded}");
        GUILayout.Label($"isJumpReleaseNeeded={controls.isJumpReleaseNeeded}");
    }
}
