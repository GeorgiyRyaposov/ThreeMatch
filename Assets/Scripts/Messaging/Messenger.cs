namespace Assets.Scripts.Messaging
{
  using System.Collections.Generic;
  using UnityEngine;
  using UnityEngine.Events;

  public class Messenger : MonoBehaviour 
  {
    private readonly Dictionary<string, UnityEvent> _messageMap = new Dictionary<string, UnityEvent>();

    private static Messenger _instance;
    public static Messenger Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = FindObjectOfType<Messenger>();

          if (_instance == null)
          {
            var go = new GameObject("Messenger");
            
            DontDestroyOnLoad(go);
            
            _instance = go.AddComponent<Messenger>();
          }
        }

        return _instance;
      }
    }

    protected void Awake()
    {
      if (_instance == null)
      {
        _instance = this;

        DontDestroyOnLoad(this);
      }
      else
      {
        if (this != _instance)
          Destroy(this.gameObject);
      }
    }

    public void AddHandler(string message, UnityAction handler)
    {
      UnityEvent thisEvent = null;
      if (_messageMap.TryGetValue(message, out thisEvent))
      {
        thisEvent.AddListener(handler);
      }
      else
      {
        thisEvent = new UnityEvent();
        thisEvent.AddListener(handler);
        _messageMap.Add(message, thisEvent);
      }
    }

    public void RemoveHandler(string message, UnityAction handler)
    {
      UnityEvent thisEvent = null;
      if (_messageMap.TryGetValue(message, out thisEvent))
      {
        thisEvent.RemoveListener(handler);
      }
    }

    public new void SendMessage(string message)
    {
      UnityEvent thisEvent = null;
      if (_messageMap.TryGetValue(message, out thisEvent))
      {
        thisEvent.Invoke();
      }
    }
  }
}
