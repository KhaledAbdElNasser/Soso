using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour
{
    [Header("Rendering")]
    public SpriteRenderer spriteRenderer;
    public Sprite backSprite;

    [HideInInspector] public Sprite faceSprite;
    [HideInInspector] public string cardId;

    [Header("Flip Settings")]
    public float flipDuration = 0.15f;
    public float matchPulseScale = 1.2f;
    public float matchPulseTime = 0.2f;
    public bool IsAnimating { get; private set; } = false;
    public bool IsRevealed { get; private set; } = false;
    public bool IsMatched { get; private set; } = false;

    public bool IsActive { get; set; } = true;

    // Callback to GameManager
    System.Action<CardView> onClicked;

    // Track running animations
    Coroutine flipRoutine;

    void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Initializes the card with new data.
    /// </summary>
    public void Initialize(Sprite face, string id, Sprite back, System.Action<CardView> clickCallback)
    {
        faceSprite = face;
        cardId = id;
        backSprite = back;

        spriteRenderer.sprite = backSprite;

        onClicked = clickCallback;
        IsRevealed = false;
        IsMatched = false;

        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    // ---------------------------
    //     PLAYER INPUT
    // ---------------------------

    void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (IsMatched || IsRevealed || !IsActive)
            return;

        onClicked?.Invoke(this);
    }

    // ---------------------------
    //  PUBLIC ANIMATION ACTIONS
    // ---------------------------

    public void RevealInstant()
    {
        StopActiveAnim();
        spriteRenderer.sprite = faceSprite;
        IsRevealed = true;
        transform.localScale = Vector3.one;
    }

    public IEnumerator RevealCoroutine()
    {
        StopActiveAnim();
        flipRoutine = StartCoroutine(FlipToSprite(faceSprite));
        yield return flipRoutine;
        IsRevealed = true;
    }

    public IEnumerator HideCoroutine()
    {
        StopActiveAnim();
        flipRoutine = StartCoroutine(FlipToSprite(backSprite));
        yield return flipRoutine;
        IsRevealed = false;
    }

    public void SetMatched()
    {
        IsMatched = true;
        StartCoroutine(MatchPulse());
    }

    // ---------------------------
    //     FLIP ANIMATION
    // ---------------------------

    IEnumerator FlipToSprite(Sprite target)
    {
        IsAnimating = true;
        AudioManager.Instance.PlayFlip();
        float half = flipDuration;
        float t = 0;

        // Shrink horizontally
        while (t < half)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / half);
            transform.localScale = new Vector3(Mathf.Lerp(1f, 0f, p), 1f, 1f);
            yield return null;
        }

        spriteRenderer.sprite = target;

        t = 0;
        while (t < half)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / half);
            transform.localScale = new Vector3(Mathf.Lerp(0f, 1f, p), 1f, 1f);
            yield return null;
        }

        transform.localScale = Vector3.one;

        IsAnimating = false;
    }


    // ---------------------------
    //     MATCH EFFECT
    // ---------------------------

    IEnumerator MatchPulse()
    {
        Vector3 original = Vector3.one;
        Vector3 enlarged = Vector3.one * matchPulseScale;

        float t = 0;
        while (t < matchPulseTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / matchPulseTime);
            transform.localScale = Vector3.Lerp(original, enlarged, p);
            yield return null;
        }

        t = 0;
        while (t < matchPulseTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / matchPulseTime);
            transform.localScale = Vector3.Lerp(enlarged, original, p);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    // ---------------------------
    //     UTILITY
    // ---------------------------

    void StopActiveAnim()
    {
        if (flipRoutine != null)
            StopCoroutine(flipRoutine);

        flipRoutine = null;
    }
}
