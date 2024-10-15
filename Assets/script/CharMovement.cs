using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System;

public class CharMovement : MonoBehaviour
{
    Animator anim;
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float force = 700.0f;
    public float jumpForce = 700.0f;
    public float gravity = 20.0f;

    public bool isGrounded = false;

    public bool IsDef = false;
    public bool IsDancing = false;
    public bool IsWalking = false;

    CharacterController controller;

    Vector3 inputDirection = Vector3.zero;
    Vector3 targetDirection = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        anim.SetBool("IsDef", true);
        anim.SetBool("IsDancing", false);
        Time.timeScale = 1;
    }

    void Update()
    {
        float x = -(Input.GetAxis("Vertical"));
        float z = Input.GetAxis("Horizontal");
        inputDirection = new Vector3(x, 0, z);

        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("IsDef", true);
            anim.SetBool("IsWalking", true);
            Debug.Log("W key is pressed: " + IsWalking);
        }
        else if (Input.GetKey(KeyCode.S))
        {

            anim.SetBool("IsDef", true);
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsDancing", false);
            Debug.Log("S key is pressed: " + IsDef);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            anim.SetBool("IsDef", true);
            anim.SetBool("IsDancing", true);
            Debug.Log("D key is pressed: " + IsDancing);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            anim.SetBool("IsDancing", true);
            anim.SetBool("IsWalking", true);
            Debug.Log("A key is pressed: " + IsWalking);
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            anim.SetBool("IsDancing", true);
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsDef", false);
            Debug.Log("Space key is pressed: " + IsWalking);
        }
    }
}