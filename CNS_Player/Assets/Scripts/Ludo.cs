using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ludo : MonoBehaviour
{

    public List<PieceHandler> player;
    public List<PieceHandler> cmp;
    public int diceMax;

    public bool humanIsPlayer1 = true;

    private int diceValue;
    private bool waitingForMove;
    public bool isPlayerUp;

    public Sprite[] diceStates;

    [SerializeField] private float rollDuration = 0.8f;
    [SerializeField] private float rollFrameDelay = 0.05f;
    private Coroutine rollRoutine;

    public Image enemyDice;

    public enum diceType
    {
        Dice6,
        Coin,
        Tetrahedra
    };

    public diceType dropDown;
    public bool waitForDiceRoll;


    void Start()
    {
        if (humanIsPlayer1)
        {
            isPlayerUp = true;
        }
        else
        {
            isPlayerUp = false;
        }
        StartTurn();
    }

    void StartTurn()
    {
        RollDice();

        if (isPlayerUp)
        {
            // Player logic
            bool movable = true;
            foreach (var p in player)
            {
                if (p.CanMove(diceValue))
                {
                    bool canMove = true;
                    foreach (var other in player)
                    {
                        if(other.realSpot == p.positions[p.currentPos + diceValue].id)
                        {
                            canMove = false;
                            break;
                        }
                    }
                    if (canMove)
                    {
                        movable = true;
                        break;
                    }
                }
                else
                {
                    movable = false;
                }
            }
            if (movable)
            {
                waitForDiceRoll = true;
            }
            else
            {
                waitingForMove = false;
                waitForDiceRoll = false;
                NoMove();
            }
            
        } else
        {
            // Ai logic
            StartCoroutine(AiMovementCoroutine());
        }

        if (WinCondition())
        {
            return;
        }
    }

    public bool WinCondition()
    {
        List<PieceHandler> won = new List<PieceHandler>(); 
        foreach (var p in player)
        {
            if(p.currentPos == p.positions.Length - 1)
            {
                won.Add(p);
            }
        }
        if(won.Count == player.Count)
        {
            Debug.Log("Player won!!");
            return true;
        }

        won = new List<PieceHandler>();
        foreach (var p in cmp)
        {
            if (p.currentPos == p.positions.Length - 1)
            {
                won.Add(p);
            }
        }
        if (won.Count == cmp.Count)
        {
            Debug.Log("Enemy won :(");
            return true;
        }

        return false;

    }

    public void PieceToMove(PieceHandler pieceToMove)
    {
        if (!waitingForMove)
        {
            return;
        }
        if(pieceToMove.CanMove(diceValue))
        {
            bool canMove = true;
            foreach (var p in player)
            {
                if(p.realSpot == pieceToMove.positions[pieceToMove.currentPos + diceValue].id)
                {
                    canMove = false;
                    break;
                }
            }
            if (canMove)
            {
                pieceToMove.MovePiece(diceValue, () => EndTurn(pieceToMove));
            }
            else
            {
                Debug.Log("This piece cannot move");
            }
        }
        else
        {
            Debug.Log("This piece cannot move");
        }
    }

    IEnumerator AiMovementCoroutine()
    {
        diceValue = Random.Range(1, diceMax + 1);
        Debug.Log($"AI Rolled {diceValue}");

        float elapsed = 0f;
        while (elapsed < rollDuration)
        {
            int randomFace = Random.Range(0, diceStates.Length);
            enemyDice.sprite = diceStates[randomFace];

            elapsed += rollFrameDelay;
            yield return new WaitForSeconds(rollFrameDelay);
        }
        enemyDice.sprite = diceStates[diceValue - 1];

        List<PieceHandler> movable = new List<PieceHandler>();
        foreach (var pieceToMove in cmp)
        {
            if (pieceToMove.CanMove(diceValue))
            {
                bool canMove = true;
                foreach (var other in cmp)
                {
                    if (other.realSpot == pieceToMove.positions[pieceToMove.currentPos + diceValue].id)
                    {
                        canMove = false;
                        break;
                    }
                }
                if (canMove) movable.Add(pieceToMove);
            }
        }

        if (movable.Count != 0)
        {
            PieceHandler choice = movable[Random.Range(0, movable.Count)];
            choice.MovePiece(diceValue, () => EndTurn(choice));
        }
        else
        {
            NoMove();
        }

        yield break;
    }

    void EndTurn(PieceHandler moved)
    {
        if (isPlayerUp)
        {
            foreach (var e in cmp)
            {
                if(moved.realSpot == e.realSpot)
                {
                    e.KillPiece();
                }
            }
        }
        else
        {
            foreach (var e in player)
            {
                if (moved.realSpot == e.realSpot)
                {
                    e.KillPiece();
                }
            }
        }

        isPlayerUp = !isPlayerUp;
        ChangeButtons();
        StartTurn();
    }

    void NoMove()
    {
        Debug.Log("The current player couldn't make a move");

        isPlayerUp = !isPlayerUp;
        ChangeButtons();
        StartTurn();
    }

    void RollDice()
    {

        diceValue = Random.Range(1, diceMax + 1);
        Debug.Log($"Rolled {diceValue}");
        if (isPlayerUp)
        {
            waitForDiceRoll = true;
        }
    }

    public void PlayerRollDice(Image show)
    {
        if (!waitForDiceRoll) return;

        if (dropDown == diceType.Dice6)
        {
            if (rollRoutine != null)
                StopCoroutine(rollRoutine);

            rollRoutine = StartCoroutine(RollDiceAnimation(show));
        }
    }

    IEnumerator RollDiceAnimation(Image show)
    {
        waitForDiceRoll = false;

        float elapsed = 0f;

        while (elapsed < rollDuration)
        {
            int randomFace = Random.Range(0, diceStates.Length);
            show.sprite = diceStates[randomFace];

            elapsed += rollFrameDelay;
            yield return new WaitForSeconds(rollFrameDelay);
        }

        show.sprite = diceStates[diceValue - 1];
        waitingForMove = true;
    }

    void ChangeButtons()
    {
        foreach (var piece in player)
        {
            piece.ChangeButton();
        }
    }

}
