using UnityEngine;
using Assets.Scripts.Messaging;

public class BlockMover : MonoBehaviour
{
  [HideInInspector]
  public Vector3 TargetPosition;

  protected void Update()
  {
    transform.position = Vector3.Lerp(transform.position, TargetPosition, 5.0f * Time.deltaTime);

    //if (Vector2.Distance(transform.position, TargetPosition) < float.Epsilon)
    //{
    //  Messenger.Instance.SendMessage("BlockDestroyed");

    //  Destroy(this.gameObject);
    //}
  }
}
