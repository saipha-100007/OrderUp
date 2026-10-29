using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ServingZone : MonoBehaviour
{
    [Header("Current Customer")]
    [SerializeField] private CustomerOrder customerOrder;

    [Header("Payment")]
    [SerializeField] private GameWallet wallet;
    [SerializeField, Min(1)] private int reward = 50;

    private void OnTriggerEnter(Collider other)
    {
        TryServe(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryServe(other);
    }

    private void TryServe(Collider other)
    {
        if (customerOrder == null || wallet == null)
            return;

        CustomerMovement movement =
            customerOrder.GetComponent<CustomerMovement>();

        // รับอาหารเฉพาะตอนลูกค้ายืนรอ
        if (movement == null ||
            !movement.isActiveAndEnabled ||
            !customerOrder.isActiveAndEnabled ||
            !movement.IsWaiting)
        {
            return;
        }

        Rigidbody body = other.attachedRigidbody;

        if (body == null)
            return;

        FoodItem food = body.GetComponent<FoodItem>();

        // ต้องเป็นอาหารสำเร็จรูป
        if (food == null)
            return;

        XRGrabInteractable grab =
            body.GetComponent<XRGrabInteractable>();

        // ต้องปล่อยอาหารก่อนจึงจะรับ
        if (grab != null && grab.isSelected)
            return;

        // อาหารผิดเมนู ให้หยิบกลับได้
        if (food.Menu != customerOrder.RequestedMenu)
            return;

        // หยุดรับออเดอร์ทันที แล้วกระโดดก่อนเดินออก
        movement.CelebrateAndLeave();

        wallet.AddMoney(reward);

        Debug.Log($"ส่ง {food.Menu} สำเร็จ", this);

        body.gameObject.SetActive(false);
        Destroy(body.gameObject);
    }

    public void SetCustomer(CustomerOrder customer)
    {
        customerOrder = customer;
    }
}