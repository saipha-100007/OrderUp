using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WorldObjectAudio : MonoBehaviour
{
    public enum TriggerMode
    {
        GrabOrClick,
        Spawn,
        Manual
    }

    [Header("When and What")]
    [SerializeField]
    private TriggerMode triggerMode =
        TriggerMode.GrabOrClick;

    [SerializeField]
    private WorldSound sound =
        WorldSound.Cloth;

    [SerializeField] private AudioClip overrideClip;

    [Header("Interaction - Auto Finds On This Object")]
    [SerializeField] private XRBaseInteractable interactable;

    [Header("3D Audio")]
    [SerializeField, Range(0f, 1f)]
    private float volume = 0.7f;

    [SerializeField, Min(0.01f)]
    private float minDistance = 1f;

    [SerializeField, Min(0.02f)]
    private float maxDistance = 8f;

    private AudioSource source;

    private void Awake()
    {
        source = WorldAudio.CreateSource(
            transform,
            "Object Audio"
        );

        source.loop = false;

        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        if (triggerMode == TriggerMode.GrabOrClick &&
            interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelected);
        }
    }

    private void Start()
    {
        if (triggerMode == TriggerMode.Spawn)
        {
            WorldAudio.PlayAt(
                sound,
                transform.position,
                volume,
                minDistance,
                maxDistance,
                overrideClip
            );
        }
        else if (
            triggerMode == TriggerMode.GrabOrClick &&
            interactable == null)
        {
            Debug.LogWarning(
                "WorldObjectAudio: ใส่บนวัตถุเดียวกับ " +
                "XR Grab Interactable หรือ XR Simple Interactable",
                this
            );
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        // มือที่สองเข้ามาจับชิ้นเดิม ไม่เล่นเสียงซ้ำ
        if (interactable != null &&
            interactable.interactorsSelecting.Count > 1)
        {
            return;
        }

        Play();
    }

    public void Play()
    {
        if (!isActiveAndEnabled || source == null)
            return;

        AudioClip clip =
            WorldAudio.GetClip(sound, overrideClip);

        if (clip == null)
            return;

        WorldAudio.Configure(
            source,
            volume,
            minDistance,
            maxDistance
        );

        source.PlayOneShot(clip);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelected);

        if (source != null)
            source.Stop();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            WorldAudio.Configure(
                source,
                volume,
                minDistance,
                maxDistance
            );
        }
    }
}