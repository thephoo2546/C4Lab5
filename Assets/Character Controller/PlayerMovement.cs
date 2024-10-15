using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // พารามิเตอร์การเคลื่อนที่ของตัวละคร
    public float speed = 10.0f;          // ความเร็วในการเคลื่อนที่
    public float jumpForce = 8.0f;       // แรงกระโดด
    public float gravity = 20.0f;        // แรงโน้มถ่วง
    public float rotationSpeed = 100.0f; // ความเร็วในการหมุน

    // สถานะการอนิเมชัน
    public bool isGrounded = false;      // ตัวละครอยู่บนพื้นหรือไม่
    public bool IsDef = false;           // สถานะป้องกัน
    public bool IsWalking = false;       // สถานะเดิน
    public bool IsTaking = false;        // สถานะเก็บไอเทม

    private Animator animator;           // อ้างอิงถึง Animator
    private CharacterController characterController; // อ้างอิงถึง CharacterController
    private Vector3 inputVector = Vector3.zero;     // เวกเตอร์อินพุต
    private Vector3 targetDirection = Vector3.zero; // ทิศทางเป้าหมาย
    private Vector3 moveDirection = Vector3.zero;   // ทิศทางการเคลื่อนที่

    //------------------------------------------------------------
    GameController gameController; // อ้างอิงถึง GameController
    //------------------------------------------------------------

    void Awake()
    {
        // เริ่มต้นคอมโพเนนต์
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // ตั้งค่าสเกลเวลาในเกม
        Time.timeScale = 1;
        isGrounded = characterController.isGrounded; // ตรวจสอบว่าตัวละครอยู่บนพื้น
        //------------------------------------------------------------
        if (gameController == null)
        {
            gameController = GameObject.Find("GameController").GetComponent<GameController>();
        }
        //------------------------------------------------------------
    }

    void Update()
    {
        // ตรวจจับการกดปุ่ม X เพื่อสลับระหว่างการ Defeated และการเดิน
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (IsDef)
            {
                IsDef = false;
                animator.SetBool("IsDef", IsDef);
            }
            else
            {
                IsDef = true;
                animator.SetBool("IsDef", IsDef);
            }
        }

        // ตรวจสอบสถานะการเก็บไอเทม
        if (IsTaking)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                animator.SetBool("IsTaking", IsTaking);
                StartCoroutine(WaitForTaking(4.7f));
                gameController.GetItem();
                Debug.Log("Item:" + gameController.getItem);
            }
        }

        // ถ้าตัวละครอยู่ในสถานะ Defeated จะไม่เคลื่อนที่
        if (IsDef) 
        {
            return; // หยุดการอัปเดตการเคลื่อนที่เมื่ออยู่ในสถานะ Defeated
        }

        // รับค่าการเคลื่อนที่จากผู้เล่น
        float x = (Input.GetAxis("Horizontal"));
        float z = (Input.GetAxis("Vertical"));

        // อัปเดตพารามิเตอร์อนิเมชัน
        animator.SetFloat("InputX", x);
        animator.SetFloat("InputZ", z);

        // ตรวจสอบว่าตัวละครกำลังเดินหรือไม่
        IsWalking = x != 0 || z != 0;
        animator.SetBool("IsWalking", IsWalking);

        // ตรวจสอบว่าตัวละครอยู่บนพื้น
        isGrounded = characterController.isGrounded;
        if (isGrounded)
        {
            moveDirection = new Vector3(x, 0.0f, z); // ใช้ค่าที่แก้ไขแล้ว
            moveDirection *= speed;

            // เปิดใช้งานการกระโดดหากผู้เล่นกดปุ่มกระโดด
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }

        // ใช้แรงโน้มถ่วงสำหรับตัวละคร
        moveDirection.y -= gravity * Time.deltaTime;

        // เคลื่อนที่ตัวละคร
        characterController.Move(moveDirection * Time.deltaTime);

        // อัปเดตการเคลื่อนที่
        inputVector = new Vector3(x, 0, z);
        viewRelativeMovement(); // Update targetDirection first
        updateMovement();       // Then update movement and rotation
    }

    IEnumerator WaitForTaking(float time)
    {
        yield return new WaitForSeconds(time);
        IsTaking = false;
        animator.SetBool("IsTaking", IsTaking);
    }

    void updateMovement()
    {
        Vector3 motion = inputVector;
        // ปรับขนาดอินพุตการเคลื่อนที่
        motion = (Mathf.Abs(motion.x) > 1 || Mathf.Abs(motion.z) > 1) ? motion.normalized : motion;

        // หมุนตัวละครไปในทิศทางที่เคลื่อนที่
        rotatTowardMovement();
    }

    void rotatTowardMovement()
    {
        // หมุนตัวละครไปในทิศทางที่เคลื่อนที่
        if (inputVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void viewRelativeMovement()
    {
        // คำนวณการเคลื่อนที่ตามมุมมองของกล้อง
        Transform cameraTransform = Camera.main.transform;
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        targetDirection = (Input.GetAxis("Horizontal") * right) + (Input.GetAxis("Vertical") * forward);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item")
        {
            IsTaking = true;
            Debug.Log("IsTaking:" + IsTaking);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "item")
        {
            IsTaking = false;
            Debug.Log("IsTaking:" + IsTaking);
        }
    }
}
