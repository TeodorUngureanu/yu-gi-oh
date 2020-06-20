using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    public float speed = 6.0F;
    public float gravity = 20.0F;

    private Vector3 moveDirection = Vector3.zero;
    private float turner;
    private float looker;
    private float sensitivity = 5.0f;
    private CharacterController controller;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        gameObject.transform.position = new Vector3(-26f, 1.1f, 2f);
        gameObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Feed moveDirection with input.
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);

        // Multiply it by speed.
        moveDirection.y = 0f;
        moveDirection *= speed;

        turner = Input.GetAxis("Mouse X") * sensitivity;
        looker = -Input.GetAxis("Mouse Y") * sensitivity;

        if (turner != 0)
        {
            // Code for action on mouse moving right
            transform.eulerAngles += new Vector3(0, turner, 0);
        }

        if (looker != 0)
        {
            // Code for action on mouse moving right
            if (transform.eulerAngles.x > 180)
            {
                if (transform.eulerAngles.x < 310)
                {
                    transform.eulerAngles = new Vector3(310, transform.eulerAngles.y, transform.eulerAngles.z);
                }
                else
                {
                    transform.eulerAngles += new Vector3(looker, 0, 0);
                }
            }
            else
            {
                if (transform.eulerAngles.x > 50)
                {
                    transform.eulerAngles = new Vector3(50, transform.eulerAngles.y, transform.eulerAngles.z);
                }
                else
                {
                    transform.eulerAngles += new Vector3(looker, 0, 0);
                }
            }
        }
        
        // Making the character move
        controller.Move(moveDirection * Time.deltaTime);
    }
}