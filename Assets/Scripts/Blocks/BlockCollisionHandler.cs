using UnityEngine;

public class BlockCollisionHandler : MonoBehaviour
{
    // Reference to the Blocks script on the parent object
    private Blocks blocksScript;

    void Start()
    {
        // Find the Blocks script on the parent object
        blocksScript = GetComponentInParent<Blocks>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {   
        BlockGroundCollision blockCollision = gameObject.GetComponent<BlockGroundCollision>();

        // Check if the BlockGroundCollision component is not null
        if (blockCollision != null)
        {
            // Check if the block is the currentBlock or part of the playedBlocks list
            if ((gameObject == blocksScript.currentBlock || blocksScript.playedBlocks.Contains(gameObject)) && !blockCollision.hasCollidedWithGround)
            {
                // Pass both the block and the collided object
                blocksScript.HandleBlockCollision(gameObject, collision.gameObject);
            }
        }
    }

}
