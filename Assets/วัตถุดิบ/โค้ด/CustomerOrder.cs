using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CustomerMovement))]
public class CustomerOrder : MonoBehaviour
{
    public enum MenuType
    {
        Hamburger,
        Hotdog,
        Fries
    }

    [Header("Order")]
    [SerializeField] private MenuType requestedMenu = MenuType.Hamburger;

    [Header("Patience")]
    [SerializeField, Min(1f)] private float patienceSeconds = 60f;
    [SerializeField] private Slider patienceBar;

    [Header("Warning Sound")]
    [SerializeField, Range(0.01f, 0.99f)]
    private float warningThreshold = 0.2f;

    [Header("Display")]
    [SerializeField] private GameObject orderCanvas;
    [SerializeField] private TMP_Text orderText;
    [SerializeField] private Transform playerCamera;

    private CustomerMovement movement;
    private bool orderShown;
    private bool waitingStarted;
    private bool warningPlayed;
    private float remainingPatience;

    public MenuType RequestedMenu => requestedMenu;

    private void Awake()
    {
        movement = GetComponent<CustomerMovement>();

        if (orderCanvas == null || orderText == null)
        {
            Debug.LogError(
                "CustomerOrder: ใส่ Order Canvas และ Order Text ให้ครบ",
                this
            );
            enabled = false;
            return;
        }

        remainingPatience = Mathf.Max(1f, patienceSeconds);
        orderCanvas.SetActive(false);

        if (patienceBar != null)
        {
            patienceBar.minValue = 0f;
            patienceBar.maxValue = 1f;
            patienceBar.wholeNumbers = false;
            patienceBar.interactable = false;
            patienceBar.value = 1f;
        }
    }

    private void Update()
    {
        if (movement.IsWaiting)
        {
            if (!waitingStarted)
            {
                waitingStarted = true;

                if (GameAudioManager.Instance != null)
                    GameAudioManager.Instance.PlayOrder();

                Debug.Log("ลูกค้าสั่ง: " + requestedMenu, this);
            }

            remainingPatience = Mathf.Max(
                0f,
                remainingPatience - Time.deltaTime
            );

            if (remainingPatience <= 0f)
            {
                movement.Leave();

                if (GameAudioManager.Instance != null)
                    GameAudioManager.Instance.PlayPatienceExpired();

                Debug.Log("ลูกค้าหมดความอดทน เดินออกจากร้าน", this);
            }
            else if (!warningPlayed &&
                     remainingPatience <=
                     Mathf.Max(1f, patienceSeconds) * warningThreshold)
            {
                warningPlayed = true;

                if (GameAudioManager.Instance != null)
                    GameAudioManager.Instance.PlayPatienceWarning();
            }
        }

        bool shouldShow = movement.IsWaiting;

        if (shouldShow)
        {
            orderText.text = requestedMenu.ToString();

            if (patienceBar != null)
            {
                patienceBar.value =
                    remainingPatience / Mathf.Max(1f, patienceSeconds);
            }
        }

        if (shouldShow != orderShown)
        {
            orderCanvas.SetActive(shouldShow);
            orderShown = shouldShow;
        }
    }

    private void LateUpdate()
    {
        if (!orderShown || playerCamera == null)
            return;

        orderCanvas.transform.rotation = playerCamera.rotation;
    }

    public void Configure(MenuType menu, Transform cameraTransform)
    {
        requestedMenu = menu;
        playerCamera = cameraTransform;
    }
}