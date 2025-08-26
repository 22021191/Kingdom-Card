using config;
using events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler
{
    private const float EPS = 0.01f;
    public int cardId;
    private Card card;

    #region UI
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public Image cardImage;

    public float targetRotation;
    public Vector2 targetPosition;
    public float targetVerticalDisplacement;
    public int uiLayer;

    private RectTransform rectTransform;
    public Canvas canvas;
    #endregion

    #region Container
    public ZoomConfig zoomConfig;
    public AnimationSpeedConfig animationSpeedConfig;
    public CardContainer container;
    #endregion

    #region Drag
    private bool isHovered;
    private bool isDragged;
    private Vector2 dragStartPos;
    public EventsConfig eventsConfig;
    public bool preventCardInteraction;
    #endregion
    public void Init( int id)
    {
        cardId=id;
        card=CardData.Instance.Cards[cardId];
        SetCardSprite(card.IdImage);

    }
    public float width
    {
        get => rectTransform.rect.width * rectTransform.localScale.x;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetCardSprite(string spriteName)
    {
        string cleanName = Regex.Replace(spriteName, @"\s+", "");
        
        foreach (var sprite in CardData.Instance.bubbleSprites)
        {
            Debug.Log(sprite.name);

            if (sprite.name.Equals(cleanName))
            {
                cardImage.sprite = sprite;
                break;
            }
        }
    }
    private void Start()
    {
        canvas = GetComponent<Canvas>();
        Init(0);
    }
    // Update is called once per frame
    void Update()
    {
        UpdateRotation();
        UpdatePosition();
        UpdateScale();
        UpdateUILayer();

    }
    #region Update Method
    private void UpdateUILayer()
    {
        if (!isHovered && !isDragged)
        {
            canvas.sortingOrder = uiLayer;
        }
        if (card == null) return;
        nameText.text = card.Name;
        descriptionText.text = card.Description;
        priceText.text = card.Price.ToString();
    }

    private void UpdatePosition()
    {
        if (!isDragged)
        {
            var target = new Vector2(targetPosition.x, targetPosition.y + targetVerticalDisplacement);
            if (isHovered && zoomConfig.overrideYPosition != -1)
            {
                target = new Vector2(target.x, zoomConfig.overrideYPosition);
            }

            var distance = Vector2.Distance(rectTransform.position, target);
            var repositionSpeed = rectTransform.position.y > target.y || rectTransform.position.y < 0
                ? animationSpeedConfig.releasePosition
                : animationSpeedConfig.position;
            rectTransform.position = Vector2.Lerp(rectTransform.position, target,
                repositionSpeed / distance * Time.deltaTime);
        }
        else
        {
            var delta = ((Vector2)Input.mousePosition + dragStartPos);
            rectTransform.position = new Vector2(delta.x, delta.y);
        }
    }

    private void UpdateScale()
    {
        var targetZoom = (isDragged || isHovered) && zoomConfig.zoomOnHover ? zoomConfig.multiplier : 1;
        var delta = Mathf.Abs(rectTransform.localScale.x - targetZoom);
        var newZoom = Mathf.Lerp(rectTransform.localScale.x, targetZoom,
            animationSpeedConfig.zoom / delta * Time.deltaTime);
        rectTransform.localScale = new Vector3(newZoom, newZoom, 1);
    }

    private void UpdateRotation()
    {
        var crtAngle = rectTransform.rotation.eulerAngles.z;
        // If the angle is negative, add 360 to it to get the positive equivalent
        crtAngle = crtAngle < 0 ? crtAngle + 360 : crtAngle;
        // If the card is hovered and the rotation should be reset, set the target rotation to 0
        var tempTargetRotation = (isHovered || isDragged) && zoomConfig.resetRotationOnZoom
            ? 0
            : targetRotation;
        tempTargetRotation = tempTargetRotation < 0 ? tempTargetRotation + 360 : tempTargetRotation;
        var deltaAngle = Mathf.Abs(crtAngle - tempTargetRotation);
        if (!(deltaAngle > EPS)) return;

        // Adjust the current angle and target angle so that the rotation is done in the shortest direction
        var adjustedCurrent = deltaAngle > 180 && crtAngle < tempTargetRotation ? crtAngle + 360 : crtAngle;
        var adjustedTarget = deltaAngle > 180 && crtAngle > tempTargetRotation
            ? tempTargetRotation + 360
            : tempTargetRotation;
        var newDelta = Mathf.Abs(adjustedCurrent - adjustedTarget);

        var nextRotation = Mathf.Lerp(adjustedCurrent, adjustedTarget,
            animationSpeedConfig.rotation / newDelta * Time.deltaTime);
        rectTransform.rotation = Quaternion.Euler(0, 0, nextRotation);
    }

    public void SetAnchor(Vector2 min, Vector2 max)
    {
        rectTransform.anchorMin = min;
        rectTransform.anchorMax = max;
    }
    #endregion
    #region Pointer

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragged = false;
        container.OnCardDragEnd();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragged)
        {
            // Avoid hover events while dragging
            return;
        }
        if (zoomConfig.bringToFrontOnHover)
        {
            canvas.sortingOrder = zoomConfig.zoomedSortOrder;
        }

        eventsConfig?.OnCardHover?.Invoke(new CardHover(this));
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragged)
        {
            // Avoid hover events while dragging
            return;
        }
        canvas.sortingOrder = uiLayer;
        isHovered = false;
        eventsConfig?.OnCardUnhover?.Invoke(new CardUnhover(this));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (preventCardInteraction) return;
        isDragged = true;
        dragStartPos = new Vector2(transform.position.x - eventData.position.x,
            transform.position.y - eventData.position.y);
        container.OnCardDragStart(this);
        eventsConfig?.OnCardUnhover?.Invoke(new CardUnhover(this));
    }

    #endregion
}
