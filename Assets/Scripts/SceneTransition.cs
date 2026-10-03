using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Covers the screen with squares from left to right, loads the scene, then uncovers it left to right.
// It creates itself the first time it's used, so it doesn't have to be placed in any scene
// (place it on an empty GameObject in the first scene if you want to tweak the values in the inspector).
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private Color squareColor = new Color(1f, 0.8f, 0.2f);
    [SerializeField] private int rows = 8;
    [SerializeField] private float squareAnimationTime = 0.3f;
    [SerializeField] private float delayPerColumn = 0.03f;

    private Canvas canvas;
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

    private void StartTransition(Func<AsyncOperation> _loadScene)
    {
        if (isTransitioning)
        {
            return;
        }
        StartCoroutine(Transition(_loadScene));
    }

    private IEnumerator Transition(Func<AsyncOperation> _loadScene)
    {
        isTransitioning = true;
        BuildSquares();
        canvas.enabled = true;

        yield return AnimateSquares(true);

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
