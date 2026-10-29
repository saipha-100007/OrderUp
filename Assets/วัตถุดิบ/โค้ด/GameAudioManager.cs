using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("Music and Ambience")]
    [SerializeField] private AudioClip sunrise;
    [SerializeField] private AudioClip ambientCity;

    [Header("Original Sound Effects")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip fryingSound;
    [SerializeField] private AudioClip burntSound;
    [SerializeField] private AudioClip orderSound;
    [SerializeField] private AudioClip patienceWarningSound;
    [SerializeField] private AudioClip patienceExpiredSound;
    [SerializeField] private AudioClip trashSound;

    [Header("Map Audio Clips")]
    [SerializeField] private AudioClip playerFootstep;
    [SerializeField] private AudioClip customerFootstep;
    [SerializeField] private AudioClip cloth;
    [SerializeField] private AudioClip ceramicPickup;
    [SerializeField] private AudioClip pepperShake;
    [SerializeField] private AudioClip mayonnaisePickup;
    [SerializeField] private AudioClip ketchupPickup;
    [SerializeField] private AudioClip friesCooking;
    [SerializeField] private AudioClip grillIdle;
    [SerializeField] private AudioClip grillCooking;
    [SerializeField] private AudioClip assemblyComplete;
    [SerializeField] private AudioClip moneyReceived;
    [SerializeField] private AudioClip npcHappy;

    [Header("Legacy Pickup - Keep Off To Avoid Double Sounds")]
    [SerializeField] private bool enableLegacyPickupSound = false;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)]
    private float musicVolume = 0.15f;

    [SerializeField, Range(0f, 1f)]
    private float ambienceVolume = 0.2f;

    [SerializeField, Range(0f, 1f)]
    private float effectsVolume = 0.7f;

    [Header("Hands - Optional")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    private AudioSource musicSource;
    private AudioSource ambienceSource;
    private AudioSource effectsSource;
    private AudioSource leftPickupSource;
    private AudioSource rightPickupSource;

    public float WorldEffectsVolume => effectsVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        musicSource = CreateSource("Music", transform, false);
        ambienceSource = CreateSource("Ambience", transform, false);
        effectsSource = CreateSource("Effects", transform, false);

        if (leftHand != null)
        {
            leftPickupSource =
                CreateSource("Pickup Audio", leftHand, true);
        }

        if (rightHand != null)
        {
            rightPickupSource =
                CreateSource("Pickup Audio", rightHand, true);
        }

        ApplyVolumes();
    }

    private void Start()
    {
        // Sunrise รอให้ MenuAudioController เรียกเมื่อเริ่มเกม
        PlayLoop(ambienceSource, ambientCity);
    }

    public void StartGameplayMusic()
    {
        if (musicSource == null || musicSource.isPlaying)
            return;

        PlayLoop(musicSource, sunrise);
    }

    public void StopGameplayMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public AudioClip GetWorldClip(WorldSound sound)
    {
        switch (sound)
        {
            case WorldSound.PlayerFootstep:
                return playerFootstep;

            case WorldSound.CustomerFootstep:
                return customerFootstep;

            case WorldSound.Cloth:
                return cloth;

            case WorldSound.CeramicPickup:
                return ceramicPickup;

            case WorldSound.PepperShake:
                return pepperShake;

            case WorldSound.MayonnaisePickup:
                return mayonnaisePickup;

            case WorldSound.KetchupPickup:
                return ketchupPickup;

            case WorldSound.FriesCooking:
                return friesCooking;

            case WorldSound.GrillIdle:
                return grillIdle;

            case WorldSound.GrillCooking:
                return grillCooking;

            case WorldSound.AssemblyComplete:
                return assemblyComplete;

            case WorldSound.MoneyReceived:
                return moneyReceived;

            case WorldSound.NPCHappy:
                return npcHappy;

            default:
                return null;
        }
    }

    private AudioSource CreateSource(
        string sourceName,
        Transform parent,
        bool spatial)
    {
        GameObject audioObject = new GameObject(sourceName);
        audioObject.transform.SetParent(parent, false);

        AudioSource source =
            audioObject.AddComponent<AudioSource>();

        source.playOnAwake = false;
        source.spatialBlend = spatial ? 1f : 0f;
        source.dopplerLevel = 0f;
        source.minDistance = 0.3f;
        source.maxDistance = 10f;
        source.rolloffMode = AudioRolloffMode.Linear;

        return source;
    }

    private void PlayLoop(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null)
            return;

        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    private void PlayEffect(
        AudioClip clip,
        AudioSource source = null)
    {
        if (clip == null)
            return;

        AudioSource target =
            source != null ? source : effectsSource;

        if (target != null)
            target.PlayOneShot(clip);
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume;

        if (ambienceSource != null)
            ambienceSource.volume = ambienceVolume;

        if (effectsSource != null)
            effectsSource.volume = effectsVolume;

        if (leftPickupSource != null)
            leftPickupSource.volume = effectsVolume;

        if (rightPickupSource != null)
            rightPickupSource.volume = effectsVolume;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            ApplyVolumes();
    }

    public void PlayPickupLeft()
    {
        if (enableLegacyPickupSound)
            PlayEffect(pickupSound, leftPickupSource);
    }

    public void PlayPickupRight()
    {
        if (enableLegacyPickupSound)
            PlayEffect(pickupSound, rightPickupSource);
    }

    public void PlayFrying()
    {
        // เก็บเมธอดไว้ให้โค้ดอาหารเดิมเรียกได้
        // เสียงทอดใหม่ควบคุมโดย CookingStation
    }

    public void PlayBurnt()
    {
        PlayEffect(burntSound);
    }

    public void PlayOrder()
    {
        PlayEffect(orderSound);
    }

    public void PlayPatienceWarning()
    {
        PlayEffect(patienceWarningSound);
    }

    public void PlayPatienceExpired()
    {
        PlayEffect(patienceExpiredSound);
    }

    public void PlayTrash()
    {
        PlayEffect(trashSound);
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Instance = null;

        if (leftPickupSource != null)
            Destroy(leftPickupSource.gameObject);

        if (rightPickupSource != null)
            Destroy(rightPickupSource.gameObject);
    }
}