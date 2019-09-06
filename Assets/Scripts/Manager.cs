using UnityEngine;

public class Manager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static void FindInstance()
    {
        _instance = (T)FindObjectOfType(typeof(T));

        if (_instance == null)
        {
            Debug.LogError("Error: a reference to " + typeof(T) + " was requested, but no instance of it exists.");
        }
    }

    public static T Instance {
        get {
            if (_instance == null)
            {
                FindInstance();
            }

            return _instance;
        }
    }

    public static bool Exists {
        get {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }

            if (_instance == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}