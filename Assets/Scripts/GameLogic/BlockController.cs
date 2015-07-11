namespace Assets.Scripts.GameLogic
{
  using UnityEngine;
  using UnityEngine.UI;

  [RequireComponent(typeof(SpriteRenderer))]
  public class BlockController : MonoBehaviour
  {
    [HideInInspector]
    public Sprite ReplacementSprite;

    private SpriteRenderer _image;

    protected void Awake()
    {
      _image = GetComponent<SpriteRenderer>();
    }

    public void ReplaceSprite()
    {
      if (ReplacementSprite)
      {
        _image.sprite = ReplacementSprite;

        ReplacementSprite = null;
      }    
    }
  }
}
