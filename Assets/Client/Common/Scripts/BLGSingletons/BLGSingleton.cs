using UnityEngine;

namespace BLG
{
	/// <summary>
	/// Singleton pattern.
	/// </summary>
	public class BLGSingleton<T> : MonoBehaviour	where T : Component
	{
		protected static T _instance;

		/// <summary>
		/// Singleton design pattern
		/// </summary>
		/// <value>The instance.</value>
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T> ();
					if (_instance == null)
					{
						GameObject obj = new GameObject();
						obj.name = typeof(T).Name;
						_instance = obj.AddComponent<T>();

					}
				}
				return _instance;
			}
		}

	    /// <summary>
	    /// On awake, we initialize our instance. Make sure to call base.Awake() in override if you need awake.
	    /// </summary>
	    protected virtual void Awake ()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			_instance = this as T;			
		}
	}
}
