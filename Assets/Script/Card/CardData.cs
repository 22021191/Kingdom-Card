using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    [Serializable]
    public class CardParam
    {
        public int Index;          // ID của card
        public int Illustration;   // ID ảnh minh họa
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
    }

    public List<CardParam> Cards = new List<CardParam>();

    public void LoadFromCSV(string path)
    {
        Cards.Clear();
        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++) // bỏ header
        {
            string[] values = lines[i].Split(',');

            CardParam card = new CardParam();
            card.Index = int.Parse(values[0]);
            card.Illustration = int.Parse(values[1]);
            card.Type = int.Parse(values[2]);
            card.Rarity = int.Parse(values[3]);
            card.Name = values[4];
            card.Cost = int.Parse(values[5]);
            card.PopCost = int.Parse(values[6]);
            card.EffectValueA = int.Parse(values[7]);
            card.EffectValueB = int.Parse(values[8]);
            card.EffectValueC = int.Parse(values[9]);
            card.EffectValueD = int.Parse(values[10]);
            card.EffectType = int.Parse(values[11]);
            card.Effect = values[12];
            card.Description = values[13];
            card.Price = int.Parse(values[14]);

            Cards.Add(card);
        }
    }
}
