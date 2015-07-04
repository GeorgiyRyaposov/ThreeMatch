using System;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoreController : MonoBehaviour 
{
  private class StoreItem
  {
    public string Name;
    public int Amount;

    public override string ToString()
    {
      return Name.PadRight(12, '.') + Amount;
    }
  }

  public Text Log;

  private readonly StoreItem[] _store = new[]
  {
    new StoreItem() { Name = "Food" },
    new StoreItem() { Name = "Alchemy" },
    new StoreItem() { Name = "Leather" },
    new StoreItem() { Name = "Ore" },
    new StoreItem() { Name = "Wood" },
  };

  protected void Awake()
  {
    UpdateLog();
  }

  private void UpdateLog()
  {
    var result = new StringBuilder();
    foreach (var item in _store)
    {
      result.AppendLine(item.ToString());
    }

    Log.text = result.ToString();
  }
}
