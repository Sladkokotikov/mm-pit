using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldowns : MonoBehaviour
{
    [SerializeField] private float dashCooldown = 0.5f; 
    private bool _dashReady;
    private float _dashTimer;
    private void Start()
    {
        _dashReady = true;
    }

    public void DashCooldownStart()
    {
        _dashReady = false;
        _dashTimer = 0f;
    }
    private void DashCooldownUpdate()
    {
        if (_dashReady) return;
        _dashTimer += Time.deltaTime;
        if (_dashTimer > dashCooldown)
        {
            _dashReady = true;
        }
    }

    public bool DashReady()
    {
        return _dashReady;
    }
    
    private void Update()
    {
        DashCooldownUpdate();
    }
}
