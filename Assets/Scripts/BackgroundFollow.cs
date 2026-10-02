using UnityEngine;

/// <summary>
/// Bu script arka planı her zaman kameranın tam önüne yerleştirir.
/// Böylece oyuncu nereye giderse gitsin arka plan ekranı kaplar.
/// </summary>
public class BackgroundFollow : MonoBehaviour
{
    Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null) mainCam = FindObjectOfType<Camera>();
        if (mainCam == null) return;

        // Kameranın child'ı yap - kamera ne kadar hızlı hareket ederse etsin arka plan gram titremez
        transform.SetParent(mainCam.transform);
    }
        
    void LateUpdate()
    {
        if (mainCam == null) return;

        // Kameranın child'ı yap (sabitlik için)
        if (transform.parent != mainCam.transform)
        {
            transform.SetParent(mainCam.transform);
        }

        // Sabit Z konumu ve rotasyon
        transform.localPosition = new Vector3(0f, 0f, 50f);
        transform.localRotation = Quaternion.identity;

        FitToCamera();
    }

    void FitToCamera()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (mainCam == null || sr == null || sr.sprite == null) return;

        // Kamera görüş alanını hesapla
        float camH = mainCam.orthographicSize * 2f;
        float camW = camH * mainCam.aspect;

        // Sprite doğal boyutunu al (world units)
        float sprW = sr.sprite.bounds.size.x;
        float sprH = sr.sprite.bounds.size.y;

        // Tüm ekranı kaplayacak ölçek (X ve Y bağımsız olarak ekrana sığdırılır, böylece resmin tamamı görünür)
        float scaleX = camW / sprW;
        float scaleY = camH / sprH;

        transform.localScale = new Vector3(scaleX, scaleY, 1f);
        
        sr.sortingOrder = -9999;
    }
}
