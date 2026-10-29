using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Customer Prefab")]
    [SerializeField] private GameObject customerPrefab;

    [Header("Scene References")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform orderPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private ServingZone servingZone;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float nextCustomerDelay = 1f;

    private GameObject currentCustomer;
    private bool shopClosed;

    private readonly CustomerOrder.MenuType[] menuBag =
    {
        CustomerOrder.MenuType.Hamburger,
        CustomerOrder.MenuType.Hotdog,
        CustomerOrder.MenuType.Fries
    };

    private int menuIndex = 3;

    private CustomerOrder.MenuType GetNextMenu()
    {
        if (menuIndex >= menuBag.Length)
        {
            for (int i = menuBag.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);

                CustomerOrder.MenuType temp = menuBag[i];
                menuBag[i] = menuBag[j];
                menuBag[j] = temp;
            }

            menuIndex = 0;
        }

        return menuBag[menuIndex++];
    }

    private IEnumerator Start()
    {
        if (customerPrefab == null ||
            spawnPoint == null ||
            orderPoint == null ||
            exitPoint == null ||
            playerCamera == null ||
            servingZone == null)
        {
            Debug.LogError(
                "CustomerSpawner: ใส่ช่องอ้างอิงให้ครบ", this);
            yield break;
        }

        if (!customerPrefab.activeSelf ||
            customerPrefab.GetComponent<CustomerMovement>() == null ||
            customerPrefab.GetComponent<CustomerOrder>() == null)
        {
            Debug.LogError(
                "Customer Prefab ต้องเปิดใช้งาน และมี " +
                "CustomerMovement กับ CustomerOrder บนตัวแม่",
                this);
            yield break;
        }

        servingZone.SetCustomer(null);

        while (!shopClosed)
        {
            currentCustomer = Instantiate(
                customerPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            CustomerMovement movement =
                currentCustomer.GetComponent<CustomerMovement>();

            CustomerOrder order =
                currentCustomer.GetComponent<CustomerOrder>();

            movement.Configure(spawnPoint, orderPoint, exitPoint);
            order.Configure(GetNextMenu(), playerCamera);
            servingZone.SetCustomer(order);

            yield return new WaitUntil(
                () => currentCustomer == null
            );

            servingZone.SetCustomer(null);

            yield return new WaitForSeconds(nextCustomerDelay);
        }
    }

    public void StopSpawning()
    {
        if (shopClosed)
            return;

        shopClosed = true;
        StopAllCoroutines();

        if (servingZone != null)
            servingZone.SetCustomer(null);

        if (currentCustomer == null)
            return;

        CustomerMovement movement =
            currentCustomer.GetComponent<CustomerMovement>();

        if (movement != null && movement.IsWaiting)
        {
            // ลูกค้าที่ยืนรอเดินออกโดยไม่ได้รับเงิน
            movement.Leave();
        }
        else if (movement == null ||
                 movement.State != CustomerMovement.CustomerState.Leaving)
        {
            // เอาลูกค้าที่ยังเดินเข้าร้านออกเมื่อปิดร้าน
            Destroy(currentCustomer);
        }
    }
}