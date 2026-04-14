using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Image))]
public class BackgroundScaler : MonoBehaviour
{
    public RectTransform parentRect;

    private RectTransform rectTransform;
    private Image image;
    private bool isUpdating;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    void Update()
    {
        UpdateSize();
    }

    void UpdateSize()
    {
        if (isUpdating)
            return;
        if (parentRect == null || image.sprite == null)
            return;

        isUpdating = true;

        Vector2 parentSize = parentRect.rect.size;
        Vector2 spriteSize = image.sprite.rect.size;

        float spriteAspect = spriteSize.x / spriteSize.y;
        float parentAspect = parentSize.x / parentSize.y;

        Vector2 newSize;

        if (parentAspect > spriteAspect)
        {
            // match width
            newSize.x = parentSize.x;
            newSize.y = parentSize.x / spriteAspect;
        }
        else
        {
            // match height
            newSize.y = parentSize.y;
            newSize.x = parentSize.y * spriteAspect;
        }

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y);

        isUpdating = false;
    }
}
