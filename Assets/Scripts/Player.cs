﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {	

	public float moveSpeed = 6;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	Vector3 velocity;
	float velocityXSmoothing;

	float gravity;

	public float jumpHeight = 8f;
	public float timeToJumpApex = 0.4f;
	float jumpVelocity;

	Controller2D controller;

	void Start () {

		controller = GetComponent<Controller2D>();
		gravity = -(2*jumpHeight)/Mathf.Pow(timeToJumpApex,2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

	}

    void Update () {

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		int wallDirectionX = (controller.collisions.left)?-1:1;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, controller.collisions.below?accelerationTimeGrounded:accelerationTimeAirborne);

		bool wallSlinding = false;

		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0){
			wallSlinding = true;

			if (velocity.y < -wallSlideSpeedMax){
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {

				velocityXSmoothing = 0;
				velocity.x = 0;

				if (input.x != wallDirectionX && input.x != 0){
					timeToWallUnstick -= Time.deltaTime;
				} else {
					timeToWallUnstick = wallStickTime;
				}

			} else {
				timeToWallUnstick = wallStickTime;
			}

		}

		if (controller.collisions.above || controller.collisions.below) {

			velocity.y = 0;

		}

		if (Input.GetKeyDown(KeyCode.Space)) {

			if (wallSlinding){
				
				if (wallDirectionX == input.x){

					velocity.x = -wallDirectionX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;

				} else if (input.x == 0){

					velocity.x = -wallDirectionX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;

				} else {

					velocity.x = -wallDirectionX * wallLeap.x;
					velocity.y = wallLeap.y;

				}
			}

			if (controller.collisions.below){
				velocity.y = jumpVelocity;
			}
			
		}
		
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);

	}
}
