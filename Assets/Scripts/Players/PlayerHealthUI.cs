using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject healthPanel;
    [SerializeField] GameObject healthHeartPrefab;

    public void UpdateHealthUI(int currentHealthHearts, int maxHealthHearts)
    {
        ClearHealthUI();

        // Create new hearts based on current health
        for (int i = 0; i < maxHealthHearts; i++)
        {
            GameObject heart = Instantiate(healthHeartPrefab, healthPanel.transform);
            Image heartImage = heart.GetComponent<Image>();

            if (i < currentHealthHearts)
            {
                heartImage.color = Color.white; // Full heart
            }
            else
            {
                heartImage.color = Color.gray; // Empty heart
            }
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
