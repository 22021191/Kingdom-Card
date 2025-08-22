using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardView : MonoBehaviour
{
    public int cardId;
    private Card card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    void Start()
    {
        card=CardData.Instance.Cards[cardId];
    }

    // Update is called once per frame
    void Update()
    {
        if (card == null) return;
        nameText.text = card.Name;
        descriptionText.text=card.Description;
        priceText.text=card.Price.ToString();
    }
}
