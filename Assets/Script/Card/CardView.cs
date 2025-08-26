using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public int cardId;
    private Card card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI priceText;
    public Image cardImage;
    public void Init( int id)
    {
        cardId=id;
        card=CardData.Instance.Cards[cardId];
        SetCardSprite(card.IdImage);

    }

    // Update is called once per frame
    void Update()
    {
        if (card == null) return;
        nameText.text = card.Name;
        descriptionText.text=card.Description;
        priceText.text=card.Price.ToString();
        
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
}
