using UnityEngine;
using Platformer.Mechanics;

/// <summary>
/// Oyuncunun saldırı seslerini yöneten bileşen.
/// PlayerController.Awake() tarafından otomatik eklenir.
/// </summary>
public class CombatSFX : MonoBehaviour
{
    public AudioClip swordSwing;
    public AudioClip kickSound;
    public AudioClip hitImpact;
    public AudioClip enemyDeath;

    private AudioSource sfxSource;

    void Start()
    {
        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        // Prosedürel sesleri üret (ölüm sesi hariç)
        swordSwing = SoundGenerator.GenerateSwordSwing();
        kickSound  = SoundGenerator.GenerateKick();
        hitImpact  = SoundGenerator.GenerateHitImpact();
        
#if UNITY_EDITOR
        enemyDeath = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Death.wav");
#endif
        if (enemyDeath == null)
            enemyDeath = SoundGenerator.GenerateEnemyDeath();
    }

    public void PlaySwordSwing()
    {
        if (sfxSource != null && swordSwing != null)
            sfxSource.PlayOneShot(swordSwing, 0.6f);
    }

    public void PlayKick()
    {
        if (sfxSource != null && kickSound != null)
            sfxSource.PlayOneShot(kickSound, 0.7f);
    }

    public void PlayHitImpact()
    {
        if (sfxSource != null && hitImpact != null)
            sfxSource.PlayOneShot(hitImpact, 0.8f);
    }

    public void PlayEnemyDeath()
    {
        if (sfxSource != null && enemyDeath != null)
            sfxSource.PlayOneShot(enemyDeath, 0.9f);
    }
}
