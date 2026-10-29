using UnityEngine;

public class FrenchFriesAssemblyStation : MonoBehaviour
{
    [Header("Assembly")]
    public FrenchFriesAssemblyArea assemblyArea;
    public Transform assemblyPoint;

    [Header("Prefab")]
    public GameObject friesPrefab;

    [Header("Effect")]
    public GameObject explosionEffect;

    private void OnTriggerEnter(Collider other)
    {
        // ต้องเป็นกล่อง Fries
        if (!other.CompareTag("FriesBox"))
            return;

        if (assemblyArea == null)
            return;

        // ต้องมี Fries สุกอยู่
        if (!assemblyArea.HasCookedFries())
            return;

        AssembleFries(other.gameObject);
    }

    private void AssembleFries(GameObject friesBox)
    {
        FrenchFriesCook fries =
            assemblyArea.GetFries();

        if (fries == null)
            return;

        // Reset Assembly Area
        assemblyArea.ClearFries();

        Vector3 spawnPosition =
            assemblyPoint.position;

        Quaternion spawnRotation =
            assemblyPoint.rotation;

        // ทำลายของเดิม
        Destroy(friesBox);
        Destroy(fries.gameObject);

        // สร้าง Fries สำเร็จรูป
        if (friesPrefab != null)
        {
            Instantiate(
                friesPrefab,
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