using UnityEngine;
using System.Collections;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Kılıç savurulduğunda beyaz yarım ay şeklinde (slash) iz efekti oluşturur.
    /// Tamamen kod üzerinden (LineRenderer ile) çizilir, harici materyal gerektirmez.
    /// </summary>
    public class SwordSlashEffect : MonoBehaviour
    {
        private SpriteRenderer sr;

        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public void PlaySlash()
        {
            StartCoroutine(SlashRoutine());
        }

        IEnumerator SlashRoutine()
        {
            // Efekti taşıyacak boş obje
            GameObject slashObj = new GameObject("SlashEffect");
            slashObj.transform.position = transform.position;
            slashObj.transform.SetParent(transform);

            LineRenderer lr = slashObj.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            
            // Temel beyaz/şeffaf materyal ataması
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = new Color(1f, 1f, 1f, 0.8f);
            lr.endColor = new Color(1f, 1f, 1f, 0f); // Uca doğru tamamen şeffaflaşsın
            
            // Yarım ay (kavis) şeklini hesapla
            int segments = 15;
            lr.positionCount = segments;
            
            // Oyuncu sola bakıyorsa kılıç da sola savrulur
            float direction = (sr != null && sr.flipX) ? -1f : 1f;
            
            // Açısal yay (yukarıdan aşağıya veya aşağıdan yukarıya doğru savrulma)
            float startAngle = -50f * Mathf.Deg2Rad;
            float endAngle = 70f * Mathf.Deg2Rad;
            float radius = 1.8f; // Kılıç izinin genişliği

            for (int i = 0; i < segments; i++)
            {
                float t = (float)i / (segments - 1);
                float angle = Mathf.Lerp(startAngle, endAngle, t);
                
                float x = Mathf.Cos(angle) * radius * direction;
                float y = Mathf.Sin(angle) * radius + 0.6f; // Biraz yukarıda (belden yukarı)

                lr.SetPosition(i, new Vector3(x, y, 0f));
            }

            // Çizginin kalınlığını ortada şişman, uçlarda ince yap (Kılıç kesiği şekli)
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 0.05f); // Başlangıç ince
            curve.AddKey(0.5f, 0.5f);  // Orta kalın
            curve.AddKey(1f, 0.05f); // Bitiş ince
            lr.widthCurve = curve;
            lr.widthMultiplier = 1f;

            // Efektin çok hızlı bir şekilde yok olması (0.15 saniye)
            float duration = 0.15f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = 1f - (elapsed / duration); // 1'den 0'a
                
                lr.startColor = new Color(1f, 1f, 1f, alpha * 0.8f);
                lr.endColor = new Color(1f, 1f, 1f, 0f);
                
                // Efekt yok olurken aynı zamanda hafifçe dışa doğru büyüsün (savrulma hissi)
                float scale = 1f + (elapsed / duration) * 0.4f;
                slashObj.transform.localScale = new Vector3(scale, scale, 1f);
                
                yield return null;
            }

            Destroy(slashObj);
        }
    }
}
