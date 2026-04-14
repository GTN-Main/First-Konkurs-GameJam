using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField]
    private GameObject healthPanel;

    [SerializeField]
    GameObject healthHeartPrefab;
    Image[] heartGameObjectsImages;

    [SerializeField]
    Sprite fullHeartSprite;

    [SerializeField]
    Sprite emptyHeartSprite;

    public void Init(int maxHealth)
    {
        UpdateHealthUI(maxHealth, maxHealth);
    }

    public void ShowHealthUI(int heartsCount)
    {
        for (int i = 0; i < heartGameObjectsImages.Length; i++)
        {
            heartGameObjectsImages[i].sprite = i < heartsCount ? fullHeartSprite : emptyHeartSprite;
        }
    }

    public void UpdateHealthUI(int currentHealthHearts, int maxHealthHearts)
    {
        ClearHealthUI();
        heartGameObjectsImages = new Image[maxHealthHearts];
        // Create new hearts based on current health
        for (int i = 0; i < maxHealthHearts; i++)
        {
            GameObject heart = Instantiate(healthHeartPrefab, healthPanel.transform);
            Image heartImage = heart.GetComponent<Image>();

            if (i < currentHealthHearts)
            {
                heartImage.sprite = fullHeartSprite; // Full heart
            }
            else
            {
                heartImage.sprite = emptyHeartSprite; // Empty heart
            }
            heartGameObjectsImages[i] = heartImage;
        }
    }

    void ClearHealthUI()
    {
        foreach (Transform child in healthPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
