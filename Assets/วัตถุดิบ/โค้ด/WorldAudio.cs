using UnityEngine;

public enum WorldSound
{
    PlayerFootstep,
    CustomerFootstep,
    Cloth,
    CeramicPickup,
    PepperShake,
    MayonnaisePickup,
    KetchupPickup,
    FriesCooking,
    GrillIdle,
    GrillCooking,
    AssemblyComplete,
    MoneyReceived,
    NPCHappy
}

public static class WorldAudio
{
    public static AudioClip GetClip(
        WorldSound sound,
        AudioClip overrideClip = null)
    {
        if (overrideClip != null)
            return overrideClip;

        return GameAudioManager.Instance != null
            ? GameAudioManager.Instance.GetWorldClip(sound)
            : null;
    }

    public static AudioSource CreateSource(
        Transform parent,
        string sourceName)
    {
        GameObject child = new GameObject(sourceName);
        child.transform.SetParent(parent, false);

        AudioSource source =
            child.AddComponent<AudioSource>();

        Configure(source, 1f, 1f, 8f);

        return source;
    }

    public static void Configure(
        AudioSource source,
        float volume,
        float minDistance,
        float maxDistance)
    {
        if (source == null)
            return;

        source.playOnAwake = false;
        source.spatialBlend = 1f;
        source.dopplerLevel = 0f;
        source.rolloffMode = AudioRolloffMode.Linear;

        source.minDistance =
            Mathf.Max(0.01f, minDistance);

        source.maxDistance = Mathf.Max(
            source.minDistance + 0.01f,
            maxDistance
        );

        float master = GameAudioManager.Instance != null
            ? GameAudioManager.Instance.WorldEffectsVolume
            : 1f;

        source.volume =
            Mathf.Clamp01(volume) * master;
    }

    public static void PlayAt(
        WorldSound sound,
        Vector3 position,
        float volume,
        float minDistance,
        float maxDistance,
        AudioClip overrideClip = null)
    {
        AudioClip clip = GetClip(sound, overrideClip);

        if (clip == null)
            return;

        // แยกตัวเล่นเสียงเพื่อไม่ให้เสียงถูกตัด
        // เมื่ออาหารหรือลูกค้าถูกทำลาย
        GameObject emitter =
            new GameObject("One Shot - " + sound);

        emitter.transform.position = position;

        AudioSource source =
            emitter.AddComponent<AudioSource>();

        Configure(
            source,
            volume,
            minDistance,
            maxDistance
        );

        source.loop = false;
        source.clip = clip;
        source.Play();

        Object.Destroy(emitter, clip.length + 0.1f);
    }
}