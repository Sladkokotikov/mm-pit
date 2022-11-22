using UnityEngine;

public class AcidWorm : MonoBehaviour
{
    [SerializeField]private Rigidbody2D rb;
    [SerializeField]private SpriteRenderer rend;

    
    [SerializeField] private float horizontalVelocity;
    [SerializeField] private float rayDistance;
    [SerializeField] private Enemy enemy;
    private bool _faceLeft;
    private bool FaceLeft
    {
        get => _faceLeft;
        set
        {
            _faceLeft = value;
            rend.flipX = value;
        }
    }


    private void FixedUpdate()
    {
        if (!enemy.Alive)
            return;
        Move();
        TurnAround();
    }

    private void TurnAround()
    {
        var hit = Physics2D.Raycast(rb.position, (FaceLeft?-1:1)*Vector2.right, rayDistance, LayerMask.GetMask("Walls"));
        if (hit.collider)
            FaceLeft = !FaceLeft;
    }

    private void Move()
    {
        rb.MovePosition(rb.position + (FaceLeft?-1:1)*Vector2.right * horizontalVelocity * Time.fixedDeltaTime);
    }
}
