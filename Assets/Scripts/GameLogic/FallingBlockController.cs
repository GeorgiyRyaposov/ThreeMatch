namespace Assets.Scripts.GameLogic
{
  using Messaging;
  using UnityEngine;
  using UnityEngine.UI;

  public class FallingBlockController : MonoBehaviour
  {
    [HideInInspector]
    public Vector3 TargetPosition;

    private Image _image;

    protected void Awake()
    {
      _image = GetComponent<Image>();
      _image.CrossFadeAlpha(1.0f, 0.0f, false);
    }

    protected void Update()
    {
      transform.position = Vector3.Lerp(transform.position, TargetPosition, 5.0f * Time.deltaTime);

      if (Vector2.Distance(transform.position, TargetPosition) < 0.0001f)
      {
        _image.CrossFadeAlpha(0.0f, 0.1f, false);

        Messenger.Instance.SendMessage("BlockDestroyed");

        Destroy(this.gameObject);
      }
    }
  }
}
