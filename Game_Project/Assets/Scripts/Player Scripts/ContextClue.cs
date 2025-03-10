using UnityEngine;

public class ContextClue : MonoBehaviour
{

    public GameObject contextClue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enable()
    {
        contextClue.SetActive(true);
    }
    public void Disable()
    {
        contextClue.SetActive(false);
    }
}
