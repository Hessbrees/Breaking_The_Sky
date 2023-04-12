using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private float moveSpeed;
    private bool isPlayerMovementDisabled = false;
    private StatusEffects statusEffects;
    private float movementStatusEffect;
    private void Awake()
    {
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();

        statusEffects = GetComponent<StatusEffects>();
    }

    private void Start()
    {
        SetPlayerAnimationSpeed();
    }

    private void OnEnable()
    {
        statusEffects.statusEffectsEvent.OnStatusEffects += MovementStatusEffect_OnStatusEffects;
    }
    private void OnDisable()
    {
        statusEffects.statusEffectsEvent.OnStatusEffects -= MovementStatusEffect_OnStatusEffects;
    }

    private void MovementStatusEffect_OnStatusEffects(StatusEffectsEvent statusEffectsEvent, StatusEffectsArgs statusEffectsArgs)
    {
        MovementStatusEffect(statusEffectsArgs.movementReduction);
        
        SetPlayerAnimationSpeed();
    }
    private void MovementStatusEffect(float movementReduction)
    {
        movementStatusEffect = movementReduction;
    }

    private void SetPlayerAnimationSpeed()
    {
        player.animator.speed = (moveSpeed - moveSpeed * movementStatusEffect) / Settings.baseSpeedForPlayerAnimations;
    }

    private void Update()
    {
        if (isPlayerMovementDisabled) return;

        MovementInput();
    }

    private void MovementInput()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        if(horizontalMovement != 0 && verticalMovement != 0)
        {
            direction *= 0.7f;
        }

        if (direction != Vector2.zero)
        {
            player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
        }
        else
        {
            player.idleEvent.CallIdleEvent();

        }

    }
}
