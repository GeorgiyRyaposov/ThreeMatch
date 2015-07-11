using UnityEngine;

namespace Assets.Scripts.GameLogic
{
  public class BlockMover : MonoBehaviour
  {
    [HideInInspector]
    public Vector3 TargetPosition;
  
    protected void Update()
    {
      transform.position = Vector3.Lerp(transform.position, TargetPosition, 5.0f * Time.deltaTime);
    }
  }
}
