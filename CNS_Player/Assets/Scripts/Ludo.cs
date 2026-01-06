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

    public AutoPopupManager popup;
    public AutoPopupManager popupWin;
    public LudoStrategyManager strategyManager;
    List<(string, int)> game;

    public Image enemyDice;

    public MessageScriptTest sender;

    public enum diceType
    {
        Dice6,
        Coin,
        Tetrahedra
    };

    public string[] easyNorm;
    public string stageName;

    public diceType dropDown;
    public MultiScene.difficulty chosen;

    string chosenDiffName;

    public bool waitForDiceRoll;

    public bool playerHasPlayed;


    void Start()
    {
        game = new List<(string, int)>();
        chosen = MultiScene.Instance.diff;

        bool humanIsPlayer1 = Random.value > 0.5f;
        playerHasPlayed = humanIsPlayer1;

        switch (chosen)
        {
            case MultiScene.difficulty.Random:
                chosenDiffName = "";
                game.Add(($"{stageName}:random",0));
                break;
            case MultiScene.difficulty.Easy:
                chosenDiffName = easyNorm[0];
                game.Add(($"{stageName}:{easyNorm[0]}", 0));
                break;
            case MultiScene.difficulty.Normal:
                chosenDiffName = easyNorm[1];
                game.Add(($"{stageName}:{easyNorm[1]}", 0));
                break;
            case MultiScene.difficulty.Hard:
                chosenDiffName = easyNorm[2];
                game.Add(($"{stageName}:{easyNorm[2]}", 0));
                break;
            case MultiScene.difficulty.Demon:
                chosenDiffName = easyNorm[3];
                game.Add(($"{stageName}:{easyNorm[3]}", 0));
                break;
            default:
                chosenDiffName = "";
                game.Add(($"{stageName}:random", 0));
                break;
        }

        if (strategyManager != null && chosenDiffName != "")
        {
            strategyManager.UseStrategy(chosenDiffName);
        }


        if (humanIsPlayer1)
        {
            popup.ShowPopup("Začínáš! Klikni na kostku!", 200f);
            isPlayerUp = true;
        }
        else
        {
            ChangeButtons();
            popup.ShowPopup("Začíná soupeř! Až dohraje klikni na kostku", 200f);
            isPlayerUp = false;
        }
        StartTurn();
    }

    void StartTurn()
    {

        if (!WinCondition())
        {
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
                            if (other.realSpot == p.positions[Mathf.Min(p.currentPos + diceValue,p.positions.Length-1)].id)
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

            }
            else
            {
                // Ai logic
                if (chosenDiffName == "")
                {
                    StartCoroutine(AiMovementCoroutine());
                }
                else
                {
                    StartCoroutine(AiLoadMovementCoroutine());
                }
            }
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
            // Restartovat hru / Jít do menu
            popupWin.ShowPopup("Vyhrál jsi!", float.MaxValue);
            Debug.Log("Player won!!");

            sender.SendJsonFile(game);
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
            // Restartovat hru / Jít do menu
            popupWin.ShowPopup("Prohrál jsi :(", float.MaxValue);
            Debug.Log("Enemy won :(");

            sender.SendJsonFile(game);
            return true;
        }

        return false;

    }

    public void PieceToMove(PieceHandler pieceToMove)
    {

        if (!waitingForMove)
        {
            if (waitForDiceRoll)
            {
                popup.ShowPopup("Hoď kostkou!");
            }
            return;
        }
        else if(pieceToMove.CanMove(diceValue))
        {
            bool canMove = true;
            foreach (var p in player)
            {
                if(p.realSpot == pieceToMove.positions[Mathf.Min(pieceToMove.currentPos + diceValue,pieceToMove.positions.Length-1)].id)
                {
                    canMove = false;
                    break;
                }
            }
            if (canMove)
            {
                game.Add((BuildKey(player, cmp, diceValue), player.IndexOf(pieceToMove)));
                pieceToMove.MovePiece(diceValue, () => EndTurn(pieceToMove));
            }
            else
            {
                // Tato figurka se nemůže hýbat
                popup.ShowPopup("Tat figurka se nemůže hýbat", 2);
                Debug.Log("This piece cannot move");
            }
        }
        else
        {
            // Tato figurka se nemůže hýbat
            popup.ShowPopup("Tat figurka se nemůže hýbat", 2);
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
                    if (other.realSpot == pieceToMove.positions[Mathf.Min(pieceToMove.currentPos + diceValue,pieceToMove.positions.Length -1)].id)
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
            int rand = Random.Range(0, movable.Count);
            PieceHandler choice = movable[rand];
            game.Add((BuildKey(player, cmp, diceValue), cmp.IndexOf(choice)));
            choice.MovePiece(diceValue, () => EndTurn(choice));
        }
        else
        {
            NoMove();
        }

        yield break;
    }

    IEnumerator AiLoadMovementCoroutine()
    {
        // Roll the dice
        diceValue = Random.Range(1, diceMax + 1);
        Debug.Log($"AI Rolled {diceValue}");

        // Animate dice
        float elapsed = 0f;
        while (elapsed < rollDuration)
        {
            int randomFace = Random.Range(0, diceStates.Length);
            enemyDice.sprite = diceStates[randomFace];

            elapsed += rollFrameDelay;
            yield return new WaitForSeconds(rollFrameDelay);
        }
        enemyDice.sprite = diceStates[diceValue - 1];

        string key = BuildKey(cmp, player, diceValue);
        

        Debug.Log($"Strategy key: {key}");

        PieceHandler pieceToMove = null;

        // Check strategy first
        int? moveIndex = strategyManager.GetMove(key);
        if (moveIndex.HasValue)
        {
            pieceToMove = cmp[moveIndex.Value];
            Debug.Log($"AI uses strategy to move piece {moveIndex.Value}");
        }
        else
        {
            Debug.Log("Strategy did not have a move, falling back to random");

            // Fallback to random if strategy doesn't have a move
            List<PieceHandler> movable = new List<PieceHandler>();
            foreach (var piece in cmp)
            {
                if (piece.CanMove(diceValue))
                {
                    bool canMove = true;
                    foreach (var other in cmp)
                    {
                        if (other.realSpot == piece.positions[Mathf.Min(piece.currentPos + diceValue,piece.positions.Length -1)].id)
                        {
                            canMove = false;
                            break;
                        }
                    }
                    if (canMove) movable.Add(piece);
                }
            }

            if (movable.Count != 0)
                pieceToMove = movable[Random.Range(0, movable.Count)];
        }

        // Execute move or no move
        if (pieceToMove != null)
        {
            game.Add((BuildKey(player, cmp, diceValue), cmp.IndexOf(pieceToMove)));
            pieceToMove.MovePiece(diceValue, () => EndTurn(pieceToMove));
        }
        else
        {
            NoMove();
        }
    }

    string BuildKey(List<PieceHandler> playerPieces, List<PieceHandler> aiPieces, int dice)
    {
        string PositionsToString(List<PieceHandler> pieces)
        {
            int[] pos = new int[pieces.Count];
            for (int i = 0; i < pieces.Count; i++)
                pos[i] = pieces[i].currentPos; // Use realSpot to match your JSON
            System.Array.Sort(pos);
            return "[" + string.Join(",", pos) + "]";
        }

        string playerStr = PositionsToString(playerPieces);
        string aiStr = PositionsToString(aiPieces);

        return $"{playerStr};{aiStr};{dice}";
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

        popup.KillPopup();

        player.Sort((a, b) => a.currentPos.CompareTo(b.currentPos));
        cmp.Sort((a, b) => a.currentPos.CompareTo(b.currentPos));

        if (!playerHasPlayed)
        {
            popup.ShowPopup("Klikni na svou kostku!", 200f);
            playerHasPlayed = true;
        }

        if (waitingForMove) waitingForMove = false;

        isPlayerUp = !isPlayerUp;
        ChangeButtons();
        StartTurn();
    }

    void NoMove()
    {
        // Nelze učinit tah
        if (isPlayerUp)
        {
            popup.ShowPopup("Hráč nemohl učinit tah. \n Hýbe se počítač.", 4);
        }
        else
        {
            popup.ShowPopup("Počítač nemohl učinit tah. \n Můžeš hrát.", 4);
        }
        Debug.Log("The current player couldn't make a move");

        isPlayerUp = !isPlayerUp;
        ChangeButtons();
        StartTurn();
    }

    void RollDice()
    {
        // Zmáčkni kostku
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

        diceValue = Random.Range(1, diceMax + 1);
        Debug.Log(diceValue - 1);
        Debug.Log(diceStates.Length);

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
