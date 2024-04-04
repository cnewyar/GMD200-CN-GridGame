using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerUnit : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private Color tileColor = Color.green; // Color to change the tile to

    private bool _isMoving = false;
    private Vector2Int _previousTileCoords;

    private HashSet<GridTile> touchedTiles = new HashSet<GridTile>();

    public void OnTileTouched(GridTile tile)
    {
        touchedTiles.Add(tile);
    }

    // Check if all tiles have been touched
    public bool IsGridComplete(int totalTiles)
    {
        return touchedTiles.Count == totalTiles;
    }

    private void Start()
    {
        // Position the player at the center of tile (0,0)
        transform.position = _gridManager.GetTile(new Vector2Int(0, 0)).transform.position;
        _previousTileCoords = new Vector2Int(0, 0);

        // Change the color of the (0,0) tile to green
        GridTile startTile = _gridManager.GetTile(new Vector2Int(0, 0));
        if (startTile != null)
        {
            startTile.SetColor(tileColor);
        }
    }

    private void Update()
    {
        if (!_isMoving && Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                GridTile clickedTile = hit.collider.GetComponent<GridTile>();

                if (clickedTile != null)
                {
                    Vector2Int targetTileCoords = clickedTile.gridCoords;

                    // Check if the target tile coordinates are valid for movement
                    if ((Mathf.Abs(targetTileCoords.x - _previousTileCoords.x) == 1 && Mathf.Abs(targetTileCoords.y - _previousTileCoords.y) == 0) ||
                        (Mathf.Abs(targetTileCoords.y - _previousTileCoords.y) == 1 && Mathf.Abs(targetTileCoords.x - _previousTileCoords.x) == 0))
                    {
                        AttemptMove(targetTileCoords);
                    }
                    else
                    {
                        Debug.Log("Invalid move. Movement canceled.");
                    }
                }
            }
        }
    }

    private void AttemptMove(Vector2Int targetTileCoords)
    {
        GridTile targetTile = _gridManager.GetTile(targetTileCoords);

        // Check if the target tile is valid and not occupied
        if (targetTile != null && !targetTile.IsOccupied())
        {
            // Start moving the player towards the target tile
            StartCoroutine(MoveToTile(targetTile.transform.position));
            // Occupy the target tile
            targetTile.OccupyTile();
            // Update previous tile coordinates
            _previousTileCoords = targetTileCoords;

            // Change the color of the tile to green
            targetTile.SetColor(tileColor);
        }
        else
        {
            Debug.Log("Target tile is invalid or occupied. Movement canceled.");
        }
    }

    private IEnumerator MoveToTile(Vector3 targetPosition)
    {
        _isMoving = true;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;
        _isMoving = false;
    }
}