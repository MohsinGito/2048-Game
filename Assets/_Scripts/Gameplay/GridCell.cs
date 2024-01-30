using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; }
    public GridTile tile { get; set; }

    public bool Empty => tile == null;
    public bool Occupied => tile != null;
}
