using UnityEngine;

public class BurgerPatty : MonoBehaviour
{
    public enum CookState
    {
        Raw,
        Ripe,
        Buen
    }

    public CookState currentState;

    public GameObject meatRaw;
    public GameObject meatRipe;
    public GameObject meatBuen;

    public float ripeTime = 5f;
    public float buenTime = 10f;

    private float cookingTime = 0f;
    private bool isCooking = false;
    private bool fryingSoundPlayed = false;

    private void Start()
    {
        SetRaw();
    }

    private void Update()
    {
        if (!isCooking || currentState == CookState.Buen)
            return;

        cookingTime += Time.deltaTime;

        if (cookingTime >= buenTime)
        {
            SetBuen();
        }
        else if (cookingTime >= ripeTime &&
                 currentState == CookState.Raw)
        {
            SetRipe();
        }
    }

    public void StartCooking()
    {
        if (currentState == CookState.Buen)
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

        meatRaw.SetActive(true);
        meatRipe.SetActive(false);
        meatBuen.SetActive(false);
    }

    private void SetRipe()
    {
        currentState = CookState.Ripe;

        meatRaw.SetActive(false);
        meatRipe.SetActive(true);
        meatBuen.SetActive(false);
    }

    private void SetBuen()
    {
        if (currentState == CookState.Buen)
            return;

        currentState = CookState.Buen;
        isCooking = false;

        meatRaw.SetActive(false);
        meatRipe.SetActive(false);
        meatBuen.SetActive(true);

        if (GameAudioManager.Instance != null)
            GameAudioManager.Instance.PlayBurnt();
    }

    public bool IsCooked()
    {
        return currentState == CookState.Ripe;
    }
}