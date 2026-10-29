using UnityEngine;

public class MenuRayVisibility : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject resultPanel;

    [Header("Line Visual Components Only")]
    [SerializeField] private Behaviour[] lineVisuals;

    [Header("Line Renderer Components")]
    [SerializeField] private LineRenderer[] lineRenderers;

    private bool previousVisible;
    private bool initialized;

    private void LateUpdate()
    {
        bool visible =
            (menuCanvas != null && menuCanvas.activeInHierarchy) ||
            (resultPanel != null && resultPanel.activeInHierarchy);

        if (initialized && visible == previousVisible)
            return;

        initialized = true;
        previousVisible = visible;

        if (lineVisuals != null)
        {
            foreach (Behaviour visual in lineVisuals)
            {
                if (visual != null)
                    visual.enabled = visible;
            }
        }

        if (lineRenderers != null)
        {
            foreach (LineRenderer line in lineRenderers)
            {
                if (line != null)
                    line.enabled = visible;
            }
        }
    }
}