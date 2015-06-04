using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
  public class GameController : MonoBehaviour {

    public GameObject[] Blocks;
    private int _blockTypesCount;
    // Use this for initialization
    void Start ()
    {
      FillGrid();
      UpdateGridDisplay();
    }
	
    // Update is called once per frame
    void Update () {
      
    }

    public void SpawnNext() 
    {
        var blockTypesCount = System.Enum.GetNames(typeof(BlockTypes)).Length;
        // Random Index
        int i = Random.Range(0, blockTypesCount);
		
        // Spawn Group at current Position
        Instantiate(Blocks[i],
            transform.position,
            Quaternion.identity);
    }

    public void FillGrid() 
    {
      _blockTypesCount = Enum.GetNames(typeof(BlockTypes)).Length-1;
		
      for (var x = 0; x < Grid.GridSize; x++)
      {
        for (var y = 0; y < Grid.GridSize; y++)
        {
          //Add first block without check
          if (x == 0 && y == 0)
          {
            AddBlock(0, 0, Random.Range(0, _blockTypesCount));
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
            randomBlockType = Random.Range(0, _blockTypesCount);
            nearBlocks = Grid.CountNearBlocks(x, y, (BlockTypes)randomBlockType);
        } 
        while (nearBlocks >= 2);

        return randomBlockType;
    }

    public void AddBlock(int x, int y, int blockType)
    {
        var newBlock = Instantiate(Blocks[blockType],
            new Vector3(x, y, 0),
            Quaternion.identity);
        Grid.BlocksGrid[x, y] = (GameObject)newBlock;
    }

    public void AddBlock(int x, int y)
    {
      var newBlock = Instantiate(Blocks[Random.Range(0, _blockTypesCount)],
          new Vector3(x, Grid.GridSize-1, 0),
          Quaternion.identity) as GameObject;

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
      
      UpdateGridDisplay();
    }

    void CollapseBlocks(int x, int y, BlockTypes blockType)
    {
      var blocksToCollapse = Grid.GetSameNearBlocks(x, y, blockType);
      if(blocksToCollapse.Count < 3)
        return;

      foreach (var block in blocksToCollapse)
      {
        Grid.DeleteBlock(block.X, block.Y);
        Grid.DecreaseColumns(block.X);
      }
        
    }

    void UpdateGridDisplay()
    {
        var displayText = GameObject.FindGameObjectsWithTag("UserInterface").First();
        if (displayText == null)
        {
            return;
        }
        var txtMesh = displayText.GetComponentInChildren<Text>();
        if (txtMesh == null)
        {
            Debug.LogWarning("Text controll not found");
            return;
        }
        txtMesh.text = String.Empty;

        var maxY = Grid.GridSize - 1;
        for (int y = maxY; y >= 0; --y)
        {
            txtMesh.text += y.ToString("00") + " ";
            for (var x = 0; x < Grid.GridSize; ++x)
            {
                txtMesh.text += (int)Grid.BlocksGrid[x, y].GetComponent<Block>().BlockType;
            }
            txtMesh.text += "\n";
        }
    }
  }
}
