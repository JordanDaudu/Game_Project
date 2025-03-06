using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfFullHeart;
    public Sprite emptyHeart;
    public FloatValue heartContainers;
    public FloatValue playerCurrentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitHeart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitHeart()
    {
        for (int i = 0; i < heartContainers.initialValue; i++)
        {
            hearts[i].gameObject.SetActive(true);
            hearts[i].sprite = fullHeart;
        }
    }

    public void UpdateHearts()
    {
        float tempHealth = playerCurrentHealth.RuntimeValue / 2;
        for (int i = 0; i < heartContainers.initialValue; i++)
        {
            if(i <= tempHealth - 1)
            {
                // Full heart
                hearts[i].sprite = fullHeart;
            }
            else if(i >= tempHealth)
            {
                // Half heart
                hearts[i].sprite = emptyHeart;
            }
            else
            {
                // Empty heart
                hearts[i].sprite = halfFullHeart;
            }
        }
    }
}
