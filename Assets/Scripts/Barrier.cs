using UnityEngine;

public class Barrier : MonoBehaviour {

   // ///////////////////////////////
    // Barrier piece prefabs: ////////
    // These contain the GameObjects used to create individual pieces of a barrier    
    // ///////////////////////////////
    private static GameObject m_PieceContainer;
    private static GameObject m_DefaultBarrier;
    private static GameObject m_CircleBarrier;
    private static GameObject m_TriangleBarrier;
    private static GameObject m_SquareBarrier;
    // ///////////////////////////////
    public BarrierManager.Shape Type = BarrierManager.Shape.UNINITIALIZED;

    private Rigidbody2D rb;             // 
    private int slotIndex;              // Represents index of barrier components that is the slot for the player to goto
    private Player resident;            // Represents the player object is spawned and moves with this barrier


    public static Barrier CreateNewBarrier(RectTransform spawnPoint) {
        Barrier newBarrier = Instantiate(m_PieceContainer, spawnPoint).GetComponent<Barrier>();
        newBarrier.Type = Player.Current.Type;
        newBarrier.slotIndex = (int)Random.Range(1, BarrierManager.ComponentCount - 2);

        // Create a reference that will be used when generating pieces of the barrier
        GameObject newPiece;
        
        // Create the pieces of the barrier:
        for (int c = 0; c < BarrierManager.ComponentCount; c++) {
            if (c == newBarrier.slotIndex) {
                // If c and keyholeIndex match, we need to instiate a piece that matches the type in MustInclude.
                switch (newBarrier.Type) {
                    case BarrierManager.Shape.CIRCLE:
                        newPiece = Instantiate(m_CircleBarrier, newBarrier.transform);
                        break;

                    case BarrierManager.Shape.SQUARE:
                        newPiece = Instantiate(m_SquareBarrier, newBarrier.transform);
                        break;

                    case BarrierManager.Shape.TRIANGLE:
                        newPiece = Instantiate(m_TriangleBarrier, newBarrier.transform);
                        break;

                    default:
                        Debug.LogError("Failure creating a keyhole! Generating non-keyhole barrier piece.");
                        newPiece = Instantiate(m_DefaultBarrier, newBarrier.transform);
                        break;
                }
            }

            else
                newPiece = Instantiate(m_DefaultBarrier, newBarrier.transform);

            // Properly scale the new piece
            newPiece.transform.localScale = new Vector3(BarrierManager.PieceScale, BarrierManager.PieceScale, 1);
        }

        return newBarrier;
    }

	// AttachPlayer():		Sets this.resident to the Player object and scales it
    public void AttachPlayer(Player player) {
        resident = player;
        player.transform.SetParent(transform.GetChild(slotIndex));
        player.transform.localPosition = new Vector3(0, 1.5f, 1);
        player.transform.localScale = Vector3.one;
    }

	// Move():		Add a force to the rigidbody of this object
	public void Move() {
        rb.AddForce(Vector2.down * BarrierManager.Speed);
    }

}
