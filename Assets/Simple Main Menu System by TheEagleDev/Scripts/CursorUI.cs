using UnityEngine;

public class CursorUI : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public void OnPointerEnter()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    public void OnPointerExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}