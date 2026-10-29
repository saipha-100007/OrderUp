using UnityEngine;

public class AssemblyStation : MonoBehaviour
{
    [Header("Assembly")]
    public BurgerAssemblyArea assemblyArea;
    public Transform assemblyPoint;

    [Header("Prefab")]
    public GameObject hamburgerPrefab;

    [Header("Effect")]
    public GameObject explosionEffect;

    private void OnTriggerEnter(Collider other)
    {
        // ต้องเป็นขนมปัง Burger
        if (!other.CompareTag("BurgerBun"))
            return;

        // ต้องมี Assembly Area
        if (assemblyArea == null)
            return;

        // ต้องมีเนื้อสุกอยู่ใน Assembly Area
        if (!assemblyArea.HasCookedPatty())
            return;

        AssembleBurger(other.gameObject);
    }

    private void AssembleBurger(GameObject bun)
    {
        // -------------------------
        // หา Patty ที่สุก
        // -------------------------

        BurgerPatty patty = assemblyArea.GetPatty();

        if (patty == null)
            return;

        assemblyArea.ClearPatty();

        // -------------------------
        // จำตำแหน่ง Spawn
        // -------------------------

        Vector3 spawnPosition = assemblyPoint.position;
        Quaternion spawnRotation = assemblyPoint.rotation;

        // -------------------------
        // ทำลายวัตถุดิบเดิม
        // -------------------------

        Destroy(bun);
        Destroy(patty.gameObject);

        // -------------------------
        // สร้าง Hamburger ใหม่
        // -------------------------

        if (hamburgerPrefab != null)
        {
            Instantiate(
                hamburgerPrefab,
                spawnPosition,
                spawnRotation
            );
        }

        // -------------------------
        // Explosion
        // -------------------------

        PlayExplosion(spawnPosition, spawnRotation);
    }

    private void PlayExplosion(
        Vector3 position,
        Quaternion rotation)
    {
        if (explosionEffect == null)
            return;

        explosionEffect.transform.position = position;
        explosionEffect.transform.rotation = rotation;

        explosionEffect.SetActive(true);

        ParticleSystem particle =
            explosionEffect.GetComponentInChildren<ParticleSystem>();

        if (particle != null)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particle.Play();
        }
    }
}