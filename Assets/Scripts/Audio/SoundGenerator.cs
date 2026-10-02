using UnityEngine;

/// <summary>
/// Kod ile ses efektleri üreten sistem.
/// Kılıç savurma, tekme atma ve isabet sesleri prosedürel olarak üretilir.
/// </summary>
public static class SoundGenerator
{
    /// <summary>
    /// Kılıç savurma sesi — hızlı swoosh efekti
    /// </summary>
    public static AudioClip GenerateSwordSwing()
    {
        int sampleRate = 44100;
        float duration = 0.25f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            // Frekans aşağı doğru kayan beyaz gürültü + sinüs karışımı
            float freq = Mathf.Lerp(2000f, 400f, t); // Yüksekten alçağa swoosh
            float sine = Mathf.Sin(2f * Mathf.PI * freq * t * 0.3f);
            float noise = (Random.value * 2f - 1f) * 0.3f;
            float envelope = Mathf.Pow(1f - t, 2f); // Hızla sönümlenme
            data[i] = (sine * 0.5f + noise) * envelope * 0.6f;
        }

        var clip = AudioClip.Create("SwordSwing", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    /// <summary>
    /// Tekme atma sesi — kısa ve sert vuruş havası
    /// </summary>
    public static AudioClip GenerateKick()
    {
        int sampleRate = 44100;
        float duration = 0.15f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            // Düşük frekanslı darbe
            float freq = Mathf.Lerp(300f, 80f, t);
            float sine = Mathf.Sin(2f * Mathf.PI * freq * t * 0.5f);
            float noise = (Random.value * 2f - 1f) * 0.15f;
            float envelope = Mathf.Pow(1f - t, 3f); // Çok hızlı sönüm
            data[i] = (sine * 0.7f + noise) * envelope * 0.7f;
        }

        var clip = AudioClip.Create("KickSound", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    /// <summary>
    /// İsabet sesi — ete kılıç/yumruk çarpma sesi
    /// </summary>
    public static AudioClip GenerateHitImpact()
    {
        int sampleRate = 44100;
        float duration = 0.2f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            // Sert ve tok bir çarpma hissi
            float freq1 = Mathf.Lerp(500f, 100f, t);
            float freq2 = Mathf.Lerp(1200f, 200f, t);
            float sine1 = Mathf.Sin(2f * Mathf.PI * freq1 * t * 0.4f);
            float sine2 = Mathf.Sin(2f * Mathf.PI * freq2 * t * 0.2f) * 0.5f;
            float noise = (Random.value * 2f - 1f) * 0.4f;
            float envelope = Mathf.Pow(1f - t, 4f); // Anlık patlama
            data[i] = (sine1 + sine2 + noise) * envelope * 0.5f;
        }

        var clip = AudioClip.Create("HitImpact", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }

    /// <summary>
    /// Düşman ölüm sesi — dramatik düşüş efekti
    /// </summary>
    public static AudioClip GenerateEnemyDeath()
    {
        int sampleRate = 44100;
        float duration = 0.5f;
        int samples = (int)(sampleRate * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float freq = Mathf.Lerp(600f, 50f, t); // Dramatik düşüş
            float sine = Mathf.Sin(2f * Mathf.PI * freq * t * 0.3f);
            float noise = (Random.value * 2f - 1f) * 0.2f;
            float envelope = Mathf.Pow(1f - t, 1.5f);
            data[i] = (sine * 0.6f + noise) * envelope * 0.5f;
        }

        var clip = AudioClip.Create("EnemyDeath", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
