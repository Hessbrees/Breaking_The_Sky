using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();

        player.InitAllComponents();
    }

    private void OnEnable()
    {
        player.idleEvent.OnIdle += IdleEvent_OnIdle;

        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocity_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;

        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocity_OnMovementByVelocity;
    }
    private void MovementByVelocity_OnMovementByVelocity(MovementByVelocityEvent movementByVelocityEvent, MovementByVelocityArgs movementByVelocityArgs)
    {
        SetMovementAnimationParameters(movementByVelocityArgs.moveDirection);

    }
    private void SetMovementAnimationParameters(Vector2 direction)
    {
        player.animator.SetBool(Settings.isMoving, true);
        player.animator.SetBool(Settings.isIdle, false);

        player.animator.SetBool(Settings.isMovingRight, false);
        player.animator.SetBool(Settings.isMovingLeft, false);
        player.animator.SetBool(Settings.isMovingBack, false);
        player.animator.SetBool(Settings.isMovingFront, false);

        if (direction.x > 0)
        {
            player.animator.SetBool(Settings.isMovingRight, true);
        }
        else if (direction.x < 0)
        {
            player.animator.SetBool(Settings.isMovingLeft, true);
        }
        else if (direction.y > 0)
        {
            player.animator.SetBool(Settings.isMovingBack, true);
        }
        else
        {
            player.animator.SetBool(Settings.isMovingFront, true);
        }
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        SetIdleAnimationsParameters();
    }

    private void SetIdleAnimationsParameters()
    {
        player.animator.SetBool(Settings.isMovingLeft, false);
        player.animator.SetBool(Settings.isMovingRight, false);
        player.animator.SetBool(Settings.isMovingFront, false);
        player.animator.SetBool(Settings.isMovingBack, false);
        player.animator.SetBool(Settings.isMoving, false);
        player.animator.SetBool(Settings.isIdle, true);
    }
}
