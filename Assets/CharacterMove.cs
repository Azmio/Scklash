using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to test character animations with movement
/// </summary>

public class CharacterMove : MonoBehaviour
{
    private CharacterController controller;
    private Animator _anim;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public Transform cam;

    private float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        _anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            _anim.SetBool("isRunning", true);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
        else
        {
            _anim.SetBool("isRunning", false);
        }
    }
}
