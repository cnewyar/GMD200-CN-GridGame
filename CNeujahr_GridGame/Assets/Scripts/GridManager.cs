using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    public event Action<GridTile> TileSelected;

    public int numRows = 5;
    public int numColumns = 6;
    public float padding = 0.1f;
    public Color greenColor = Color.green;

    [SerializeField] private GridTile tilePrefab;
    [SerializeField] private TextMeshProUGUI text;
    private GridTile[] _tiles;
    private Vector2 _tileSize;
    public int totalTiles;

    private HashSet<GridTile> touchedTiles = new HashSet<GridTile>(); // Declare touchedTiles here

    private void Awake()
    {
        InitGrid();
    }

    public void UpdateGrid()
    {
        CheckGridCompletion();
    }

    // Reset the grid
    private void ResetGrid()
    {
        DestroyOldGrid();
        InitGrid();
    }

    public void InitGrid()
    {
        // Destroy the old grid before creating a new one
        DestroyOldGrid();

        // Initialize the tile size
        _tileSize = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;

        _tiles = new GridTile[numRows * numColumns];

        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numColumns; x++)
            {
                GridTile tile = Instantiate(tilePrefab, transform);
                Vector2 tilePos = new Vector2(x * (_tileSize.x + padding), y * (_tileSize.y + padding));
                tile.transform.localPosition = tilePos;
                tile.name = $"Tile_{x}_{y}";
                tile.gridManager = this;
                tile.gridCoords = new Vector2Int(x, y);
                _tiles[y * numColumns + x] = tile;

                // Add the tile at position (0,0) to touchedTiles initially
                if (x == 0 && y == 0)
                {
                    touchedTiles.Add(tile);
                }
            }
        }

        // Calculate total number of tiles
        totalTiles = numRows * numColumns;
    }

    private void DestroyOldGrid()
    {
        if (_tiles != null)
        {
            foreach (GridTile tile in _tiles)
            {
                if (tile != null)
                {
                    Destroy(tile.gameObject);
                }
            }
        }
    }

    public void CheckGridCompletion()
    {
        bool allGreen = true;

        foreach (GridTile tile in _tiles)
        {
            if (tile != null)
            {
                Debug.Log($"Tile color: {tile.tileColor}");
                if (!ColorsApproximatelyEqual(tile.tileColor, greenColor))
                {
                    allGreen = false;
                    break;
                }
            }
        }

        if (allGreen)
        {
            // All tiles are green, load the next scene
            Debug.Log("All tiles are green. Loading next scene...");
            LoadNextScene();
        }
        else
        {
            Debug.Log("Not all tiles are green.");
        }
    }

    private bool ColorsApproximatelyEqual(Color a, Color b)
    {
        float threshold = 0.01f; // Adjust as needed
        return Mathf.Abs(a.r - b.r) < threshold &&
               Mathf.Abs(a.g - b.g) < threshold &&
               Mathf.Abs(a.b - b.b) < threshold &&
               Mathf.Abs(a.a - b.a) < threshold;
    }

    public void OnTileHoverEnter(GridTile gridTile)
    {
        text.text = gridTile.gridCoords.ToString();
    }

    public void OnTileHoverExit(GridTile gridTile)
    {
        text.text = "---";
    }

    public void OnTileSelected(GridTile gridTile)
    {
        TileSelected?.Invoke(gridTile);
    }

    public GridTile GetTile(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= numColumns || pos.y < 0 || pos.y >= numRows)
        {
            Debug.LogError($"Invalid Coordinate{pos}");
            return null;
        }
        return _tiles[pos.y * numColumns + pos.x];
    }

    public Vector2Int GetGridCoordinates(Vector3 worldPosition)
    {
        Vector2Int gridCoordinates = new Vector2Int(
            Mathf.FloorToInt((worldPosition.x + _tileSize.x / 2) / (_tileSize.x + padding)),
            Mathf.FloorToInt((worldPosition.y + _tileSize.y / 2) / (_tileSize.y + padding))
        );

        return gridCoordinates;
    }

    private void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    // Method to handle tile touched by player
    public void OnTileTouched(GridTile gridTile)
    {
        touchedTiles.Add(gridTile);

        // Check if all tiles have been touched
        if (touchedTiles.Count == totalTiles)
        {
            Debug.Log("All tiles touched!");
            // Load the next scene
            LoadNextScene();
        }
    }
}
