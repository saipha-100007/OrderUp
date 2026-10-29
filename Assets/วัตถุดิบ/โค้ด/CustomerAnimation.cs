using UnityEngine;

public class CustomerAnimation : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator;

    private CustomerMovement movement;
    private int lastState = -1;

    private static readonly int IdleState =
        Animator.StringToHash("Base Layer.Idle");

    private static readonly int WalkState =
        Animator.StringToHash("Base Layer.Walk");

    private void OnEnable()
    {
        lastState = -1;
    }

    private void Start()
    {
        movement = GetComponentInParent<CustomerMovement>();

        if (movement == null || targetAnimator == null)
        {
            Debug.LogError(
                "CustomerAnimation: ต้องอยู่ภายใน Customer " +
                "และใส่ช่อง Target Animator ให้ครบ",
                this
            );

            enabled = false;
            return;
        }

        targetAnimator.applyRootMotion = false;

        if (!targetAnimator.HasState(0, IdleState) ||
            !targetAnimator.HasState(0, WalkState))
        {
            Debug.LogError(
                "CustomerAnimation: Controller ต้องมี State " +
                "ชื่อ Idle และ Walk อยู่ใน Base Layer",
                this
            );

            enabled = false;
        }
    }

    private void Update()
    {
        if (movement == null ||
            targetAnimator == null ||
            !targetAnimator.isActiveAndEnabled)
        {
            return;
        }

        int nextState = movement.IsWaiting ? 0 : 1;

        if (nextState == lastState)
            return;

        lastState = nextState;

        int animationState =
            nextState == 0 ? IdleState : WalkState;

        targetAnimator.CrossFadeInFixedTime(
            animationState,
            0.15f,
            0
        );
    }
}