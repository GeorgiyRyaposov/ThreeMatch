namespace Assets.Scripts.GameLogic
{
  using System.Linq;
  using UnityEngine;

  public class BlockFactory : MonoBehaviour 
  {
    public GameObject[] Blocks;

    public GameObject CreateBlockAt(int x, int y, params string[] exceptBlocks)
    {
      var blocks = Blocks.Where(b => exceptBlocks.All(ex => ex != b.name)).ToList();

      var index = Random.Range(0, blocks.Count);
      var block = Instantiate(blocks[index],
        new Vector3(x, y, 0),
        Quaternion.identity) as GameObject;

      if (block)
      {
        block.transform.parent = transform;
        block.name = blocks[index].name;
      }

      return block;
    }
  }
}
