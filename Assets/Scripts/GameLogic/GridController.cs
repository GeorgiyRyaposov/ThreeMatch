namespace Assets.Scripts.GameLogic
{
  using System.Collections.Generic;
  using System.Linq;
  using UnityEngine;
  using UnityEngine.UI;

  public class GridController : MonoBehaviour
  {
    public GameObject Slot;
    public Sprite[] Sprites;

    public static int GridWidth = 8;
    public static int GridHeight = 8;
    private readonly Sprite[,] _cells = new Sprite[GridWidth, GridHeight];
    private readonly GameObject[,] _cells2 = new GameObject[GridWidth, GridHeight];

    protected void Start()
    {
      for (int x = 0; x < GridWidth; x++)
      {
        for (int y = 0; y < GridHeight; y++)
        {
          var slot = Instantiate(Slot);
          slot.transform.SetParent(transform, false);          

          var matches = new List<Sprite>();
          
          if (x > 1 && _cells[x - 2, y] == _cells[x - 1, y])
          {
            matches.Add(_cells[x - 1, y]);
          }
          if (y > 1 && _cells[x, y - 2] == _cells[x, y - 1])
          {
            matches.Add(_cells[x, y - 1]);
          }

          var sprites = Sprites.Except(matches).ToList();
          var sprite = sprites[Random.Range(0, sprites.Count)];
          _cells2[x, y] = slot;
          slot.GetComponent<Image>().sprite = _cells[x, y] = sprite;
        }
      }
    }

    private Vector2 GetCell(GameObject block)
    {
      for (int x = 0; x < GridWidth; x++)
      {
        for (int y = 0; y < GridHeight; y++)
        {
          if (block == _cells2[x, y])
          {
            return new Vector2(x, y);
          }
        }
      }

      return Vector2.zero;
    }

    public bool CanFlip(GameObject first, GameObject second)
    {
      return !(Vector2.Distance(GetCell(first), GetCell(second)) > 1);
    }
  }
}
