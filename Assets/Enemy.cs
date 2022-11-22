using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private static readonly int Death = Animator.StringToHash("death");
    public bool Alive { get; private set; }

    private void Start()
    {
        Alive = true;
    }

    public void Die()
    {
        Alive = false;
        animator.SetTrigger(Death);
        StartCoroutine(Extensions.Delay(2, ()=>Destroy(gameObject)));
    }
}
