using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterWalk : CharacterState
{
    public const float WALK_ACCELERATION_TIME = 0.35f;
    public const float WALK_DECELERATION_TIME = 0.2f;
    public float WalkAccelTimerHelper = 0f;
    public float WalkDecelTimerHelper = 0f;

    public CharacterWalk(CharacterController2D context) : base(context)
    {
    }

    public override void OnEnter()
    {
        //Debug.Log($"Entering CharacterWalk");
        m_Context.IsJumping = false;
        m_Context.IsFalling = false;
        m_Context.CharacterVelocity.y = 0f;
    }

    public override void OnExit()
    {
        //Debug.Log($"Exiting CharacterWalk");
    }

    protected override void PreUpdate()
    {
        // Groundcheck
        m_Context.GroundCheck();

        if (!m_Context.IsGrounded)
        {
            m_Context.ChangeState(new CharacterAir(m_Context));
        }

        // Moving Platform?
        Rigidbody platformRb = m_Context.LeftCastHit.rigidbody ?? m_Context.RightCastHit.rigidbody;
        if (platformRb && platformRb.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            TogglePositionBlockScript tpbs = platformRb.GetComponent<TogglePositionBlockScript>();
            if (tpbs && tpbs.IsMoving)
            {
                Debug.Log("RIDING ON MOVING PLATFORM");
                m_Context.Rigidbody.velocity = Vector3.zero;
                //m_Context.Rigidbody.MovePosition(platformRb.position + Vector3.up);
                //Vector3 relOffset = m_Context.Rigidbody.position - platformRb.position;
                m_Context.Rigidbody.MovePosition(m_Context.Rigidbody.position + ((tpbs.OffOffset - tpbs.OnOffset) / tpbs.MoveTime) * Time.deltaTime);

            }
        }
    }

    protected override void MidUpdate()
    {
        if (m_Context.PlayerMovementVector != Vector2.zero)
        {
            // Move
            float accelerationWalkSpeed = Mathf.Lerp(0f, m_Context.WalkSpeed, WalkAccelTimerHelper / WALK_ACCELERATION_TIME);
            m_Context.CharacterVelocity.x = m_Context.PlayerMovementVector.x * accelerationWalkSpeed;
        }
        else
        {
            // Stop Moving
            float decelerationWalkSpeed = Mathf.Lerp(m_Context.CharacterVelocity.x, 0f, WalkDecelTimerHelper / WALK_DECELERATION_TIME);
            m_Context.CharacterVelocity.x = decelerationWalkSpeed;
        }

        // Finally, MovePosition
        m_Context.Rigidbody.MovePosition(m_Context.Rigidbody.position + new Vector3((m_Context.CharacterVelocity.x * Time.deltaTime), 0f));
    }

    protected override void PostUpdate()
    {
        Mathf.Clamp(WalkAccelTimerHelper += Time.deltaTime, 0f, WALK_ACCELERATION_TIME);
        Mathf.Clamp(WalkDecelTimerHelper += Time.deltaTime, 0f, WALK_DECELERATION_TIME);
    }

    // + + + + | InputActions | + + + + 

    public override void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            WalkAccelTimerHelper = 0f;

            // Update Facing Direction
            m_Context.FacingRight = m_Context.PlayerMovementVector.x >= 0f;
        }
        if (ctx.canceled)
        {
            WalkDecelTimerHelper = 0f;
        }
    }

    public override void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            // Add Velocity, swap to AirState
            m_Context.CharacterVelocity.y += m_Context.JumpForce;
            m_Context.ChangeState(new CharacterAir(m_Context));
        }
    }

    public override void OnAbility(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Ability currAbility = m_Context.AbilityManager.CurrentAbility;
            if (currAbility != null)
            {
                switch (currAbility)
                {
                    case PendulumAbility:
                        m_Context.ChangeState(new CharacterPendulumState(m_Context));
                        break;
                }
            }
        }
    }

    public override void AdvanceState()
    {
        //
    }

    // + + + + | Collision Handling | + + + + 

    public override void OnTriggerEnter(Collider other)
    {
        //
    }

    public override void OnTriggerStay(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        //{
        //    //Debug.Log("Found a platform!");
        //    // Moving Platform?
        //    TogglePositionBlockScript tpbs = other.gameObject.GetComponent<TogglePositionBlockScript>();
        //    if (tpbs != null)
        //    {
        //        // Try and bind to the platform as it moves...
        //        Debug.Log("Setting velocity to zero...");
        //        m_Context.Rigidbody.velocity = Vector3.zero;
                
        //        // If there's a horizontal component to the movement,
        //        if (tpbs.OnOffset.x - tpbs.OffOffset.x != 0f)
        //        {
        //            //
        //        }

        //    }
        //}
    }

    public override void OnTriggerExit(Collider other)
    {
        //
    }

}
