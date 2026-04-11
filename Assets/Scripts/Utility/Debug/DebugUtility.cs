using UnityEngine;

/// <summary>
/// DebugUtility.WriteInColor("Text", " #ff9516");
/// </summary>
public class DebugUtility : MonoBehaviour
{
    public static void WriteInColor(string text, Color color)
    {
        Debug.Log(
            string.Format(
                "<color=#{0:X2}{1:X2}{2:X2}>{3}</color>",
                (byte)(color.r * 255f),
                (byte)(color.g * 255f),
                (byte)(color.b * 255f),
                text
            )
        );
    }

    public static void WriteInColor(string text, string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        Debug.Log(
            string.Format(
                "<color=#{0:X2}{1:X2}{2:X2}>{3}</color>",
                (byte)(color.r * 255f),
                (byte)(color.g * 255f),
                (byte)(color.b * 255f),
                text
            )
        );
    }
}
