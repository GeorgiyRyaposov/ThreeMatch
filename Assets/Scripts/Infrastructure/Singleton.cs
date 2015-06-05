namespace Assets.Scripts.Infrastructure
{
  using UnityEngine;

  public class Singleton<T> : MonoBehaviour where T : Component
  {
    private static T _instance;
    public static T Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = FindObjectOfType<T>();

          if (_instance == null)
          {
            var obj = new GameObject(typeof(T).Name);
            obj.hideFlags = HideFlags.HideAndDontSave;
            
            _instance = obj.AddComponent<T>();
          }
        }
        return _instance;
      }
    }

    public virtual void Awake()
    {
      DontDestroyOnLoad(this.gameObject);

      if (_instance == null)
      {
        _instance = this as T;
      }
      else
      {
        Destroy(gameObject);
      }
    }
  }

}
