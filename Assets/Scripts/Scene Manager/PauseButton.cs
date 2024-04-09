using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Sprite pauseSprite; // The sprite to show when the game is paused
    [SerializeField] private Sprite playSprite; // The sprite to show when the game is playing
    private Button button; // The button component
    private Image buttonImage; // The image component of the button
    private bool isPaused; // A flag to check if the game is paused
    public static bool IsPauseClicked; // A static flag to check if the pause button was clicked

    void Start()
    {
        button = GetComponent<Button>(); // Get the Button component attached to this GameObject
        buttonImage = GetComponentInChildren<Image>(); // Get the Image component from the child of this GameObject
        isPaused = false; // Initially, the game is not paused
        IsPauseClicked = false; // Initially, the pause button is not clicked
        button.onClick.AddListener(TogglePauseState); // Add a listener to the button to call TogglePauseState when clicked
    }

    void TogglePauseState()
    {
        IsPauseClicked = true; // Set the flag to true when the button is clicked

        // Toggle the isPaused flag
        if (isPaused)
        {
            isPaused = false; // Unpause the game
            buttonImage.sprite = pauseSprite; // Change the button image to the pause sprite
            Time.timeScale = 1; // Resume the game
        }
        else
        {
            isPaused = true; // Pause the game
            buttonImage.sprite = playSprite; // Change the button image to the play sprite
            Time.timeScale = 0; // Pause the game
        }

        Debug.Log("TogglePauseState: IsPauseClicked = " + IsPauseClicked); // Log the value of IsPauseClicked

        // Reset the IsPauseClicked flag after a short delay
        Invoke("ResetPauseClickedFlag", 0.1f);
    }

    void ResetPauseClickedFlag()
    {
        IsPauseClicked = false; // Reset the flag to false
    }
}
