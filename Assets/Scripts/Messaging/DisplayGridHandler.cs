namespace Assets.Scripts.Messaging
{
  using System;
  using GameLogic;
  using UnityEngine;
  using UnityEngine.UI;

  [RequireComponent(typeof(Text))]
  public class DisplayGridHandler : MonoBehaviour
  {
    private Text _gridDisplay;

    protected void OnEnable()
    {
      Messenger.Instance.AddHandler("DisplayGrid", DisplayGrid);
    }

    protected void OnDisable()
    {
      Messenger.Instance.RemoveHandler("DisplayGrid", DisplayGrid);
    }

    protected void Awake()
    {
      _gridDisplay = GetComponent<Text>();
    }

    private void DisplayGrid()
    {
      _gridDisplay.text = String.Empty;

      var maxY = Grid.GridSize - 1;
      for (int y = maxY; y >= 0; --y)
      {
        _gridDisplay.text += y.ToString("00") + " ";
        for (var x = 0; x < Grid.GridSize; ++x)
        {
          _gridDisplay.text += (int)Grid.BlockTypesGrid[x, y];
        }
        _gridDisplay.text += "\n";
      }
    }
  }
}
