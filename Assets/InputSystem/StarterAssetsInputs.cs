using Mirror;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace InputSystem
{
	public class StarterAssetsInputs : NetworkBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool fire;
		public bool stay;
		public bool passive;
		public bool active;
		public bool aggressive;
		public bool moveAi;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnFire(InputValue value)
		{
			FireInput(value.isPressed);
		}
		
		public void OnStay(InputValue value)
		{
			StayInput(value.isPressed);
		}
		
		public void OnPassive(InputValue value)
		{
			PassiveInput(value.isPressed);
		}
		
		public void OnActive(InputValue value)
		{
			ActiveInput(value.isPressed);
		}
		
		public void OnAggressive(InputValue value)
		{
			AggressiveInput(value.isPressed);
		}

		public void OnMoveAi(InputValue value)
		{
			MoveAiInput(value.isPressed);
		}
		
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void FireInput(bool newFireState)
		{
			fire = newFireState;
		}
		
		public void StayInput(bool newState)
		{
			stay = newState;
		}
		
		public void PassiveInput(bool newState)
		{
			passive = newState;
		}
		
		public void ActiveInput(bool newState)
		{
			active = newState;
		}
		
		public void AggressiveInput(bool newState)
		{
			aggressive = newState;
		}
		
		public void MoveAiInput(bool valueIsPressed)
		{
			moveAi = valueIsPressed;
		}

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

	}
	
}