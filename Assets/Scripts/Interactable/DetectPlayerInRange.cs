using System.Linq;
using UnityEngine;

public class DetectPlayerInRange : MonoBehaviour
{
    [SerializeField]
    float range = 5f;

    [SerializeField]
    LayerMask playerLayer;

    [SerializeField]
    bool changingColors = true;

    [SerializeField]
    Color onEnterColor = Color.green;

    [SerializeField]
    Color onNotAllEnteredColor = Color.yellow;

    [SerializeField]
    Color normalColor = Color.red;

    [SerializeField]
    Color desabledColor = Color.gray;

    [SerializeField]
    Sprite normalSprite,
        disabledSprite;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    bool isActiveted = false;

    public bool IsActiveted() => isActiveted;

    [SerializeField]
    int playersInRangeCount = 0;

    public int GetPlayersInRangeCount() => playersInRangeCount;

    public int GetPlayersNeedCount() => playersNeedCount;

    int playersNeedCount = 2;
    int playersInteractingNeedCount = 1;
    bool conBeUsed = true;

    void Start()
    {
        if (changingColors && spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }

    public void SetDetectionUsability(bool canBeUsed)
    {
        conBeUsed = canBeUsed;
        if (!conBeUsed)
        {
            isActiveted = false;
            if (changingColors)
                spriteRenderer.color = desabledColor;
            spriteRenderer.sprite = disabledSprite;
        }
    }

    void Update()
    {
        if (!conBeUsed)
            return;
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(
            transform.position,
            range,
            playerLayer
        );
        playersInRangeCount = playersInRange.Length;
        // If no players are in range, reset to normal color and deactivate
        if (playersInRangeCount == 0)
        {
            if (changingColors)
                spriteRenderer.color = normalColor;
            spriteRenderer.sprite = normalSprite;
            isActiveted = false;
            return;
        }

        int numberOfPlayerInteracting = playersInRange
            .Select(collider => collider.GetComponent<PlayerController>().IsInteracting())
            .Count(isInteracting => isInteracting);

        // If enough players are in range and interacting, activate and change color
        if (
            playersInRangeCount >= playersNeedCount
            && numberOfPlayerInteracting >= playersInteractingNeedCount
        )
        {
            if (changingColors)
                spriteRenderer.color = onEnterColor;
            isActiveted = true;
            return;
        }
        // If not enough players are in range or interacting, change to not all entered color
        else if (playersInRangeCount > 0)
        {
            if (changingColors)
                spriteRenderer.color = onNotAllEnteredColor;
        }
        // If no players are in range, reset to normal color and deactivate
        else
        {
            if (changingColors)
                spriteRenderer.color = normalColor;
            spriteRenderer.sprite = normalSprite;
        }
        isActiveted = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
