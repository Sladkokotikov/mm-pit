using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private float _horizontalVelocity = 10; 
    private float _jumpVelocity = 10;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var dx = Input.GetAxisRaw("Horizontal");

        _rigidbody.velocity = new Vector2(dx * _horizontalVelocity, _rigidbody.velocity.y);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpVelocity);
            print(_rigidbody.velocity.y);
        }
    }
}