using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Board", menuName = "Create Game Board")]
public class GameBoard : ScriptableObject
{

    [field: SerializeField] public int Rows { private set; get; } = 4;
    [field: SerializeField] public int Columns { private set; get; } = 4;
    [field: SerializeField] public int CellSize { private set; get; } = 100;
    [field: SerializeField] public Vector2 Padding { private set; get; } = new Vector2(15f, 15f);
    [field: SerializeField] public Vector2 Spacing { private set; get; } = new Vector2(120f, 120f);
    [field: SerializeField] public Vector2 CenterOffset { private set; get; } = new Vector2(0, 0);
    [field: SerializeField] public int InitialTiles { private set; get; } = 2;
    [field: SerializeField] public float TilesMoveAnimDuration { private set; get; } = 0.1f;
    [field: SerializeField] public float TilesScaleAnimDuration { private set; get; } = 0.05f;
    [field: SerializeField] public List<TileInfo> TileInfos { private set; get; } = new List<TileInfo>();

}

[System.Serializable]
public class TileInfo
{
    public int Number;
    public Color TextColor = Color.white;
    public Color BackgroundColor = Color.white;
    public Sprite BackgroundSprite;
    public bool UserBackgroundSprite;
}