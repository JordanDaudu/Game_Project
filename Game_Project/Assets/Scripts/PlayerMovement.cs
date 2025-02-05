using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        UpdateAnimationAndMove();
    }

    void UpdateAnimationAndMove()
    {
        bool flipped = change.x < 0;
        if (change != Vector3.zero)
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
            MoveCharacter();
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    void MoveCharacter()
    {
        myRigidbody.MovePosition(transform.position + change * speed * Time.deltaTime); 
    }
}
