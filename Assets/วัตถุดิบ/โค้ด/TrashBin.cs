using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TrashBin : MonoBehaviour
{
    private readonly HashSet<GameObject> disposingItems =
        new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        TryDispose(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDispose(other);
    }

    private void TryDispose(Collider other)
    {
        Rigidbody itemBody = other.attachedRigidbody;

        if (itemBody == null)
            return;

        GameObject item = itemBody.gameObject;

        if (item.GetComponent<TrashableItem>() == null)
            return;

        XRGrabInteractable grab =
            item.GetComponent<XRGrabInteractable>();

        // ต้องปล่อยจากมือก่อน
        if (grab != null && grab.isSelected)
            return;

        // ป้องกันทิ้งวัตถุเดิมซ้ำจากหลาย Collider
        if (!disposingItems.Add(item))
            return;

        if (GameAudioManager.Instance != null)
            GameAudioManager.Instance.PlayTrash();

        item.SetActive(false);
        Destroy(item);
    }

    private void LateUpdate()
    {
        disposingItems.Clear();
    }
}