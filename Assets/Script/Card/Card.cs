using UnityEngine;

public class Card : MonoBehaviour
{
    public CardData Data;
    public CardView View;
    public CardEffect Effect;

    public enum CardState { InHand, OnBoard, Used, Disabled }
    public CardState State = CardState.InHand;


    public bool CanUse()
    {
        return true;
    }

    public void Use()
    {
        
    }
}
