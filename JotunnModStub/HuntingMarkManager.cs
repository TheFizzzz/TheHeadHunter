using System.Collections;
using System.Collections.Generic;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace TheHeadHunter;

internal static class HuntingMarkManager
{
    private const string VfxPrefabName = "HuntersArsenalMarkVfx";
    private const string VanillaVfxName = "vfx_WishbonePing";

    private static readonly Dictionary<ZDOID, HuntingMark> Marks = new();
    private static readonly Dictionary<ZDOID, HuntingMarkVfx> Visuals = new();
    private static CustomRPC? _markRpc;
    private static GameObject? _vfxPrefab;

    public static void Initialize()
    {
        _markRpc = NetworkManager.Instance.AddRPC(
            "HuntersArsenalMark",
            ReceiveMark,
            ReceiveMark);
    }

    public static void RegisterVisual()
    {
        GameObject? original = PrefabManager.Instance.GetPrefab(VanillaVfxName);
        if (original is null)
        {
            Jotunn.Logger.LogWarning(
                $"Could not find {VanillaVfxName}; hunting marks will stay invisible");
            return;
        }

        GameObject clone = Object.Instantiate(original);
        clone.name = VfxPrefabName;
        clone.SetActive(false);
        Object.DontDestroyOnLoad(clone);
        StripTransientComponents(clone);
        foreach (ParticleSystem particle in clone.GetComponentsInChildren<ParticleSystem>(true))
        {
            ParticleSystem.MainModule main = particle.main;
            main.loop = false;
            main.playOnAwake = true;
        }

        _vfxPrefab = clone;
        Jotunn.Logger.LogInfo("Registered hunting mark particle effect");
    }

    public static void Apply(Character prey, float chance)
    {
        if (prey is null)
        {
            return;
        }

        float duration = TheHeadHunterPlugin.HuntingMarkDuration.Value;
        SpawnVisual(prey, duration);

        if (ZNet.instance is null || ZNet.instance.IsServer())
        {
            StoreMark(prey.GetZDOID(), chance, duration);
            return;
        }

        if (_markRpc is null || ZRoutedRpc.instance is null)
        {
            return;
        }

        ZPackage package = new();
        package.Write(prey.GetZDOID());
        package.Write(chance);
        _markRpc.SendPackage(ZRoutedRpc.instance.GetServerPeerID(), package);
    }

    public static float Consume(Character prey)
    {
        ZDOID id = prey.GetZDOID();
        ClearVisual(id);
        if (!Marks.TryGetValue(id, out HuntingMark mark))
        {
            return 0f;
        }

        Marks.Remove(id);
        return mark.ExpiresAt >= Time.time ? mark.Chance : 0f;
    }

    internal static void NotifyVisualDestroyed(ZDOID prey, HuntingMarkVfx visual)
    {
        if (Visuals.TryGetValue(prey, out HuntingMarkVfx current) && current == visual)
        {
            Visuals.Remove(prey);
        }
    }

    private static IEnumerator ReceiveMark(long sender, ZPackage package)
    {
        ZDOID prey = package.ReadZDOID();
        float chance = package.ReadSingle();
        float duration = TheHeadHunterPlugin.HuntingMarkDuration.Value;
        StoreMark(prey, chance, duration);

        GameObject instance = ZNetScene.instance.FindInstance(prey);
        Character? character = instance == null ? null : instance.GetComponent<Character>();
        if (character is not null)
        {
            SpawnVisual(character, duration);
        }

        yield break;
    }

    private static void StoreMark(ZDOID prey, float chance, float duration)
    {
        float expiresAt = Time.time + duration;
        if (!Marks.TryGetValue(prey, out HuntingMark current) ||
            current.ExpiresAt < Time.time ||
            chance >= current.Chance)
        {
            Marks[prey] = new HuntingMark(chance, expiresAt);
        }
        else
        {
            Marks[prey] = new HuntingMark(current.Chance, expiresAt);
        }
    }

    private static void SpawnVisual(Character prey, float duration)
    {
        if (_vfxPrefab is null || prey is null)
        {
            return;
        }

        try
        {
            ZDOID id = prey.GetZDOID();
            ClearVisual(id);

            Transform parent = prey.transform;
            GameObject visualRoot = prey.GetVisual();
            if (visualRoot is not null)
            {
                parent = visualRoot.transform;
            }

            GameObject vfx = Object.Instantiate(_vfxPrefab, parent, false);
            vfx.name = VfxPrefabName;
            vfx.transform.localPosition = parent.InverseTransformPoint(
                prey.transform.position + Vector3.up * Mathf.Max(0.8f, prey.GetRadius()));
            vfx.transform.localRotation = Quaternion.identity;
            vfx.transform.localScale = Vector3.one * Mathf.Clamp(prey.GetRadius(), 0.75f, 2.5f);
            vfx.SetActive(true);
            HuntingMarkVfx tracker = vfx.AddComponent<HuntingMarkVfx>();
            tracker.Initialize(id, duration);
            Visuals[id] = tracker;
        }
        catch (System.Exception exception)
        {
            Jotunn.Logger.LogWarning($"Failed to spawn hunting mark VFX: {exception.Message}");
        }
    }

    private static void ClearVisual(ZDOID prey)
    {
        if (!Visuals.TryGetValue(prey, out HuntingMarkVfx visual))
        {
            return;
        }

        Visuals.Remove(prey);
        if (visual is not null)
        {
            Object.Destroy(visual.gameObject);
        }
    }

    private static void StripTransientComponents(GameObject vfx)
    {
        foreach (TimedDestruction timed in vfx.GetComponentsInChildren<TimedDestruction>(true))
        {
            Object.DestroyImmediate(timed);
        }

        foreach (ZNetView view in vfx.GetComponentsInChildren<ZNetView>(true))
        {
            Object.DestroyImmediate(view);
        }

        foreach (ZSyncTransform sync in vfx.GetComponentsInChildren<ZSyncTransform>(true))
        {
            Object.DestroyImmediate(sync);
        }

        foreach (ZSFX sfx in vfx.GetComponentsInChildren<ZSFX>(true))
        {
            Object.DestroyImmediate(sfx);
        }

        foreach (AudioSource audio in vfx.GetComponentsInChildren<AudioSource>(true))
        {
            Object.DestroyImmediate(audio);
        }
    }

    private readonly struct HuntingMark
    {
        public HuntingMark(float chance, float expiresAt)
        {
            Chance = chance;
            ExpiresAt = expiresAt;
        }

        public float Chance { get; }
        public float ExpiresAt { get; }
    }
}

internal sealed class HuntingMarkVfx : MonoBehaviour
{
    private const float PulseInterval = 0.5f;

    private ZDOID _prey;
    private float _expiresAt;
    private float _nextPulse;
    private ParticleSystem[] _particles = System.Array.Empty<ParticleSystem>();

    public void Initialize(ZDOID prey, float duration)
    {
        _prey = prey;
        _expiresAt = Time.time + duration;
        _particles = GetComponentsInChildren<ParticleSystem>(true);
        _nextPulse = Time.time + PulseInterval;
    }

    private void Update()
    {
        if (Time.time >= _expiresAt)
        {
            Destroy(gameObject);
            return;
        }

        if (Time.time < _nextPulse)
        {
            return;
        }

        _nextPulse = Time.time + PulseInterval;
        foreach (ParticleSystem particle in _particles)
        {
            if (particle is not null)
            {
                particle.Play(true);
            }
        }
    }

    private void OnDestroy()
    {
        HuntingMarkManager.NotifyVisualDestroyed(_prey, this);
    }
}
