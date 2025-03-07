using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    public Vector2 playerPosition;
    public VectorValue playerStorage;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            // Get the PlayerMovement component from the player
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // Store the player's position
                playerStorage.initialValue = playerPosition;

                // Store the player's facing direction from the animator
                Animator animator = player.GetComponent<Animator>();
                if (animator != null)
                {
                    playerStorage.facingDirectionX = animator.GetFloat("moveX");
                    playerStorage.facingDirectionY = animator.GetFloat("moveY");
                    playerStorage.isFromTransition = true; // Mark as transition
                }
                else
                {
                    Debug.LogWarning("No Animator found on Player!");
                }
            }
            else
            {
                Debug.LogWarning("No PlayerMovement component found on Player!");
            }

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
