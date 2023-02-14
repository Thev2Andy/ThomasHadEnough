using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController2D Controller;
	public float RunSpeed = 40f;

	// Private / Hidden variables..
	[HideInInspector] public float HorizontalMove = 0f;
	private bool Jump = false;
	
	// Update is called once per frame
	private void Update()
    {
		HorizontalMove = Input.GetAxisRaw("Horizontal") * RunSpeed;

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
			Jump = true;
		}

	}

	private void FixedUpdate() {
		Controller.Move(HorizontalMove * Time.fixedDeltaTime, Jump);
		Jump = false;
	}
}
