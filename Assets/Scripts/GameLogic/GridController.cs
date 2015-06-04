namespace Assets.Scripts.GameLogic
{
  using Messaging;
  using UnityEngine;

  public class GridController : MonoBehaviour
  {
    private BlockFactory _factory;

    protected void Start()
    {
      _factory = FindObjectOfType<BlockFactory>();

      FillGrid();

      Messenger.Instance.SendMessage("DisplayGrid");
    }

    private void FillGrid()
    {
      for (var y = 0; y < Grid.GridSize; y++)
      {
        for (var x = 0; x < Grid.GridSize; x++)
        {
          string xMatch = null;
          if (x > 1 && Grid.BlocksGrid[x - 2, y].name == Grid.BlocksGrid[x - 1, y].name)
          {
            xMatch = Grid.BlocksGrid[x - 1, y].name;
          }

          string yMatch = null;
          if (y > 1 && Grid.BlocksGrid[x, y - 2].name == Grid.BlocksGrid[x, y - 1].name)
          {
            yMatch = Grid.BlocksGrid[x, y - 1].name;
          }

          Grid.BlocksGrid[x, y] = _factory.CreateBlockAt(x, y, xMatch, yMatch);
        }
      }
    }





    public void AddBlock(int x, int y)
    {
      var obj = _factory.CreateBlockAt(x, Grid.GridSize - 1);

      var block = obj.GetComponent<Block>();
      block.MoveToNewPoint(x, y);
      Grid.BlocksGrid[x, y] = obj;
    }

    public void DragBlock(Block draggebleBlock, int blockX, int blockY, int newX, int newY)
    {
      if (!Grid.CheckBorder(newX) || !Grid.CheckBorder(newY))
      {
        draggebleBlock.MoveToPreviousPoint();
        return;
      }

      if (Grid.CanMoveBlock(blockX, blockY, newX, newY) == false)
      {
        draggebleBlock.MoveToPreviousPoint();
        return;
      }

      var currBlock = Grid.BlocksGrid[blockX, blockY];
      var draggableBlockPosition = currBlock.GetComponent<Transform>().position;

      var nextBlock = Grid.BlocksGrid[newX, newY];
      var nextBlockPosition = nextBlock.GetComponent<Transform>().position;

      draggebleBlock.MoveToNewPoint(nextBlockPosition);
      nextBlock.GetComponent<Block>().MoveToNewPoint(draggableBlockPosition);

      Grid.BlocksGrid[blockX, blockY] = nextBlock;
      Grid.BlocksGrid[newX, newY] = currBlock;

      CollapseBlocks(newX, newY, draggebleBlock.BlockType);

      Messenger.Instance.SendMessage("DisplayGrid");
    }

    void CollapseBlocks(int x, int y, BlockTypes blockType)
    {
      var blocksToCollapse = Grid.GetSameNearBlocks(x, y, blockType);
      if (blocksToCollapse.Count < 3)
        return;

      foreach (var block in blocksToCollapse)
      {
        Grid.DeleteBlock(block.X, block.Y);
        Grid.DecreaseColumns(block.X);
      }

    }
  }
}
