using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SetupMobileUI : EditorWindow
{
    [MenuItem("Tools/4 - MOBİL KONTROLLERİ VE YATAY EKRANI DÜZELT", false, 4)]
    public static void CreateMobileUI()
    {
        // 1. Ekranı Yatay Kilit Yap
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;

        // Önceki mobil UI varsa temizle
        var oldMobile = GameObject.Find("MobileCanvas");
        if (oldMobile != null) DestroyImmediate(oldMobile);

        // 2. Mobile Canvas Oluştur
        GameObject canvasObj = new GameObject("MobileCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // En üstte görünsün

        // Sahne değişince silinmesin diye DontDestroyOnLoad için küçük bir script ekliyoruz
        canvasObj.AddComponent<PersistCanvas>();

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();

        // MobileInputHandler'ı Ekle (Reflection ile, Assembly hatasını önlemek için)
        System.Type handlerType = System.Type.GetType("MobileInputHandler, Assembly-CSharp");
        MonoBehaviour handler = (MonoBehaviour)canvasObj.AddComponent(handlerType);

        // 3. EventSystem kontrolü (Sahnede yoksa ekle)
        GameObject esObj = GameObject.Find("EventSystem");
        if (esObj == null)
        {
            esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }
        // EventSystem'i de silinmez yap ki sahneler arası butonlar tıklanabilsin
        if (esObj.GetComponent<PersistCanvas>() == null)
            esObj.AddComponent<PersistCanvas>();

        // 4. Joystick Arka Planı
        GameObject joyBg = new GameObject("JoystickBG");
        joyBg.transform.SetParent(canvasObj.transform, false);
        Image bgImg = joyBg.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.4f);
        RectTransform bgRT = joyBg.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 0);
        bgRT.anchorMax = new Vector2(0, 0);
        bgRT.pivot = new Vector2(0.5f, 0.5f);
        bgRT.anchoredPosition = new Vector2(300, 300);
        bgRT.sizeDelta = new Vector2(250, 250);

        // Joystick Topu
        GameObject joyKnob = new GameObject("JoystickKnob");
        joyKnob.transform.SetParent(joyBg.transform, false);
        Image knobImg = joyKnob.AddComponent<Image>();
        knobImg.color = new Color(1, 1, 1, 0.8f);
        RectTransform knobRT = joyKnob.GetComponent<RectTransform>();
        knobRT.sizeDelta = new Vector2(100, 100);

        // Joystick EventTrigger
        EventTrigger joyTrigger = joyBg.AddComponent<EventTrigger>();
        AddEventTrigger(joyTrigger, EventTriggerType.PointerDown, handler, "OnJoystickPointerDown");
        AddEventTrigger(joyTrigger, EventTriggerType.Drag, handler, "OnJoystickDrag");
        AddEventTrigger(joyTrigger, EventTriggerType.PointerUp, handler, "OnJoystickPointerUp");

        // 5. Kılıç Butonu (Kırmızımsı)
        GameObject swordBtn = CreateButton("SwordButton", canvasObj.transform, new Vector2(-400, 250), new Color(0.8f, 0.2f, 0.2f, 0.8f), "KILIÇ");
        EventTrigger swordTrigger = swordBtn.AddComponent<EventTrigger>();
        AddEventTrigger(swordTrigger, EventTriggerType.PointerDown, handler, "OnSwordPointerDown");

        // 6. Zıplama Butonu (Mavimsi)
        GameObject jumpBtn = CreateButton("JumpButton", canvasObj.transform, new Vector2(-200, 450), new Color(0.2f, 0.5f, 0.8f, 0.8f), "ZIPLA");
        EventTrigger jumpTrigger = jumpBtn.AddComponent<EventTrigger>();
        AddEventTrigger(jumpTrigger, EventTriggerType.PointerDown, handler, "OnJumpPointerDown");
        AddEventTrigger(jumpTrigger, EventTriggerType.PointerUp, handler, "OnJumpPointerUp");

        Debug.Log(">>> EKRAN YATAY MODA SABİTLENDİ VE MOBİL KONTROLLER OLUŞTURULDU! <<<");
    }

    private static GameObject CreateButton(string name, Transform parent, Vector2 pos, Color color, string text)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        Image img = btnObj.AddComponent<Image>();
        img.color = color;
        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0); // Sağ alt
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(200, 200);

        // Yazı ekle
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        Text t = txtObj.AddComponent<Text>();
        t.text = text;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t.fontSize = 40;
        t.fontStyle = FontStyle.Bold;
        RectTransform trt = txtObj.GetComponent<RectTransform>();
        trt.sizeDelta = rt.sizeDelta;

        return btnObj;
    }

    private static void AddEventTrigger(EventTrigger trigger, EventTriggerType type, MonoBehaviour handler, string methodName)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => {
            handler.GetType().GetMethod(methodName).Invoke(handler, new object[] { data });
        });
        trigger.triggers.Add(entry);
    }
}
