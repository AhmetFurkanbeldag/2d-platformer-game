using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Platformer.UI;

namespace Platformer.EditorScripts
{
    public class ForceMainMenuBuilder
    {
        [MenuItem("Tools/1 - ANA MENUYU YARAT")]
        public static void BuildMenu()
        {
            string scenePath = "Assets/Scenes/MainMenu.unity";

            // Kaydedilmemiş değişiklikleri kaydet (popup çıkmasını engelle)
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                EditorSceneManager.SaveOpenScenes();
            }

            // Yeni tertemiz bir sahne oluştur
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // ================== KAMERA ==================
            GameObject cameraObj = new GameObject("Main Camera");
            Camera cam = cameraObj.AddComponent<Camera>();
            cam.backgroundColor = new Color(0.08f, 0.08f, 0.1f); 
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.orthographic = true;
            cameraObj.tag = "MainCamera";

            // ================== CANVAS ==================
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // ================== EVENTSYSTEM ==================
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

            // Controller
            MainMenuController controller = canvasObj.AddComponent<MainMenuController>();

            // ================== INTRO GRUBU ==================
            GameObject introGroupObj = new GameObject("IntroGroup");
            introGroupObj.transform.SetParent(canvasObj.transform, false);
            RectTransform introRect = introGroupObj.AddComponent<RectTransform>();
            introRect.anchorMin = Vector2.zero; introRect.anchorMax = Vector2.one; introRect.sizeDelta = Vector2.zero;
            CanvasGroup introGroup = introGroupObj.AddComponent<CanvasGroup>();

            // Siyah arka plan
            Image introBg = introGroupObj.AddComponent<Image>();
            introBg.color = new Color(0.12f, 0.12f, 0.12f, 1f); 

            // Font bul
            Font myFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (myFont == null) myFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            // MADE WITH Yazısı
            GameObject madeWithObj = new GameObject("MadeWith");
            madeWithObj.transform.SetParent(introGroupObj.transform, false);
            Text mwText = madeWithObj.AddComponent<Text>();
            mwText.text = "MADE\nWITH";
            mwText.font = myFont;
            mwText.fontSize = 40;
            mwText.alignment = TextAnchor.MiddleRight;
            mwText.fontStyle = FontStyle.Bold;
            mwText.color = Color.white;
            mwText.rectTransform.anchoredPosition = new Vector2(-180, 0);
            mwText.rectTransform.sizeDelta = new Vector2(200, 100);

            // Unity Yazısı
            GameObject unityTextObj = new GameObject("UnityText");
            unityTextObj.transform.SetParent(introGroupObj.transform, false);
            Text uText = unityTextObj.AddComponent<Text>();
            uText.text = "Unity";
            uText.font = myFont;
            uText.fontSize = 90;
            uText.alignment = TextAnchor.MiddleLeft;
            uText.fontStyle = FontStyle.Bold;
            uText.color = Color.white;
            uText.rectTransform.anchoredPosition = new Vector2(170, 0);
            uText.rectTransform.sizeDelta = new Vector2(300, 150);

            // Unity Logosunu Editor'ün içinden Cek ve PNG olarak kaydet
            string logoPath = "Assets/Resources/UnityLogo.png";
            Texture2D sourceTex = EditorGUIUtility.IconContent("d_UnityLogo").image as Texture2D;
            if (sourceTex != null)
            {
                RenderTexture rt = RenderTexture.GetTemporary(sourceTex.width, sourceTex.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
                Graphics.Blit(sourceTex, rt);
                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = rt;
                Texture2D readableTex = new Texture2D(sourceTex.width, sourceTex.height);
                readableTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                readableTex.Apply();
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(rt);

                byte[] bytes = readableTex.EncodeToPNG();
                if (!System.IO.Directory.Exists("Assets/Resources")) System.IO.Directory.CreateDirectory("Assets/Resources");
                System.IO.File.WriteAllBytes(logoPath, bytes);
                AssetDatabase.Refresh();

                TextureImporter importer = AssetImporter.GetAtPath(logoPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.alphaIsTransparency = true;
                    importer.SaveAndReimport();
                }
            }

            Sprite logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(logoPath);

            // Gerçek Orijinal Unity Logosu Objesi
            GameObject cubeObj = new GameObject("UnityLogoImg");
            cubeObj.transform.SetParent(introGroupObj.transform, false);
            Image cubeImg = cubeObj.AddComponent<Image>();
            cubeImg.sprite = logoSprite;
            cubeImg.preserveAspect = true;
            cubeImg.rectTransform.anchoredPosition = new Vector2(-15, 0);
            cubeImg.rectTransform.sizeDelta = new Vector2(100, 100);

            // ================== MENU GRUBU ==================
            GameObject menuGroupObj = new GameObject("MenuGroup");
            menuGroupObj.transform.SetParent(canvasObj.transform, false);
            RectTransform menuRect = menuGroupObj.AddComponent<RectTransform>();
            menuRect.anchorMin = Vector2.zero; menuRect.anchorMax = Vector2.one; menuRect.sizeDelta = Vector2.zero;
            CanvasGroup menuGroup = menuGroupObj.AddComponent<CanvasGroup>();

            // Arka Plan Fotoğrafı
            GameObject bgObj = new GameObject("MenuBackground");
            bgObj.transform.SetParent(menuGroupObj.transform, false);
            Image bgImg = bgObj.AddComponent<Image>();
            Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Environment/Sprites/JapanBackground.png");
            bgImg.sprite = bgSprite;
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one; bgRect.sizeDelta = Vector2.zero;
            
            // Bulanıklık / Karartma Katmanı 
            GameObject darkOverlay = new GameObject("DarkOverlay");
            darkOverlay.transform.SetParent(menuGroupObj.transform, false);
            Image overlayImg = darkOverlay.AddComponent<Image>();
            overlayImg.color = new Color(0f, 0f, 0f, 0.85f); // %85 karanlık
            RectTransform overlayRect = darkOverlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero; overlayRect.anchorMax = Vector2.one; overlayRect.sizeDelta = Vector2.zero;

            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(menuGroupObj.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "NINJA GAIDEN";
            titleText.font = myFont;
            titleText.fontSize = 120;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = new Color(0.9f, 0.2f, 0.2f);
            titleText.rectTransform.anchoredPosition = new Vector2(0, 250);
            titleText.rectTransform.sizeDelta = new Vector2(1200, 300);

            // Butonlar
            Button playBtn = CreateButton(menuGroupObj.transform, "Oyuna Basla", new Vector2(0, -50), myFont);
            UnityEngine.Events.UnityAction actionPlay = new UnityEngine.Events.UnityAction(controller.PlayGame);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(playBtn.onClick, actionPlay);

            Button controlsBtn = CreateButton(menuGroupObj.transform, "Kontroller", new Vector2(0, -200), myFont);
            UnityEngine.Events.UnityAction actionControls = new UnityEngine.Events.UnityAction(controller.ShowControls);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(controlsBtn.onClick, actionControls);

            // ================== KONTROLLER PANELI ==================
            GameObject panelObj = new GameObject("ControlsPanel");
            panelObj.transform.SetParent(menuGroupObj.transform, false);
            Image panelImg = panelObj.AddComponent<Image>();
            panelImg.color = new Color(0, 0, 0, 0.95f);
            RectTransform pRect = panelObj.GetComponent<RectTransform>();
            pRect.anchorMin = Vector2.zero; pRect.anchorMax = Vector2.one; pRect.sizeDelta = Vector2.zero;

            Text infoText = new GameObject("InfoText").AddComponent<Text>();
            infoText.transform.SetParent(panelObj.transform, false);
            infoText.text = "HAREKET: A / D\n\nZIPLAMA: Space\n\nDASH: Shift\n\nKILIC: Sol Tik\n\nTEKME: J";
            infoText.font = myFont;
            infoText.fontSize = 60;
            infoText.alignment = TextAnchor.MiddleCenter;
            infoText.color = Color.white;
            infoText.rectTransform.sizeDelta = new Vector2(1000, 800);

            Button closeBtn = CreateButton(panelObj.transform, "Geri", new Vector2(0, -400), myFont);
            UnityEngine.Events.UnityAction actionClose = new UnityEngine.Events.UnityAction(controller.HideControls);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(closeBtn.onClick, actionClose);

            panelObj.SetActive(false);

            // Bağlantıları yap
            controller.introGroup = introGroup;
            controller.menuGroup = menuGroup;
            controller.controlsPanel = panelObj;

            // Sahneyi kaydet
            EditorSceneManager.SaveScene(newScene, scenePath);

            // Build Settings güncelle (MainMenu ilk sırada, SampleScene ikinci sırada)
            var newScenes = new EditorBuildSettingsScene[2];
            newScenes[0] = new EditorBuildSettingsScene(scenePath, true);
            newScenes[1] = new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true);
            EditorBuildSettings.scenes = newScenes;

            Debug.Log(">>> MAIN MENU BASARIYLA OLUSTURULDU VE KAYDEDILDI! <<<");
        }

        private static Button CreateButton(Transform parent, string text, Vector2 pos, Font font)
        {
            GameObject btnObj = new GameObject(text + "Button");
            btnObj.transform.SetParent(parent, false);
            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            Button btn = btnObj.AddComponent<Button>();
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 100);
            rect.anchoredPosition = pos;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            Text btnText = textObj.AddComponent<Text>();
            btnText.text = text;
            btnText.font = font;
            btnText.fontSize = 45;
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.color = Color.white;
            btnText.rectTransform.sizeDelta = new Vector2(400, 100);

            return btn;
        }
    }
}
