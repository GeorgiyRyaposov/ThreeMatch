namespace Assets.Scripts.GameLogic
{
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using Infrastructure;
  using Messaging;
  using UnityEngine;

  public class GridController : MonoBehaviour
  {
    public GameObject Slot;
    public GameObject ColumnGrouper;
    public List<Sprite> DaylightSprites;
    public List<Sprite> DarknessSprites;

    public static int Columns = 8;
    public static int Rows = 8;
    public static int BlockSize = 100;
    
    private readonly SpriteRenderer[,] _cells = new SpriteRenderer[Columns, Rows];
    private readonly Vector3[,] _defaultPositions = new Vector3[Columns, Rows];
    private StoreController _storeController;
    private bool _isDaylight = true;
    private const float SwapTime = 0.5f;

    protected void Awake()
    {
      _storeController = FindObjectOfType<StoreController>();
      
      Messenger.Instance.AddHandler("ReplaceBlocks", ReplaceAllBlocks);
    }
    
    protected void Start()
    {
      for (int column = 0; column < Columns; column++)
      {
        for (int row = 0; row < Rows; row++)
        {
          var matches = new List<Sprite>();
          
          if (column > 1 && _cells[column - 2, row].sprite == _cells[column - 1, row].sprite)
          {
            matches.Add(_cells[column - 1, row].sprite);
          }
          if (row > 1 && _cells[column, row - 2] == _cells[column, row - 1])
          {
            matches.Add(_cells[column, row - 1].sprite);
          }

          var sprites = CurrentSprites.Except(matches).ToList();
          var sprite = sprites[Random.Range(0, sprites.Count)];

          var slot = InitBlock(column, row, sprite);

          _cells[column, row] = slot.GetComponent<SpriteRenderer>();
          _defaultPositions[column, row] = slot.transform.position;
        }
      }

      FixGrid();
    }
    
    private GameObject InitBlock(int column, int row, Sprite sprite)
    {
      var slot = Instantiate(Slot);
      slot.transform.SetParent(transform, false);
      slot.transform.localPosition = new Vector3(column, row, 0.0f);

      slot.GetComponent<SpriteRenderer>().sprite = sprite;
      return slot;
    }
    
    public IEnumerator TrySwap(Vector3 startPosition, GameObject draggableBlock, GameObject swappingBlock)
    {
      SwapBlocks(draggableBlock, swappingBlock);
      yield return new WaitForSeconds(SwapTime);
      var matches = FindMatch();

      //not enough matches, return block 
      if (matches.Count < 3)
      {
        SwapBlocks(swappingBlock, draggableBlock);
        yield return new WaitForSeconds(SwapTime);
        yield break;
      }
      
      FixGrid();
    }

    private void SwapBlocks(GameObject firstBlock, GameObject secondBlock)
    {
      var indexFirst = _cells.IndexOf(firstBlock);
      var indexSecond = _cells.IndexOf(secondBlock);

      var defPosFirst = _defaultPositions[indexFirst.X, indexFirst.Y];
      var defPosSecond = _defaultPositions[indexSecond.X, indexSecond.Y];

      _cells[indexFirst.X, indexFirst.Y] = secondBlock.GetComponent<SpriteRenderer>();
      _cells[indexSecond.X, indexSecond.Y] = firstBlock.GetComponent<SpriteRenderer>();

      StartCoroutine(Move(firstBlock, defPosSecond));
      StartCoroutine(Move(secondBlock, defPosFirst));
    }

    private void TestMatches(IEnumerable<SpriteRenderer> matches)
    {
      foreach (var match in matches)
      {
        match.GetComponent<SpriteRenderer>().color = Color.red;
      }
    }

    public static IEnumerator Move(GameObject blockToMove, Vector3 targetPosition)
    {
      var blockMover = blockToMove.GetComponent<BlockMover>();
      blockMover.TargetPosition = targetPosition;
      blockMover.enabled = true;
      yield return new WaitForSeconds(SwapTime);
    }

//    public bool CanFlip(GameObject first, GameObject second)
//    {
//      return !(Vector2.Distance(GetCell(first), GetCell(second)) > 1);
//    }

    public void FixGrid()
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

            var matchIndex = _cells.IndexOf(match.gameObject);

            MoveColumn(match);
            MoveBlockOnTop(match.gameObject, matchIndex.X);
          }

//          yield return new WaitForSeconds(SwapTime);
        }
        else
        {
          yield break;
        }

        yield return new WaitForSeconds(SwapTime);
        yield return null;
      }
    }

    private void MoveColumn(SpriteRenderer match)
    {
      var cell = _cells.IndexOf(match.gameObject);
      var column = cell.X;
      var trgRow = cell.Y;
      
      for (int curRow = trgRow + 1; curRow < Rows; curRow++)
      {
        var curBlock = _cells[column, curRow];
        var bottomBlock = _cells[column, trgRow];
        SwapBlocks(curBlock.gameObject, bottomBlock.gameObject);
        trgRow = curRow;
      }
    }

    private void MoveBlockOnTop(GameObject match, int column)
    {
      var topPosition = _defaultPositions[column, Rows-1];

      match.transform.position = topPosition + Vector3.up*3;
      StartCoroutine(Move(match, topPosition));
      match.GetComponent<SpriteRenderer>().enabled = true;
    }
    
    private void ReplaceAllBlocks()
    {
      for (int x = 0; x < Columns; x++)
      {
        for (int y = 0; y < Rows; y++)
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
      var totalMatches = new List<SpriteRenderer>();
      for (int x = 0; x < Columns; x++)
      {
        var column = Enumerable.Range(0, Rows)
          .Select(row => _cells[row, x])
          .ToList();

        var matches = column
          .FindMatches((item1, item2) => item1.sprite == item2.sprite)
          .TakeMatches((item1, item2) => item1.sprite == item2.sprite)
          .ToList();

        if (matches.Count > 2)
        {
          totalMatches.AddRange(matches);
        }
      }

      for (int y = 0; y < Rows; y++)
      {
        var row = Enumerable.Range(0, Columns)
          .Select(column => _cells[y, column])
          .ToList();

        var matches = row
          .FindMatches((item1, item2) => item1.sprite == item2.sprite)
          .TakeMatches((item1, item2) => item1.sprite == item2.sprite)
          .ToList();

        if (matches.Count > 2)
        {
          totalMatches.AddRange(matches);
        }
      }
      if (totalMatches.Count > 2)
      {
        return totalMatches;
      }
      return Enumerable.Empty<SpriteRenderer>().ToList();
    }
  }

  public class Index
  {
    public Index(int x, int y)
    {
      X = x;
      Y = y;
    }
    public int X { get; private set; }
    public int Y { get; private set; }
  }
}
