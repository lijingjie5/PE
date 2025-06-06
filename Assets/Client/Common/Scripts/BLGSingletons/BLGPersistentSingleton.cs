using UnityEngine;

namespace BLG
{
	/// <summary>
	/// Persistent singleton.
	/// </summary>
	public class BLGPersistentSingleton<T> : MonoBehaviour where T : Component
	{
		protected static T _instance;
		protected bool _enabled;

		/// <summary>
		/// Singleton design pattern
		/// </summary>
		/// <value>The instance.</value>
		public static T Instance
		{
			get
			{
				return _instance;
			}
		}

	    /// <summary>
	    /// On awake, we check if there's already a copy of the object in the scene. If there's one, we destroy it.
	    /// </summary>
	    protected virtual void Awake ()
		{
			if (!Application.isPlaying)
			{
				return;
            }

            if (_instance == null)
			{
				//If I am the first instance, make me the Singleton
				_instance = this as T;
				DontDestroyOnLoad (transform.gameObject);
				_enabled = true;
			}
			else
			{
				//If a Singleton already exists and you find
				//another reference in scene, destroy it!
				if(this != _instance)
				{
					DestroyImmediate(this.gameObject);
				}
			}
		}

		
	}
}
