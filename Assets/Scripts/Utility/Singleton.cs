using UnityEngine;

public abstract class Singleton<TMono> : MonoBehaviour where TMono : MonoBehaviour
{
    private static TMono _instance;

    public static TMono Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TMono>();
                if (_instance == null)
                {
                    var go = new GameObject(typeof(TMono).Name);
                    _instance = go.AddComponent<TMono>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as TMono;
            Init();
            return;
        }

        Destroy(gameObject);
    }

    public static bool CheckNull()
    {
        return _instance == null;
    }
    protected virtual void Init() { }
}