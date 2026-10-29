using UnityEngine;

public class HotDogAssemblyStation : MonoBehaviour
{
    [Header("Assembly")]
    public HotDogAssemblyArea assemblyArea;
    public Transform assemblyPoint;

    [Header("Prefab")]
    public GameObject hotdogPrefab;

    [Header("Effect")]
    public GameObject explosionEffect;

    private void OnTriggerEnter(Collider other)
    {
        // ต้องเป็นขนมปัง Hot Dog
        if (!other.CompareTag("HotDogBread"))
            return;

        if (assemblyArea == null)
            return;

        // ต้องมี Sausage สุกอยู่
        if (!assemblyArea.HasCookedSausage())
            return;

        AssembleHotDog(other.gameObject);
    }

    private void AssembleHotDog(GameObject bread)
    {
        HotDogSausage sausage =
            assemblyArea.GetSausage();

        if (sausage == null)
            return;

        // Reset Assembly Area ก่อน
        assemblyArea.ClearSausage();

        Vector3 spawnPosition =
            assemblyPoint.position;

        Quaternion spawnRotation =
            assemblyPoint.rotation;

        // ทำลายวัตถุดิบ
        Destroy(bread);
        Destroy(sausage.gameObject);

        // สร้าง Hotdog ใหม่
        if (hotdogPrefab != null)
        {
            Instantiate(
                hotdogPrefab,
                spawnPosition,
                spawnRotation
            );
        }

        PlayExplosion(
            spawnPosition,
            spawnRotation
        );
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
            particle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            particle.Play();
        }
    }
}