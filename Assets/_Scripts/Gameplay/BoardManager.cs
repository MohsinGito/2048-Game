using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Utilities.Audio;

public class BoardManager : MonoBehaviour
{

    [SerializeField] private BoardGrid Grid;
    [SerializeField] private GridCell GridCell;
    [SerializeField] private GridTile GridTile;
    [SerializeField] private GameBoard Gameboard;

    private List<GridCell> girdCells;
    private List<GridRow> gridRows;
    private List<GridTile> tiles;
    private bool waiting;
    private bool isPaused;
    private Tween pauseTween;

    public bool InputPaused
    {
        set
        {
            if (pauseTween != null)
                pauseTween.Kill();

            DOVirtual.DelayedCall(0.25f, () => isPaused = value);
        }
    }

    private void Awake()
    {
        gridRows = new List<GridRow>();
        girdCells = new List<GridCell>();
        tiles = new List<GridTile>(Gameboard.Rows * Gameboard.Columns);
    }

    public void Init()
    {
        CreateBoardLayout();
        Grid.Init(gridRows, girdCells);
    }

    private void CreateBoardLayout()
    {
        Grid.GetComponent<RectTransform>().anchoredPosition = Gameboard.CenterOffset;
        float gridWidth = (Gameboard.CellSize * Gameboard.Columns) + (Gameboard.Spacing.x * (Gameboard.Columns - 1)) + (Gameboard.Padding.x * 2);
        float gridHeight = (Gameboard.CellSize * Gameboard.Rows) + (Gameboard.Spacing.y * (Gameboard.Rows - 1)) + (Gameboard.Padding.y * 2);
        Grid.GetComponent<RectTransform>().sizeDelta = new Vector2(gridWidth, gridHeight);
        float startX = -gridWidth / 2 + Gameboard.CellSize / 2 + Gameboard.Padding.x;
        float startY = gridHeight / 2 - Gameboard.CellSize / 2 - Gameboard.Padding.y;

        for (int i = 0; i < Gameboard.Rows; i++)
        {
            gridRows.Add(new GridRow());
            for (int j = 0; j < Gameboard.Columns; j++)
            {
                GridCell newCell = Instantiate(GridCell, Grid.transform);
                float posX = startX + j * (Gameboard.CellSize + Gameboard.Spacing.x);
                float posY = startY - i * (Gameboard.CellSize + Gameboard.Spacing.y);
                newCell.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
                newCell.GetComponent<RectTransform>().sizeDelta = new Vector2(Gameboard.CellSize, Gameboard.CellSize);

                gridRows[i].AssignCell(newCell);
                girdCells.Add(newCell);
            }
        }
    }

    public void SetUpBoard()
    {
        for (int i = 0; i < Gameboard.InitialTiles; i++)
            CreateTile();
    }

    public void ClearBoard()
    {
        foreach (var cell in Grid.cells) {
            cell.tile = null;
        }

        foreach (var tile in tiles) {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        GridTile tile = Instantiate(GridTile, Grid.transform);
        tile.Init(Grid.GetRandomEmptyCell(), Gameboard.TilesMoveAnimDuration);
        tile.SetState(Gameboard.TileInfos[0]);
        tiles.Add(tile);
    }

    public void RemoveTile(GridTile tile)
    {
        tiles.Remove(tile);
    }

    public bool CanActivateHammer()
    {
        return tiles.Count > 1;
    }

    private void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                MoveUp();
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                MoveLeft();
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                MoveDown();
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                MoveRight();
        }
    }

    public void MoveUp(float _force = 0) => Move(Vector2Int.up, 0, 1, 1, 1);

    public void MoveLeft(float _force = 0) => Move(Vector2Int.left, 1, 1, 0, 1);

    public void MoveDown(float _force = 0) => Move(Vector2Int.down, 0, 1, Grid.Height - 2, -1);

    public void MoveRight(float _force = 0) => Move(Vector2Int.right, Grid.Width - 2, -1, 0, 1);

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        if (isPaused)
            return;

        bool changed = false;
        for (int x = startX; x >= 0 && x < Grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < Grid.Height; y += incrementY)
            {
                GridCell cell = Grid.GetCell(x, y);

                if (cell.Occupied) {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed)
        {
            AudioController.Instance.PlayAudio(AudioName.TILES_MOVE);
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(GridTile tile, Vector2Int direction)
    {
        GridCell newCell = null;
        GridCell adjacent = Grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = Grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(GridTile a, GridTile b)
    {
        return a.info.Number == b.info.Number && !b.locked;
    }

    private void MergeTiles(GridTile a, GridTile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell, () =>
        {
            int index = Mathf.Clamp(IndexOf(b.info) + 1, 0, Gameboard.TileInfos.Count - 1);
            TileInfo newInfo = new TileInfo(); 
            newInfo.BackgroundSprite = Gameboard.TileInfos[index].BackgroundSprite;
            newInfo.TextColor = Gameboard.TileInfos[index].TextColor;
            newInfo.UserBackgroundSprite = Gameboard.TileInfos[index].UserBackgroundSprite;
            newInfo.Number = b.info.Number * 2;

            b.SetState(newInfo, Gameboard.TilesScaleAnimDuration); 
            GameManager.Instance.IncreaseScore(newInfo.Number);
            AudioController.Instance.PlayAudio(AudioName.TILES_MERGE);
        });
    }

    private int IndexOf(TileInfo state)
    {
        for (int i = 0; i < Gameboard.TileInfos.Count; i++)
        {
            if (state.Number == Gameboard.TileInfos[i].Number) {
                return i;
            }
        }

        return Gameboard.TileInfos.Count - 1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(Gameboard.TilesMoveAnimDuration);

        waiting = false;

        foreach (var tile in tiles) {
            tile.locked = false;
        }

        if (tiles.Count != Grid.Size) {
            CreateTile();
        }

        if (CheckForGameOver()) {
            GameManager.Instance.GameOver();
        }
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != Grid.Size) {
            return false;
        }

        foreach (var tile in tiles)
        {
            GridCell up = Grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            GridCell down = Grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            GridCell left = Grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            GridCell right = Grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }
        }

        return true;
    }

}

public class GridRow
{

    public List<GridCell> cells { get; private set; }

    public void AssignCell(GridCell _cell)
    {
        if (cells == null)
            cells = new List<GridCell>();

        cells.Add(_cell);
    }

}