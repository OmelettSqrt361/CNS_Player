using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceHandler : MonoBehaviour
{

    public Spot[] positions;
    public int currentPos;
    public Button buttonControl;

    [SerializeField] private float moveSpeed = 5f; // units per second
    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = positions[0].thisSpot.position;
        currentPos = 0;
    }

    // Update is called once per frame
    public void MovePiece(int dice, System.Action onComplete)
    {
        if (!CanMove(dice))
            return;

        StartCoroutine(MoveStepByStep(dice, onComplete));
    }

    private IEnumerator MoveStepByStep(int dice, System.Action onComplete)
    {
        isMoving = true;

        int targetPos = currentPos + dice;

        while (currentPos < targetPos)
        {
            Vector3 start = transform.position;
            Vector3 end = positions[currentPos + 1].thisSpot.position;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
            currentPos++;
        }
        currentPos = targetPos;

        isMoving = false;


        onComplete.Invoke();
    }

    public void KillPiece()
    {
        transform.position = positions[0].thisSpot.position;
        currentPos = 0;
    }

    public bool CanMove(int dice)
    {
        return dice + currentPos < positions.Length;
    }

    public void ChangeButton()
    {
        buttonControl.enabled = !buttonControl.enabled;
    }

    public bool CompareSpots(PieceHandler toCompare, int dice)
    {
        return toCompare.positions[toCompare.currentPos].id == positions[currentPos + dice].id; 
    }


}
