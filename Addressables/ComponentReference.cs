using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ComponentReference<TComponent> : AssetReferenceT<TComponent> where TComponent : UnityEngine.Component 
{
    public ComponentReference(string guid) : base(guid)
    {
    }

    public AsyncOperationHandle<TComponent> InstantiateComponentAsync(Transform parent = null, bool instantiateInWorldSpace = false)
    {
        return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.InstantiateAsync(parent, instantiateInWorldSpace), GameObjectReadyHandler);
    }

    public AsyncOperationHandle<TComponent> InstantiateComponentAsync(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.InstantiateAsync(position, rotation, parent), GameObjectReadyHandler);
    }

    AsyncOperationHandle<TComponent> GameObjectReadyHandler(AsyncOperationHandle<GameObject> arg)
    {
        var comp = arg.Result.GetComponent<TComponent>();
        return Addressables.ResourceManager.CreateCompletedOperation<TComponent>(comp, string.Empty);
    }

    public override bool ValidateAsset(Object obj)
    {
        var go = obj as GameObject;
        return go != null && go.GetComponent<TComponent>() != null;
    }
    
    public override bool ValidateAsset(string path)
    {
#if UNITY_EDITOR
        //this load can be expensive...
        var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        return go != null && go.GetComponent<TComponent>() != null;
#else
            return false;
#endif
    }
    
}