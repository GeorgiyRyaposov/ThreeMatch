namespace Assets.Scripts.UI
{
  using Messaging;
  using UnityEngine;

  public class LoadOnClick : MonoBehaviour {

    public GameObject loadingImage;
	
    public void LoadScene(int level)
    {
      loadingImage.SetActive(true);

      Messenger.Instance.SendMessage("LevelLoading");

      Application.LoadLevel(level);
    }
  }
}
