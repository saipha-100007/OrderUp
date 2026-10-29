using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    public enum CustomerState
    {
        Arriving,
        Waiting,
        Leaving
    }

    [Header("Scene Points")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform orderPoint;
    [SerializeField] private Transform exitPoint;

    [Header("Movement")]
    [SerializeField, Min(0.1f)]
    private float moveSpeed = 1.2f;

    [SerializeField, Min(1f)]
    private float turnSpeed = 360f;

    [Header("Customer Models")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private GameObject[] modelVariants;

    [Header("Happy Jump")]
    [SerializeField, Min(0f)]
    private float jumpHeight = 0.2f;

    [SerializeField, Min(0.05f)]
    private float jumpDuration = 0.3f;

    [SerializeField, Min(1)]
    private int jumpCount = 2;

    [Header("Serving Sounds - 3D")]
    [SerializeField, Range(0f, 1f)]
    private float moneySoundVolume = 0.7f;

    [SerializeField, Range(0f, 1f)]
    private float happySoundVolume = 0.8f;

    [SerializeField, Min(0.01f)]
    private float soundMinDistance = 1f;

    [SerializeField, Min(0.02f)]
    private float soundMaxDistance = 10f;

    private bool celebrating;
    private float celebrationTime;
    private Vector3 visualRestPosition;

    public CustomerState State { get; private set; }

    public bool IsWaiting => State == CustomerState.Waiting;

    public bool IsWalking { get; private set; }

    private void Start()
    {
        if (spawnPoint == null ||
            orderPoint == null ||
            exitPoint == null)
        {
            Debug.LogError(
                "Customer: กรุณาใส่ Spawn, Order และ Exit Point ให้ครบ",
                this
            );

            enabled = false;
            return;
        }

        SelectRandomModel();

        transform.position = spawnPoint.position;

        transform.rotation = Quaternion.Euler(
            0f,
            spawnPoint.eulerAngles.y,
            0f
        );

        State = CustomerState.Arriving;
    }

    private void Update()
    {
        IsWalking = false;

        if (celebrating)
        {
            UpdateHappyJump();
            return;
        }

        switch (State)
        {
            case CustomerState.Arriving:
                if (MoveTo(orderPoint.position))
                {
                    State = CustomerState.Waiting;

                    Debug.Log(
                        "ลูกค้ามาถึงแล้ว รอสั่งอาหาร",
                        this
                    );
                }
                break;

            case CustomerState.Waiting:
                Quaternion facing = Quaternion.Euler(
                    0f,
                    orderPoint.eulerAngles.y,
                    0f
                );

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    facing,
                    turnSpeed * Time.deltaTime
                );
                break;

            case CustomerState.Leaving:
                if (MoveTo(exitPoint.position))
                    Destroy(gameObject);
                break;
        }
    }

    private bool MoveTo(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion facing =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                facing,
                turnSpeed * Time.deltaTime
            );
        }

        Vector3 beforeMove = transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            moveSpeed * Time.deltaTime
        );

        Vector3 actualMove = transform.position - beforeMove;
        actualMove.y = 0f;

        IsWalking = actualMove.sqrMagnitude > 0.00000001f;

        return (transform.position - destination).sqrMagnitude
            < 0.0001f;
    }

    public void Leave()
    {
        if (!IsWaiting)
            return;

        State = CustomerState.Leaving;
    }

    public void CelebrateAndLeave()
    {
        if (!IsWaiting)
            return;

        // ป้องกันการรับอาหารและให้รางวัลซ้ำ
        State = CustomerState.Leaving;
        IsWalking = false;

        Vector3 soundPosition =
            transform.position + Vector3.up * 0.8f;

        WorldAudio.PlayAt(
            WorldSound.MoneyReceived,
            soundPosition,
            moneySoundVolume,
            soundMinDistance,
            soundMaxDistance
        );

        WorldAudio.PlayAt(
            WorldSound.NPCHappy,
            soundPosition,
            happySoundVolume,
            soundMinDistance,
            soundMaxDistance
        );

        if (!HasValidVisualRoot() ||
            !visualRoot.gameObject.activeInHierarchy ||
            jumpHeight <= 0f)
        {
            return;
        }

        visualRestPosition = visualRoot.localPosition;
        celebrationTime = 0f;
        celebrating = true;
    }

    private bool HasValidVisualRoot()
    {
        return visualRoot != null &&
            visualRoot != transform &&
            visualRoot.IsChildOf(transform);
    }

    private bool IsValidModel(GameObject model)
    {
        return model != null &&
            model.transform != visualRoot &&
            model.transform.IsChildOf(visualRoot);
    }

    private void SelectRandomModel()
    {
        if (!HasValidVisualRoot() ||
            modelVariants == null ||
            modelVariants.Length == 0)
        {
            Debug.LogWarning(
                "CustomerMovement: กรุณาใส่ Visual Root และ Model Variants",
                this
            );

            return;
        }

        int validCount = 0;

        foreach (GameObject model in modelVariants)
        {
            if (IsValidModel(model))
            {
                model.SetActive(false);
                validCount++;
            }
        }

        if (validCount == 0)
        {
            Debug.LogWarning(
                "CustomerMovement: โมเดลต้องอยู่ภายใน Visual Root",
                this
            );

            return;
        }

        int selectedIndex = Random.Range(0, validCount);

        foreach (GameObject model in modelVariants)
        {
            if (!IsValidModel(model))
                continue;

            if (selectedIndex-- == 0)
            {
                model.SetActive(true);
                break;
            }
        }
    }

    private void UpdateHappyJump()
    {
        if (!HasValidVisualRoot())
        {
            celebrating = false;
            return;
        }

        celebrationTime += Time.deltaTime;

        float duration = Mathf.Max(0.05f, jumpDuration);
        int count = Mathf.Max(1, jumpCount);

        if (celebrationTime >= duration * count)
        {
            visualRoot.localPosition = visualRestPosition;
            celebrating = false;
            return;
        }

        float phase = (celebrationTime % duration) / duration;

        float height =
            Mathf.Sin(phase * Mathf.PI) *
            Mathf.Max(0f, jumpHeight);

        Vector3 offset =
            visualRoot.parent.InverseTransformVector(
                Vector3.up * height
            );

        visualRoot.localPosition =
            visualRestPosition + offset;
    }

    private void OnDisable()
    {
        IsWalking = false;

        if (celebrating && visualRoot != null)
            visualRoot.localPosition = visualRestPosition;

        celebrating = false;
    }

    [ContextMenu("Test Leave")]
    private void TestLeave()
    {
        if (Application.isPlaying)
            Leave();
    }

    public void Configure(
        Transform spawn,
        Transform order,
        Transform exit)
    {
        spawnPoint = spawn;
        orderPoint = order;
        exitPoint = exit;
    }
}