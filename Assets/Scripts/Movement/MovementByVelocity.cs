using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(StatusEffects))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D movementRigidbody;
    private MovementByVelocityEvent movementByVelocityEvent;
    private StatusEffects statusEffects;

    private float movementStatusEffect;

    private void Awake()
    {
        movementRigidbody = GetComponent<Rigidbody2D>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        statusEffects = GetComponent<StatusEffects>();
    }

    private void OnEnable()
    {
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
        statusEffects.statusEffectsEvent.OnStatusEffects += MovementStatusEffect_OnStatusEffects;
    }
    private void OnDisable()
    {
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
        statusEffects.statusEffectsEvent.OnStatusEffects -= MovementStatusEffect_OnStatusEffects;
    }

    private void MovementStatusEffect_OnStatusEffects(StatusEffectsEvent statusEffectsEvent, StatusEffectsArgs statusEffectsArgs)
    {
        MovementStatusEffect(statusEffectsArgs.movementReduction);
    }
    private void MovementStatusEffect(float movementReduction)
    {
        movementStatusEffect = movementReduction;
    }
    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        MoveRigidBody(movementByVelocityArgs.moveDirection, movementByVelocityArgs.moveSpeed);
    }

    private void MoveRigidBody(Vector2 moveDirection, float moveSpeed)
    {
        movementRigidbody.velocity = moveDirection * (moveSpeed - moveSpeed * movementStatusEffect);
    }
}
