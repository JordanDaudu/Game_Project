using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    public Vector2 playerPosition;
    public VectorValue playerStorage;
    public GameObject FadeInPanel;
    public GameObject FadeOutPanel;
    public float fadeWait;

    private void Awake()
    {
        if(FadeInPanel != null)
        {
             GameObject panel = Instantiate(FadeInPanel, Vector3.zero, Quaternion.identity) as GameObject;
            Destroy(panel, 1);
        }
    }
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

            StartCoroutine(FadeCo());
            //SceneManager.LoadScene(sceneToLoad);
        }
    }

    public IEnumerator FadeCo()
    {
        if (FadeOutPanel != null)
        {
            Instantiate(FadeOutPanel, Vector3.zero, Quaternion.identity);
        }
        yield return new WaitForSeconds(fadeWait);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        while(!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}
