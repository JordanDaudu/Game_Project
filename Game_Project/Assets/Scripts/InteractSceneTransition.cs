using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InteractSceneTransition : MonoBehaviour
{
    public InputSystem_Actions playerControls;
    private InputAction interact;
    public bool playerInRange;
    public string sceneToLoad;
    public Vector2 playerPosition;
    public VectorValue playerStorage;
    private PlayerMovement player; // Reference to the player's script

    private void Awake()
    {
        playerControls = new InputSystem_Actions();
    }
    private void Interact(InputAction.CallbackContext context)
    {
        if (!string.IsNullOrEmpty(sceneToLoad) && playerInRange)
        {
            if(player != null)
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
                Debug.LogWarning("PlayerMovement reference not set!");
            }

            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name is not set or player not in range in InteractSceneTransition!");
        }
    }

    private void OnEnable()
    {
        interact = playerControls.Player.Interact;
        interact.Enable();
        interact.performed += Interact;
    }

    private void OnDisable()
    {
        interact.performed -= Interact;
        interact.Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Store reference to PlayerMovement when player enters range
            player = other.GetComponent<PlayerMovement>();
            if (player == null)
            {
                Debug.LogWarning("No PlayerMovement component found on Player!");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
