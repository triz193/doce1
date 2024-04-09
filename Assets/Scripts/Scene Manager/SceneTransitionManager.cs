using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public Image fadePanel; // Assign the UI Panel's Image component in the inspector
    public float fadeDuration = 1f; // Duration of the fade effect


    void Start()
    {
        // Set the fade panel color to bluish purple and fade in
        Color bluishPurple = new Color(0.592f, 0.56f, 0.871f, 1f);
        fadePanel.color = bluishPurple;
        StartCoroutine(FadeTo(0f));
    }

    // Call this method to restart the game with a fade effect
    public void RestartGameWithFade()
    {
        StartCoroutine(FadeAndRestart());
    }

    // Call this method to go to the next level with a fade effect
    public void GoToNextLevelWithFade()
    {
        GameManager.Instance.comingFromNextLevelState = true;
        StartCoroutine(FadeAndLoadNextLevel());
    }

    // Coroutine for fading and restarting the game
    IEnumerator FadeAndRestart()
    {
        // Fade to black
        yield return StartCoroutine(FadeTo(1f));

        // Wait for a moment
        yield return new WaitForSeconds(1f);

        // Restart the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Wait for the scene to load
        yield return null;

        // Fade back to transparent
        StartCoroutine(FadeTo(0f));
    }

    // Coroutine for fading and loading the next level
    IEnumerator FadeAndLoadNextLevel()
    {
        // Fade to black
        yield return StartCoroutine(FadeTo(1f));

        // Wait for a moment
        yield return new WaitForSeconds(1f);

        // Load the next level
        SceneManager.LoadScene("NextLevelState");

        // Wait for the scene to load
        yield return null;

        // Fade back to transparent
        StartCoroutine(FadeTo(0f));
    }

    // Coroutine for changing the alpha value of the fade panel
    IEnumerator FadeTo(float targetAlpha)
    {
        Color currentColor = fadePanel.color;
        float startAlpha = currentColor.a;

        for (float t = 0f; t < 1f; t += Time.deltaTime / fadeDuration)
        {
            Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(startAlpha, targetAlpha, t));
            fadePanel.color = newColor;
            yield return null;
        }

        fadePanel.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }
}
