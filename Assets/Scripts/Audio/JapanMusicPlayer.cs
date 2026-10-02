using UnityEngine;

/// <summary>
/// Projedeki orijinal yüksek kaliteli müziği çalan sistem.
/// </summary>
public class JapanMusicPlayer : MonoBehaviour
{
    private AudioSource musicSource;
    private AudioClip   musicClip;

    void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        
#if UNITY_EDITOR
        // Editör içindeyken projenin kendi orijinal müziğini yükle
        musicClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Music.wav");
#endif

        if (musicClip != null)
        {
            musicSource.clip   = musicClip;
            musicSource.loop   = true;
            musicSource.volume = 0.35f; // Normal bir seviyede çal
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Orijinal müzik bulunamadı!");
        }
    }
}
