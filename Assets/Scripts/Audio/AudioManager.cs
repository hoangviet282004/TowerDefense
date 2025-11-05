using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource effectSource; // cho hiệu ứng
    //[SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip shootClip;
    //[SerializeField] private AudioClip deathClip;

    public bool hasPlayEffectSound =false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public bool HasPlayEffectSound()
    {
        return hasPlayEffectSound;
    }

    public void SetHasPlayEffectSound(bool value)
    {
        hasPlayEffectSound = value;
    }

    private void Start()
    {
        effectSource.Stop();
        hasPlayEffectSound = true;
    }

    public void ShootClip()
    {
        if (!hasPlayEffectSound) return; // <── thêm dòng này
        effectSource.PlayOneShot(shootClip, 0.3f);
    }
    //public void DeathClip() => effectSource.PlayOneShot(deathClip, 0.3f); // <<< đây
    public void ToggleSFX(bool enable)
    {
        hasPlayEffectSound = enable;
        effectSource.mute = !enable;
    }

}
