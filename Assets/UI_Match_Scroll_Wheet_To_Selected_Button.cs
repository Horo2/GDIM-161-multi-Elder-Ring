using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


namespace Horo
{
    public class UI_Match_Scroll_Wheet_To_Selected_Button : MonoBehaviour
    {
        [SerializeField] GameObject currentSlected;
        [SerializeField] GameObject previouslySelected;
        [SerializeField] RectTransform currentSlectedTransform;

        [SerializeField] RectTransform contentPanel;
        [SerializeField] ScrollRect scrollRect;

        private void Update()
        {
            currentSlected = EventSystem.current.currentSelectedGameObject;
            if(currentSlected != null )
            {
                previouslySelected = currentSlected;
                currentSlectedTransform = currentSlected.GetComponent<RectTransform>();
                SnapTo(currentSlectedTransform);
            }
        }

        private void SnapTo(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();

            Vector2 newPosition = 
                (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

            // only want to lock the position on the y axis
            newPosition.x = 0;

            contentPanel.anchoredPosition = newPosition;
        }
    }
}

