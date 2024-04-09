using UnityEngine;
using UnityEngine.SceneManagement;

// Handles exiting the game
public class GameExitHandler : MonoBehaviour
{
    // Call this function when the exit button is clicked
    public void OnExitButtonClick()
    {
        Application.Quit();
        Debug.Log("Exit button clicked. Game would quit here.");

    }

    void Update()
    {
        // Check if the Esc key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Exit button clicked. Game would quit here.");

        }
    }
}
