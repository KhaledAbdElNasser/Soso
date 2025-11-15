using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs & Sprites")]
    public GameObject cardPrefab;
    public Sprite backSprite;
    public List<Sprite> faceSprites;

    [Header("Board")]
    public int columns = 4;
    public int rows = 4;
    public Transform boardParent;

    [Header("Match Settings")]
    [Range(1, 200)]
    public float matchTime;

    [Header("Target Area (World Units)")]
    public Vector2 targetSize = new Vector2(8f, 6f); // width x height

    [Header("UI")]
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI matchesText;
    public TextMeshProUGUI winningText;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;

    int moves = 0;
    float elapsed = 0f;
    int matches = 0;
    int pairsToMatch;
    bool inMatch;
    List<CardView> allCards = new List<CardView>();
    private List<CardView> flipQueue = new List<CardView>();
    private bool comparing = false;

    void Start()
    {
        restartButton.onClick.AddListener(Restart);
        StartGame();
    }

    void Update()
    {
        if (inMatch)
        {
            elapsed += Time.deltaTime;
            if (elapsed > matchTime )
            {
                //GameOver
                inMatch = false;
                gameOverText.text = "Game Over Loser";
                allCards.ForEach(c => c.IsActive = false );
            }
            else
            {
                UpdateUI();
            }

            if(matches >= pairsToMatch)
            {
                inMatch = false;
                SaveLoadManager.Instance.SaveMatch(matches + "/" + pairsToMatch, moves, elapsed);
            }
        }
    }

    void UpdateUI()
    {
        movesText.text = moves.ToString();
        timerText.text = Mathf.FloorToInt(elapsed) + "s";
        matchesText.text = matches + "/" + pairsToMatch;
    }

    void StartGame()
    {
        ClearBoard();
        SetupBoard();

        moves = 0;
        elapsed = 0f;
        matches = 0;
        inMatch = true;
        winningText.text = "";
        gameOverText.text = "";
        UpdateUI();
    }

    void Restart()
    {
        StartGame();
    }

    void ClearBoard()
    {
        foreach (Transform t in boardParent)
            Destroy(t.gameObject);

        allCards.Clear();
        flipQueue.Clear();
        comparing = false;
    }

    void SetupBoard()
    {
        int total = rows * columns;
        pairsToMatch = total / 2;

        if (faceSprites.Count < pairsToMatch)
        {
            Debug.LogError("Not enough face sprites assigned!");
            return;
        }

        // Prepare pairs
        List<Sprite> chosen = new List<Sprite>();
        for (int i = 0; i < pairsToMatch; i++)
        {
            chosen.Add(faceSprites[i]);
            chosen.Add(faceSprites[i]);
        }

        // Shuffle
        for (int i = 0; i < chosen.Count; i++)
        {
            Sprite tmp = chosen[i];
            int r = Random.Range(i, chosen.Count);
            chosen[i] = chosen[r];
            chosen[r] = tmp;
        }

        int idx = 0;

        float marginFactor = 0.85f; // 15% padding
        float cellWidth = targetSize.x / columns;
        float cellHeight = targetSize.y / rows;

        // Top-left start position
        float startX = -targetSize.x / 2 + cellWidth / 2f;
        float startY = targetSize.y / 2 - cellHeight / 2f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector3 pos = new Vector3(startX + c * cellWidth, startY - r * cellHeight, 0f);
                GameObject go = Instantiate(cardPrefab, pos, Quaternion.identity, boardParent);

                // Initialize card first
                CardView cv = go.GetComponent<CardView>();
                cv.Initialize(chosen[idx], chosen[idx].name, backSprite, OnCardClicked);

                // Now scale the **actual sprite in the instantiated card**
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                Vector2 spriteSize = sr.sprite.bounds.size;

                float scaleX = (cellWidth * marginFactor) / spriteSize.x;
                float scaleY = (cellHeight * marginFactor) / spriteSize.y;
                float scale = Mathf.Min(scaleX, scaleY);

                go.transform.localScale = Vector3.one * scale;

                allCards.Add(cv);
                idx++;
            }
        }
    }


    // ---------------------------
    // CARD CLICK EVENT
    // ---------------------------
    void OnCardClicked(CardView card)
    {
        if (card.IsMatched || card.IsRevealed)
            return;

        flipQueue.Add(card);
        StartCoroutine(card.RevealCoroutine());

        if (!comparing)
            StartCoroutine(ProcessQueue());
    }

    // ---------------------------
    // ASYNC COMPARISON
    // ---------------------------
    IEnumerator ProcessQueue()
    {
        comparing = true;

        while (flipQueue.Count >= 2)
        {
            CardView c1 = flipQueue[0];
            CardView c2 = flipQueue[1];

            // Wait a short time to allow smooth flip animation
            yield return new WaitForSeconds(0.2f);

            moves++;
            UpdateUI();

            if (c1.cardId == c2.cardId)
            {
                AudioManager.Instance.PlayMatch();
                c1.SetMatched();
                c2.SetMatched();
                matches++;

                if (matches >= pairsToMatch)
                { 
                    Debug.Log("All matched! Game finished.");
                    winningText.text = "You Win!";
                }
            }
            else
            {
                AudioManager.Instance.PlayMismatch();
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(c1.HideCoroutine());
                StartCoroutine(c2.HideCoroutine());
            }

            flipQueue.RemoveRange(0, 2);
        }

        comparing = false;
    }
}
