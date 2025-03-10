using UnityEngine;

public class PlayerTransparency : MonoBehaviour
{
    public string obstacleTag = "Obstacle"; // Tag for trees, rocks, etc.
    private SpriteRenderer playerRenderer;
    private Color normalColor;
    private Color transparentColor;
    public float transparencyDistance = 1.5f; // Max distance for transparency

    void Start()
    {
        playerRenderer = GetComponent<SpriteRenderer>();
        normalColor = playerRenderer.color;
        transparentColor = new Color(normalColor.r, normalColor.g, normalColor.b, 0.5f); // 50% transparent
    }

    void Update()
    {
        bool behindObstacle = false;

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag(obstacleTag);
        foreach (GameObject obstacle in obstacles)
        {
            SpriteRenderer obstacleRenderer = obstacle.GetComponent<SpriteRenderer>();
            if (obstacleRenderer == null) continue;

            float obstacleBottomY = obstacleRenderer.bounds.min.y; // Bottom of tree
            float playerFeetY = playerRenderer.bounds.min.y; // Bottom of player (feet)
            float obstacleWidth = obstacleRenderer.bounds.extents.x; // Half width
            float distance = Vector2.Distance(transform.position, obstacle.transform.position);

            // FIX: Player's feet must be ABOVE the tree’s bottom, and within X range
            if (playerFeetY > obstacleBottomY && // Player is actually behind
                transform.position.x > obstacle.transform.position.x - obstacleWidth && 
                transform.position.x < obstacle.transform.position.x + obstacleWidth &&
                distance < transparencyDistance) // Ensures player is close enough
            {
                behindObstacle = true;
                break; // Stop checking further obstacles
            }
        }

        // Apply transparency smoothly
        if (behindObstacle)
        {
            playerRenderer.color = Color.Lerp(playerRenderer.color, transparentColor, Time.deltaTime * 5f);
        }
        else
        {
            playerRenderer.color = Color.Lerp(playerRenderer.color, normalColor, Time.deltaTime * 5f);
        }
    }
}
