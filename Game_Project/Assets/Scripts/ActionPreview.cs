using UnityEngine;

public class ActionPreview : MonoBehaviour
{
    public Transform player;  // Assign the player in the Inspector
    public SpriteRenderer previewSprite; // Assign the sprite in the Inspector

    void Update()
    {
        previewSprite.enabled = Input.GetMouseButton(0); // Show only when an action button is pressed
        transform.position = GetPreviewPosition();
    }

    Vector2 GetPreviewPosition()
    {
        Vector2 direction = GetFacingDirection();
        return (Vector2)transform.position + direction * 0.5f; // Adjust the multiplier to control the distance
    }

    Vector2 GetFacingDirection()
    {
        if (Input.GetKey(KeyCode.W)) return Vector2.up;
        if (Input.GetKey(KeyCode.S)) return Vector2.down;
        if (Input.GetKey(KeyCode.A)) return Vector2.left;
        if (Input.GetKey(KeyCode.D)) return Vector2.right;
        return Vector2.zero;
    }
}
