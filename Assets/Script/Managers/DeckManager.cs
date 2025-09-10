using System.Collections.Generic;
using SinuousProductions;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> allCards = new List<Card>();

    public int startingHandSize = 6;
    public int maxHandSize = 12;
    public CardContainer handManager;
    private DrawPileManager drawPileManager;
    public bool startBattleRun = true;

    void Start()
    {
        //Load all card assets from the Resources folder
        Card[] cards = CardData.Instance.Cards.ToArray();

        //Add the loaded cardsInHand to the allCards list
        allCards.AddRange(cards);
    }

    void Awake()
    {
        if (drawPileManager == null)
        {
            drawPileManager = FindObjectOfType<DrawPileManager>();
        }
        if (handManager == null)
        {
            handManager = FindObjectOfType<CardContainer>();
        }
    }

    void Update()
    {
        if (startBattleRun)
        {
            BattleSetup();
        }
    }

    public void BattleSetup()
    {
        Debug.Log("ok");
        drawPileManager.MakeDrawPile(allCards);
        drawPileManager.BattleSetup(startingHandSize, maxHandSize);
        startBattleRun = false;
    }
}
