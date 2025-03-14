using System.Collections;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float thrust;
    public float knockTime;
    public float damage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("breakable") && this.gameObject.CompareTag("Player"))
        {
            other.GetComponent<box>().Smash();
        }

        if (other.gameObject.CompareTag("enemy") || other.gameObject.CompareTag("Player"))
        {
            // hit is the object that was hit
            Rigidbody2D hit = other.GetComponent<Rigidbody2D>();
            if(hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                
                // Knockback for Enemy
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null && enemy.CompareTag("enemy") && enemy.CanBeHit && this.gameObject.CompareTag("Player"))
                {
                    enemy.currentState = EnemyState.stagger;
                    enemy.Knock(hit, knockTime, damage); 
                }
                // Knockback for Player
                PlayerMovement player = other.GetComponent<PlayerMovement>();
                if (player != null && player.CompareTag("Player") && player.CanBeHit)
                {
                    if (player.currentState != PlayerState.stagger)
                    {
                        player.currentState = PlayerState.stagger;
                        player.Knock(knockTime, damage);
                    }
                }
            }
        }
    }

}
