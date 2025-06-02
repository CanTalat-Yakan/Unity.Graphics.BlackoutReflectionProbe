#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class BlackoutReflectionProbeEditor
    {
        [MenuItem("GameObject/Essentials/Blackout Reflection Probe", false, priority = 122)]
        private static void InstantiateBlackoutReflectionProbe(MenuCommand menuCommand)
        {
            var go = new GameObject("Blackout Reflection Probe");
            go.transform.localScale = Vector3.one * 10f; // Default scale

            var probe = go.AddComponent<BlackoutReflectionProbe>();

            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create Blackout Reflection Probe");
            Selection.activeObject = go;
        }

    }
}
#endif