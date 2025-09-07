using System.Collections.Generic;
using SinuousProductions;
using TMPro;
using UnityEngine;

public class DrawPileManager : MonoBehaviour
{
    public List<Card> drawPile = new List<Card>();

    private int currentIndex = 0;
    public int maxHandSize;
    public int currentHandSize;
    private CardContainer handManager;
    private DiscardManager discardManager;

    public TextMeshProUGUI drawPileCounter;

    void Start()
    {
        handManager = FindObjectOfType<CardContainer>();
    }

    void Update()
    {
        if (handManager != null)
        {
            currentHandSize = handManager.cardsInHand.Count;
        }
    }

    public void MakeDrawPile(List<Card> cardsToAdd)
    {
        drawPile.AddRange(cardsToAdd);
        Utility.Shuffle(drawPile);
        UpdateDrawPileCount();
    }

    public void BattleSetup(int numberOfCardsToDraw, int setMaxHandSize)
    {
        maxHandSize = setMaxHandSize;
        for (int i = 0; i < numberOfCardsToDraw; i++)
        {
            DrawCard(handManager);
        }
    }

    public void DrawCard(CardContainer handManager)
    {
        if (drawPile.Count == 0)
        {
            RefillDeckFromDiscard();
        }

        if (drawPile.Count > 0 && currentHandSize < maxHandSize)
        {
            Card nextCard = drawPile[currentIndex];
            handManager.AddCardToHand(nextCard);
            drawPile.RemoveAt(currentIndex);
            if (drawPile.Count > 0) currentIndex %= drawPile.Count; // Ensure currentIndex is always valid
        }
        UpdateDrawPileCount();
    }

    private void RefillDeckFromDiscard()
    {
        if (discardManager == null)
        {
            discardManager = FindObjectOfType<DiscardManager>();
        }

        if (discardManager != null && discardManager.discardCardsCount > 0)
        {
            drawPile = discardManager.PullAllFromDiscard();
            Utility.Shuffle(drawPile);
            currentIndex = 0;
        }
        UpdateDrawPileCount();
    }

    private void UpdateDrawPileCount()
    {
        drawPileCounter.text = drawPile.Count.ToString();
    }
}
