using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public PlayerControl playerControl;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public Health health;

    private bool isComponentsInitialized = false;
    private void Awake()
    {
        InitAllComponents();
    }

    public void InitAllComponents()
    {
        if (isComponentsInitialized) return;

        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        idleEvent = GetComponent<IdleEvent>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        playerControl = GetComponent<PlayerControl>();

        isComponentsInitialized = true;
    }

    public void Start()
    {
        health.SetStartingHealth();
    }
}
