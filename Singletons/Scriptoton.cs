using UnityEngine;

namespace PM.UsefulThings
{
	// singleton based on scriptable object
	/// <summary>
	/// to create instance copypast this and change name
	/// [CreateAssetMenu(fileName = "ManagerName", menuName = "Scriptoton/ManagerName", order = 10)]
	/// </summary>
	public class Scriptoton<T> : ScriptableObject where T : ScriptableObject, IInitializable
	{
		private static T _instance;

		private static object _lock = new object();

		public static bool HasInstance
		{
			get
			{
				return _instance != null;
			}
		}

		public static T Instance
		{
			get
			{
				if (applicationIsQuitting)
				{
					Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
						"' already destroyed on application quit." +
						" Won't create again - returning null.");
					return null;
				}

				lock (_lock)
				{
					if (_instance == null)
					{
						//trying to find created instance
						_instance = (T)FindObjectOfType(typeof(T));

						if (FindObjectsOfType(typeof(T)).Length > 1)
						{
							Debug.LogError("[Scriptoton] Something went really wrong " +
								" - there should never be more than 1 singleton!");
						}

						//trying to load instance
						if (_instance == null)
						{
							_instance = Resources.Load($"Scriptotons/{typeof(T).Name}") as T;
						}

						if (_instance == null)
						{
							Debug.LogError("Scriptoton can't create instance");
							return null;
						}
					}

					return _instance;
				}
			}
		}

		private static bool applicationIsQuitting = false;
		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		///   it will create a buggy ghost object that will stay on the Editor scene
		///   even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		public void OnDestroy()
		{
			if (this.isInstance)
			{
				applicationIsQuitting = true;
			}
		}

		protected bool isInstance { get; set; }
		/// <summary>
		/// If we will need to override Awake we must be sure it won't do anything
		/// if it isn't an instance. 
		/// </summary>
		protected virtual void Awake()
		{
			if (_instance == null)
			{
				_instance = this as T;
				isInstance = true;
				_instance.Init();
			}
			else if (_instance != this)
			{
				isInstance = false;
				Debug.LogError("There is two instances of singletons " + typeof(T).ToString());
			}
			else
			{
				isInstance = true;
			}
		}
	}
}