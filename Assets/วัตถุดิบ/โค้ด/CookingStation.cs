using System.Collections.Generic;
using UnityEngine;

public class CookingStation : MonoBehaviour
{
    public enum StationType
    {
        Grill,
        Fryer
    }

    [Header("Station Settings")]
    [SerializeField]
    private StationType stationType =
        StationType.Grill;

    [Header("3D Audio")]
    [SerializeField, Range(0f, 1f)]
    private float idleVolume = 0.2f;

    [SerializeField, Range(0f, 1f)]
    private float cookingVolume = 0.7f;

    [SerializeField, Min(0.01f)]
    private float minDistance = 1f;

    [SerializeField, Min(0.02f)]
    private float maxDistance = 8f;

    [Header("Optional Clip Overrides")]
    [SerializeField] private AudioClip idleClipOverride;
    [SerializeField] private AudioClip cookingClipOverride;

    private AudioSource idleSource;
    private AudioSource cookingSource;

    private readonly Dictionary<Collider, MonoBehaviour> contacts =
        new Dictionary<Collider, MonoBehaviour>();

    private readonly List<Collider> staleContacts =
        new List<Collider>();

    private readonly HashSet<MonoBehaviour> currentFoods =
        new HashSet<MonoBehaviour>();

    private readonly HashSet<MonoBehaviour> previousFoods =
        new HashSet<MonoBehaviour>();

    private void Awake()
    {
        idleSource =
            WorldAudio.CreateSource(transform, "Grill Idle Audio");

        cookingSource =
            WorldAudio.CreateSource(transform, "Cooking Audio");
    }

    private void OnTriggerEnter(Collider other)
    {
        Track(other);
    }

    private void OnTriggerStay(Collider other)
    {
        Track(other);
    }

    private void OnTriggerExit(Collider other)
    {
        contacts.Remove(other);
    }

    private void Track(Collider other)
    {
        if (!isActiveAndEnabled || other == null)
            return;

        MonoBehaviour food = FindAllowedFood(other);

        if (food != null)
            contacts[other] = food;
    }

    private MonoBehaviour FindAllowedFood(Collider other)
    {
        if (stationType == StationType.Fryer)
        {
            return other.GetComponentInParent<FrenchFriesCook>();
        }

        BurgerPatty patty =
            other.GetComponentInParent<BurgerPatty>();

        if (patty != null)
            return patty;

        return other.GetComponentInParent<HotDogSausage>();
    }

    private void LateUpdate()
    {
        currentFoods.Clear();
        staleContacts.Clear();

        foreach (KeyValuePair<Collider, MonoBehaviour> contact in contacts)
        {
            Collider collider = contact.Key;
            MonoBehaviour food = contact.Value;

            if (collider == null ||
                !collider.enabled ||
                !collider.gameObject.activeInHierarchy ||
                food == null ||
                !food.isActiveAndEnabled ||
                FindAllowedFood(collider) != food)
            {
                staleContacts.Add(collider);
                continue;
            }

            currentFoods.Add(food);
        }

        foreach (Collider collider in staleContacts)
            contacts.Remove(collider);

        // หยุดปรุงเฉพาะอาหารที่ออกจากพื้นที่แล้ว
        foreach (MonoBehaviour food in previousFoods)
        {
            if (food != null && !currentFoods.Contains(food))
                SetCooking(food, false);
        }

        // เริ่มปรุงอาหารที่เพิ่งเข้ามา
        foreach (MonoBehaviour food in currentFoods)
        {
            if (!previousFoods.Contains(food))
                SetCooking(food, true);
        }

        previousFoods.Clear();
        previousFoods.UnionWith(currentFoods);

        bool cooking = false;

        foreach (MonoBehaviour food in currentFoods)
        {
            if (CanStillCook(food))
            {
                cooking = true;
                break;
            }
        }

        AudioClip idleClip = stationType == StationType.Grill
            ? WorldAudio.GetClip(
                WorldSound.GrillIdle,
                idleClipOverride)
            : null;

        WorldSound cookingSound = stationType == StationType.Fryer
            ? WorldSound.FriesCooking
            : WorldSound.GrillCooking;

        AudioClip activeClip = cooking
            ? WorldAudio.GetClip(cookingSound, cookingClipOverride)
            : null;

        UpdateLoop(idleSource, idleClip, idleVolume);
        UpdateLoop(cookingSource, activeClip, cookingVolume);
    }

    private void UpdateLoop(
        AudioSource source,
        AudioClip clip,
        float volume)
    {
        WorldAudio.Configure(
            source,
            volume,
            minDistance,
            maxDistance
        );

        if (clip == null)
        {
            source.Stop();
            source.clip = null;
            return;
        }

        if (source.clip == clip && source.isPlaying)
            return;

        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    private static bool CanStillCook(MonoBehaviour food)
    {
        if (food is BurgerPatty patty)
            return patty.currentState != BurgerPatty.CookState.Buen;

        if (food is HotDogSausage sausage)
            return sausage.currentState != HotDogSausage.CookState.Burn;

        if (food is FrenchFriesCook fries)
            return fries.currentState != FrenchFriesCook.CookState.Burn;

        return false;
    }

    private static void SetCooking(
        MonoBehaviour food,
        bool cooking)
    {
        if (food is BurgerPatty patty)
        {
            if (cooking)
                patty.StartCooking();
            else
                patty.StopCooking();
        }
        else if (food is HotDogSausage sausage)
        {
            if (cooking)
                sausage.StartCooking();
            else
                sausage.StopCooking();
        }
        else if (food is FrenchFriesCook fries)
        {
            if (cooking)
                fries.StartCooking();
            else
                fries.StopCooking();
        }
    }

    private void OnDisable()
    {
        foreach (MonoBehaviour food in previousFoods)
        {
            if (food != null)
                SetCooking(food, false);
        }

        contacts.Clear();
        previousFoods.Clear();
        currentFoods.Clear();

        if (idleSource != null)
            idleSource.Stop();

        if (cookingSource != null)
            cookingSource.Stop();
    }
}