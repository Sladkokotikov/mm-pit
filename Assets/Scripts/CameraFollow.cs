using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    private float _damping = 3f;
    private Vector2 _offset = new Vector2(1f, 1f);
    private Transform _player;

    private void Start ()
    {
        _offset = new Vector2(Mathf.Abs(_offset.x), _offset.y);
        FindPlayer();
    }

    private void FindPlayer()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = new Vector3(_player.position.x + _offset.x, _player.position.y + _offset.y, transform.position.z);
    }

    private void Update()
    {
        Vector3 target;
        if (PlayerMovement.FaceLeft)
        {
            target = new Vector3(_player.position.x - _offset.x, _player.position.y + _offset.y, transform.position.z);
        }
        else 
        {
            target = new Vector3(_player.position.x + _offset.x, _player.position.y + _offset.y, transform.position.z);
        }
        transform.position = Vector3.Lerp(transform.position, target, _damping * Time.deltaTime);
    }
}