using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Blocks blockScript; // Reference to the Blocks script
    public float smoothSpeed = 0.125f; // Smoothness of the camera movement
    public float topOffset = 3f; // Offset from the top of the screen for the block generator
    public float bottomOffset = 2f; // Offset from the bottom of the screen for the played block

    void LateUpdate()
    {

        // Get the position of the block generator
        Vector3 generatorPosition = blockScript.transform.position;

        // Desired position based on the block generator
        float desiredPositionY = generatorPosition.y - topOffset;

        // Adjust desired position based on the topmost block
        Vector3 topmostBlockPosition = blockScript.GetTopmostBlockPosition();
        if (!float.IsPositiveInfinity(topmostBlockPosition.y))
        {
            desiredPositionY = Mathf.Min(desiredPositionY, topmostBlockPosition.y + bottomOffset);
        }

        // Update the camera position
        float newPositionY = Mathf.Lerp(transform.position.y, desiredPositionY, smoothSpeed * Time.deltaTime);
        newPositionY = Mathf.Max(newPositionY, 0); // Ensure the camera does not go below y = 0
        transform.position = new Vector3(transform.position.x, newPositionY, transform.position.z);

        //Debug.Log("Generator Position: " + generatorPosition.y);
        //Debug.Log("Topmost Block Position: " + topmostBlockPosition.y);
    }




}
