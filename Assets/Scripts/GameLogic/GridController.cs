namespace Assets.Scripts.GameLogic
{
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using Infrastructure;
  using Messaging;
  using UnityEngine;
  using UnityEngine.UI;  

  public class GridController : MonoBehaviour
  {
    public GameObject Slot;
    public List<Sprite> DaylightSprites;
    public List<Sprite> DarknessSprites;

    public static int GridWidth = 8;
    public static int GridHeight = 8;

    public GameObject FallingBlock;
    
    private readonly SpriteRenderer[,] _cells = new SpriteRenderer[GridWidth, GridHeight];
    private StoreController _storeController;
    private bool _isDaylight = true;

    protected void Awake()
    {
      _storeController = FindObjectOfType<StoreController>();
      

      Messenger.Instance.AddHandler("ReplaceBlocks", ReplaceAllBlocks);
      Messenger.Instance.AddHandler("BlockDestroyed", () => { _blockCount--; });
    }

    protected void Start()
    {
      for (int x = 0; x < GridWidth; x++)
      {
        for (int y = 0; y < GridHeight; y++)
        {
          var slot = Instantiate(Slot) as GameObject;
          slot.transform.SetParent(transform, false);
          slot.transform.localPosition = new Vector3(x, y, 0.0f);

          // To fix sorting order of canvas' children
          //var canvas = slot.AddComponent<Canvas>();
          //canvas.overrideSorting = true;
          //canvas.sortingOrder = 1;

          var matches = new List<Sprite>();
          
          if (x > 1 && _cells[x - 2, y].sprite == _cells[x - 1, y].sprite)
          {
            matches.Add(_cells[x - 1, y].sprite);
          }
          if (y > 1 && _cells[x, y - 2] == _cells[x, y - 1])
          {
            matches.Add(_cells[x, y - 1].sprite);
          }

          var sprites = CurrentSprites.Except(matches).ToList();
          var sprite = sprites[Random.Range(0, sprites.Count)];

          slot.GetComponent<SpriteRenderer>().sprite = sprite;
          _cells[x, y] = slot.GetComponent<SpriteRenderer>();
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

    public IEnumerator TrySwap(Vector3 startPosition, GameObject draggableBlock, GameObject swappingBlock)
    {
      var cellIndex = GetCell(swappingBlock);
      var matchingSprite = swappingBlock.GetComponent<SpriteRenderer>().sprite.name;
      var matches = new List<GameObject>();
      var matchesByX = new List<GameObject>();
      var matchesByY = new List<GameObject>();
      var matchIndexes = new List<Index>();
      var cellX = (int)cellIndex.x;
      var cellY = (int)cellIndex.y;

      for (int x = cellX; x < GridHeight; x++)
      {
        if(_cells[x, cellY].sprite.name == matchingSprite)
        {
          matchIndexes.Add(new Index(x, cellY));
          matchesByX.Add(_cells[x, cellY].gameObject);
        }
        else
        {
          break;
        }
      }

      for (int x = cellX; x >= 0; x--)
      {
        if (_cells[x, cellY].sprite.name == matchingSprite)
        {
          matchIndexes.Add(new Index(x, cellY));
          matchesByX.Add(_cells[x, cellY].gameObject);
        }
        else
        {
          break;
        }
      }

      for (int y = cellY; y < GridWidth; y++)
      {
        if (_cells[cellX, y].sprite.name == matchingSprite)
        {
          matchIndexes.Add(new Index(cellX, y));
          matchesByY.Add(_cells[cellX, y].gameObject);
        }
        else
        {
          break;
        }
      }

      for (int y = cellY; y >= 0; y--)
      {
        if (_cells[cellX, y].sprite.name == matchingSprite)
        {
          matchIndexes.Add(new Index(cellX, y));
          matchesByY.Add(_cells[cellX, y].gameObject);
        }
        else
        {
          break;
        }
      }

      //not enough matches, return block 
      Debug.Log("matches: " + matches.Count);
      if (matches.Count < 3)
      {
        draggableBlock.GetComponent<BlockMover>().TargetPosition = startPosition;
        yield return new WaitForSeconds(1.0f);
        yield break;
      }

      var swappingTransform = swappingBlock.GetComponent<Transform>();

      var draggableBlockMover = draggableBlock.GetComponent<BlockMover>();
      var swappingBlockMover = swappingBlock.GetComponent<BlockMover>();

      draggableBlockMover.TargetPosition = swappingTransform.position;
      swappingBlockMover.TargetPosition = startPosition;

      var swapCellIndex = GetCell(swappingBlock);
      _cells[cellX, cellY] = swappingBlock.GetComponent<SpriteRenderer>();
      _cells[(int)swapCellIndex.x, (int)swapCellIndex.y] = draggableBlock.GetComponent<SpriteRenderer>();
    }

    class Index
    {
      public Index(int x, int y)
      {
        X = x;
        Y = y;
      }
      public int X { get; set; }
      public int Y { get; set; }
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
      yield return new WaitForSeconds(0.2f);

      while (true)
      {
        var matches = FindMatch();
        if (matches.Count > 0)
        {
          // TODO: Replace with messaging system
          _storeController.AddItem(matches.First().sprite.name, matches.Count);

          _blockCount = 0;

          foreach (var match in matches)
          {
            //match.CrossFadeAlpha(0.0f, 0.0f, false);

            var cell = GetCell(match.gameObject);
            var trgRow = (int) cell.x;
            var column = (int) cell.y;

            for (int curRow = (int) cell.x - 1; curRow >= 0; curRow--)
            {
              var image = _cells[curRow, column];

              if (image.color.a > 0)
              {
                var block = Instantiate(FallingBlock, image.transform.position, Quaternion.identity) as GameObject;
                if (block)
                {
                  block.transform.SetParent(GameObject.Find("Canvas").transform, true);
                  block.transform.localScale = Vector3.one;
                  block.GetComponent<Image>().sprite = image.sprite;
                  block.GetComponent<FallingBlockController>().TargetPosition = _cells[trgRow, column].transform.position;
                  block.GetComponent<FallingBlockController>().enabled = true;

                  _blockCount++;
                }

                _cells[trgRow, column].sprite = image.sprite;

                trgRow = curRow;
                //image.CrossFadeAlpha(0.0f, 0.0f, false);
              }
            }

            var finalSprite = CurrentSprites[Random.Range(0, CurrentSprites.Count)];
            var finalBlock = Instantiate(FallingBlock, _cells[trgRow, column].transform.position + new Vector3(0.0f, 0.9f, 0.0f), Quaternion.identity) as GameObject;
            if (finalBlock)
            {
              finalBlock.transform.SetParent(GameObject.Find("Canvas").transform, true);
              finalBlock.transform.localScale = Vector3.one;
              finalBlock.GetComponent<Image>().sprite = finalSprite;
              finalBlock.GetComponent<FallingBlockController>().TargetPosition = _cells[trgRow, column].transform.position;
              finalBlock.GetComponent<FallingBlockController>().enabled = true;

              _blockCount++;
            }

            _cells[trgRow, column].sprite = finalSprite;
          }

//          var fallingBlocksExist = true;
//          while (fallingBlocksExist)
//          {
//            fallingBlocksExist = FindObjectsOfType<FallingBlockController>().Length > 0;
//
//            yield return null;
//          }

          while (_blockCount > 0)
          {
            yield return null;
          }

          for (int x = 0; x < GridWidth; x++)
          {
            for (int y = 0; y < GridHeight; y++)
            {
              //_cells[x, y].CrossFadeAlpha(1.0f, 0.0f, false);
            }
          }

          yield return null;
        }
        else
        {
          yield break;          
        }

        yield return null;
      }      
    }

    private int _blockCount;

    private void ReplaceAllBlocks()
    {
      for (int x = 0; x < GridWidth; x++)
      {
        for (int y = 0; y < GridHeight; y++)
        {
          var index = CurrentSprites.IndexOf(_cells[x, y].sprite);

          var sprite = HiddenSprites[index];
          _cells[x, y].gameObject.GetComponent<BlockController>().ReplacementSprite = sprite;
          _cells[x, y].gameObject.GetComponent<Animation>().Play();
        }
      }

      _isDaylight = !_isDaylight;
    }

    private List<Sprite> CurrentSprites
    {
      get { return _isDaylight ? DaylightSprites : DarknessSprites; }
    }

    private List<Sprite> HiddenSprites
    {
      get { return _isDaylight ? DarknessSprites : DaylightSprites; }
    }

    private List<SpriteRenderer> FindMatch()
    {
      // TODO: Return matches with max count

      for (int x = 0; x < GridWidth; x++)
      {
        var column = Enumerable.Range(0, GridHeight)
          .Select(row => _cells[row, x])
          .ToList();

        var matches = column
          .FindMatches((item1, item2) => item1.sprite == item2.sprite)
          .TakeMatches((item1, item2) => item1.sprite == item2.sprite)
          .ToList();

        if (matches.Count > 0)
        {
          return matches;
        }
      }

      for (int y = 0; y < GridHeight; y++)
      {
        var row = Enumerable.Range(0, GridWidth)
          .Select(column => _cells[y, column])
          .ToList();

        var matches = row
          .FindMatches((item1, item2) => item1.sprite == item2.sprite)
          .TakeMatches((item1, item2) => item1.sprite == item2.sprite)
          .ToList();

        if (matches.Count > 0)
        {
          return matches;
        }
      }

      return Enumerable.Empty<SpriteRenderer>().ToList();
    }
  }
}
