using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuToggleColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] private Color defaultColor, hoverColor, pressedColor;
    [SerializeField] private Graphic image;
    [SerializeField] private List<Graphic> otherImages;
    [SerializeField] private bool usePressedColor = false;

    private bool hovering, clicking;

    private void Update() {

        var color = usePressedColor
            ?
                clicking ? pressedColor
                : hovering ? hoverColor
                : defaultColor
            :
                hovering || clicking ? hoverColor : defaultColor;

        if (image != null) image.color = color;
        if (otherImages != null) otherImages.ForEach(i => i.color = color);
    }

    private void OnDisable() {
        hovering = clicking = false;
    }

    public void OnPointerEnter(PointerEventData eventData) => hovering = true;
    public void OnPointerExit(PointerEventData eventData) => hovering = false;

    public void OnPointerDown(PointerEventData eventData) => clicking = true;
    public void OnPointerUp(PointerEventData eventData) => clicking = false;
}
