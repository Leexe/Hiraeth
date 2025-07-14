using UnityEngine;

/// <summary>
/// Similar to a Singleton except it will override the current instance instead of destroying it
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour{
    public static T Instance {get; private set;}
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// A Singleton that will destroy any existing instances and replace it with a new instance
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour{
    protected override void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
        base.Awake();
    }
}

/// <summary>
/// A Singleton that does not get destroyed on scene loads
/// </summary>
public abstract class PersistantSingleton<T> : Singleton<T> where T : MonoBehaviour{
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}