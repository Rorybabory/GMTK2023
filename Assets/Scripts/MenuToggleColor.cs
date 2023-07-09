using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuToggleColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] private Color defaultColor, hoverColor, pressedColor;
    [SerializeField] private Graphic image;
    [SerializeField] private bool usePressedColor = false;

    private bool hovering, clicking;

    private void Update() {

        image.color = usePressedColor
            ?
                clicking ? pressedColor
                : hovering ? hoverColor
                : defaultColor
            :
                hovering || clicking ? hoverColor : defaultColor;
    }

    private void OnDisable() {
        hovering = clicking = false;
    }

    public void OnPointerEnter(PointerEventData eventData) => hovering = true;
    public void OnPointerExit(PointerEventData eventData) => hovering = false;

    public void OnPointerDown(PointerEventData eventData) => clicking = true;
    public void OnPointerUp(PointerEventData eventData) => clicking = false;
}
