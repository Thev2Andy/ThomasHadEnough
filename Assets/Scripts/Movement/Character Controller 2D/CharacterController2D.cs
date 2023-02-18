using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 0.3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	// [SerializeField] private Camera LookCamera;

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	[HideInInspector] public bool m_Grounded;  // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	[HideInInspector] public bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private bool Jumped;
	private bool OneFrameDelay;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
        {
            OnLandEvent = new UnityEvent();
        }
	}

	private void LateUpdate()
	{
		// if (PauseMenu.Instance.Paused) return;
		

		/*Vector2 MousePos = LookCamera.ScreenToWorldPoint(Input.mousePosition);
		
		if (this.transform.position.x > MousePos.x) {
			if(m_FacingRight) Flip();
		}
		
		else if (this.transform.position.x < MousePos.x){
			if(!m_FacingRight) Flip();
		}
		
		else {
			// Do nothing.
		}*/

		if (OneFrameDelay) {
			OneFrameDelay = false;
		}
	}

    private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool jump)
	{
		if (this.enabled)
		{
            // Only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                // And then smoothing it out and applying it to the character
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            }


            // If the player should jump...
            if (m_Grounded && jump && !Jumped)
            {
                // Add a vertical force to the player.
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                m_Grounded = false;
                Jumped = true;
				OneFrameDelay = true;
            }
        }
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

    private void OnCollisionStay2D(Collision2D collision)
    {
		if (Jumped && m_Grounded && !OneFrameDelay) {
			Debug.Log($"Stayed in collision with {collision.gameObject.name}, setting Jumped to false..");
			Jumped = false;
		}
    }
}