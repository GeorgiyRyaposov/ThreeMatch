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
                    var blockType = GenerateBlockType(x, y);
                    AddBlock(x, y, blockType);
                }
            }
        }

        private int GenerateBlockType(int x, int y)
        {
            var nearBlocks = 0;
            var randomBlockType = 0;

            do
            {
                randomBlockType = Random.Range(0, _blockTypesCount);
                nearBlocks = Grid.CountNearBlocks(x, y, (BlockTypes)randomBlockType);
            } 
            while (nearBlocks >= 2);

            return randomBlockType;
        }

        private void AddBlock(int x, int y, int blockType)
        {
            var newBlock = Instantiate(Blocks[blockType],
                new Vector3(x, y, 0),
                Quaternion.identity);
            Grid.BlocksGrid[x, y] = ((GameObject)newBlock).transform;
            Grid.BlockTypesGrid[x, y] = (BlockTypes)blockType;
        }

        public void DragBlock(int blockX, int blockY, int newX, int newY)
        {
            if (!Grid.CheckBorder(newX) || !Grid.CheckBorder(newY))
                return;

            if (Grid.CanMoveBlock(blockX, blockY, newX, newY) == false)
                return;

            var currBlockPosition = Grid.BlocksGrid[blockX, blockY];
            var currBlockType = Grid.BlockTypesGrid[blockX, blockY];

            var nextBlockPosition = Grid.BlocksGrid[newX, newY];
            var nextBlockType = Grid.BlockTypesGrid[newX, newY];

            Grid.BlocksGrid[blockX, blockY] = nextBlockPosition;
            Grid.BlocksGrid[newX, newY] = currBlockPosition;

            Grid.BlockTypesGrid[blockX, blockY] = nextBlockType;
            Grid.BlockTypesGrid[newX, newY] = currBlockType;
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
                    txtMesh.text += (int)Grid.BlockTypesGrid[x, y];
                }
                txtMesh.text += "\n";
            }
        }
    }
}
