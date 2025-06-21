using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class GridPuzzleManager : MonoBehaviour {
    [Header("Pengaturan Grid")]
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float tileSize = 1.0f;
    public Transform gridOrigin;

    [Header("Referensi Objek Puzzle")]
    public GridObject playerObject;
    public List<GridObject> obstacles;
    public List<GridObject> walls;
    public Transform targetTile;
    
    [Tooltip("Event ini untuk komunikasi antar-skrip (C# Action). Tidak terlihat di Inspector.")]
    public event Action OnPuzzleCompleted;

    // Variabel internal
    private GridObject[,] grid;
    private Vector2Int targetGridPosition;
    private bool isMoving = false;
    public bool IsSolved { get; private set; } = false;
    private Dictionary<GridObject, Vector2Int> initialPositions;
    private GridObject selectedObject;
    private List<GridObject> selectableObjects;
    private int selectedObjectIndex = -1;

    void Start() {
        if (gridOrigin == null) {
            Debug.LogError("PENTING: 'Grid Origin' belum di-assign!");
            this.enabled = false;
            return;
        }
        InitializeGrid();
    }

    public void InitializeGrid() {
        grid = new GridObject[gridWidth, gridHeight];
        initialPositions = new Dictionary<GridObject, Vector2Int>();
        selectableObjects = new List<GridObject>();

        foreach (var wall in walls) {
            Vector2Int pos = WorldToGridPosition(wall.transform.position);
            PlaceObjectOnGrid(wall, pos);
            wall.transform.position = GridToWorldPosition(wall, pos);
        }

        if (playerObject != null) selectableObjects.Add(playerObject);
        if (obstacles != null && obstacles.Count > 0) selectableObjects.AddRange(obstacles);

        foreach (var selectable in selectableObjects) {
            Vector2Int pos = WorldToGridPosition(selectable.transform.position);
            PlaceObjectOnGrid(selectable, pos);
            selectable.transform.position = GridToWorldPosition(selectable, pos);
            initialPositions[selectable] = pos;
        }

        if (targetTile != null) {
            targetGridPosition = WorldToGridPosition(targetTile.position);
            targetTile.position = GridToWorldPosition(null, targetGridPosition);
        }

        CheckWinCondition();
        if (IsSolved) Debug.LogWarning("PERINGATAN: Puzzle sudah selesai saat dimulai! Posisi awal Player sama dengan Target.");

        if (playerObject != null) {
            selectedObjectIndex = selectableObjects.IndexOf(playerObject);
            SelectObject(playerObject);
        } else if (selectableObjects.Count > 0) {
            selectedObjectIndex = 0;
            SelectObject(selectableObjects[0]);
        }
    }

    void Update() {
        if (isMoving || IsSolved || !this.enabled) return;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("Tombol Escape ditekan, keluar dan mereset puzzle...");
            ResetPuzzle();
            PuzzleModeController.Instance.EndPuzzle();
            return;
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            SelectNextObject();
        } else if (Input.GetKeyDown(KeyCode.Q)) {
            SelectPreviousObject();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            ResetPuzzle();
            return;
        }

        if (selectedObject != null) {
            Vector2Int direction = Vector2Int.zero;
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) direction = Vector2Int.up;
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) direction = Vector2Int.down;
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) direction = Vector2Int.left;
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) direction = Vector2Int.right;

            if (direction != Vector2Int.zero) {
                TryMoveObject(selectedObject, direction);
            }
        }
    }

    private void SelectNextObject() {
        if (selectableObjects == null || selectableObjects.Count == 0) return;
        selectedObjectIndex = (selectedObjectIndex + 1) % selectableObjects.Count;
        SelectObject(selectableObjects[selectedObjectIndex]);
    }

    private void SelectPreviousObject() {
        if (selectableObjects == null || selectableObjects.Count == 0) return;
        selectedObjectIndex--;
        if (selectedObjectIndex < 0) {
            selectedObjectIndex = selectableObjects.Count - 1;
        }
        SelectObject(selectableObjects[selectedObjectIndex]);
    }

    private void SelectObject(GridObject objectToSelect) {
        if (selectedObject == objectToSelect) return;
        if (selectedObject != null) {
            selectedObject.Deselect();
        }
        selectedObject = objectToSelect;
        selectedObject.Select();
    }

    private void TryMoveObject(GridObject objectToMove, Vector2Int direction) {
        if (objectToMove == null) return;
        Vector2Int targetPos = objectToMove.gridPosition + direction;
        if (!IsInBounds(targetPos)) return;
        GridObject objectAtTarget = grid[targetPos.x, targetPos.y];
        if (objectAtTarget == null) {
            MoveObject(objectToMove, targetPos);
        } else if (objectAtTarget.type == ObjectType.Obstacle && objectToMove.type == ObjectType.Player) {
            TryMoveObject(objectAtTarget, direction);
        }
    }

    private void MoveObject(GridObject objectToMove, Vector2Int newGridPos) {
        isMoving = true;
        grid[objectToMove.gridPosition.x, objectToMove.gridPosition.y] = null;
        grid[newGridPos.x, newGridPos.y] = objectToMove;
        objectToMove.gridPosition = newGridPos;
        Vector3 newWorldPos = GridToWorldPosition(objectToMove, newGridPos);
        StartCoroutine(HandleMovementAnimation(objectToMove, newWorldPos));
    }

    private IEnumerator HandleMovementAnimation(GridObject movedObject, Vector3 targetWorldPos) {
        yield return StartCoroutine(movedObject.MoveToPosition(targetWorldPos, 0.2f));
        isMoving = false;
        if (movedObject.type == ObjectType.Player) CheckWinCondition();
    }

    private void CheckWinCondition() {
        if (!IsSolved && playerObject != null && targetTile != null && playerObject.gridPosition == targetGridPosition) {
            Debug.Log("<color=green><b>SELAMAT! Puzzle Selesai!</b></color>");
            IsSolved = true;

            OnPuzzleCompleted?.Invoke();
        }
    }

    public void ResetPuzzle() {
        if (isMoving) return;
        Debug.Log("Mereset puzzle...");
        grid = new GridObject[gridWidth, gridHeight];

        if (selectedObject != null) selectedObject.Deselect();

        foreach (KeyValuePair<GridObject, Vector2Int> entry in initialPositions) {
            GridObject obj = entry.Key;
            Vector2Int initialPos = entry.Value;
            obj.transform.position = GridToWorldPosition(obj, initialPos);
            PlaceObjectOnGrid(obj, initialPos);
        }

        foreach (var wall in walls) {
            PlaceObjectOnGrid(wall, WorldToGridPosition(wall.transform.position));
        }

        IsSolved = false;
        isMoving = false;

        if (playerObject != null && selectableObjects.Contains(playerObject)) {
            selectedObjectIndex = selectableObjects.IndexOf(playerObject);
            SelectObject(playerObject);
        } else if (selectableObjects.Count > 0) {
            selectedObjectIndex = 0;
            SelectObject(selectableObjects[0]);
        }
    }

    // --- BAGIAN INI TELAH DIUBAH FORMATNYA SESUAI PERMINTAAN ---

    private bool IsInBounds(Vector2Int pos) {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }

    private void PlaceObjectOnGrid(GridObject obj, Vector2Int pos) {
        if (IsInBounds(pos)) {
            if (grid[pos.x, pos.y] != null && grid[pos.x, pos.y] != obj) {
                Debug.LogWarning($"Posisi {pos} sudah terisi oleh {grid[pos.x, pos.y].name}, akan ditimpa oleh {obj.name}");
            }
            grid[pos.x, pos.y] = obj;
            obj.gridPosition = pos;
        }
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition) {
        if (gridOrigin == null) {
            return Vector2Int.zero;
        }
        Vector3 relativePos = worldPosition - gridOrigin.position;
        int x = Mathf.RoundToInt(relativePos.x / tileSize);
        int y = Mathf.RoundToInt(relativePos.z / tileSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorldPosition(GridObject obj, Vector2Int gridPosition) {
        if (gridOrigin == null) {
            return Vector3.zero;
        }
        Vector3 relativePos = new Vector3(gridPosition.x * tileSize, 0, gridPosition.y * tileSize);
        float yPos = (obj != null) ? obj.transform.position.y : gridOrigin.position.y;
        if (obj == null && targetTile != null) {
            yPos = targetTile.position.y;
        }
        return relativePos + new Vector3(gridOrigin.position.x, yPos, gridOrigin.position.z);
    }

    private void OnDrawGizmos() {
        if (gridOrigin == null) {
            return;
        }
        Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                Vector3 cellCenter = GridToWorldPosition(null, new Vector2Int(x, y));
                Gizmos.DrawWireCube(cellCenter, new Vector3(tileSize, 0.1f, tileSize));
            }
        }
    }
}