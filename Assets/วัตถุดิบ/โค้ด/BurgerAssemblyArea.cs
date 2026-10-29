using UnityEngine;

public class BurgerAssemblyArea : MonoBehaviour
{
    private BurgerPatty currentPatty;

    private void OnTriggerEnter(Collider other)
    {
        BurgerPatty patty =
            other.GetComponentInParent<BurgerPatty>();

        if (patty == null)
            return;

        if (!patty.IsCooked())
            return;

        currentPatty = patty;

        Debug.Log("Cooked patty entered Assembly Area.");
    }

    private void OnTriggerExit(Collider other)
    {
        BurgerPatty patty =
            other.GetComponentInParent<BurgerPatty>();

        // ถ้าวัตถุที่ออกไม่ใช่ BurgerPatty
        // ไม่ต้องทำอะไร
        if (patty == null)
            return;

        // ต้องเป็น Patty ตัวเดียวกับที่ถูกบันทึกไว้
        if (patty != currentPatty)
            return;

        currentPatty = null;

        Debug.Log("Cooked patty left Assembly Area.");
    }

    public bool HasCookedPatty()
    {
        return currentPatty != null;
    }

    public BurgerPatty GetPatty()
    {
        return currentPatty;
    }
    public void ClearPatty()
    {
        currentPatty = null;
    }
}