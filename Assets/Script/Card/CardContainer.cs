using System.Collections.Generic;
using System.Linq;
using config;
using events;
using UnityEngine;
using UnityEngine.UI;

public class CardContainer : MonoBehaviour {
    [Header("Constraints")]
    [SerializeField]
    private bool forceFitContainer;

    [SerializeField]
    private bool preventCardInteraction;


    [SerializeField]
    private bool allowCardRepositioning = true;

    [Header("Rotation")]
    [SerializeField]
    [Range(-90f, 90f)]
    private float maxCardRotation;

    [SerializeField]
    private float maxHeightDisplacement;

    [SerializeField]
    private ZoomConfig zoomConfig;

    [SerializeField]
    private AnimationSpeedConfig animationSpeedConfig;

    [SerializeField]
    private CardPlayConfig cardPlayConfig;
    
    [Header("Events")]
    [SerializeField]
    private EventsConfig eventsConfig;
    public GameObject cardPrefab;
    public List<CardView> cardsInHand = new();

    private RectTransform rectTransform;
    private CardView currentDraggedCard;

    private void Start() {
        rectTransform = GetComponent<RectTransform>();
        InitCards();
        for(int i = 0; i < 5; i++)
        {
            AddCardToHand(CardData.Instance.Cards[0]);

        }
    }
    void Update() {
        UpdateCards();
    }

    private void InitCards() {
        SetUpCards();
        SetCardsAnchor();
    }
    public void AddCardToHand(Card cardData)
    {
        GameObject newCard = Instantiate(cardPrefab, transform.position, Quaternion.identity,transform);
        CardView newCardView=newCard.GetComponent<CardView>();
        newCardView.Init(cardData);
        cardsInHand.Add(newCardView);

        AddOtherComponentsIfNeeded(newCardView);

        // Pass child card any extra config it should be aware of
        newCardView.zoomConfig = zoomConfig;
        newCardView.animationSpeedConfig = animationSpeedConfig;
        newCardView.eventsConfig = eventsConfig;
        newCardView.preventCardInteraction = preventCardInteraction;
        newCardView.container = this;
    }

    private void SetCardsRotation() {
        for (var i = 0; i < cardsInHand.Count; i++) {
            cardsInHand[i].targetRotation = GetCardRotation(i);
            cardsInHand[i].targetVerticalDisplacement = GetCardVerticalDisplacement(i);
        }
    }

    private float GetCardVerticalDisplacement(int index) {
        if (cardsInHand.Count < 3) return 0;
        // Associate a vertical displacement based on the index in the cardsInHand list
        // so that the center card is at max displacement while the edges are at 0 displacement
        return maxHeightDisplacement *
               (1 - Mathf.Pow(index - (cardsInHand.Count - 1) / 2f, 2) / Mathf.Pow((cardsInHand.Count - 1) / 2f, 2));
    }

    private float GetCardRotation(int index) {
        if (cardsInHand.Count < 3) return 0;
        // Associate a rotation based on the index in the cardsInHand list
        // so that the first and last cardsInHand are at max rotation, mirrored around the center
        return -maxCardRotation * (index - (cardsInHand.Count - 1) / 2f) / ((cardsInHand.Count - 1) / 2f);
    }


    void SetUpCards() {
        cardsInHand.Clear();
        foreach (Transform card in transform) {
            var wrapper = card.GetComponent<CardView>();
            if (wrapper == null) {
                wrapper = card.gameObject.AddComponent<CardView>();
            }

            cardsInHand.Add(wrapper);

            AddOtherComponentsIfNeeded(wrapper);

            // Pass child card any extra config it should be aware of
            wrapper.zoomConfig = zoomConfig;
            wrapper.animationSpeedConfig = animationSpeedConfig;
            wrapper.eventsConfig = eventsConfig;
            wrapper.preventCardInteraction = preventCardInteraction;
            wrapper.container = this;
        }
    }

    private void AddOtherComponentsIfNeeded(CardView wrapper) {
        var canvas = wrapper.GetComponent<Canvas>();
        if (canvas == null) {
            canvas = wrapper.gameObject.AddComponent<Canvas>();
            wrapper.canvas=wrapper.GetComponent<Canvas>();
        }

        canvas.overrideSorting = true;

        if (wrapper.GetComponent<GraphicRaycaster>() == null) {
            wrapper.gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    private void UpdateCards() {
        if (transform.childCount != cardsInHand.Count) {
            InitCards();
        }

        if (cardsInHand.Count == 0) {
            return;
        }

        SetCardsPosition();
        SetCardsRotation();
        SetCardsUILayers();
        UpdateCardOrder();
    }

    private void SetCardsUILayers() {
        for (var i = 0; i < cardsInHand.Count; i++) {
            cardsInHand[i].uiLayer = zoomConfig.defaultSortOrder + i;
        }
    }

    private void UpdateCardOrder() {
        if (!allowCardRepositioning || currentDraggedCard == null) return;

        // Get the index of the dragged card depending on its position
        var newCardIdx = cardsInHand.Count(card => currentDraggedCard.transform.position.x > card.transform.position.x);
        var originalCardIdx = cardsInHand.IndexOf(currentDraggedCard);
        if (newCardIdx != originalCardIdx) {
            cardsInHand.RemoveAt(originalCardIdx);
            if (newCardIdx > originalCardIdx && newCardIdx < cardsInHand.Count - 1) {
                newCardIdx--;
            }

            cardsInHand.Insert(newCardIdx, currentDraggedCard);
        }
        // Also reorder in the hierarchy
        currentDraggedCard.transform.SetSiblingIndex(newCardIdx);
    }

    private void SetCardsPosition() {
        // Compute the total width of all the cardsInHand in global space
        var cardsTotalWidth = cardsInHand.Sum(card => card.width * card.transform.lossyScale.x);
        // Compute the width of the container in global space
        var containerWidth = rectTransform.rect.width * transform.lossyScale.x;
        if (forceFitContainer && cardsTotalWidth > containerWidth) {
            DistributeChildrenToFitContainer(cardsTotalWidth);
        }
        else {
            DistributeChildrenWithoutOverlap(cardsTotalWidth);
        }
    }

    private void DistributeChildrenToFitContainer(float childrenTotalWidth) {
        // Get the width of the container
        var width = rectTransform.rect.width * transform.lossyScale.x;
        // Get the distance between each child
        var distanceBetweenChildren = (width - childrenTotalWidth) / (cardsInHand.Count - 1);
        // Set all children's positions to be evenly spaced out
        var currentX = transform.position.x - width / 2;
        foreach (CardView child in cardsInHand) {
            var adjustedChildWidth = child.width * child.transform.lossyScale.x;
            child.targetPosition = new Vector2(currentX + adjustedChildWidth / 2, transform.position.y);
            currentX += adjustedChildWidth + distanceBetweenChildren;
        }
    }

    private void DistributeChildrenWithoutOverlap(float childrenTotalWidth) {
        var currentPosition = GetAnchorPositionByAlignment(childrenTotalWidth);
        foreach (CardView child in cardsInHand) {
            var adjustedChildWidth = child.width * child.transform.lossyScale.x;
            child.targetPosition = new Vector2(currentPosition + adjustedChildWidth / 2, transform.position.y);
            currentPosition += adjustedChildWidth;
        }
    }

    private float GetAnchorPositionByAlignment(float childrenWidth) {
        var containerWidthInGlobalSpace = rectTransform.rect.width * transform.lossyScale.x;
        return transform.position.x - childrenWidth / 2;
    }

    private void SetCardsAnchor() {
        foreach (CardView child in cardsInHand) {
            child.SetAnchor(new Vector2(0, 0.5f), new Vector2(0, 0.5f));
        }
    }

    public void OnCardDragStart(CardView card) {
        currentDraggedCard = card;
    }

    public void OnCardDragEnd() {
        // If card is in play area, play it!
        if (IsCursorInPlayArea()) {
            eventsConfig?.OnCardPlayed?.Invoke(new CardPlayed(currentDraggedCard));
            if (cardPlayConfig.destroyOnPlay) {
                DestroyCard(currentDraggedCard);
            }
        }
        currentDraggedCard = null;
    }
    
    public void DestroyCard(CardView card) {
        cardsInHand.Remove(card);
        eventsConfig.OnCardDestroy?.Invoke(new CardDestroy(card));
        Destroy(card.gameObject);
    }

    private bool IsCursorInPlayArea() {
        if (cardPlayConfig.playArea == null) return false;
        
        var cursorPosition = Input.mousePosition;
        var playArea = cardPlayConfig.playArea;
        var playAreaCorners = new Vector3[4];
        playArea.GetWorldCorners(playAreaCorners);
        return cursorPosition.x > playAreaCorners[0].x &&
               cursorPosition.x < playAreaCorners[2].x &&
               cursorPosition.y > playAreaCorners[0].y &&
               cursorPosition.y < playAreaCorners[2].y;
        
    }
}
