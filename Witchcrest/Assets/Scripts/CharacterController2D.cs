using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	
	
	[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching

	

	private bool grounded;            // Whether or not the player is grounded.
	private bool isClimbing;

	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	Vector3 _groundCheck;

	[Header("Ground Check")]
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	[SerializeField]
	float groundCheckOffset = 16;

	[Header("Ceiling Check")]
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	[SerializeField]
	float ceilingCheckOffset = 16;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;
	public bool floorIsPassThrough = false;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = grounded;
		grounded = false;

		_groundCheck = m_GroundCheck.position;
		_groundCheck.y -= (gameObject.GetComponent<SpriteRenderer>().sprite.rect.height / 2 / groundCheckOffset);


		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			
			if (colliders[i].gameObject != gameObject)
			{
				if (colliders[i].gameObject.layer == 12)
				{
					floorIsPassThrough = true;
				}
				else
					floorIsPassThrough = false;
				grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

	public bool Jump(bool isCrouching = false)
	{
		if (!IsGrounded() && !IsClimbing())
			return false;

		if (IsClimbing())
		{
			isClimbing = false;
			StartCoroutine("PassThroughFloor");
		}

		if (IsGroundPassThrough() && isCrouching)
		{
			grounded = false;
			StartCoroutine("PassThroughFloor");
			return false;
		}

		if (m_Rigidbody2D.velocity.y > 0) m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
		float jumpVelocity = (0.5f * -Physics2D.gravity.y) * m_JumpForce;
		m_Rigidbody2D.AddForce(new Vector2(0f, jumpVelocity),ForceMode2D.Impulse);
		
		grounded = false;
		return true;
		
	}

	private IEnumerator PassThroughFloor()
	{
		Physics2D.IgnoreLayerCollision(11, 12, true);
		yield return new WaitForSeconds(0.5f);
		Physics2D.IgnoreLayerCollision(11, 12, false);
	}

	public bool CanCrouch()
	{
		if (!IsGrounded())
			return false;

		return true;
	}

	public void Climb(Vector2 movement)
	{
		isClimbing = true;
		// Move the character by finding the target velocity
		Vector3 targetVelocity = movement * 100f * m_CrouchSpeed;
		// And then smoothing it out and applying it to the character
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
	}

	public void Move(float move, bool crouch, bool climbing)
	{

		if (climbing)
		{
			return;
		}

		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			Collider2D colliderAbove = Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround);
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (colliderAbove)
			{
				//If the above layer is pass through, we can stand
				if (colliderAbove.gameObject.layer != 12)
				{
					crouch = true;
				}
			}
		}

		//only control the player if grounded or airControl is turned on
		if (IsGrounded() || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} 
			else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 100f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousepos.z = 0;
			Vector3 viewDirection = (mousepos - transform.position).normalized;

			// If the mouse is to the right of the player and the player is facing left...
			if (viewDirection.x > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the mouse is to the left of the player and the player is facing right...
			else if (viewDirection.x < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
	}

	/// <summary>
	/// Returns the normalised direction on the x-axis (1 = right)
	/// </summary>
	/// <returns></returns>
	public int GetDirection()
	{
		if (m_FacingRight)
			return 1;

		return -1;
	}

	public bool IsGroundPassThrough()
	{
		if (!IsGrounded())
			return false;

		return floorIsPassThrough;
	}

	public bool IsGrounded()
	{
		return grounded;
	}

	public bool IsClimbing()
	{
		return isClimbing;
	}

	private void OnDrawGizmosSelected()
	{
		_groundCheck = m_GroundCheck.position;
		_groundCheck.y -= (gameObject.GetComponent<SpriteRenderer>().sprite.rect.height / 2 / groundCheckOffset);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(_groundCheck, k_GroundedRadius);
	}
}
