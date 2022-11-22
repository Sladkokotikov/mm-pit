using System;
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
    [SerializeField] private float attackDistance = 2f;
    [SerializeField] private float gravityForce = 1;
    [SerializeField] private float damping = 0.3f;

    [SerializeField] private float horizontalVelocity = 7;
    [SerializeField] private float maximumHorizontalVelocity = 10;
    [SerializeField] private float horizontalVelocityOnAir = 2;
    [SerializeField] private float normalizationVelocity = 0.4f;
    [SerializeField] private float runToJumpImpulse = 0.5f;
    [SerializeField] private float impulseAfterDash = 0.5f;
    [SerializeField] private float jumpVelocity = 8f;
    [SerializeField] private float dashVelocity = 17f;
    [SerializeField] private float dashDuration = 0.2f;

    [SerializeField] private Vector2 velocity;

    [SerializeField] private bool jumping;
    [SerializeField] private bool onGround;
    public bool FaceLeft { get; private set; }
    private bool _dashUsed;
    private bool _dashActive;
    private bool _activeControl = true;

    [SerializeField] private float lastHorizontalMove;

    private static readonly int Running = Animator.StringToHash("running");
    private static readonly int Jumping = Animator.StringToHash("jumping");
    private static readonly int Dashing = Animator.StringToHash("dashing");
    private static readonly int Attack1 = Animator.StringToHash("attack");

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _cooldowns = GetComponent<Cooldowns>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        FaceLeft = false;
        _dashUsed = false;
        _rigidbody.gravityScale = 0;
    }

    private void MoveHorizontally()
    {
        var dx = Input.GetAxisRaw("Horizontal");
        if (!onGround)
            MoveAndJump(dx);
        else
            MoveOnGround(dx);

        if (dx == 0f) return;
        FaceLeft = dx < 0f;
    }

    private void MoveAndJump(float dx)
    {
        lastHorizontalMove =
            Mathf.Clamp(lastHorizontalMove + horizontalVelocityOnAir * dx * Time.fixedDeltaTime, -1, 1);
        Move(lastHorizontalMove * horizontalVelocity);

        lastHorizontalMove *= 1 - normalizationVelocity * Time.fixedDeltaTime;

        if (Mathf.Abs(lastHorizontalMove) >= 1e-6 && !_dashActive)
            _spriteRenderer.flipX = lastHorizontalMove < 0f;
    }

    private void MoveOnGround(float dx)
    {
        lastHorizontalMove = dx * runToJumpImpulse;
        _spriteRenderer.flipX = FaceLeft;

        if (Mathf.Abs(dx) < 1e-6)
        {
            _animator.SetBool(Running, false);
            return;
        }

        _animator.SetBool(Running, true);

        Move(dx * horizontalVelocity);
    }

    private void Move(float deltaMove)
    {
        if (Mathf.Abs(deltaMove) >= 1e-6 || !_dashActive)
            velocity.x = Mathf.Clamp(velocity.x + deltaMove, -maximumHorizontalVelocity, maximumHorizontalVelocity);
    }

    private void ApplyGravity()
    {
        if (_dashActive)
        {
            velocity.y = 0f;
            return;
        }

        if (!onGround)
            velocity.y -= gravityForce * Time.fixedDeltaTime;
        else
            velocity.y = -1f;
    }

    private void Jump()
    {
        velocity.y = jumpVelocity;
    }

    private void DashStart()
    {
        if (!onGround && _dashUsed)
            return;

        _dashUsed = true;
        _dashActive = true;
        _activeControl = false;
        velocity = Vector2.zero;
        _animator.SetBool(Dashing, true);
        velocity.x = (FaceLeft ? -1 : 1) * dashVelocity;
        _spriteRenderer.flipX = FaceLeft;


        StartCoroutine(Extensions.Delay(dashDuration, DashEnd));
    }

    private void DashEnd()
    {
        _rigidbody.velocity = Vector2.zero;
        _animator.SetBool(Dashing, false);
        _activeControl = true;
        _dashActive = false;
        _dashUsed = false;
        lastHorizontalMove =
            (FaceLeft ? -1 : 1) * Mathf.Min(Mathf.Abs(impulseAfterDash), Mathf.Abs(lastHorizontalMove));
        _cooldowns.DashCooldownStart();
    }

    private void CheckGround()
    {
        var layers = new[] {"Ground", "Enemy"};
        onGround = Physics2D.Raycast(_rigidbody.position, Vector2.down, rayDistance, LayerMask.GetMask(layers))
            .collider;
        if (onGround)
            _dashUsed = false;
    }

    private void FixedUpdate()
    {
        CheckGround();
        ApplyGravity();
        if (_activeControl)
        {
            if (onGround && Input.GetButton("Jump"))
            {
                Jump();
            }

            Fall();
            MoveHorizontally();
            _animator.SetBool(Jumping, jumping);

            if (_cooldowns.DashReady && !_dashUsed && Input.GetKeyDown(KeyCode.LeftShift))
            {
                DashStart();
            }
        }

        _rigidbody.MovePosition(_rigidbody.position + velocity * Time.fixedDeltaTime);
        if (!_dashActive)
            velocity.x *= (1 - damping * Time.fixedDeltaTime);

        if (Input.GetKeyDown(KeyCode.Z))
            Attack();
    }

    private void Attack()
    {
        _animator.SetTrigger(Attack1);
        var hit = Physics2D.BoxCast(_rigidbody.position, Vector2.one, 0, (FaceLeft ? -1 : 1) * Vector2.right, attackDistance, LayerMask.GetMask("Enemy"));
        
        if (hit.collider)
        {
            hit.transform.GetComponent<Enemy>().Die();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(_rigidbody.position, (FaceLeft ? -1 : 1) * Vector2.right * attackDistance);
        

    }


    private void Fall()
    {
        jumping = !onGround;
        if (onGround)
            lastHorizontalMove = 0;
    }
}