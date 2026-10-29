using UnityEngine;

public class MovementFootsteps : MonoBehaviour
{
    [Header("Movement Target")]
    [SerializeField] private Transform movementTarget;
    [SerializeField] private CustomerMovement customer;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Footsteps - Use A Single Step Clip")]
    [SerializeField]
    private WorldSound sound =
        WorldSound.PlayerFootstep;

    [SerializeField] private AudioClip overrideClip;

    [SerializeField, Min(0.05f)]
    private float stepDistance = 0.55f;

    [SerializeField, Min(0.001f)]
    private float minimumSpeed = 0.05f;

    [SerializeField, Min(0.1f)]
    private float teleportThreshold = 1.5f;

    [SerializeField] private bool requireGrounded = true;

    [Header("3D Audio")]
    [SerializeField, Range(0f, 1f)]
    private float volume = 0.55f;

    [SerializeField, Min(0.01f)]
    private float minDistance = 1f;

    [SerializeField, Min(0.02f)]
    private float maxDistance = 8f;

    private AudioSource source;
    private Vector3 previousPosition;
    private float distanceSinceStep;
    private bool wasMoving;

    private void Start()
    {
        if (customer == null)
            customer = GetComponentInParent<CustomerMovement>();

        if (movementTarget == null)
        {
            movementTarget = customer != null
                ? customer.transform
                : transform;
        }

        if (playerController == null)
        {
            playerController =
                movementTarget.GetComponent<CharacterController>();
        }

        source = WorldAudio.CreateSource(
            movementTarget,
            "Footsteps"
        );

        source.loop = false;
        previousPosition = movementTarget.position;
    }

    private void LateUpdate()
    {
        if (movementTarget == null || source == null)
            return;

        Vector3 currentPosition = movementTarget.position;
        Vector3 delta = currentPosition - previousPosition;

        previousPosition = currentPosition;
        delta.y = 0f;

        float distance = delta.magnitude;

        bool blocked =
            Time.deltaTime <= 0f ||
            (mainMenuPanel != null &&
                mainMenuPanel.activeInHierarchy) ||
            (customer != null &&
                (!customer.isActiveAndEnabled ||
                 !customer.IsWalking)) ||
            (requireGrounded &&
                playerController != null &&
                playerController.enabled &&
                !playerController.isGrounded);

        bool moving =
            !blocked &&
            distance <= teleportThreshold &&
            distance > minimumSpeed * Time.deltaTime;

        if (!moving)
        {
            if (wasMoving)
                source.Stop();

            wasMoving = false;
            distanceSinceStep = 0f;
            return;
        }

        if (!wasMoving)
        {
            PlayStep();
            distanceSinceStep = 0f;
        }
        else
        {
            distanceSinceStep += distance;

            if (distanceSinceStep >= stepDistance)
            {
                distanceSinceStep %=
                    Mathf.Max(0.05f, stepDistance);

                PlayStep();
            }
        }

        wasMoving = true;
    }

    private void PlayStep()
    {
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

    private void OnEnable()
    {
        wasMoving = false;
        distanceSinceStep = 0f;

        if (movementTarget != null)
            previousPosition = movementTarget.position;
    }

    private void OnDisable()
    {
        if (source != null)
            source.Stop();

        wasMoving = false;
        distanceSinceStep = 0f;
    }
}