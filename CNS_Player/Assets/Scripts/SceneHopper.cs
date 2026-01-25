using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHopper : MonoBehaviour
{
    public int nextScene;

    public void SceneHop(int sceneToHop)
    {
        SceneManager.LoadScene(sceneToHop);
    }

    public void GetSceneId(int stageId)
    {
        MultiScene.Instance.SetStage(stageId);
        SceneManager.LoadScene(nextScene);
    }

    public void GetSceneFromMultiScene(int diff)
    {
        switch (diff)
        {
            case 0:
                MultiScene.Instance.SetDiff(MultiScene.difficulty.Random);
                break;
            case 1:
                MultiScene.Instance.SetDiff(MultiScene.difficulty.Easy);
                break;
            case 2:
                MultiScene.Instance.SetDiff(MultiScene.difficulty.Normal);
                break;
            case 3:
                MultiScene.Instance.SetDiff(MultiScene.difficulty.Hard);
                break;
            case 4:
                MultiScene.Instance.SetDiff(MultiScene.difficulty.Demon);
                break;
            default:
                MultiScene.Instance.SetDiff(MultiScene.difficulty.Random);
                break;
        }


        SceneManager.LoadScene(MultiScene.Instance.stage);
    }
}
