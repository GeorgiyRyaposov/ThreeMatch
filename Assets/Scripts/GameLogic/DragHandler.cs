namespace Assets.Scripts.GameLogic
{
  using UnityEngine;
  using UnityEngine.EventSystems;
  using UnityEngine.UI;

  [RequireComponent(typeof(Image))]
  public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
  {
    private RectTransform _draggingPlane;
    private bool _isDragging;
    private Vector3 _originalTransform;

    public void OnBeginDrag(PointerEventData eventData)
    {
      var canvas = FindInParents<Canvas>(gameObject);
      if (canvas == null)
        return;

      var group = gameObject.AddComponent<CanvasGroup>();
      group.blocksRaycasts = false;
		
      _draggingPlane = transform as RectTransform;

      _isDragging = true;
      _originalTransform = transform.position;

      SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData data)
    {
      if (_isDragging)
        SetDraggedPosition(data);
    }

    private void SetDraggedPosition(PointerEventData data)
    {
      if (data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
        _draggingPlane = data.pointerEnter.transform as RectTransform;

      var rt = GetComponent<RectTransform>();
      Vector3 globalMousePos;
      if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_draggingPlane, data.position, data.pressEventCamera, out globalMousePos))
      {
        rt.position = globalMousePos;
        rt.rotation = _draggingPlane.rotation;
      }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      _isDragging = false;    
    
      Destroy(GetComponent<CanvasGroup>());

      transform.position = _originalTransform;
    }

    static public T FindInParents<T>(GameObject go) where T : Component
    {
      if (go == null) return null;
      var comp = go.GetComponent<T>();

      if (comp != null)
        return comp;
		
      Transform t = go.transform.parent;
      while (t != null && comp == null)
      {
        comp = t.gameObject.GetComponent<T>();
        t = t.parent;
      }
      return comp;
    }
  }
}
