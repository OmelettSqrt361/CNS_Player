using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiScene : MonoBehaviour
{

    public static MultiScene Instance;

    public enum difficulty
    {
        Random,
        Easy,
        Normal,
        Hard,
        Demon
    };

    public difficulty diff;
    public int stage;

    public void SetDiff(difficulty diffSet)
    {
        diff = diffSet;
    }

    public void SetStage(int toHop)
    {
        stage = toHop;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


}
