using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class Blocks : MonoBehaviour
{
    [Header("Prefabs")]

    [SerializeField] private GameObject cakeBlockPrefab;
    [SerializeField] private GameObject fillingBlockPrefab;
    
    [Header("Block Generator Movement")]

    [SerializeField] private float a = 2f; // Semi-major axis of the ellipse
    [SerializeField] private float b = 0.5f; // Semi-minor axis of the ellipse
    [SerializeField] private float speed = 2f; // Speed of the elliptical motion


    [Header("Camera Movement")]

    [SerializeField] private float bottomOffset = 2f; // Camera offset

    [Header("Progress Bar")]
    public Slider progressBar; // Reference to the Progress Bar slider

    public GameObject currentBlock;
    private GameObject topMostBlock;
    private bool isFalling = false;
    private float angle = 0f;
    private bool collisionHandled = false;

    public SceneTransitionManager sceneTransitionManager;

    private Camera mainCamera;
    public List<GameObject> playedBlocks = new List<GameObject>();
   
    // Handles the heart Bar:
    public Slider heartsBar; // Reference to the Hearts Bar slider
    private bool isFirstBlock = true;

    // Handles the Current Score:
    public CurrentScore currentScore;


    

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera
        GenerateBlock();

    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFalling && !EventSystem.current.IsPointerOverGameObject())
        {
            isFalling = true;
            currentBlock.GetComponent<Rigidbody2D>().isKinematic = false;
        }

        if (!isFalling)
        {
            MoveInEllipse();
            MoveGenerationSpotUpwards();
        }
            
    }

    void GenerateBlock()
    {
        float randomValue = Random.value;
        if (randomValue <= 0.8f) // 80% chance
        {
            currentBlock = Instantiate(cakeBlockPrefab, transform.position, Quaternion.identity, transform);
        }
        else // 20% chance
        {
            currentBlock = Instantiate(fillingBlockPrefab, transform.position, Quaternion.identity, transform);
        }
        currentBlock.GetComponent<Rigidbody2D>().isKinematic = true;

        // Access the Collider components of the child objects
        Collider[] childColliders = currentBlock.GetComponentsInChildren<Collider>();
        foreach (Collider collider in childColliders)
        {
            collider.enabled = true;
        }
        Debug.Log("Block generated at position: " + transform.position);
        
        collisionHandled = false; // Reset the flag for the new block
            
        BlockGroundCollision blockCollision = currentBlock.AddComponent<BlockGroundCollision>();
        blockCollision.hasCollidedWithGround = false;
    }

    void MoveInEllipse()
    {
        angle += speed * Time.deltaTime;
        float x = Mathf.Cos(angle) * a;
        float y = Mathf.Sin(angle) * b;
        currentBlock.transform.localPosition = new Vector3(x, y, 0);
    }

    void MoveGenerationSpotUpwards()
    {
        if (mainCamera != null)
        {
            Vector3 cameraPos = mainCamera.transform.position;
            float offset = 3f; // Adjust this value to control the distance from the top
            float yPos = cameraPos.y + offset;
            transform.position = new Vector3(cameraPos.x, yPos, transform.position.z);
            
        }
    }

    public void HandleBlockCollision(GameObject block, GameObject collidedObject)
    {    
        Debug.Log($"Collision detected between {block.name} and {collidedObject.name}");


        bool isGroundCollision = collidedObject.CompareTag("Ground");

        // Check if the block is the current block or a played block
        if (block == currentBlock || playedBlocks.Contains(block))
        {
            // Handle the collision with the ground
            HeartsBarCalculator(block, collidedObject);
        }


        // If it's the current block, handle additional logic
        if (block == currentBlock && isFalling)
        {

            // Update the topmost block
            UpdateTopmostBlock();

            playedBlocks.Add(currentBlock);
            
            // Calculate the overlap percentage and points for the block
            GameObject lastPlayedBlock = playedBlocks[playedBlocks.Count - 1];
            BlockPoints blockPoints = lastPlayedBlock.GetComponent<BlockPoints>();

            if (!blockPoints.PointsAdded)
            {

                float overlapPercentage = CalculateOverlapPercentage(lastPlayedBlock, collidedObject);
                float overlapThreshold = 80f;
                int points = overlapPercentage > overlapThreshold ? 20 : 10;

                currentScore.AddScore(points);
                blockPoints.Points = points;
                blockPoints.PointsAdded = true;
                Debug.Log($"Points added: {points}");

            }
            
            if (playedBlocks.Contains(collidedObject) || isGroundCollision)
            {
            // Handle collision with a played block or the ground (for the current block only)
            if (block == currentBlock && isFalling)
            {
                ProgressBarCalculator(isGroundCollision, block); // Update the progress bar based on collision type
                Debug.Log("Collision with ground detected, updating progress bar");

            }
        }
          
            // Clear the currentBlock variable and generate a new block
            currentBlock = null;
            isFalling = false;

            GenerateBlock();
        }
        
        collisionHandled = true; // Set the flag to indicate that the collision has been handled
        

    }


    void UpdateTopmostBlock()
    {
        topMostBlock = null;
        foreach (GameObject block in playedBlocks)
        {
            // Check if the block has not been destroyed
            if (block != null && (topMostBlock == null || block.transform.position.y > topMostBlock.transform.position.y))
            {
                topMostBlock = block;
            }
        }
    }




    public Vector3 GetTopmostBlockPosition()
    {
        if (topMostBlock != null)
        {
            return topMostBlock.transform.position;
        }
        return new Vector3(transform.position.x, mainCamera.transform.position.y - (mainCamera.orthographicSize * 2) / 2 + bottomOffset, transform.position.z); // Return a position at the bottom of the camera view
    }


    // Calculates the mistakes, updates the Hearts Bar and resets the scene if player dies
    private void HeartsBarCalculator(GameObject block, GameObject collidedObject)
    {
        BlockGroundCollision blockCollision = block.GetComponent<BlockGroundCollision>();

        if (collidedObject.CompareTag("Ground") && !blockCollision.hasCollidedWithGround)
        {
            Debug.Log("Block collided with ground");
            blockCollision.hasCollidedWithGround = true;


            // Check if it's not the first block
            if (!isFirstBlock)
            {
                // Decrease hearts bar value
                heartsBar.value -= 1;

                // Fade out and destroy the block
                StartCoroutine(FadeOutBlock(block));

                // Restart the scene if the hearts bar value reaches 0
                if (heartsBar.value <= 0)
                {
                    sceneTransitionManager.RestartGameWithFade();
                }
            }
            else
            {
                // Set isFirstBlock to false after the first block touches the ground
                isFirstBlock = false;
            }
        }
    }

    

    IEnumerator FadeOutBlock(GameObject block)
    {
        SpriteRenderer spriteRenderer = block.GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        float fadeDuration = 1.0f; // Duration in seconds
        float fadeSpeed = 1 / fadeDuration;

        for (float t = 0; t < 1; t += Time.deltaTime * fadeSpeed)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, t));
            yield return null;
        }
        // Get the points from the BlockPoints component
        int points = block.GetComponent<BlockPoints>().Points;

        // Subtract the points from the score
        currentScore.SubtractScore(points);

        // Get the BlockPoints component
        BlockPoints blockPoints = block.GetComponent<BlockPoints>();
        if (blockPoints != null && blockPoints.ProgressBarIncreased)
        {
            // Decrease the progress bar value by 1
            progressBar.value -= 1;

            blockPoints.ProgressBarIncreased = false; // Reset the flag
        }

        Destroy(block);

    }

    private void ProgressBarCalculator(bool isGroundCollision, GameObject block)
    {
        Debug.Log($"ProgressBarCalculator called. isFirstBlock: {isFirstBlock}, isGroundCollision: {isGroundCollision}");

        // Increase the progress bar value by 1 only if it's the first block
        // or if the collision is with the topmost block
        if (isFirstBlock && isGroundCollision || !isFirstBlock && !isGroundCollision)
        {
            progressBar.value += 1;
            Debug.Log("Progress bar updated");
            Debug.Log($"isFirstBlock: {isFirstBlock}, isGroundCollision: {isGroundCollision}, progressBar.value: {progressBar.value}");

            if (progressBar.value >= 10)
            {
                sceneTransitionManager.GoToNextLevelWithFade();
            }

            // Set ProgressBarIncreased to true for the block
            BlockPoints blockPoints = block.GetComponent<BlockPoints>();
            if (blockPoints != null)
            {
                blockPoints.ProgressBarIncreased = true;
            }
        }

    }

    private float CalculateOverlapPercentage(GameObject block, GameObject collidedObject)
    {
        // Get the center positions of the block and the collided object
        Vector2 blockCenter = block.GetComponent<Collider2D>().bounds.center;
        Vector2 collidedObjectCenter = collidedObject.GetComponent<Collider2D>().bounds.center;

        // Calculate the horizontal distance between the centers
        float horizontalDistance = Mathf.Abs(blockCenter.x - collidedObjectCenter.x);

        // Get the width of the block (assuming the block and the collided object have similar widths)
        float blockWidth = block.GetComponent<Collider2D>().bounds.size.x;

        // Calculate the overlap percentage based on the horizontal distance
        // If the distance is less than half the width of the block, we consider it a good overlap (100%)
        // If the distance is greater than half the width of the block, the overlap percentage decreases
        float overlapPercentage = Mathf.Max(0f, (blockWidth / 2f - horizontalDistance) / (blockWidth / 2f) * 100f);

        return overlapPercentage;
    }


}
