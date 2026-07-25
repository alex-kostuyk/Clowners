using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Ladder Segment")]
    [Tooltip("Local-space offset of the bottom of the climbable segment from the transform origin")]
    public Vector3 LadderSegmentBottom = Vector3.zero;
    [Tooltip("Length of the climbable segment along the ladder's local up axis")]
    public float LadderSegmentLength = 5f;

    [Header("Release Points")]
    [Tooltip("Transform the player is moved to when dismounting at the top")]
    public Transform TopReleasePoint;
    [Tooltip("Transform the player is moved to when dismounting at the bottom")]
    public Transform BottomReleasePoint;

    /// <summary>World-space position of the bottom of the climbable segment.</summary>
    public Vector3 BottomAnchorPoint => transform.position + transform.TransformVector(LadderSegmentBottom);

    /// <summary>World-space position of the top of the climbable segment.</summary>
    public Vector3 TopAnchorPoint => BottomAnchorPoint + (transform.up * LadderSegmentLength);

    /// <summary>
    /// Returns the closest point on the ladder segment to <paramref name="fromPoint"/>.
    /// <paramref name="onSegmentState"/> is 0 when on-segment, positive when above top, negative when below bottom.
    /// </summary>
    public Vector3 ClosestPointOnLadderSegment(Vector3 fromPoint, out float onSegmentState)
    {
        Vector3 segment = TopAnchorPoint - BottomAnchorPoint;
        Vector3 toPoint = fromPoint - BottomAnchorPoint;
        float projLength = Vector3.Dot(toPoint, segment.normalized);

        if (projLength > 0f)
        {
            if (projLength <= segment.magnitude)
            {
                onSegmentState = 0f;
                return BottomAnchorPoint + (segment.normalized * projLength);
            }
            else
            {
                onSegmentState = projLength - segment.magnitude;
                return TopAnchorPoint;
            }
        }
        else
        {
            onSegmentState = projLength;
            return BottomAnchorPoint;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(BottomAnchorPoint, TopAnchorPoint);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(BottomAnchorPoint, 0.15f);
        Gizmos.DrawWireSphere(TopAnchorPoint, 0.15f);

        if (TopReleasePoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(TopReleasePoint.position, 0.2f);
        }
        if (BottomReleasePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(BottomReleasePoint.position, 0.2f);
        }
    }
}
