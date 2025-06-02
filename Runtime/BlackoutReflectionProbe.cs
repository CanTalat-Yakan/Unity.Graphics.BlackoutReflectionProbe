using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEssentials
{
    [ExecuteAlways]
    public class BlackoutReflectionProbe : MonoBehaviour
    {
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

        private void OnDisable()
        {
            if (_probe == null)
                return;

            if (Application.isEditor)
                DestroyImmediate(_probe.gameObject);
            else Destroy(_probe.gameObject);
        }

        private void OnValidate() =>
            ApplySettings();

        private void Update()
        {
            if (_probe == null ||
                _lastScale != transform.localScale ||
                _lastBlend != Blend)
            {
                _lastScale = transform.localScale;
                _lastBlend = Blend;

                OnDisable();
                EnsureProbe();
                ApplySettings();
            }
        }

        private void EnsureProbe()
        {
            if (_probe == null)
            {
                var go = new GameObject("BlackoutReflectionProbe (Hidden)");
                go.hideFlags = HideFlags.HideInHierarchy;
                go.transform.SetParent(transform, false);
                _probe = go.AddComponent<ReflectionProbe>();
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
