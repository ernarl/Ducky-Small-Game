using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// The ending for getting every star: the duck slowly pops up in the middle of the screen looking at the player,
// then grows and flies into the camera until it fills the screen.
// Runs while the scene transition covers the screen, before the next scene loads.
public static class DuckFinale
{
    private const float SPAWN_TIME = 1.5f;
    private const float HOLD_TIME = 0.8f;
    private const float FLY_TIME = 2.5f;
    private const float FADE_OUT_PART = 0.25f; // At the end of the flight the duck fades into the transition's yellow squares
    private const float START_SCREEN_HEIGHT_PART = 0.35f; // How much of the screen height the duck takes when it appears
    private const float FIELD_OF_VIEW = 40f;
    private const float WOBBLE_ANGLE = 6f;
    private const float WOBBLE_SPEED = 3f;

    private static readonly Color BACKGROUND_COLOR = new Color(0.55f, 0.8f, 0.95f); // Bath water blue, so the yellow duck stands out
    private static readonly Vector3 FINALE_POSITION = new Vector3(0f, -1000f, 0f); // Far from the level, so the camera only sees the duck

    public static IEnumerator Play()
    {
        // The duck model is copied from the level's duck
        PlayerController player = Object.FindObjectOfType<PlayerController>();
        MeshFilter playerMesh = player != null ? player.GetComponentInChildren<MeshFilter>() : null;
        if (playerMesh == null)
        {
            yield break;
        }

        Quaternion cameraRotation = GetCameraRotation();
        Vector3 cameraForward = cameraRotation * Vector3.forward;

        Camera camera = new GameObject("FinaleCamera").AddComponent<Camera>();
        camera.transform.SetPositionAndRotation(FINALE_POSITION, cameraRotation);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = BACKGROUND_COLOR;
        camera.fieldOfView = FIELD_OF_VIEW;
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 20f;
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        camera.targetTexture = renderTexture;

        // The model goes in a centered child, so the duck grows and wobbles around its middle
        GameObject duck = new GameObject("FinaleDuck");
        GameObject model = new GameObject("Model", typeof(MeshFilter), typeof(MeshRenderer));
        model.transform.SetParent(duck.transform, false);
        model.GetComponent<MeshFilter>().sharedMesh = playerMesh.sharedMesh;
        model.GetComponent<MeshRenderer>().sharedMaterials = playerMesh.GetComponent<MeshRenderer>().sharedMaterials;
        Bounds bounds = playerMesh.sharedMesh.bounds;
        model.transform.localPosition = -bounds.center;
        // The duck's beak points along the model's forward, so this makes it look at the camera
        Quaternion lookAtCamera = Quaternion.LookRotation(-cameraForward);

        float viewHeightPerDistance = 2f * Mathf.Tan(FIELD_OF_VIEW * 0.5f * Mathf.Deg2Rad);
        float startDistance = bounds.size.y / (START_SCREEN_HEIGHT_PART * viewHeightPerDistance);
        // Stops just before the beak (the front of the model) would go through the camera
        float endDistance = bounds.extents.z + camera.nearClipPlane * 3f;

        // Shows the camera's picture over the scene transition's squares
        Canvas canvas = new GameObject("FinaleCanvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1001;
        RawImage view = new GameObject("View", typeof(RectTransform)).AddComponent<RawImage>();
        view.transform.SetParent(canvas.transform, false);
        view.rectTransform.anchorMin = Vector2.zero;
        view.rectTransform.anchorMax = Vector2.one;
        view.rectTransform.offsetMin = Vector2.zero;
        view.rectTransform.offsetMax = Vector2.zero;
        view.texture = renderTexture;
        view.raycastTarget = false;

        Action<float, float> placeDuck = (distance, wobbleAmount) =>
        {
            float wobble = Mathf.Sin(Time.unscaledTime * WOBBLE_SPEED) * WOBBLE_ANGLE * wobbleAmount;
            duck.transform.SetPositionAndRotation(FINALE_POSITION + cameraForward * distance,
                lookAtCamera * Quaternion.Euler(0f, 0f, wobble));
        };

        // Fades in over the squares while the duck pops up
        yield return Animate(SPAWN_TIME, t =>
        {
            view.color = new Color(1f, 1f, 1f, Mathf.Clamp01(t * 3f));
            duck.transform.localScale = Vector3.one * LeanTween.easeOutBack(0f, 1f, t);
            placeDuck(startDistance, 1f);
        });

        yield return Animate(HOLD_TIME, t => placeDuck(startDistance, 1f));

        // Starts slowly and speeds up, so it grows bigger and bigger until it fills the screen
        yield return Animate(FLY_TIME, t =>
        {
            placeDuck(LeanTween.easeInCubic(startDistance, endDistance, t), 1f - t);
            float fadeOut = Mathf.Clamp01((t - (1f - FADE_OUT_PART)) / FADE_OUT_PART);
            view.color = new Color(1f, 1f, 1f, 1f - fadeOut);
        });

        Object.Destroy(canvas.gameObject);
        Object.Destroy(duck);
        Object.Destroy(camera.gameObject);
        renderTexture.Release();
        Object.Destroy(renderTexture);
    }

    // Looks the same way the level's sun shines (from a little to the side), so the light falls on the duck's face
    private static Quaternion GetCameraRotation()
    {
        Light sun = RenderSettings.sun;
        if (sun == null)
        {
            foreach (Light light in Object.FindObjectsOfType<Light>())
            {
                if (light.type == LightType.Directional)
                {
                    sun = light;
                    break;
                }
            }
        }

        float yaw = 0f;
        if (sun != null)
        {
            Vector3 sunDirection = sun.transform.forward;
            yaw = Mathf.Atan2(sunDirection.x, sunDirection.z) * Mathf.Rad2Deg + 30f;
        }
        return Quaternion.Euler(0f, yaw, 0f);
    }

    // Calls _step every frame with the progress from 0 to 1. Unscaled time, like the scene transition
    private static IEnumerator Animate(float _duration, Action<float> _step)
    {
        float time = 0f;
        while (time < _duration)
        {
            time += Time.unscaledDeltaTime;
            _step(Mathf.Clamp01(time / _duration));
            yield return null;
        }
    }
}
