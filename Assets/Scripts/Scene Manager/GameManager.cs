using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentLevel = 1; // Start at level 1
    public const int maxLevel = 6; // Max level
    public int levelBarValue = 1; // Start at value 1 for the level bar
   
    // Handles the Level Bar:
    public Slider levelBar; // Reference to the level bar slider
    public bool comingFromNextLevelState = false;
    public bool comingFromStartState = true; // Assume true at the beginning


    void Awake()
    {
        Debug.Log($"GameManager Awake called. Existing instance: {Instance != null}");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager instance set and marked as DontDestroyOnLoad.");
        }
        else
        {
            Debug.LogWarning("GameManager instance already exists. Destroying this duplicate.");
            Destroy(gameObject);
        }
    }



    void OnEnable()
    {
        Debug.Log("Subscribing to sceneLoaded.");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

        void OnDisable()
    {
        Debug.Log("Unsubscribing from sceneLoaded.");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        Debug.Log("Scene Loaded: " + scene.name); // Check if this logs correctly.

        if (scene.name == "PlayState") {
            // Find and reference the level bar slider
            GameObject levelBarObject = GameObject.FindGameObjectWithTag("LevelBar");
            if (levelBarObject != null)
            {
                levelBar = levelBarObject.GetComponent<Slider>();
            }
            else
            {
                Debug.LogError("Failed to find the level bar slider in the scene.");
            }

            Debug.Log("Coming from start state: " + comingFromStartState);
            Debug.Log("Coming from next level state: " + comingFromNextLevelState);

            if (comingFromStartState)
            {
                Debug.Log("Current level is " + currentLevel);

                if (levelBar != null)
                {
                    // Use the conversion method to set the slider value correctly.
                    levelBar.value = LevelToSliderValue(currentLevel);
                    Debug.Log("Level Bar Initialized to " + SliderValueToLevel((int)levelBar.value));
                }
                comingFromStartState = false;
            } 
            else if (comingFromNextLevelState)
            {
                currentLevel++;
                Debug.Log($"Current level changed to {currentLevel}.");

                if (levelBar != null) {
                    levelBar.value = LevelToSliderValue(currentLevel);
                    Debug.Log("Level Bar Updated to " + SliderValueToLevel((int)levelBar.value));
                }
                comingFromNextLevelState = false;
            }
        }
    }

    public void OnContinueButtonClicked()
    {
        Debug.Log($"Continue button clicked. Current level: {currentLevel}, Max level: {maxLevel}");


        // Update the comingFromNextLevelState flag in the SceneTransitionManager

        if (currentLevel < maxLevel)
        {
            Debug.Log("Loading PlayState");
            SceneManager.LoadScene("PlayState");
        }
        else
        {
            Debug.Log("Loading WinState");
            SceneManager.LoadScene("WinState");
        }
    }

    public void OnStartButtonClicked()
    {
          SceneManager.LoadScene("PlayState");       
    }

    // Converts the current game level to the appropriate slider value.
    private int LevelToSliderValue(int level) {
        // Since your levels start at 1 and you want the slider to start at 1 too,
        // subtract 1 from the level to align it with the slider's minimum value of 0.
        return level;
    }

    // Converts the slider value back to the game level if needed.
    private int SliderValueToLevel(int sliderValue) {
        // Add 1 to the slider value to convert it back to the game level.
        return sliderValue;
    }

}
