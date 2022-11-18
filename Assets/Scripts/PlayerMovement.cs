using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Cooldowns _cooldowns;
    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private float rayDistance = 0.1f;
    
    [SerializeField] private float horizontalVelocity = 7;
    [SerializeField] private float horizontalVelocityOnAir = 2;
    [SerializeField] private float jumpVelocity = 8f;
    [SerializeField] private float dashVelocity = 17f;
    
    [SerializeField] private float dashDuration = 0.2f;
    private float _dashTimer = 0f;
    
    private bool _dashActive = false;
    private bool _onGround;
    public bool FaceLeft { get; private set; }
    private bool _controlActive = true;
    private bool _dashUsed;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _cooldowns = GetComponent<Cooldowns>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        FaceLeft = false;
        _dashUsed = false;
    }

    private void Move(float dir)
    {
        if (Mathf.Abs(dir) < 1e-6) return;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.right * dir,
                horizontalVelocity * Time.deltaTime);
        FaceLeft = dir < 0f;
        _spriteRenderer.flipX = FaceLeft;
    }

    private void Jump()
    {
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpVelocity);
    }

    private void DashStart()
    {
        if (!_onGround && _dashUsed)
            return;
        
        _dashUsed = true;
        _dashActive = true;
        _controlActive = false;
        _rigidbody.gravityScale = 0;
        _rigidbody.velocity = FaceLeft ? Vector2.left * dashVelocity : Vector2.right * dashVelocity;
    }

    private void DashUpdate()
    {
        if (!_dashActive) return;
        _dashTimer += Time.deltaTime;
        if (_dashTimer >= dashDuration)
            DashEnd();
    }

    private void DashEnd()
    {
        _dashTimer = 0;
        _rigidbody.velocity = Vector2.zero;
        _dashActive = false;
        _controlActive = true;
        _rigidbody.gravityScale = 1;
        _cooldowns.DashCooldownStart();
    }

    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(_rigidbody.position, Vector2.down, rayDistance, LayerMask.GetMask("Ground"));
        _onGround = hit.collider;
        if (_onGround)
            _dashUsed = false;
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Update()
    {
        DashUpdate();
        if (!_controlActive) return;
        Move(Input.GetAxisRaw("Horizontal"));

        if (_onGround && Input.GetButton("Jump"))
        {
            Jump();
        }

        if (_cooldowns.DashReady() && Input.GetKeyDown(KeyCode.LeftShift))
        {
            DashStart();
        }
    }
}