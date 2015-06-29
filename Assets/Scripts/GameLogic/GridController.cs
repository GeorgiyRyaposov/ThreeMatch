namespace Assets.Scripts.GameLogic
{
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using Infrastructure;
  using UnityEngine;
  using UnityEngine.UI;  

  public class GridController : MonoBehaviour
  {
    public GameObject Slot;
    public Sprite[] Sprites;

    public static int GridWidth = 8;
    public static int GridHeight = 8;
    private readonly Image[,] _cells = new Image[GridWidth, GridHeight];

    protected void Start()
    {
      for (int x = 0; x < GridWidth; x++)
      {
        for (int y = 0; y < GridHeight; y++)
        {
          var slot = Instantiate(Slot);
          slot.transform.SetParent(transform, false);
          
          // To fix sorting order of canvas' children
          var canvas = slot.AddComponent<Canvas>();
          canvas.overrideSorting = true;
          canvas.sortingOrder = 1;

          var matches = new List<Sprite>();
          
          if (x > 1 && _cells[x - 2, y].sprite == _cells[x - 1, y].sprite)
          {
            matches.Add(_cells[x - 1, y].sprite);
          }
          if (y > 1 && _cells[x, y - 2].sprite == _cells[x, y - 1].sprite)
          {
            matches.Add(_cells[x, y - 1].sprite);
          }

          var sprites = Sprites.Except(matches).ToList();
          var sprite = sprites[Random.Range(0, sprites.Count)];

          slot.GetComponent<Image>().sprite = sprite;
          _cells[x, y] = slot.GetComponent<Image>();
        }
      }
    }

    private Vector2 GetCell(GameObject block)
    {
      for (int x = 0; x < GridWidth; x++)
      {
        for (int y = 0; y < GridHeight; y++)
        {
          if (block == _cells[x, y].gameObject)
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

    public void Refresh()
    {
      StartCoroutine(RemoveMatches());
    }

    private IEnumerator RemoveMatches()
    {
      while (true)
      {
        var matches = FindMatch();
        if (matches.Count > 0)
        {
          var changedBlocks = new List<GameObject>();
          foreach (var match in matches)
          {
              var index = GetCell(match.gameObject);
              var y = (int) index.y;
              for (var x = (int)index.x; x >= 0; x--)
              {
                  Sprite sprite;
                  if (x == 0)
                  {
                      sprite = Sprites[Random.Range(0, Sprites.Length)];
                  }
                  else
                  {
                      sprite = _cells[x - 1, y].GetComponent<Image>().sprite;
                  }

                  _cells[x, y].GetComponent<BlockController>().ReplacementSprite = sprite;
                  if (!changedBlocks.Contains(_cells[x, y].gameObject))
                  {
                      changedBlocks.Add(_cells[x, y].gameObject);
                  }
              }
          }
          foreach (var changedBlock in changedBlocks)
          {
              changedBlock.GetComponent<Animation>().Play();
          }

          yield return new WaitForSeconds(1.0f);
        }
        else
        {
          yield break;          
        }

        yield return null;
      }      
    }

    private List<Image> FindMatch()
    {
      // TODO: Return matches with max count
        var matches = new List<Image>();
      for (int x = 0; x < GridWidth; x++)
      {
        var column = Enumerable.Range(0, GridHeight)
          .Select(row => _cells[row, x])
          .ToList();

        var colMatches = column
          .FindMatches((item1, item2) => item1.sprite == item2.sprite)
          .TakeMatches((item1, item2) => item1.sprite == item2.sprite)
          .ToList();

        if (colMatches.Count > 2)
        {
            matches.AddRange(colMatches);
        }
      }

      for (int y = 0; y < GridHeight; y++)
      {
        var row = Enumerable.Range(0, GridWidth)
          .Select(column => _cells[y, column])
          .ToList();

        var rowMatches = row
          .FindMatches((item1, item2) => item1.sprite == item2.sprite)
          .TakeMatches((item1, item2) => item1.sprite == item2.sprite)
          .ToList();

        if (rowMatches.Count > 2)
        {
            matches.AddRange(rowMatches);
        }
      }

        if (matches.Count > 2)
            return matches;
      return Enumerable.Empty<Image>().ToList();
    }
  }
}
