using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class MenuFontManager : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset menuFont;
    private TMP_FontAsset currentFontPrevious;

    public void UpdateFonts()
    {
        var textList = FindObjectsOfType<TextMeshProUGUI>();
        var worldSpaceTextList = FindObjectsOfType<TextMeshPro>();

        foreach ( var text in textList )
        {
            text.font = menuFont;
        }
        foreach ( var worldSpaceText in worldSpaceTextList )
        {
            worldSpaceText.font = menuFont;
        }
    }

    private void Update()
    {
        if(menuFont != currentFontPrevious)
        {
            UpdateFonts();
            currentFontPrevious = menuFont;
        }
    }
}
