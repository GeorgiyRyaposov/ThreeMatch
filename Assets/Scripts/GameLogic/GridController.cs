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

    private void UpdateSpritesGrid()
    {
      for (int x = 0; x < GridWidth; x++)
      {
        for (int y = 0; y < GridHeight; y++)
        {
          _cells[x, y] = _cells2[x, y].GetComponent<Image>().sprite;
        }
      }
    }
    
    public void CheckGrid(bool updateGrid = false)
    {
      if (updateGrid)
      {
        UpdateSpritesGrid();
      }

      var matches = new List<Vector2>();
      for (int x = 0; x < GridWidth; x++)
      {
        for (int y = 0; y < GridHeight; y++)
        {
          matches.Add(new Vector2(x, y));

          var rowMatches = new List<Vector2>();
          rowMatches.AddRange(GetNearBlocksInRow(x, y));
          if(rowMatches.Count > 1)
            matches.AddRange(rowMatches);

          var columnMatches = new List<Vector2>();
          columnMatches.AddRange(GetNearBlocksInColumn(x, y));
          if (columnMatches.Count > 1)
            matches.AddRange(columnMatches);
          
          if (matches.Count > 2)
          {
            matches = matches.OrderByDescending(v => v.y).ToList();
            CollapseBlocks(matches);
            CheckGrid();
            return;
          }
          else
          {
            matches.Clear();
          }
        }
      }
    }

    private void CollapseBlocks(IEnumerable<Vector2> blocks)
    {
      foreach (var block in blocks)
      {
        var y = (int)Mathf.Round(block.y);
        for (var x = (int)Mathf.Round(block.x); x >= 0; x--)
        {
          if (x == 0)
          {
            var sprite = Sprites[Random.Range(0, Sprites.Length)];
            _cells2[x, y].GetComponent<Image>().sprite = _cells[x, y] = sprite;
            continue;
          }

          _cells2[x, y].GetComponent<Image>().sprite = _cells[x, y] = _cells2[x - 1, y].GetComponent<Image>().sprite;
        }
      }
    }

    private IEnumerable<Vector2> GetNearBlocksInColumn(int x, int y)
    {
      for (var nextY = y + 1; nextY < GridHeight; nextY++)
      {
        if (_cells[x, y] == _cells[x, nextY])
          yield return new Vector2(x, nextY);
        else
          break;
      }
    }

    private IEnumerable<Vector2> GetNearBlocksInRow(int x, int y)
    {
      for (var nextX = x + 1; nextX < GridWidth; nextX++)
      {
        if (_cells[x, y] == _cells[nextX, y])
          yield return new Vector2(nextX, y);
        else
          break;
      }
    }

    private void LogGrid()
    {
      var str = "  Y:    01234567\n";
      for (int x = 0; x < GridWidth; x++)
      {
        str += ("X:" + x.ToString("00") + "| ");
        for (int y = 0; y < GridHeight; y++)
        {
          for (int i = 0; i < Sprites.Length; i++)
          {
            if (Sprites[i] == _cells[x, y])
              str += (i);
          }
        }

        str += "\n";
      }
      Debug.Log(str);
    }
  }
}
