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
    public GameObject ColumnGrouper;
    public List<Sprite> DaylightSprites;
    public List<Sprite> DarknessSprites;

    public static int GridWidth = 8;
    public static int GridHeight = 8;
    
    private readonly SpriteRenderer[,] _cells = new SpriteRenderer[GridWidth, GridHeight];
    private readonly Vector3[,] _defaultPositions = new Vector3[GridWidth, GridHeight];
    private StoreController _storeController;
    private bool _isDaylight = true;
    protected void Awake()
    {
      _storeController = FindObjectOfType<StoreController>();
      
      Messenger.Instance.AddHandler("ReplaceBlocks", ReplaceAllBlocks);
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
          slot.GetComponent<BlockMover>().TargetPosition = slot.transform.position;
          
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
          _defaultPositions[x, y] = slot.transform.position;
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
      var swapCellIndex = GetCell(swappingBlock);
      var swapCellX = (int)swapCellIndex.x;
      var swapCellY = (int)swapCellIndex.y;

      var dragCellIndex = GetCell(draggableBlock);
      var swappingTransformPosition = _defaultPositions[swapCellX, swapCellY];

      var draggableBlockMover = draggableBlock.GetComponent<BlockMover>();
      var swappingBlockMover = swappingBlock.GetComponent<BlockMover>();

      draggableBlockMover.TargetPosition = swappingTransformPosition;
      swappingBlockMover.TargetPosition = startPosition;
      yield return new WaitForSeconds(0.5f);

      _cells[swapCellX, swapCellY] = draggableBlock.GetComponent<SpriteRenderer>();
      _cells[(int)dragCellIndex.x, (int)dragCellIndex.y] = swappingBlock.GetComponent<SpriteRenderer>();

      var matchingSprite = draggableBlock.GetComponent<SpriteRenderer>().sprite.name;
      var matches = new List<GameObject> { draggableBlock };
      var matchesByX = new List<GameObject>();
      var matchesByY = new List<GameObject>();
      

      for (int x = swapCellX+1; x < GridHeight; x++)
      {
        if(_cells[x, swapCellY].sprite.name == matchingSprite)
        {
          matchesByX.Add(_cells[x, swapCellY].gameObject);
        }
        else
        {
          break;
        }
      }

      for (int x = swapCellX-1; x >= 0; x--)
      {
        if (_cells[x, swapCellY].sprite.name == matchingSprite)
        {
          matchesByX.Add(_cells[x, swapCellY].gameObject);
        }
        else
        {
          break;
        }
      }

      if(matchesByX.Count > 1)
      {
        matches.AddRange(matchesByX);
      }

      for (int y = swapCellY+1; y < GridWidth; y++)
      {
        if (_cells[swapCellX, y].sprite.name == matchingSprite)
        {
          matchesByY.Add(_cells[swapCellX, y].gameObject);
        }
        else
        {
          break;
        }
      }

      for (int y = swapCellY-1; y >= 0; y--)
      {
        if (_cells[swapCellX, y].sprite.name == matchingSprite)
        {
          matchesByY.Add(_cells[swapCellX, y].gameObject);
        }
        else
        {
          break;
        }
      }

      if (matchesByY.Count > 1)
      {
        matches.AddRange(matchesByY);
      }

      //not enough matches, return block 
      if (matches.Count < 3)
      {
        _cells[swapCellX, swapCellY] = swappingBlock.GetComponent<SpriteRenderer>();
        _cells[(int)dragCellIndex.x, (int)dragCellIndex.y] = draggableBlock.GetComponent<SpriteRenderer>();
        
        draggableBlockMover.TargetPosition = startPosition;
        swappingBlockMover.TargetPosition = swappingTransformPosition;
        yield return new WaitForSeconds(1.0f);
        yield break;
      }
      
      Refresh();
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
      while (true)
      {
        var matches = FindMatch();
        
        if (matches.Count > 0)
        {
          // TODO: Replace with messaging system
          _storeController.AddItem(matches.First().sprite.name, matches.Count);
          
          foreach (var match in matches)
          {
            match.GetComponent<SpriteRenderer>().enabled = false;
            match.GetComponent<SpriteRenderer>().sprite = CurrentSprites[Random.Range(0, CurrentSprites.Count)];

            MoveColumn(match);
            RefreshTopMatchedBlock(match.gameObject);
          }

          yield return new WaitForSeconds(0.3f);
        }
        else
        {
          yield break;
        }

        yield return new WaitForSeconds(0.5f);
        yield return null;
      }
    }

    private void MoveColumn(SpriteRenderer match)
    {
      var cell = GetCell(match.gameObject);
      var column = (int) cell.x;
      var trgRow = (int) cell.y;

      var columnGrouper = Instantiate(ColumnGrouper, _defaultPositions[column, trgRow], Quaternion.identity) as GameObject;
      columnGrouper.transform.SetParent(transform);

      for (int curRow = trgRow + 1; curRow < GridHeight; curRow++)
      {
        var curBlock = _cells[column, curRow];
        var lowBlock = _cells[column, trgRow];

        _cells[column, trgRow] = curBlock;
        _cells[column, curRow] = lowBlock;

        var targetPosition = _defaultPositions[column, curRow];
        var lowerPosition = _defaultPositions[column, trgRow];

        curBlock.transform.SetParent(columnGrouper.transform);

        curBlock.GetComponent<BlockMover>().TargetPosition = lowerPosition;
        lowBlock.GetComponent<BlockMover>().TargetPosition = targetPosition;

        trgRow = curRow;
      }

      columnGrouper.GetComponent<BlockMover>().TargetPosition = _defaultPositions[column, (int) cell.y];

      
      //RefreshTopMatchedBlock(match.gameObject);
      for (int i = 0; i < columnGrouper.transform.childCount; i++)
      {
        var child = columnGrouper.transform.GetChild(i);
        child.SetParent(transform);
      }
      //Destroy(columnGrouper.gameObject, 10.0f);
    }

    private void RefreshTopMatchedBlock(GameObject match)
    {
      var blockMover = match.GetComponent<BlockMover>();
      blockMover.enabled = false;
      match.transform.position += Vector3.up*3;
      blockMover.enabled = true;
      match.GetComponent<SpriteRenderer>().enabled = true;
    }
    
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

        if (matches.Count > 2)
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

        if (matches.Count > 2)
        {
          return matches;
        }
      }

      return Enumerable.Empty<SpriteRenderer>().ToList();
    }
  }
}
