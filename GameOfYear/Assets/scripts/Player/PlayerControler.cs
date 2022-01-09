using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [SerializeField]
    private LayerMask dashLayerMash;
    private const float MOVEMENTSPEED = 10f ;
    private Rigidbody2D rigidBody2D;
    private Vector3 moveDirection;
    private Vector3 dashPosition;
    private float moveY, moveX;
    private bool isDashing = false;
    private float dashAmount;

    private void Awake()
    {   
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveX = 0f;
        moveY = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            moveY = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDashing = true;
        }
        moveDirection = new Vector3(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        rigidBody2D.velocity = moveDirection * MOVEMENTSPEED;

        if (isDashing)
        {
            dashAmount = 5f;
            Vector3 centerOfHero = new Vector3(transform.position.x, transform.position.y + 1.2f);
            bool hit = Physics2D.CircleCast(centerOfHero, .5f, moveDirection, dashAmount, dashLayerMash);
            // Casting circular rayCast, only returns bool if it hit something in specified layer, might be used to varify result;
            while (hit) {
                dashAmount -= .1f;
                hit = Physics2D.CircleCast(centerOfHero, .5f, moveDirection, dashAmount, dashLayerMash);
                if (dashAmount <= 0) break;
            }
            Debug.Log(centerOfHero);
            dashPosition = transform.position + moveDirection * dashAmount;
            rigidBody2D.MovePosition(dashPosition);
            isDashing = false;
            // function than will be created in mana/stamina controler "action(amountOfStamina)"
        }

    }
}