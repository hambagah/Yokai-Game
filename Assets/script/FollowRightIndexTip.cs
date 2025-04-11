using UnityEngine;

public class FollowRightIndexTip : MonoBehaviour
{
    [Tooltip("Drag in the right index fingertip transform (e.g., GhostHands/RightHand/Index/Tip)")]
    public Transform rightIndexTip;

    void Update()
    {
        if (rightIndexTip != null)
        {
            transform.position = rightIndexTip.position;
        }
    }
}
