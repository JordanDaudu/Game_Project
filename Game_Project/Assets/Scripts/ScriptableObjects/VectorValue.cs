using UnityEngine;

[CreateAssetMenu]
public class VectorValue : ScriptableObject, ISerializationCallbackReceiver
{
    public Vector2 initialValue; // Position
    public Vector2 defaultValue;
    public float facingDirectionX; // Facing direction (e.g., -1 for left, 1 for right)
    public float facingDirectionY; // Facing direction (e.g., -1 for down, 1 for up)
    public bool isFromTransition; // Flag to indicate if set by a transition

    public void OnAfterDeserialize()
    {
        initialValue = defaultValue;
        facingDirectionX = 0f;
        facingDirectionY = -1f;
    }
    public void OnBeforeSerialize()
    {

    }

    public void ResetToDefault()
    {
        initialValue = Vector2.zero;
        facingDirectionX = 0f;
        facingDirectionY = -1f;
        isFromTransition = false;
    }

    // THIS HERE IS USED FOR EDITOR TO RESET POSITION AFTER EVERY USE
    // IMPORTANT: ONLY WORKS FOR EDITOR
    /*
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetAllVectorValues()
    {
        VectorValue[] vectorValues = Resources.FindObjectsOfTypeAll<VectorValue>();
        foreach (VectorValue vectorValue in vectorValues)
        {
            vectorValue.ResetToDefault();
            Debug.Log($"VectorValue reset: {vectorValue.name}, Facing X={vectorValue.facingDirectionX}, Y={vectorValue.facingDirectionY}, isFromTransition={vectorValue.isFromTransition}");
        }
    }
#endif
    */
}
