using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private CursorTexture mouseUp;

    [SerializeField]
    private CursorTexture mouseDown;

    [SerializeField]
    private CursorMode cursorMode = CursorMode.Auto;

    private bool isPressed;

    void OnEnable()
    {
        isPressed = Mouse.current != null && Mouse.current.leftButton.isPressed;
        ApplyCursor();
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null)
            return;

        bool pressed = mouse.leftButton.isPressed;
        if (pressed != isPressed)
        {
            isPressed = pressed;
            ApplyCursor();
        }
    }

    private void ApplyCursor()
    {
        CursorTexture cursor = isPressed ? mouseDown : mouseUp;
        Cursor.SetCursor(cursor.texture, cursor.hotspot, cursorMode);
    }

    [Serializable]
    private struct CursorTexture
    {
        [SerializeField]
        public Texture2D texture;

        [SerializeField, Tooltip("Pixel offset from the texture's top-left corner that acts as the click point.")]
        public Vector2 hotspot;
    }
}
