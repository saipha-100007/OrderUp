using UnityEngine;

public class HotDogAssemblyArea : MonoBehaviour
{
    private HotDogSausage currentSausage;

    private void OnTriggerEnter(Collider other)
    {
        HotDogSausage sausage =
            other.GetComponentInParent<HotDogSausage>();

        if (sausage == null)
            return;

        // รับเฉพาะ Ripe
        if (!sausage.IsCooked())
            return;

        currentSausage = sausage;

        Debug.Log("[HOTDOG AREA] RIPE SAUSAGE ENTERED");
    }

    private void OnTriggerExit(Collider other)
    {
        HotDogSausage sausage =
            other.GetComponentInParent<HotDogSausage>();

        if (sausage == null)
            return;

        // ต้องเป็นตัวเดียวกับที่เคยถูกบันทึกว่า Ripe
        if (sausage != currentSausage)
            return;

        // ถ้าตอนออกไม่ใช่ Ripe แล้ว
        // ก็ล้างค่าแต่ไม่แสดงข้อความ Cooked
        if (!sausage.IsCooked())
        {
            currentSausage = null;
            return;
        }

        currentSausage = null;

        Debug.Log("[HOTDOG AREA] RIPE SAUSAGE LEFT");
    }

    public bool HasCookedSausage()
    {
        return currentSausage != null &&
               currentSausage.IsCooked();
    }

    public HotDogSausage GetSausage()
    {
        if (currentSausage != null &&
            currentSausage.IsCooked())
        {
            return currentSausage;
        }

        return null;
    }
    public void ClearSausage()
    {
        currentSausage = null;
    }
}