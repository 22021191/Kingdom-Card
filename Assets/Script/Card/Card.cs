using UnityEngine;
[System.Serializable]
public class Card
{
    public int Id;          // ID của card
    public int IdImage;   // ID ảnh minh họa
    public int Type;           // Loại card (sau có thể enum hóa)
    public int Rarity;         // Độ hiếm

    public string Name;        // Tên card
    public int Cost;           // Mana cost
    public int PopCost;        // Population cost (dùng cho Unit)

    public int EffectValueA;   // Giá trị hiệu ứng A
    public int EffectValueB;   // Giá trị hiệu ứng B
    public int EffectValueC;   // Giá trị hiệu ứng C
    public int EffectValueD;   // Giá trị hiệu ứng D

    public int EffectType;     // Kiểu hiệu ứng (sau có thể enum hóa)
    public string Effect;      // Mã hoặc tên hiệu ứng
    public string Description; // Mô tả chi tiết
    public int Price;          // Giá mua card

    public Card()
    {

    }

    public Card(int index, int illustration, int type, int rarity, 
        string name, int cost, int popCost, int effectValueA, 
        int effectValueB, int effectValueC, int effectValueD,
        int effectType, string effect, string description, int price)
    {
        Id = index;
        IdImage = illustration;
        Type = type;
        Rarity = rarity;
        Name = name;
        Cost = cost;
        PopCost = popCost;
        EffectValueA = effectValueA;
        EffectValueB = effectValueB;
        EffectValueC = effectValueC;
        EffectValueD = effectValueD;
        EffectType = effectType;
        Effect = effect;
        Description = description;
        Price = price;
    }
}
