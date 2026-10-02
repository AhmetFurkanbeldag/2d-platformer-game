using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Platformer.UI
{
    /// <summary>
    /// Ana menü ve intro ekranını kontrol eder.
    /// 1. Siyah arka planda "Made with Unity" yavaşça belirir ve kaybolur.
    /// 2. Ana Menü butonları belirir.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Intro Elements")]
        public CanvasGroup introGroup;
        
        [Header("Menu Elements")]
        public CanvasGroup menuGroup;
        public GameObject controlsPanel;

        private void Awake()
        {
            // Eğer inspector'dan atanmamışsa otomatik bul (çökme hatalarını engeller)
            if (introGroup == null) 
                introGroup = transform.Find("IntroGroup")?.GetComponent<CanvasGroup>();
                
            if (menuGroup == null) 
                menuGroup = transform.Find("MenuGroup")?.GetComponent<CanvasGroup>();
                
            if (controlsPanel == null)
            {
                var cp = menuGroup != null ? menuGroup.transform.Find("ControlsPanel") : transform.Find("MenuGroup/ControlsPanel");
                if (cp != null) controlsPanel = cp.gameObject;
            }
        }

        private void Start()
        {
            // Başlangıç durumu
            introGroup.alpha = 0f;
            menuGroup.alpha = 0f;
            menuGroup.interactable = false;
            menuGroup.blocksRaycasts = false;
            
            if (controlsPanel != null)
                controlsPanel.SetActive(false);

            StartCoroutine(IntroRoutine());
        }

        private IEnumerator IntroRoutine()
        {
            // 0.5 saniye bekle
            yield return new WaitForSeconds(0.5f);

            // Intro Fade In (1 saniye)
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                introGroup.alpha = Mathf.Clamp01(t);
                yield return null;
            }

            // 1 saniye ekranda kalsın
            yield return new WaitForSeconds(1f);

            // Intro Fade Out (1 saniye)
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                introGroup.alpha = Mathf.Clamp01(1f - t);
                yield return null;
            }

            // Menü Fade In (1 saniye)
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                menuGroup.alpha = Mathf.Clamp01(t);
                yield return null;
            }

            menuGroup.interactable = true;
            menuGroup.blocksRaycasts = true;
        }

        public void PlayGame()
        {
            // SampleScene (oyun sahnesi) yükle
            SceneManager.LoadScene("SampleScene");
        }

        public void ShowControls()
        {
            if (controlsPanel != null)
                controlsPanel.SetActive(true);
        }

        public void HideControls()
        {
            if (controlsPanel != null)
                controlsPanel.SetActive(false);
        }
    }
}
