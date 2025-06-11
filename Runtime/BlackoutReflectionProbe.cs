using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEssentials
{
    [ExecuteAlways]
    public class BlackoutReflectionProbe : MonoBehaviour
    {
        [Info(MessageType.Info)]
        [SerializeField]
        private string _info =
            "This component creates a hidden reflection probe that captures a black cubemap. " +
            "It is useful for preventing unwanted reflections in scenes where no reflections are desired.";

        [SerializeField] private bool _hideReflectionProbe = true;

        private const float Blend = 1.0f;

        private ReflectionProbe _probe;
        private static Cubemap _blackCubemap;

        private Vector3 _lastScale;
        private float _lastBlend;

        private void OnEnable()
        {
            EnsureProbe();
            ApplySettings();
        }

        private void OnDisable() =>
            Cleanup();

#if UNITY_EDITOR
        private void OnValidate() =>
            ApplySettings();
#endif

        private void Update()
        {
            if (_probe != null)
                _probe.gameObject.hideFlags = _hideReflectionProbe ? HideFlags.HideInHierarchy : HideFlags.None;

            if (_lastScale != transform.localScale ||
                _lastBlend != Blend)
            {
                Cleanup();
                EnsureProbe();
                ApplySettings();
            }

            _lastScale = transform.localScale;
            _lastBlend = Blend;
        }

        private void Cleanup()
        {
            if (_probe == null)
                return;

            if (Application.isEditor)
                for (int i = 0; i <= transform.childCount; i++)
                    DestroyImmediate(transform.GetChild(0).gameObject);
            else for (int i = 0; i <= transform.childCount; i++)
                    Destroy(transform.GetChild(0).gameObject);

            // Force Unity to update reflections
            DynamicGI.UpdateEnvironment();

#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif

            _probe = null;
        }

        private void EnsureProbe()
        {
            if (_probe == null)
            {
                var go = new GameObject("Blackout Reflection Probe", typeof(ReflectionProbe));
                go.transform.SetParent(transform, false);
                _probe = go.GetComponent<ReflectionProbe>();
            }
        }

        private void ApplySettings()
        {
            if (_probe == null)
                return;

            _probe.transform.localPosition = Vector3.zero;
            _probe.transform.localRotation = Quaternion.identity;

            _probe.blendDistance = Blend;
            _probe.size = transform.localScale;

            _probe.refreshMode = ReflectionProbeRefreshMode.OnAwake;
            _probe.mode = ReflectionProbeMode.Custom;
            _probe.customBakedTexture = _blackCubemap ??= CreateBlackCubemap();
        }

        private static Cubemap CreateBlackCubemap(int size = 16)
        {
            var cubemap = new Cubemap(size, TextureFormat.RGBA32, false);
            var blackPixels = new Color[size * size];
            for (int i = 0; i < blackPixels.Length; i++)
                blackPixels[i] = new Color(0, 0, 0, 255);

            foreach (CubemapFace face in System.Enum.GetValues(typeof(CubemapFace)))
            {
                if (face == CubemapFace.Unknown) continue;
                cubemap.SetPixels(blackPixels, face);
            }
            cubemap.Apply();
            cubemap.hideFlags = HideFlags.HideInHierarchy;
            return cubemap;
        }
    }
}
