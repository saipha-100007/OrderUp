using UnityEngine;

public class VRMenuDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject settingsPanel;

    private int originalMask;
    private CameraClearFlags originalClearFlags;
    private Color originalBackground;
    private bool cameraChanged;

    private void Awake()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError(
                "VRMenuDisplay: ใส่ Player Camera",
                this
            );
            return;
        }

        originalMask = playerCamera.cullingMask;
        originalClearFlags = playerCamera.clearFlags;
        originalBackground = playerCamera.backgroundColor;

        playerCamera.cullingMask = LayerMask.GetMask("UI");
        playerCamera.clearFlags = CameraClearFlags.SolidColor;
        playerCamera.backgroundColor = Color.black;

        cameraChanged = true;
    }

    private void OnDisable()
    {
        // ล้างรูปเคอร์เซอร์ที่แพ็กเมนูตั้งไว้ แล้วซ่อนเมาส์
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!cameraChanged || playerCamera == null)
            return;

        playerCamera.cullingMask = originalMask;
        playerCamera.clearFlags = originalClearFlags;
        playerCamera.backgroundColor = originalBackground;

        cameraChanged = false;
    }

    public void OpenSettings()
    {
        if (settingsPanel == null)
            return;

        settingsPanel.transform.SetAsLastSibling();
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}