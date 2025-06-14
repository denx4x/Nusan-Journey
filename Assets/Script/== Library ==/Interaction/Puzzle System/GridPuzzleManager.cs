using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; // Diperlukan untuk menggunakan 'event Action'

public class GridPuzzleManager : MonoBehaviour {
    [Header("Pengaturan Grid")]
    [Tooltip("Lebar grid dalam satuan kotak.")]
    public int gridWidth = 10;
    [Tooltip("Tinggi grid dalam satuan kotak.")]
    public int gridHeight = 10;
    [Tooltip("Ukuran setiap kotak di dunia game.")]
    public float tileSize = 1.0f;
    [Tooltip("Titik jangkar/awal untuk grid. Ini adalah posisi dunia yang akan dianggap sebagai grid (0,0).")]
    public Transform gridOrigin;

    [Header("Referensi Objek Puzzle")]
    [Tooltip("Masukkan objek Player (Patung) di sini.")]
    public GridObject playerObject;
    [Tooltip("Masukkan semua objek Obstacle (Pohon) di sini.")]
    public List<GridObject> obstacles;
    [Tooltip("Masukkan semua objek Wall (Blok Biru) di sini.")]
    public List<GridObject> walls;
    [Tooltip("Masukkan objek yang menandakan Target (Cahaya) di sini.")]
    public Transform targetTile;

    // Event untuk memberitahu sistem lain (seperti PuzzleModeController) bahwa puzzle sudah selesai
    public event Action OnPuzzleCompleted;

    // Variabel internal
    private GridObject[,] grid;
    private Vector2Int targetGridPosition;
    private bool isMoving = false;
    private bool puzzleSolved = false;
    private Dictionary<GridObject, Vector2Int> initialPositions;
    private GridObject selectedObject;

    void Start() {
        // Validasi penting: pastikan gridOrigin sudah di-assign
        if (gridOrigin == null) {
            Debug.LogError("PENTING: 'Grid Origin' belum di-assign di GridPuzzleManager! Buat Empty GameObject, posisikan di pojok puzzle, dan assign ke field ini.");
            this.enabled = false; // Nonaktifkan script jika setup gagal
            return;
        }
        InitializeGrid();
    }

    // Fungsi ini bisa dipanggil lagi dari luar jika ingin menginisialisasi ulang
    public void InitializeGrid() {
        grid = new GridObject[gridWidth, gridHeight];
        initialPositions = new Dictionary<GridObject, Vector2Int>();

        // 1. Tempatkan Walls (tidak perlu disimpan posisi awalnya karena statis)
        foreach (var wall in walls) {
            Vector2Int pos = WorldToGridPosition(wall.transform.position);
            PlaceObjectOnGrid(wall, pos);
            wall.transform.position = GridToWorldPosition(wall, pos);
        }

        // 2. Tempatkan Obstacles dan simpan posisi awalnya
        foreach (var obstacle in obstacles) {
            Vector2Int pos = WorldToGridPosition(obstacle.transform.position);
            PlaceObjectOnGrid(obstacle, pos);
            obstacle.transform.position = GridToWorldPosition(obstacle, pos);
            initialPositions[obstacle] = pos;
        }

        // 3. Tempatkan Player dan simpan posisi awalnya
        if (playerObject != null) {
            Vector2Int pos = WorldToGridPosition(playerObject.transform.position);
            PlaceObjectOnGrid(playerObject, pos);
            playerObject.transform.position = GridToWorldPosition(playerObject, pos);
            initialPositions[playerObject] = pos;
            SelectObject(playerObject); // Otomatis pilih Player di awal
        } else {
            Debug.LogError("Player Object belum di-assign!");
        }

        // 4. Catat posisi target dan snap juga posisinya agar pas
        if (targetTile != null) {
            targetGridPosition = WorldToGridPosition(targetTile.position);
            targetTile.position = GridToWorldPosition(null, targetGridPosition);
        } else {
            Debug.LogError("Target Tile belum di-assign!");
        }
    }

    void Update() {
        // Kunci input jika ada animasi bergerak, puzzle sudah selesai, atau script tidak aktif
        if (isMoving || puzzleSolved || !this.enabled) return;

        // Input untuk memilih objek dengan klik mouse
        if (Input.GetMouseButtonDown(0)) {
            HandleSelection();
        }

        // Input untuk mereset puzzle dengan tombol 'R'
        if (Input.GetKeyDown(KeyCode.R)) {
            ResetPuzzle();
            return; // Hentikan proses frame ini setelah reset
        }

        // Input untuk pergerakan objek yang sedang dipilih
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

    private void HandleSelection() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (hit.collider.TryGetComponent<GridObject>(out GridObject hitObject)) {
                if (hitObject.type == ObjectType.Player || hitObject.type == ObjectType.Obstacle) {
                    SelectObject(hitObject);
                }
            }
        }
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
        if (objectAtTarget == null) MoveObject(objectToMove, targetPos);
        else if (objectAtTarget.type == ObjectType.Obstacle && objectToMove.type == ObjectType.Player) {
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
        if (movedObject.type == ObjectType.Player) CheckWinCondition();
        isMoving = false;
    }

    private void CheckWinCondition() {
        if (playerObject.gridPosition == targetGridPosition) {
            Debug.Log("<color=green><b>SELAMAT! Puzzle Selesai!</b></color>");
            puzzleSolved = true;
            OnPuzzleCompleted?.Invoke(); // Panggil event
        }
    }

    public void ResetPuzzle() {
        if (isMoving) return;
        Debug.Log("Mereset puzzle...");
        grid = new GridObject[gridWidth, gridHeight];
        foreach (KeyValuePair<GridObject, Vector2Int> entry in initialPositions) {
            GridObject obj = entry.Key;
            Vector2Int initialPos = entry.Value;
            obj.transform.position = GridToWorldPosition(obj, initialPos);
            PlaceObjectOnGrid(obj, initialPos);
        }
        foreach (var wall in walls) {
            PlaceObjectOnGrid(wall, WorldToGridPosition(wall.transform.position));
        }
        puzzleSolved = false;
        isMoving = false;
        if (playerObject != null) SelectObject(playerObject);
    }

    // --- Helper Functions ---

    private bool IsInBounds(Vector2Int pos) { return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight; }
    private void PlaceObjectOnGrid(GridObject obj, Vector2Int pos) { if (IsInBounds(pos)) { if (grid[pos.x, pos.y] != null) Debug.LogWarning($"Posisi {pos} sudah terisi oleh {grid[pos.x, pos.y].name}, akan ditimpa oleh {obj.name}"); grid[pos.x, pos.y] = obj; obj.gridPosition = pos; } }
    public Vector2Int WorldToGridPosition(Vector3 worldPosition) { if (gridOrigin == null) return Vector2Int.zero; Vector3 relativePos = worldPosition - gridOrigin.position; int x = Mathf.RoundToInt(relativePos.x / tileSize); int y = Mathf.RoundToInt(relativePos.z / tileSize); return new Vector2Int(x, y); }
    public Vector3 GridToWorldPosition(GridObject obj, Vector2Int gridPosition) { if (gridOrigin == null) return Vector3.zero; Vector3 relativePos = new Vector3(gridPosition.x * tileSize, 0, gridPosition.y * tileSize); float yPos = (obj != null) ? obj.transform.position.y : gridOrigin.position.y; if (obj == null && targetTile != null) yPos = targetTile.position.y; return relativePos + new Vector3(gridOrigin.position.x, yPos, gridOrigin.position.z); }

    // --- Fungsi untuk Visualisasi di Editor ---
    private void OnDrawGizmos() {
        if (gridOrigin == null) {
            return;
        }
        Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                // Gunakan overload yang tidak memerlukan GridObject untuk menggambar grid kosong
                Vector3 cellCenter = GridToWorldPosition(null, new Vector2Int(x, y));
                Gizmos.DrawWireCube(cellCenter, new Vector3(tileSize, 0.1f, tileSize));
            }
        }
    }
}