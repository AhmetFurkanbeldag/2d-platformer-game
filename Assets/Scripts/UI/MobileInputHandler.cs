using UnityEngine;
using UnityEngine.EventSystems;
using Platformer.Mechanics;

public class MobileInputHandler : MonoBehaviour
{
    public PlayerController player;
    
    // Joystick state
    private Vector2 joystickStartPos;
    private RectTransform joystickKnob;
    private float joystickMaxRadius = 50f;
    private bool isDragging = false;

    // Her sahne değişiminde karakteri kaybedersek tekrar bulmak için
    void Update()
    {
        if (player == null)
        {
            player = Object.FindFirstObjectByType<PlayerController>();
        }

        if (player == null) return;
        
        if (!isDragging)
        {
            player.mobileMoveX = 0f;
        }
    }

    // --- JOYSTICK ---
    public void OnJoystickPointerDown(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        joystickStartPos = pointerData.position;
        isDragging = true;
        joystickKnob = pointerData.pointerPress.transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnJoystickDrag(BaseEventData data)
    {
        if (player == null) return;
        PointerEventData pointerData = data as PointerEventData;
        
        Vector2 delta = pointerData.position - joystickStartPos;
        delta = Vector2.ClampMagnitude(delta, joystickMaxRadius);
        
        if (joystickKnob != null)
        {
            joystickKnob.anchoredPosition = delta;
        }

        // -1 ile 1 arası normalize yön
        player.mobileMoveX = delta.x / joystickMaxRadius;
    }

    public void OnJoystickPointerUp(BaseEventData data)
    {
        isDragging = false;
        if (player != null) player.mobileMoveX = 0f;
        if (joystickKnob != null) joystickKnob.anchoredPosition = Vector2.zero;
    }

    // --- ZIPLAMA ---
    public void OnJumpPointerDown(BaseEventData data)
    {
        if (player != null) player.mobileJumpPressed = true;
    }

    public void OnJumpPointerUp(BaseEventData data)
    {
        if (player != null) player.mobileJumpReleased = true;
    }

    // --- KILIÇ ---
    public void OnSwordPointerDown(BaseEventData data)
    {
        if (player != null) player.mobileSwordPressed = true;
    }
}
