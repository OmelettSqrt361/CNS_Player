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
            waitingForMove = true;
            
        } else
        {
            // Ai logic
            AiMovement();
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

        bool canMove = true;
        if(pieceToMove.CanMove(diceValue))
        {
            foreach (var playerPiece in player)
            {
                if (playerPiece.positions[playerPiece.currentPos].id == pieceToMove.positions[pieceToMove.currentPos + diceValue].id)
                {
                    canMove = false;
                }
            }
            if (canMove)
            {
                pieceToMove.MovePiece(diceValue, () => EndTurn(pieceToMove));
                waitingForMove = false;
            }
            else
            {
                Debug.Log("Couldn't move the piece");
            }

        }
        else
        {
            Debug.Log("Couldn't move the piece");
        }
    }

    void AiMovement()
    {
        List<PieceHandler> movable = new List<PieceHandler>();
        foreach (var pieceToMove in cmp)
        {
            if (pieceToMove.CanMove(diceValue))
            {
                bool canMove = true;
                foreach (var pieceOther in cmp)
                {
                    if (pieceToMove.positions[pieceToMove.currentPos + diceValue].id == pieceOther.positions[pieceOther.currentPos].id)
                    {
                        canMove = false;
                    }
                }
                if (canMove)
                {
                    movable.Add(pieceToMove);
                }
            }
        }
        if(movable.Count != 0)
        {
            PieceHandler choice = movable[Random.Range(0, movable.Count)];
            choice.MovePiece(diceValue, () => EndTurn(choice));
        }
    }

    void EndTurn(PieceHandler moved)
    {
        if (isPlayerUp)
        {
            foreach (var e in cmp)
            {
                if(e.CompareSpots(moved,0))
                {
                    e.KillPiece();
                }
            }
        }
        else
        {
            foreach (var e in player)
            {
                if (e.CompareSpots(moved, 0))
                {
                    e.KillPiece();
                }
            }
        }

        isPlayerUp = !isPlayerUp;
        ChangeButtons();
        StartTurn();
    }

    void RollDice()
    {
        diceValue = Random.Range(1, diceMax+1);
        Debug.Log($"Rolled {diceValue}");
    }

    void ChangeButtons()
    {
        foreach (var piece in player)
        {
            piece.ChangeButton();
        }
    }

}
