using UnityEngine;

public class PlayAudioIfMoveing : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private AudioSource audioSourceObject;

    void Start()
    {
        if (audioSourceObject == null)
        {
            Debug.LogError("No AudioSource found in children of PlayAudioIfMoveing.");
        }
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            audioSourceObject.gameObject.SetActive(true);
        }
        else
        {
            audioSourceObject.gameObject.SetActive(false);
        }
    }
}
