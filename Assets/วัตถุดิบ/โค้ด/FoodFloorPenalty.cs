using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FoodFloorPenalty : MonoBehaviour
{
    [SerializeField] private GameWallet wallet;

    // จำว่าอาหารชิ้นไหนถูกหักเงินไปแล้ว
    private readonly HashSet<Rigidbody> chargedItems =
        new HashSet<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        CheckFood(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckFood(collision);
    }

    private void CheckFood(Collision collision)
    {
        if (!isActiveAndEnabled ||
            wallet == null ||
            !wallet.RoundActive)
        {
            return;
        }

        Rigidbody body = collision.rigidbody;

        if (body == null || chargedItems.Contains(body))
            return;

        // ตรวจเฉพาะตัวหลักของอาหารที่หยิบได้
        XRGrabInteractable grab =
            body.GetComponent<XRGrabInteractable>();

        if (grab == null || grab.isSelected)
            return;

        int penalty;

        // อาหารประกอบเสร็จ
        if (body.GetComponent<FoodItem>() != null)
        {
            penalty = 20;
        }
        // วัตถุดิบ ดิบ/สุก/ไหม้ ที่ยังไม่ประกอบ
        else if (
            body.GetComponent<BurgerPatty>() != null ||
            body.GetComponent<HotDogSausage>() != null ||
            body.GetComponent<FrenchFriesCook>() != null)
        {
            penalty = 10;
        }
        else
        {
            return;
        }

        if (wallet.TryDeductMoney(penalty))
        {
            chargedItems.Add(body);
            Debug.Log($"{body.name} ตกถึงพื้น", body);
        }
    }
}