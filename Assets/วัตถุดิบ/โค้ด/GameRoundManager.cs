using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameRoundManager : MonoBehaviour
{
    [Header("Round")]
    [SerializeField, Min(1f)] private float roundSeconds = 180f;

    [Header("References")]
    [SerializeField] private CustomerSpawner customerSpawner;
    [SerializeField] private GameWallet wallet;

    [Header("Gameplay UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;

    private float remainingTime;
    private bool roundRunning;
    private bool hasStarted;
    private bool ready;

    private static bool restartRequested;
    private bool startImmediately;

    private void Awake()
    {
        startImmediately = restartRequested;
        restartRequested = false;

        if (customerSpawner != null)
            customerSpawner.enabled = false;

        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(!startImmediately);

        SetGameplayUI(false);
    }

    private void Start()
    {
        ready =
            customerSpawner != null &&
            wallet != null &&
            timerText != null &&
            moneyText != null &&
            resultPanel != null &&
            resultText != null &&
            mainMenuPanel != null;

        if (!ready)
        {
            Debug.LogError(
                "GameRoundManager: ใส่ช่องอ้างอิงให้ครบ",
                this
            );

            return;
        }

        if (startImmediately)
            StartGame();
    }

    private void SetGameplayUI(bool visible)
    {
        if (timerText != null)
            timerText.gameObject.SetActive(visible);

        if (moneyText != null)
            moneyText.gameObject.SetActive(visible);
    }

    public void StartGame()
    {
        if (!ready || hasStarted)
            return;

        hasStarted = true;

        // เริ่มรอบด้วยเงิน 50 บาท
        wallet.BeginRound();

        remainingTime = Mathf.Max(1f, roundSeconds);

        mainMenuPanel.SetActive(false);
        resultPanel.SetActive(false);
        SetGameplayUI(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        roundRunning = true;
        customerSpawner.enabled = true;

        RefreshTimer();
    }

    private void Update()
    {
        if (!roundRunning)
            return;

        // เงินหมด ให้จบเกม
        if (wallet.Money <= 0)
        {
            EndRound();
            return;
        }

        remainingTime = Mathf.Max(
            0f,
            remainingTime - Time.deltaTime
        );

        RefreshTimer();

        // เวลาหมด ให้จบรอบ
        if (remainingTime <= 0f)
            EndRound();
    }

    private void RefreshTimer()
    {
        int total = Mathf.CeilToInt(remainingTime);

        timerText.text =
            $"Time: {total / 60:00}:{total % 60:00}";
    }

    private void EndRound()
    {
        if (!roundRunning)
            return;

        roundRunning = false;

        // ปิดการเพิ่มและหักเงินเมื่อจบรอบ
        wallet.FinishRound();
        customerSpawner.StopSpawning();

        bool outOfMoney = wallet.Money <= 0;

        timerText.text = outOfMoney
            ? "Game Over"
            : "Shop Closed";

        resultText.text = outOfMoney
            ? "GAME OVER\n\nOut of Money\nTotal Money: 0"
            : $"SHOP CLOSED\n\nTotal Money: {wallet.Money}";

        resultPanel.SetActive(true);

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.buildIndex < 0)
        {
            Debug.LogError(
                "เพิ่มฉากเกมใน Build Profiles > Scene List ก่อน",
                this
            );

            return;
        }

        restartRequested = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("ออกจากเกม — จะปิดแอปเมื่อเล่นจากไฟล์ Build");
        Application.Quit();
    }
}