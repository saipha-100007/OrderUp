using UnityEngine;

public class HotDogSausage : MonoBehaviour
{
    public enum CookState
    {
        Raw,
        Ripe,
        Burn
    }

    public CookState currentState;

    [Header("Sausage Visual")]
    public GameObject sausageRaw;
    public GameObject sausageRipe;
    public GameObject sausageBurn;

    [Header("Cooking Time")]
    public float ripeTime = 5f;
    public float burnTime = 10f;

    private float cookingTime = 0f;
    private bool isCooking = false;
    private bool fryingSoundPlayed = false;

    private void Start()
    {
        SetRaw();
    }

    private void Update()
    {
        if (!isCooking || currentState == CookState.Burn)
            return;

        cookingTime += Time.deltaTime;

        if (cookingTime >= burnTime)
        {
            SetBurn();
        }
        else if (cookingTime >= ripeTime &&
                 currentState == CookState.Raw)
        {
            SetRipe();
        }
    }

    public void StartCooking()
    {
        if (currentState == CookState.Burn)
            return;

        isCooking = true;

        if (!fryingSoundPlayed)
        {
            fryingSoundPlayed = true;

            if (GameAudioManager.Instance != null)
                GameAudioManager.Instance.PlayFrying();
        }
    }

    public void StopCooking()
    {
        isCooking = false;
    }

    private void SetRaw()
    {
        currentState = CookState.Raw;

        sausageRaw.SetActive(true);
        sausageRipe.SetActive(false);
        sausageBurn.SetActive(false);
    }

    private void SetRipe()
    {
        currentState = CookState.Ripe;

        sausageRaw.SetActive(false);
        sausageRipe.SetActive(true);
        sausageBurn.SetActive(false);
    }

    private void SetBurn()
    {
        if (currentState == CookState.Burn)
            return;

        currentState = CookState.Burn;
        isCooking = false;

        sausageRaw.SetActive(false);
        sausageRipe.SetActive(false);
        sausageBurn.SetActive(true);

        if (GameAudioManager.Instance != null)
            GameAudioManager.Instance.PlayBurnt();
    }

    public bool IsCooked()
    {
        return currentState == CookState.Ripe;
    }
}