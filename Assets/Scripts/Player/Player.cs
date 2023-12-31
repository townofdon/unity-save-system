
using System.Collections;
using UnityEngine;

struct Controls
{
    public Vector2 move;
    public bool jump;
}

public class Player : MonoSaveable
{
    [SerializeField] bool debug = false;
    [SerializeField] float initialHealth = 100;

    [Space]
    [Space]

    [SerializeField] float moveSpeed = 5;
    [SerializeField] float accelTime = 0.2f;
    [SerializeField] float jumpForce = 5;
    [SerializeField] float earlyJump = 0.2f;
    [SerializeField] float coyoteTime = 0.3f;
    [SerializeField] float gravity = 2f;
    [SerializeField] float fallGravityMod = 1.5f;
    [SerializeField] float maxFallSpeed = 35f;

    [Space]
    [Space]

    [SerializeField] bool canWallJump = true;
    [SerializeField] float wallJumpForce = 4;
    [SerializeField] float wallJumpAngle = 45f;
    [SerializeField] float maxWallGrabFallSpeed = 15f;

    [Space]
    [Space]

    [SerializeField] bool canDoubleJump = true;
    [SerializeField] int numAllowedJumps = 2;

    [Space]
    [Space]

    [SerializeField] GameObject flipSprite;
    [SerializeField] float flipSpeed = 720;

    [Space]
    [Space]

    [SerializeField] ContactFilter2D wallCheckFilter;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    Controls controls;
    Rigidbody2D body;
    new BoxCollider2D collider;

    float hp = 100;
    bool isAlive = true;
    bool isGrounded;
    bool isTouchingWall;
    bool isJumpReleaseNeeded;
    int numTimesJumped;
    float timeSinceLastGrounded = float.MaxValue;
    float timeSinceLastTouchingWall = float.MaxValue;
    float timeSinceLastWallJumped = float.MaxValue;
    float timeSinceJumpLastPressed = float.MaxValue;
    RaycastHit2D[] groundHitResults = new RaycastHit2D[1];
    RaycastHit2D[] wallCheckHits = new RaycastHit2D[1];

    Coroutine flipping;
    float deltaSpeedX;
    float initialDrag;

    public override void OnGameSave(ref GameState gameState) { }

    public override void OnGameLoad(GameState gameState)
    {
        if (gameState.GetSceneIndex() != -1 && gameState.HasPlayerSpawnPosition())
        {
            transform.position = gameState.GetPlayerSpawnPosition();
        }
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        hp = initialHealth;
        initialDrag = body.drag;
        timeSinceJumpLastPressed = float.MaxValue;
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
            if (angle < 45)
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
        UpdateControls();
        TickTimers();
    }

    void UpdateControls()
    {
        controls.move.y = Input.GetAxisRaw("Vertical");
        controls.move.x = Input.GetAxisRaw("Horizontal");
        controls.jump = Input.GetButton("Jump");
        if (Input.GetButtonDown("Jump")) timeSinceJumpLastPressed = 0;
        if (Input.GetButtonUp("Jump")) isJumpReleaseNeeded = false;
        if (!controls.jump) CancelFlip();
    }

    void TickTimers()
    {
        timeSinceLastGrounded += Time.deltaTime;
        timeSinceLastTouchingWall += Time.deltaTime;
        timeSinceLastWallJumped += Time.deltaTime;
        timeSinceJumpLastPressed += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!isAlive) return;

        GroundCheck();
        WallCheck();

        HandleSpriteTurn();
        HandleMove();
        if (!HandleWallJump())
        {
            HandleJump();
        }
        UpdateGravity();
        UpdateDrag();
    }

    void GroundCheck()
    {
        int numHits = Physics2D.CircleCastNonAlloc(groundCheck.position, 0.1f, Vector2.down, groundHitResults, 0.1f, groundLayer);
        isGrounded = numHits > 0 && body.velocity.y <= 0.2f;
        if (isGrounded)
        {
            timeSinceLastGrounded = 0;
            numTimesJumped = 0;
            CancelFlip();
        }
    }

    void WallCheck()
    {
        if (!canWallJump || isGrounded || Mathf.Abs(controls.move.x) < Mathf.Epsilon)
        {
            isTouchingWall = false;
            return;
        }
        int numHits = collider.Cast(Vector2.right * controls.move.normalized.x, wallCheckFilter, wallCheckHits, 0.1f, true);
        isTouchingWall = numHits > 0;
        if (isTouchingWall)
        {
            timeSinceLastTouchingWall = 0;
            CancelFlip();
        }
    }

    void HandleSpriteTurn()
    {
        if (flipSprite == null) return;
        if (controls.move.magnitude < Mathf.Epsilon) return;
        var direction = Mathf.Sign(controls.move.x);
        var scale = flipSprite.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        flipSprite.transform.localScale = scale;
    }

    void UpdateGravity()
    {
        if (controls.jump && (Mathf.Approximately(body.velocity.y, 0) || body.velocity.y > 0))
        {
            body.gravityScale = gravity;
        }
        else if (body.velocity.y > 0)
        {
            body.gravityScale = gravity * fallGravityMod * 2f;
        }
        else
        {
            body.gravityScale = gravity * fallGravityMod;
        }
    }

    void UpdateDrag()
    {
        if (controls.move.sqrMagnitude > 0.1f)
        {
            body.drag = 0;
        }
        else
        {
            body.drag = initialDrag;
        }
    }

    void HandleMove()
    {
        float target = Mathf.Lerp(body.velocity.x, controls.move.x * moveSpeed, Mathf.Clamp01(timeSinceLastWallJumped * 1.5f));
        float x = Mathf.SmoothDamp(body.velocity.x, target, ref deltaSpeedX, accelTime);
        float maxFallSpeedFinal = canWallJump && isTouchingWall ? maxWallGrabFallSpeed : maxFallSpeed;
        body.velocity = new Vector2(x, Mathf.Max(body.velocity.y, -maxFallSpeedFinal));
    }

    void HandleJump()
    {
        bool canDoubleJumpFinal = canDoubleJump && numTimesJumped < numAllowedJumps;
        bool isGroundedFinal = body.velocity.y <= 0.2f && (isGrounded || timeSinceLastGrounded < coyoteTime || canDoubleJumpFinal);
        bool isJumpPressedFinal = timeSinceJumpLastPressed < earlyJump;
        bool canJump = isGroundedFinal && isJumpPressedFinal && !isJumpReleaseNeeded;
        if (!canJump)
            return;

        body.velocity = new Vector2(body.velocity.x, 0);
        body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isJumpReleaseNeeded = true;
        timeSinceJumpLastPressed = float.MaxValue;
        timeSinceLastGrounded = float.MaxValue;
        numTimesJumped++;
        Flip();
    }

    bool HandleWallJump()
    {
        bool isTouchingWallFinal = isTouchingWall || (timeSinceLastTouchingWall < coyoteTime && timeSinceLastWallJumped > coyoteTime);
        bool isJumpPressedFinal = timeSinceJumpLastPressed < earlyJump;
        bool canJump = !isGrounded && canWallJump && isTouchingWallFinal && isJumpPressedFinal && !isJumpReleaseNeeded;
        if (!canJump) return false;

        body.velocity = Vector2.zero;
        var direction = controls.move.normalized.x >= 0 ? 1 : -1;
        var rotation = Quaternion.Euler(0, 0, wallJumpAngle * direction);
        body.AddForce(rotation * (Vector2.up * wallJumpForce), ForceMode2D.Impulse);
        isJumpReleaseNeeded = true;
        timeSinceJumpLastPressed = float.MaxValue;
        timeSinceLastGrounded = float.MaxValue;
        timeSinceLastWallJumped = 0;
        Flip();
        return true;
    }

    void CancelFlip()
    {
        if (flipSprite == null) return;
        if (flipping != null) StopCoroutine(flipping);
        flipping = null;
        flipSprite.transform.rotation = Quaternion.identity;
    }

    void Flip()
    {
        if (flipping != null) return;
        flipping = StartCoroutine(CFlip());
    }

    IEnumerator CFlip()
    {
        if (flipSprite == null) yield break;
        while (true)
        {
            float direction = controls.move.x >= 0 ? -1 : 1;
            var rotation = Quaternion.Euler(0, 0, flipSpeed * Time.deltaTime * direction);
            flipSprite.transform.rotation *= rotation;
            yield return null;
        }
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
        GUILayout.Label($"isTouchingWall={isTouchingWall}");
        GUILayout.Label($"isJumpReleaseNeeded={isJumpReleaseNeeded}");
    }
}
