using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    public static CardData Instance { get; private set; }

    public string path;
    public List<Card> Cards = new List<Card>();

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFromCSV(path);
    }

    public void LoadFromCSV(string fileName)
    {
        Cards.Clear();
        TextAsset csvFile = Resources.Load<TextAsset>("Data/" + fileName);
        string[] lines = csvFile.text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            Card card = new Card();
            card.Id = int.Parse(values[0]);
            card.IdImage = int.Parse(values[1]);
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
