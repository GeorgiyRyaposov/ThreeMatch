namespace Assets.Scripts.Messaging
{
  using System.Collections.Generic;
  using Infrastructure;
  using UnityEngine;
  using UnityEngine.Events;

  public class Messenger : Singleton<Messenger>
  {
    private readonly Dictionary<string, UnityEvent> _messageMap = new Dictionary<string, UnityEvent>();

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
