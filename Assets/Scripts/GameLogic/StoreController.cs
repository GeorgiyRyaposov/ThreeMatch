namespace Assets.Scripts.GameLogic
{
  using System;
  using System.Linq;
  using System.Text;
  using Messaging;
  using UnityEngine;
  using UnityEngine.UI;

  [Serializable]
  public class StoreItem
  {
    public string Name;
    public Sprite Image;  
  
    [HideInInspector]
    public int Amount;

    public override string ToString()
    {
      return Name.PadRight(12, '.') + Amount;
    }
  }

  public class StoreController : MonoBehaviour 
  {
    public Text Log;
    public StoreItem[] Store;

    public int StoreUnit = 12;

    protected void Awake()
    {
      UpdateLog();
    }

//  protected void Update()
//  {
//    if (Input.GetKeyUp(KeyCode.Space))
//    {
//      AddItem("Berry_1", 10);
//    }
//  }

    public void AddItem(string itemName, int amount)
    {
      var item = Store.FirstOrDefault(x => x.Image.name == itemName);
      if (item != null)
      {
        var units = item.Amount / StoreUnit;

        item.Amount += amount;

        if ((item.Amount/StoreUnit) > units)
        {
          Messenger.Instance.SendMessage("AddHour");
        }        

        UpdateLog();
      }
    }

    private void UpdateLog()
    {
      var result = new StringBuilder();
      foreach (var item in Store)
      {
        result.AppendLine(item.ToString());
      }

      Log.text = result.ToString();
    }
  }
}