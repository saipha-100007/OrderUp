using UnityEngine;

public class ItemDispenser : MonoBehaviour
{
    [Header("Item")]
    public GameObject itemPrefab;

    [Header("Spawn")]
    public Transform spawnPoint;

    [Header("Settings")]
    public float spawnCooldown = 0.4f;

    private GameObject currentItem;
    private float lastSpawnTime = -999f;

    public void SpawnItem()
    {
        // ไม่มี Prefab หรือ Spawn Point
        if (itemPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("ItemDispenser: Missing Item Prefab or Spawn Point.");
            return;
        }

        // กันการกดรัวเกินไป
        if (Time.time < lastSpawnTime + spawnCooldown)
            return;

        // ถ้ายังมีของชิ้นเดิมอยู่ที่จุด Spawn
        // ยังไม่สร้างชิ้นใหม่
        if (currentItem != null)
        {
            float distance =
                Vector3.Distance(
                    currentItem.transform.position,
                    spawnPoint.position
                );

            // ถ้ายังอยู่ใกล้ Spawn Point
            if (distance < 0.25f)
                return;
        }

        currentItem = Instantiate(
            itemPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        lastSpawnTime = Time.time;
    }
}