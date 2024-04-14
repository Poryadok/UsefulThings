using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PM.UsefulThings.Memory;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace PM.UsefulThings
{
    public class AddressablesProvider : IDisposable
	{
		private Dictionary<string, AsyncOperationHandle> completedCache = new();
		private Dictionary<string, List<AsyncOperationHandle>> handles = new();
		private Dictionary<string, int> counters = new();

		public AddressablesProvider()
		{
			Addressables.InitializeAsync();
		}

		public async UniTask<T> LoadAsync<T>(string assetKey) where T : Object
		{
			if (!(typeof(T).IsAssignableFrom(typeof(Component))) 
			      && !(typeof(T).IsAssignableFrom(typeof(GameObject)))
			      || (typeof(T).IsAssignableFrom(typeof(ScriptableObject))))
			{
				if (counters.TryGetValue(assetKey, out var count))
				{
					counters[assetKey] = count + 1;
				}
				else
				{
					counters[assetKey] = 1;
				}
			}
			
			if (completedCache.TryGetValue(assetKey, out AsyncOperationHandle completedHandle))
				return completedHandle.Result as T;

			var handle = Addressables.LoadAssetAsync<T>(assetKey);
			var result = await RunWithCacheOnComplete(handle, cacheKey: assetKey);
			return result;
		}

		private async UniTask<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey)
			where T : class
		{
			handle.Completed += completeHandle => { completedCache[cacheKey] = completeHandle; };
			AddHandle<T>(cacheKey, handle);
			var result = await handle.Task;
			return result;
		}

		private void AddHandle<T>(string key, AsyncOperationHandle handle) where T : class
		{
			if (!handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles))
			{
				resourceHandles = new List<AsyncOperationHandle>();
				handles[key] = resourceHandles;
			}

			resourceHandles.Add(handle);
		}

		public T Load<T>(string assetKey) where T : class
		{
			if (!(typeof(T).IsAssignableFrom(typeof(Component))) 
			    && !(typeof(T).IsAssignableFrom(typeof(GameObject)))
			    || (typeof(T).IsAssignableFrom(typeof(ScriptableObject))))
			{
				if (counters.TryGetValue(assetKey, out var count))
				{
					counters[assetKey] = count + 1;
				}
				else
				{
					counters[assetKey] = 1;
				}
			}

			if (completedCache.TryGetValue(assetKey, out AsyncOperationHandle completedHandle))
				return completedHandle.Result as T;

			var handle = Addressables.LoadAssetAsync<T>(assetKey);
			RunWithCacheOnComplete(handle, cacheKey: assetKey).Forget();
			var result = handle.WaitForCompletion();
			return result;
		}


		public async UniTask<GameObject> InstantiateAsync(AssetReference assetRef, Transform parent = null)
		{
			var result = await Addressables.InstantiateAsync(assetRef);
			return result;
		}

		public async UniTask<T> InstantiateAsync<T>(ComponentReference<T> assetRef, Transform parent = null)
			where T : Component
		{
			var gameObject = await Addressables.InstantiateAsync(assetRef);
			var result = gameObject.GetComponent<T>();
			return result;
		}
		
		public T Instantiate<T>(ComponentReference<T> assetRef, Transform parent = null) where T : Component
		{
			var handle = Addressables.InstantiateAsync(assetRef, parent).WaitForCompletion();
			var result = handle.GetComponent<T>();
			return result;
		}

		public async UniTask<GameObject> InstantiateAsync(AssetReferenceGameObject assetRef, Transform parent = null)
		{
			var result = await Addressables.InstantiateAsync(assetRef);
			return result;
		}

		public GameObject Instantiate(AssetReferenceGameObject assetRef, Transform parent = null)
		{
			var result = Addressables.InstantiateAsync(assetRef, parent).WaitForCompletion();
			return result;
		}

		public bool Release(Object obj)
		{
			var isInHandles = false;
			var isReleased = false;
			foreach (var item in handles)
			{
				var handles = item.Value;
				for (var i = 0; i < handles.Count; i++)
				{
					var handle = handles[i];
					if ((Object)handle.Result == obj)
					{
						isInHandles = true;
						
						if (counters.TryGetValue(item.Key, out var count))
						{
							counters[item.Key] = count - 1;
							if (count > 1)
								return false;
							
							counters.Remove(item.Key);
						}

						if (completedCache.ContainsKey(item.Key) && completedCache[item.Key].Result == obj)
							completedCache.Remove(item.Key);

						isReleased = true;
						Addressables.Release(handle);
						this.handles[item.Key].Remove(handle);
						break;
					}
				}
			}

			if (!isInHandles)
			{
				isReleased = true;
				Addressables.Release(obj);
			}

			return isReleased;
		}

		public bool ReleaseWithKey(string key)
		{
			var isReleased = false;
			if (counters.TryGetValue(key, out var count))
			{
				counters[key] = count - 1;
				if (count > 1)
					return false;
							
				counters.Remove(key);
			}

			if (completedCache.ContainsKey(key))
				completedCache.Remove(key);
			else
				Debug.LogError($"{this} asset key: {key} not found in {nameof(completedCache)}");

			if (handles.ContainsKey(key))
			{
				var handles = this.handles[key];
				for (var i = 0; i < handles.Count; i++)
				{
					var handle = handles[i];
					isReleased = true;
					Addressables.Release(handle);
					handles.Remove(handle);
				}
			}
			else
				Debug.LogError($"{this} asset key: {key} not found in {nameof(handles)}");

			return isReleased;
		}

		public void ReleaseAll()
		{
			foreach (var handles in handles)
			{
				foreach (AsyncOperationHandle handle in handles.Value)
					Addressables.Release(handle);
			}

			completedCache.Clear();
			handles.Clear();
			counters.Clear();
		}

		public void Dispose()
		{
			ReleaseAll();
		}
    }
}
