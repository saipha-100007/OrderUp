using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuAudioController : MonoBehaviour
{
    [Header("Start Menu Only")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Sounds")]
    [SerializeField] private AudioClip sdPop;
    [SerializeField] private AudioClip transition;
    [SerializeField] private AudioClip sdNpc1;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)]
    private float uiVolume = 0.7f;

    [SerializeField, Range(0f, 1f)]
    private float voiceVolume = 0.7f;

    private AudioSource hoverSource;
    private AudioSource clickSource;
    private AudioSource voiceSource;

    private readonly HashSet<Button> registeredButtons =
        new HashSet<Button>();

    private bool clickInProgress;
    private bool optionsWereOpen;
    private bool musicStarted;

    private void Awake()
    {
        hoverSource = gameObject.AddComponent<AudioSource>();
        clickSource = gameObject.AddComponent<AudioSource>();
        voiceSource = gameObject.AddComponent<AudioSource>();

        AudioSource[] sources =
        {
            hoverSource,
            clickSource,
            voiceSource
        };

        foreach (AudioSource source in sources)
        {
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
        }

        hoverSource.volume = uiVolume;
        clickSource.volume = uiVolume;
        voiceSource.volume = voiceVolume;
    }

    private void Start()
    {
        if (mainMenuPanel == null || optionsPanel == null)
        {
            Debug.LogError(
                "MenuAudioController: ใส่ Main Menu Panel และ Options Panel ให้ครบ",
                this
            );

            enabled = false;
            return;
        }

        RegisterButtons(mainMenuPanel);
        RegisterButtons(optionsPanel);
    }

    private void RegisterButtons(GameObject panel)
    {
        Button[] buttons =
            panel.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (!registeredButtons.Add(button))
                continue;

            EventTrigger trigger =
                button.GetComponent<EventTrigger>();

            if (trigger == null)
                trigger = button.gameObject.AddComponent<EventTrigger>();

            if (trigger.triggers == null)
                trigger.triggers = new List<EventTrigger.Entry>();

            EventTrigger.Entry hoverEntry =
                new EventTrigger.Entry();

            hoverEntry.eventID = EventTriggerType.PointerEnter;

            hoverEntry.callback.AddListener(_ =>
            {
                if (!clickInProgress &&
                    button.IsInteractable() &&
                    sdPop != null)
                {
                    hoverSource.PlayOneShot(sdPop);
                }
            });

            trigger.triggers.Add(hoverEntry);

            Button.ButtonClickedEvent originalAction =
                button.onClick;

            Button.ButtonClickedEvent wrappedAction =
                new Button.ButtonClickedEvent();

            wrappedAction.AddListener(() =>
            {
                if (!clickInProgress && button.IsInteractable())
                    StartCoroutine(ClickThenAct(originalAction));
            });

            button.onClick = wrappedAction;
        }
    }

    private IEnumerator ClickThenAct(
        Button.ButtonClickedEvent originalAction)
    {
        clickInProgress = true;
        hoverSource.Stop();

        if (transition != null)
        {
            clickSource.clip = transition;
            clickSource.Play();

            yield return new WaitForSecondsRealtime(
                transition.length
            );
        }

        clickInProgress = false;
        originalAction.Invoke();
    }

    private void LateUpdate()
    {
        // เริ่ม Sunrise เมื่อปิดหน้าเริ่มเกมเพื่อเข้าเล่น
        if (!musicStarted &&
            mainMenuPanel != null &&
            !mainMenuPanel.activeInHierarchy &&
            GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.StartGameplayMusic();
            musicStarted = true;
        }

        // เล่นเสียง NPC หนึ่งครั้งต่อการเปิด Options
        bool optionsOpen =
            optionsPanel != null &&
            optionsPanel.activeInHierarchy;

        if (optionsOpen == optionsWereOpen)
            return;

        optionsWereOpen = optionsOpen;
        voiceSource.Stop();

        if (optionsOpen && sdNpc1 != null)
            voiceSource.PlayOneShot(sdNpc1);
    }
}