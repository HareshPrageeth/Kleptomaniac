using UnityEngine;

public class SoundEffectsPlayer : MonoBehaviour
{
    public static SoundEffectsPlayer Instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip itemPickupClip;
    [SerializeField] private AudioClip itemSellClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip questCompleteClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // === Public functions you can call anywhere ===
    public void PlayItemPickup() => PlaySFX(itemPickupClip);
    public void PlayItemSell() => PlaySFX(itemSellClip);
    public void PlayDamage() => PlaySFX(damageClip);
    public void PlayQuestComplete() => PlaySFX(questCompleteClip);
}
