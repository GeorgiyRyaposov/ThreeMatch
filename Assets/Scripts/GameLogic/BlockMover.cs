namespace Assets.Scripts.GameLogic
{
  using UnityEngine;
  public class BlockMover : MonoBehaviour
  {
    [HideInInspector]
    public Vector3 TargetPosition;
    private Vector3 _velocity = Vector3.zero;
    private const float SmoothTime = 0.2F;

    protected void Update()
    {
      transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref _velocity, SmoothTime);

      if (transform.position == TargetPosition)
        this.enabled = false;
    }
    
  }
}
