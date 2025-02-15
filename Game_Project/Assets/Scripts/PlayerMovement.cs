using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        change = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        if (change != Vector3.zero)
        {
            change.Normalize(); // Prevents diagonal speed boost
        }

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void UpdateAnimation()
    {
        if (change != Vector3.zero)
        {
            spriteRenderer.flipX = (change.x < 0);
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
        myRigidbody.MovePosition(myRigidbody.position + (Vector2)change * speed * Time.fixedDeltaTime);
    }
}
