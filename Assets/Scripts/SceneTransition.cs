using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Covers the screen with squares from left to right, loads the scene, then uncovers it left to right.
// It can also show a message (like "Thanks for playing!") and play an extra animation on the covered screen before loading.
// It creates itself the first time it's used, so it doesn't have to be placed in any scene
// (place it on an empty GameObject in the first scene if you want to tweak the values in the inspector).
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private Color squareColor = new Color(1f, 0.8f, 0.2f);
    [SerializeField] private int rows = 8;
    [SerializeField] private float squareAnimationTime = 0.3f;
    [SerializeField] private float delayPerColumn = 0.03f;

    [Header("Message")]
    [SerializeField] private Color messageColor = new Color(0.26f, 0.19f, 0.19f);
    [SerializeField] private float messageScreenHeightPart = 0.1f; // Font size, as a part of the screen height
    [SerializeField] private float messageWordSpacing = 30f; // Extra space between words (the game's font has narrow spaces)
    [SerializeField] private float messagePopTime = 0.35f;
    [SerializeField] private float messageShowTime = 2.5f;

    private Canvas canvas;
    private TextMeshProUGUI messageText;
    private RectTransform[] squares;
    private float[] squareDelays;
    private float animationTotalTime;
    private int builtForWidth;
    private int builtForHeight;
    private bool isTransitioning = false;

    public static void LoadScene(string _sceneName)
    {
        GetInstance().StartTransition(() => SceneManager.LoadSceneAsync(_sceneName));
    }

    public static void LoadScene(int _sceneIndex)
    {
        GetInstance().StartTransition(() => SceneManager.LoadSceneAsync(_sceneIndex));
    }

    // _afterMessage runs after the message, while the screen is still covered
    public static void LoadScene(string _sceneName, string _message, Func<IEnumerator> _afterMessage = null)
    {
        GetInstance().StartTransition(() => SceneManager.LoadSceneAsync(_sceneName), _message, _afterMessage);
    }

    private static SceneTransition GetInstance()
    {
        if (Instance == null)
        {
            new GameObject("SceneTransition").AddComponent<SceneTransition>();
        }
        return Instance;
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
        CreateCanvas();
    }

    private void CreateCanvas()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        // Blocks clicks on the scene's UI while the squares cover it
        gameObject.AddComponent<GraphicRaycaster>();
        canvas.enabled = false;
    }

    private void StartTransition(Func<AsyncOperation> _loadScene, string _message = null, Func<IEnumerator> _afterMessage = null)
    {
        if (isTransitioning)
        {
            return;
        }
        StartCoroutine(Transition(_loadScene, _message, _afterMessage));
    }

    private IEnumerator Transition(Func<AsyncOperation> _loadScene, string _message, Func<IEnumerator> _afterMessage)
    {
        isTransitioning = true;
        BuildSquares();
        canvas.enabled = true;
        AudioManager.Play(SoundNames.SceneTransition);

        yield return AnimateSquares(true);

        if (_message != null)
        {
            yield return ShowMessage(_message);
        }
        if (_afterMessage != null)
        {
            yield return _afterMessage();
        }

        AsyncOperation loading = _loadScene();
        while (!loading.isDone)
        {
            yield return null;
        }
        // Let the new scene run its first frame before showing it
        yield return null;

        yield return AnimateSquares(false);

        canvas.enabled = false;
        isTransitioning = false;
    }

    // Rebuilds the grid only when the screen size changed since the last transition
    private void BuildSquares()
    {
        if (squares != null && builtForWidth == Screen.width && builtForHeight == Screen.height)
        {
            return;
        }

        if (squares != null)
        {
            foreach (RectTransform square in squares)
            {
                Destroy(square.gameObject);
            }
        }

        builtForWidth = Screen.width;
        builtForHeight = Screen.height;

        int rowCount = Mathf.Max(1, rows);
        float squareSize = Mathf.Ceil((float)Screen.height / rowCount);
        int columnCount = Mathf.CeilToInt(Screen.width / squareSize);

        squares = new RectTransform[rowCount * columnCount];
        squareDelays = new float[squares.Length];
        animationTotalTime = (columnCount - 1) * delayPerColumn + squareAnimationTime;

        int i = 0;
        for (int column = 0; column < columnCount; column++)
        {
            for (int row = 0; row < rowCount; row++)
            {
                GameObject square = new GameObject("Square", typeof(RectTransform), typeof(Image));
                square.transform.SetParent(transform, false);
                square.GetComponent<Image>().color = squareColor;

                RectTransform squareRect = square.GetComponent<RectTransform>();
                squareRect.anchorMin = Vector2.zero;
                squareRect.anchorMax = Vector2.zero;
                // A little bigger than the grid cell so there are no gaps between squares
                squareRect.sizeDelta = Vector2.one * (squareSize + 2f);
                squareRect.anchoredPosition = new Vector2((column + 0.5f) * squareSize, (row + 0.5f) * squareSize);
                squareRect.localScale = Vector3.zero;

                squares[i] = squareRect;
                squareDelays[i] = column * delayPerColumn;
                i++;
            }
        }
    }

    // Pops the message in over the covered screen, keeps it there for a moment, then pops it away
    private IEnumerator ShowMessage(string _message)
    {
        // Taken before the message text is shown, so the font found is one of the scene's texts
        TMP_Text sceneText = FindObjectOfType<TMP_Text>();

        TextMeshProUGUI text = GetMessageText();
        if (sceneText != null)
        {
            text.font = sceneText.font;
        }
        text.fontSize = Screen.height * messageScreenHeightPart;
        text.text = _message;
        // In front of the squares, which are rebuilt when the screen size changes
        text.transform.SetAsLastSibling();
        text.transform.localScale = Vector3.zero;
        text.gameObject.SetActive(true);
        AudioManager.Play(SoundNames.QuestComplete);

        yield return AnimateMessage(true);
        yield return new WaitForSecondsRealtime(messageShowTime);
        yield return AnimateMessage(false);

        text.gameObject.SetActive(false);
    }

    // Created the first time a message is shown. This object isn't placed in any scene, so there's no font
    // to set in the inspector: it uses the font of the scene's texts instead (the game's font)
    private TextMeshProUGUI GetMessageText()
    {
        if (messageText == null)
        {
            GameObject textObject = new GameObject("Message", typeof(RectTransform));
            textObject.transform.SetParent(transform, false);

            messageText = textObject.AddComponent<TextMeshProUGUI>();
            messageText.rectTransform.anchorMin = Vector2.zero;
            messageText.rectTransform.anchorMax = Vector2.one;
            messageText.rectTransform.offsetMin = Vector2.zero;
            messageText.rectTransform.offsetMax = Vector2.zero;
            messageText.alignment = TextAlignmentOptions.Center;
            messageText.wordSpacing = messageWordSpacing;
            messageText.color = messageColor;
            messageText.raycastTarget = false;
        }
        return messageText;
    }

    private IEnumerator AnimateMessage(bool _show)
    {
        float time = 0f;
        while (time < messagePopTime)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / messagePopTime);
            messageText.transform.localScale = Vector3.one * (_show ? EaseOutBack(t) : 1f - EaseInBack(t));
            yield return null;
        }
    }

    // Unscaled time, so it still plays if the game is paused
    private IEnumerator AnimateSquares(bool _cover)
    {
        float time = 0f;
        while (time < animationTotalTime)
        {
            time += Time.unscaledDeltaTime;
            SetSquareScales(time, _cover);
            yield return null;
        }
        SetSquareScales(animationTotalTime, _cover);
    }

    private void SetSquareScales(float _time, bool _cover)
    {
        for (int i = 0; i < squares.Length; i++)
        {
            float t = Mathf.Clamp01((_time - squareDelays[i]) / squareAnimationTime);
            float scale = _cover ? EaseOutBack(t) : 1f - EaseInBack(t);
            squares[i].localScale = Vector3.one * scale;
        }
    }

    // Overshoots a bit past 1 before settling, so the squares "pop" in
    private float EaseOutBack(float _t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(_t - 1f, 3f) + c1 * Mathf.Pow(_t - 1f, 2f);
    }

    // Grows a bit before shrinking away
    private float EaseInBack(float _t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return c3 * _t * _t * _t - c1 * _t * _t;
    }
}
