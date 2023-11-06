using UnityEditor;
using UnityEngine;

public static class MonoBehaviourExtension
{
    public static void DestroyGameObjectInAnyContext(this MonoBehaviour monoBehaviour, GameObject gameObjectToDestroy)
    {
#if UNITY_EDITOR
         Object.DestroyImmediate(gameObjectToDestroy);
#else
        Object.Destroy(gameObjectToDestroy);
#endif
    }

    public static void ApplyChanges(this MonoBehaviour monoBehaviour)
    {
#if UNITY_EDITOR
        PrefabUtility.RecordPrefabInstancePropertyModifications(monoBehaviour);
#endif
    }
}