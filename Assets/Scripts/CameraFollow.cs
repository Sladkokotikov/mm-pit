using UnityEngine;
using System.Collections;
[DefaultExecutionOrder(100)]
public class CameraFollow : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private GameObject _player;
    private Transform _playerTransform;

    [SerializeField] private float damping = 3f;
    [SerializeField] private Vector2 offset = new Vector2(1f, 1f);

    private void Start ()
    {
        offset = new Vector2(Mathf.Abs(offset.x), offset.y);
        FindPlayer();
    }

    private void FindPlayer()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = _player.transform;
        _playerMovement = _player.GetComponent<PlayerMovement>();
        transform.position = new Vector3(_playerTransform.position.x + offset.x, _playerTransform.position.y + offset.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        Vector3 target;
        if (_playerMovement.FaceLeft)
        {
            target = new Vector3(_playerTransform.position.x - offset.x, _playerTransform.position.y + offset.y, transform.position.z);
        }
        else 
        {
            target = new Vector3(_playerTransform.position.x + offset.x, _playerTransform.position.y + offset.y, transform.position.z);
        }
        transform.position = Vector3.Lerp(transform.position, target, damping * Time.fixedDeltaTime);
    }
}