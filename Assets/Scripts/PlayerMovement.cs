using UnityEngine;

[RequireComponent(typeof(Cooldowns))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Cooldowns _cooldowns;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private float rayDistance = 0.1f;

    [SerializeField] private float horizontalVelocity = 7;
    [SerializeField] private float horizontalVelocityOnAir = 2;
    [SerializeField] private float jumpVelocity = 8f;
    [SerializeField] private float dashVelocity = 17f;

    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private bool jumping;
    [SerializeField] private bool onGround;
    public bool FaceLeft { get; private set; }
    private bool _dashUsed;

    [SerializeField] private float lastHorizontalMove;

    private static readonly int Running = Animator.StringToHash("running");
    private static readonly int Jumping = Animator.StringToHash("jumping");
    private static readonly int Dashing = Animator.StringToHash("dashing");

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _cooldowns = GetComponent<Cooldowns>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        FaceLeft = false;
        _dashUsed = false;
    }

    private void MoveHorizontally()
    {
        var dx = Input.GetAxisRaw("Horizontal");
        if (!onGround)
        {
            lastHorizontalMove += horizontalVelocityOnAir * dx * Time.deltaTime;
            transform.position = Vector3.MoveTowards(
                transform.position,
                transform.position + Vector3.right * lastHorizontalMove * horizontalVelocity,
                horizontalVelocity * Time.deltaTime);
        }
        
        if(onGround)
            lastHorizontalMove = dx;

        if (Mathf.Abs(dx) < 1e-6)
        {
            _animator.SetBool(Running, false);
            return;
        }

        _animator.SetBool(Running, true);
        var velocity = onGround ? horizontalVelocity : horizontalVelocityOnAir;
        transform.position = Vector3.MoveTowards(
            transform.position,
            transform.position + Vector3.right * dx * velocity,
            velocity * Time.deltaTime);
        FaceLeft = dx < 0f;
        
        _spriteRenderer.flipX = FaceLeft;
    }

    private void Jump()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpVelocity);
    }

    private void DashStart()
    {
        if (!onGround && _dashUsed)
            return;

        _dashUsed = true;
        _animator.SetBool(Dashing, true);
        _rigidbody.gravityScale = 0;
        _rigidbody.velocity = (FaceLeft ? Vector2.left : Vector2.right) * dashVelocity;

        StartCoroutine(Extensions.Delay(dashDuration, DashEnd));
    }

    private void DashEnd()
    {
        _rigidbody.velocity = Vector2.zero;
        _animator.SetBool(Dashing, false);
        _rigidbody.gravityScale = 1;
        _cooldowns.DashCooldownStart();
    }

    private void CheckGround()
    {
        var hit = Physics2D.Raycast(_rigidbody.position, Vector2.down, rayDistance, LayerMask.GetMask("Ground"));
        onGround = hit.collider;
        if (onGround)
            _dashUsed = false;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        

        if (onGround && Input.GetButton("Jump"))
        {
            Jump();
        }
        Fall();
        
        MoveHorizontally();
        PullBackHorizontally();

        
        _animator.SetBool(Jumping, jumping);

        if (_cooldowns.DashReady && Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashStart();
        }
    }

    private void PullBackHorizontally()
    {
        var hit = Physics2D.Raycast(_rigidbody.position, Vector2.down, rayDistance, LayerMask.GetMask("Walls"));
        if(hit.collider)
            _rigidbody.MovePosition(_rigidbody.position + 3 * Vector2.right * (FaceLeft?1:-1));
    }

    private void Fall()
    {
        jumping = !onGround;
        if (onGround)
            lastHorizontalMove = 0;
    }
}