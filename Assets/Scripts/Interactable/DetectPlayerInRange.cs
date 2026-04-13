using System.Linq;
using UnityEngine;

public class DetectPlayerInRange : MonoBehaviour
{
    [SerializeField] float range = 5f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Color onEnterColor = Color.green;
    [SerializeField] Color onNotAllEnteredColor = Color.yellow;
    [SerializeField] Color normalColor = Color.red;
    [SerializeField] Color desabledColor = Color.gray;
    [SerializeField] SpriteRenderer spriteRenderer;
    
    [SerializeField] bool isActiveted = false;
    public bool IsActiveted() => isActiveted;
    [SerializeField] int playersInRangeCount = 0;
    public int GetPlayersInRangeCount() => playersInRangeCount;
    public int GetPlayersNeedCount() => playersNeedCount;

    int playersNeedCount = 2;
    int playersInteractingNeedCount = 1;
    bool conBeUsed = true;

    public void SetDetectionUsability(bool canBeUsed)
    {
        conBeUsed = canBeUsed;
        if (!conBeUsed)
        {
            isActiveted = false;
            spriteRenderer.color = desabledColor;
        }
    }

    void Update()
    {
        if (!conBeUsed) return;
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, range, playerLayer);
        playersInRangeCount = playersInRange.Length;
        if (playersInRangeCount == 0)
        {
            spriteRenderer.color = normalColor;
            isActiveted = false;
            return;
        }

        int numberOfPlayerInteracting = playersInRange.Select(collider => collider.GetComponent<PlayerController>().IsInteracting()).Count(isInteracting => isInteracting);

        if (playersInRangeCount >= playersNeedCount && numberOfPlayerInteracting >= playersInteractingNeedCount)
        {
            spriteRenderer.color = onEnterColor;
            isActiveted = true;
            return;
        }
        else if (playersInRangeCount > 0)
        {
            spriteRenderer.color = onNotAllEnteredColor;
        }
        else
        {
            spriteRenderer.color = normalColor;
        }
        isActiveted = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
