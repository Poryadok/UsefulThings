using System.Collections;
using System.Collections.Generic;
using PM.UsefulThings.Memory;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace PM.UsefulThings.Extensions
{
    public static class VContainerExtensions
    {
        public static T Instantiate<T>(this IObjectResolver resolver, ComponentReference<T> assetRef) where T : UnityEngine.Component
        {
            T instance;
            if (resolver.ApplicationOrigin is LifetimeScope scope)
            {
                if (scope.IsRoot)
                {
                    instance = assetRef.InstantiateComponentAsync().WaitForCompletion();
                    UnityEngine.Object.DontDestroyOnLoad(instance);
                }
                else
                {
                    // Into the same scene as LifetimeScope
                    instance = assetRef.InstantiateComponentAsync( scope.transform).WaitForCompletion();
                    ObjectResolverUnityExtensions.ResetParent(instance);
                }
            }
            else
            {
                instance = assetRef.InstantiateComponentAsync().WaitForCompletion();
            }

            ObjectResolverUnityExtensions.SetName(instance, assetRef.Asset);

            ObjectResolverUnityExtensions.InjectUnityEngineObject(resolver, instance);
            return instance;
        }

        public static T Instantiate<T>(this IObjectResolver resolver, ComponentReference<T> assetRef, Transform parent,
            bool worldPositionStays = false)
            where T : UnityEngine.Component
        {
            var instance = assetRef.InstantiateComponentAsync( parent, worldPositionStays).WaitForCompletion();
            ObjectResolverUnityExtensions.SetName(instance, assetRef.Asset);

            ObjectResolverUnityExtensions.InjectUnityEngineObject(resolver, instance);
            return instance;
        }

        public static T Instantiate<T>(
            this IObjectResolver resolver,
            ComponentReference<T> assetRef,
            Vector3 position,
            Quaternion rotation)
            where T : UnityEngine.Component
        {
            T instance;
            if (resolver.ApplicationOrigin is LifetimeScope scope)
            {
                if (scope.IsRoot)
                {
                    instance = assetRef.InstantiateComponentAsync(position, rotation).WaitForCompletion();
                    UnityEngine.Object.DontDestroyOnLoad(instance);
                }
                else
                {
                    // Into the same scene as LifetimeScope
                    instance = assetRef.InstantiateComponentAsync(position, rotation, scope.transform)
                        .WaitForCompletion();
                    ObjectResolverUnityExtensions.ResetParent(instance);
                }
            }
            else
            {
                instance = assetRef.InstantiateComponentAsync(position, rotation).WaitForCompletion();
            }

            ObjectResolverUnityExtensions.SetName(instance, assetRef.Asset);
            ObjectResolverUnityExtensions.InjectUnityEngineObject(resolver, instance);
            return instance;
        }

        public static T Instantiate<T>(
            this IObjectResolver resolver,
            ComponentReference<T> assetRef,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
            where T : UnityEngine.Component
        {
            var instance = assetRef.InstantiateComponentAsync(position, rotation, parent).WaitForCompletion();
            ObjectResolverUnityExtensions.SetName(instance, assetRef.Asset);

            ObjectResolverUnityExtensions.InjectUnityEngineObject(resolver, instance);
            return instance;
        }
    }
}