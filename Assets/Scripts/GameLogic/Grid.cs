using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public struct Cell
    {
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X;
        public int Y;
    }

    public enum BlockTypes
    {
        None = -1,
        Brick = 0,
        Corn,
        Material,
        Rock,
        Wood
    }

    public class Grid : MonoBehaviour {

        public static int GridSize = 8;
        public static Transform[,] BlocksGrid = new Transform[GridSize, GridSize];
        public static BlockTypes[,] BlockTypesGrid = new BlockTypes[GridSize, GridSize];

        public static Vector2 RoundVec2(Vector2 v) 
        {
            return new Vector2(Mathf.Round(v.x),
                Mathf.Round(v.y));
        }

        public static void DeleteBlock(int x, int y) 
        {
            Destroy(BlocksGrid[x, y].gameObject);
            BlocksGrid[x, y] = null;
            BlockTypesGrid[x, y] = BlockTypes.None;
        }

        public static bool IsColumnFull(int x) 
        {
            for (int y = 0; y < GridSize; ++y)
            {
                if (BlocksGrid[x, y] == null)
                {
                    return false;
                }
            }
            return true;
        }

        public static void DecreaseColumnsAbove(int x) 
        {
            for (int i = x; i < GridSize; ++i) 
            {
                DecreaseColumn (i);
            }
        }

        public static void DecreaseColumn(int x) 
        {
            for (int y = 0; y < GridSize; ++y) 
            {
                if (BlocksGrid[x, y] != null) 
                {
                    // Move one towards bottom
                    BlocksGrid[x, y-1] = BlocksGrid[x, y];
                    BlocksGrid[x, y] = null;
				
                    // Update Block position
                    BlocksGrid[x, y-1].position += new Vector3(0, -1, 0);
                }
            }
        }

        /// <summary>
        /// Return list of blocks coordinate which are same
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blockType"></param>
        /// <returns></returns>
        public static List<Cell> GetSameNearBlocks(int x, int y, BlockTypes blockType)
        {
            var blocksToRemove = new List<Cell> { new Cell(x, y) };
		
            var tempX = x + 1;
            while(tempX < GridSize && CheckBlock(tempX, y, blockType))
            {
                blocksToRemove.Add(new Cell(tempX, y));
                tempX++;
            }

            tempX = x - 1;
            while (tempX > -1 && CheckBlock(tempX, y, blockType))
            {
                blocksToRemove.Add(new Cell(tempX, y));
                tempX--;
            }

            var tempY = y + 1;
            while (tempY < GridSize && CheckBlock(x, tempY, blockType) )
            {
                blocksToRemove.Add(new Cell(x, tempY));
                tempY++;
            }

            tempY = y - 1;
            while (tempY > -1 && CheckBlock(x, tempY, blockType))
            {
                blocksToRemove.Add(new Cell(x, tempY));
                tempY--;
            }

            return blocksToRemove;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blockType"></param>
        /// <returns></returns>
        public static int CountNearBlocks(int x, int y, BlockTypes blockType)
        {
            var count = 0;
            var tempX = x + 1;
            while (tempX < GridSize && CheckBlock(tempX, y, blockType))
            {
                count++;
                tempX++;
            }

            tempX = x - 1;
            while (tempX >= 0 && CheckBlock(tempX, y, blockType))
            {
                count++;
                tempX--;
            }

            var tempY = y + 1;
            while (tempY < GridSize && CheckBlock(x, tempY, blockType))
            {
                count++;
                tempY++;
            }

            tempY = y - 1;
            while (tempY >= 0 && CheckBlock(x, tempY, blockType))
            {
                count++;
                tempY--;
            }

            return count;
        }

        public static bool CheckBlock(int x, int y, BlockTypes blockType)
        {
            if (BlockTypesGrid[x, y] == BlockTypes.None)
                return false;

            return BlockTypesGrid[x,y] == blockType;
        }

        public static bool CanMoveBlock(int blockX, int blockY, int newX, int newY)
        {
            var selectedBlockType = BlockTypesGrid[blockX, blockY];
            var nearBlocks = CountNearBlocks(newX, newY, selectedBlockType);
            return nearBlocks >= 2;
        }

        public static bool CheckBorder(int value)
        {
            return value <= GridSize && value >= 0;
        }
    }
}