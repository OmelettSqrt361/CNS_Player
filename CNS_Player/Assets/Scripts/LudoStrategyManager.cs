using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LudoStrategy
{
    public string name;
    public Dictionary<string, int> moves = new Dictionary<string, int>();

    public void LoadFromJson(TextAsset jsonFile)
    {
        moves = new Dictionary<string, int>();

        if (string.IsNullOrEmpty(jsonFile.text))
            return;

        string text = jsonFile.text.Trim();

        // Remove outer braces
        if (text.StartsWith("{")) text = text.Substring(1);
        if (text.EndsWith("}")) text = text.Substring(0, text.Length - 1);

        int index = 0;
        while (index < text.Length)
        {
            // Find start and end of key
            int startKey = text.IndexOf('"', index);
            if (startKey == -1) break;
            int endKey = text.IndexOf('"', startKey + 1);
            if (endKey == -1) break;

            string key = text.Substring(startKey + 1, endKey - startKey - 1);

            // Find colon
            int colon = text.IndexOf(':', endKey + 1);
            if (colon == -1) break;

            // Find end of value (comma or end of string)
            int comma = text.IndexOf(',', colon + 1);
            string valueStr;
            if (comma == -1)
            {
                valueStr = text.Substring(colon + 1).Trim();
                index = text.Length; // done
            }
            else
            {
                valueStr = text.Substring(colon + 1, comma - colon - 1).Trim();
                index = comma + 1;
            }

            if (int.TryParse(valueStr, out int value))
            {
                moves[key] = value;
            }
            else
            {
                Debug.LogWarning($"Could not parse value '{valueStr}' for key '{key}' in strategy '{name}'");
            }
        }
    }
}

public class LudoStrategyManager : MonoBehaviour
{
    public List<TextAsset> strategyJsonFiles; // Assign in Inspector
    private Dictionary<string, LudoStrategy> strategies = new Dictionary<string, LudoStrategy>();
    private LudoStrategy currentStrategy;

    void Awake()
    {
        foreach (var json in strategyJsonFiles)
        {
            LudoStrategy strat = new LudoStrategy();
            strat.name = json.name;
            strat.LoadFromJson(json);
            strategies[strat.name] = strat;
        }
    }

    public void UseStrategy(string strategyName)
    {
        if (strategies.ContainsKey(strategyName))
        {
            currentStrategy = strategies[strategyName];
            Debug.Log($"Using strategy: {strategyName}");
        }
        else
        {
            Debug.LogWarning($"Strategy '{strategyName}' not found!");
            currentStrategy = null;
        }
    }

    public int? GetMove(string key)
    {
        if (currentStrategy != null && currentStrategy.moves.ContainsKey(key))
        {
            return currentStrategy.moves[key];
        }
        return null;
    }
}

