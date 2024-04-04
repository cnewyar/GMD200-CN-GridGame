using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    public GridManager gridManager;
    public Vector2Int gridCoords;

    private SpriteRenderer _spriteRenderer;
    private Color _defaultColor;

    public Color tileColor { get { return _spriteRenderer.color; } } // New property to get the color of the tile

    private bool _isOccupied = false;

    public PlayerUnit playerUnit;

    private void OnMouseDown()
    {
        // Call the existing functionality (if any)
        // Example: Debug.Log("Tile clicked!");

        // Notify the PlayerUnit that this tile is touched
        if (playerUnit != null)
        {
            playerUnit.OnTileTouched(this);
        }
        else
        {
            Debug.LogWarning("PlayerUnit reference is not assigned.");
        }
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultColor = _spriteRenderer.color;
    }

    // Method to check if the tile is occupied
    public bool IsOccupied()
    {
        return _isOccupied;
    }

    // Method to occupy the tile
    public void OccupyTile()
    {
        _isOccupied = true;
    }

    // Method to free the tile
    public void FreeTile()
    {
        _isOccupied = false;
    }

    private void OnMouseOver()
    {
        // Tell the grid manager that this tile has been hovered
        gridManager.OnTileHoverEnter(this);
        // SetColor(Color.green);
    }

    private void OnMouseExit()
    {
        // Tell the grid manager that this tile has stopped being hovered
        gridManager.OnTileHoverExit(this);
        // ResetColor();
    }

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }

    public void ResetColor()
    {
        _spriteRenderer.color = _defaultColor;
    }
}