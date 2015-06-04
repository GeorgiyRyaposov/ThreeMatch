namespace Assets.Scripts.GameLogic
{
  using Messaging;
  using UnityEngine;

  public class GridController : MonoBehaviour
  {
    public GameObject[] Blocks;

    void Start()
    {
      FillGrid();

      Messenger.Instance.SendMessage("DisplayGrid");
    }

    public void FillGrid()
    {
      for (var x = 0; x < Grid.GridSize; x++)
      {
        for (var y = 0; y < Grid.GridSize; y++)
        {
          //Add first block without check
          if (x == 0 && y == 0)
          {
            AddBlock(0, 0, Random.Range(0, Blocks.Length));
            continue;
          }

          var blockType = GenerateBlockType(x, y);
          AddBlock(x, y, blockType);
        }
      }
    }

    private int GenerateBlockType(int x, int y)
    {
      int nearBlocks;
      int randomBlockType;

      do
      {
        randomBlockType = Random.Range(0, Blocks.Length);
        nearBlocks = Grid.CountNearBlocks(x, y, (BlockTypes)randomBlockType);
      }
      while (nearBlocks >= 2);

      return randomBlockType;
    }

    public void AddBlock(int x, int y, int blockType)
    {
      var newBlock = Instantiate(Blocks[blockType],
          new Vector3(x, y, 0),
          Quaternion.identity) as GameObject;
      newBlock.transform.parent = transform;
      Grid.BlocksGrid[x, y] = newBlock;
    }

    public void AddBlock(int x, int y)
    {
      var newBlock = Instantiate(Blocks[Random.Range(0, Blocks.Length)],
          new Vector3(x, Grid.GridSize - 1, 0),
          Quaternion.identity) as GameObject;

      newBlock.transform.parent = transform;

      var block = newBlock.GetComponent<Block>();
      block.MoveToNewPoint(x, y);
      Grid.BlocksGrid[x, y] = newBlock;
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
