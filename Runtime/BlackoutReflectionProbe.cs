using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEssentials
{
    [ExecuteAlways]
    public class BlackoutReflectionProbe : MonoBehaviour
    {
        private float _blend = 1.0f;

        private ReflectionProbe _probe;
        private static Cubemap _blackCubemap;

        private Vector3 _lastScale;
        private float _lastBlend;

        [MenuItem("GameObject/Light/Blackout Reflection Probe", false)]
        private static void InstantiateBlackoutReflectionProbe(MenuCommand menuCommand)
        {
            var go = new GameObject("Blackout Reflection Probe");
            go.transform.localScale = Vector3.one * 10f; // Default scale

            var probe = go.AddComponent<BlackoutReflectionProbe>();

#if UNITY_EDITOR
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create Blackout Reflection Probe");
            Selection.activeObject = go;
#endif
        }

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
                _lastBlend != _blend)
            {
                _lastScale = transform.localScale;
                _lastBlend = _blend;

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

            _probe.blendDistance = _blend;
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
