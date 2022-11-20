using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldowns : MonoBehaviour
{
    [SerializeField] private float dashCooldown = 0.5f;
    public bool DashReady { get; private set; }
    private void Start()
    {
        DashReady = true;
    }

    public void DashCooldownStart()
    {
        DashReady = false;
        StartCoroutine(Extensions.Delay(dashCooldown, ()=>DashReady=true));
    }
}
