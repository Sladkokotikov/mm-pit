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
    private bool _controlActive = true;
    private bool _dashUsed;
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

    private void Move(float dir)
    {
        if (Mathf.Abs(dir) < 1e-6)
        {
            _animator.SetBool(Running, false);
            return;
        }

        _animator.SetBool(Running, true);
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.right * dir,
            horizontalVelocity * Time.deltaTime);
        FaceLeft = dir < 0f;
        _spriteRenderer.flipX = FaceLeft;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(_rigidbody.position, Vector3.down * rayDistance);
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
        _controlActive = false;
        _rigidbody.gravityScale = 0;
        _rigidbody.velocity = (FaceLeft ? Vector2.left : Vector2.right) * dashVelocity;

        StartCoroutine(Extensions.Delay(dashDuration, DashEnd));
    }
    
    private void DashEnd()
    {
        _rigidbody.velocity = Vector2.zero;
        _animator.SetBool(Dashing, false);
        _controlActive = true;
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
        if (!_controlActive) return;
        Move(Input.GetAxisRaw("Horizontal"));

        if (onGround && Input.GetButton("Jump"))
        {
            Jump();
        }

        Fall();
        _animator.SetBool(Jumping, jumping);

        if (_cooldowns.DashReady && Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashStart();
        }
    }

    private void Fall()
    {
        jumping = !onGround;
    }
}