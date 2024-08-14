using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using LitMotion;
using LitMotion.Extensions;

public class GameLog : MonoBehaviour
{
    public static GameLog ins;
    void Awake()
    {
        ins = this;
    }
    void Start()
    {
        string[] words = { "Word1", "Word2", "Word3", "Word4" };
        Color[] wordColors = { Color.white, Color.red, Color.blue, Color.green};
        LogRequest(words, wordColors);
    }
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float logTypeSpeed;
    public void LogRequest(string[] words, Color[] wordColors)
    {
        string line = "";
        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            Color wordColor = wordColors[i];
            string wordColorHexCode = wordColor.ToHexString();
            line += $"<color=#{wordColorHexCode}>{word}</color>";
        }
        string currentText = text.text;
        string nextText = currentText + $"\n{line}";
        LMotion.String.Create4096Bytes(currentText, nextText, logTypeSpeed)
            .WithRichText()
            .BindToText(text);
        //text.text += $"\n{line}";
    }
}
