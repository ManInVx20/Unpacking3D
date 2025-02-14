using UnityEngine;

namespace VinhLB
{
    public class PersistentMonoSingleton<T> : MonoBehaviour where T : Component 
    {
        public bool AutoUnparentOnAwake = true;
        
        protected static T _instance;
        
        public static bool HasInstance => _instance != null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name + "(Singleton)");
                        _instance = go.AddComponent<T>();
                    }
                }
                
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (AutoUnparentOnAwake)
            {
                transform.SetParent(null);
            }

            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}