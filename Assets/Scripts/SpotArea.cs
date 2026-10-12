using UnityEngine;
using UnityEngine.EventSystems;

public class SpotArea : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RectTransform spotParent;

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(spotParent, eventData.position, null, out localPoint);

        SeabedExplorationSystem.Instance.OnAreaClicked(localPoint);
    }
}