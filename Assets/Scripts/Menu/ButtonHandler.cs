using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IDeselectHandler          //, IPointerExitHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.selectedObject.GetComponent<Button>() != null)
            GetComponent<Button>().onClick.Invoke(); // trigger on click...
        Input.ResetInputAxes(); //Avoid a double selection...
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Selectable>().Select(); // mouse select focus on button...(deselect other shit...)
    }

    public void OnDeselect(BaseEventData eventData)
    {
        GetComponent<Selectable>().OnPointerExit(null);
    }

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    GetComponent<Selectable>().OnDeselect(eventData);
    //    //make select null (for mouse usage)
    //    if(eventData.IsPointerMoving())
    //        eventData.selectedObject = null;
    //}
}
