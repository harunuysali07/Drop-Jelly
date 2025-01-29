using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class GridController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int gridSize = new(6, 6);

    [SerializeField] private float cellPadding = 0.1f;
    [SerializeField] private Cell cellPrefab;

    [Header("Cube Settings")]
    [SerializeField] private Transform cubeSpawnPoint;

    [SerializeField] private Cube cubePrefab;

    private LevelController _levelController;

    [Header("Debug Options")]
    [ShowInInspector, ReadOnly] private Cell[,] _grid;

    [ShowInInspector, ReadOnly] private List<Cell> _cells;

    [ShowInInspector, ReadOnly] private Cube _currentCube;
    [ShowInInspector, ReadOnly] private Vector2 _cubeMoveRange;

    public void Initialize()
    {
        _levelController = GameManager.Instance.levelManager.currentLevel;

        CreateGrid();
    }

    private void CreateGrid()
    {
        _grid = new Cell[gridSize.x, gridSize.y];
        _cells = new List<Cell>();

        var xOffSet = cellPadding * (gridSize.x - 1) / 2f;
        var zOffSet = cellPadding * (gridSize.y - 1) / 2f;

        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                var cell = Instantiate(cellPrefab, transform);
                cell.name = $"Cell [{x},{y}]";
                cell.transform.localPosition = new Vector3(x * cellPadding - xOffSet, 0,
                    (gridSize.y - 1 - y) * cellPadding - zOffSet);

                _grid[x, y] = cell;
                _cells.Add(cell);
                cell.gridPosition = new Vector2Int(x, y);

                if (y == 0)
                    cell.gameObject.SetActive(false);
            }
        }

        _cubeMoveRange = new Vector2(_grid[0, 0].transform.position.x,
            _grid[gridSize.x - 1, gridSize.y - 1].transform.position.x);

        SpawnBlock();
    }

    private void SpawnBlock()
    {
        if (_levelController.RemainingMoves <= 0)
            return;

        if (_currentCube != null)
            return;

        _currentCube = Instantiate(cubePrefab, cubeSpawnPoint.position, Quaternion.identity);
        _currentCube.Initialize(_levelController.GetNextColors());
    }

    public void UpdateBlockPosition(Vector3 position)
    {
        if (_levelController.RemainingMoves <= 0)
            return;

        if (_currentCube == null)
            return;

        var targetPosition = new Vector3(Mathf.Clamp(position.x, _cubeMoveRange.X, _cubeMoveRange.Y),
            cubeSpawnPoint.position.y, cubeSpawnPoint.position.z);

        _currentCube.transform.position = targetPosition;

        HighlightColumn(ClosestColumnIndex(_currentCube.transform.position));
    }

    public void DropBlock()
    {
        if (_levelController.RemainingMoves <= 0)
            return;

        if (_currentCube == null)
            return;

        _levelController.OnMove();
        HighlightColumn(-1);

        var targetCell = GetTargetCell(_currentCube.transform.position);

        if (targetCell == null)
            return;

        _currentCube.transform.position = new Vector3(targetCell.transform.position.x, cubeSpawnPoint.position.y,
            cubeSpawnPoint.position.z);
        _currentCube.transform.DOMove(targetCell.transform.position, 10f).SetSpeedBased(true).SetEase(Ease.OutCubic)
            .OnComplete(() => CheckForMatch(targetCell));

        targetCell.cube = _currentCube;
        _currentCube.name = targetCell.name;
        _currentCube = null;
    }

    private void CheckForMatch(Cell targetCell)
    {
        foreach (var comparisonType in (CubeComparisonType[])System.Enum.GetValues(typeof(CubeComparisonType)))
        {
            var neighbor = GetNeighbor(targetCell, comparisonType);
            if (neighbor == null || neighbor.cube == null)
                continue;

            var comparisonResult = targetCell.cube.CompareColors(neighbor.cube, comparisonType);

            print($"{targetCell.name} - {neighbor.name} : {comparisonResult.Count} - {comparisonType}");
        }


        targetCell.cube.ApplyMatch();

        SpawnBlock();
    }

    private Cell GetNeighbor(Cell cell, CubeComparisonType comparisonType)
    {
        var x = cell.gridPosition.x;
        var y = cell.gridPosition.y;

        return comparisonType switch
        {
            CubeComparisonType.Top => y > 0 ? _grid[x, y - 1] : null,
            CubeComparisonType.Bottom => y < gridSize.y - 1 ? _grid[x, y + 1] : null,
            CubeComparisonType.Left => x > 0 ? _grid[x - 1, y] : null,
            CubeComparisonType.Right => x < gridSize.x - 1 ? _grid[x + 1, y] : null,
            _ => throw new System.ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, null)
        };
    }

    private int ClosestColumnIndex(Vector3 position)
    {
        var closestColumnDistance = float.MaxValue;
        var closestColumnIndex = 0;

        for (var i = 0; i < gridSize.x; i++)
        {
            var distance = Mathf.Abs(_grid[i, 0].transform.position.x - position.x);

            if (distance < closestColumnDistance)
            {
                closestColumnDistance = distance;
                closestColumnIndex = i;
            }
        }

        return closestColumnIndex;
    }

    private Cell GetTargetCell(Vector3 position)
    {
        var closestColumnIndex = ClosestColumnIndex(position);

        for (var i = gridSize.y - 1; i >= 0; i--)
        {
            if (_grid[closestColumnIndex, i].cube == null)
            {
                return _grid[closestColumnIndex, i];
            }
        }

        //TODO: Game Over
        return null;
    }

    private int _lastHighlightedColumnIndex = -1;

    private void HighlightColumn(int columnIndex)
    {
        if (_lastHighlightedColumnIndex == columnIndex)
            return;

        foreach (var cell in _cells)
        {
            cell.SetHighlight(false);
        }

        _lastHighlightedColumnIndex = columnIndex;

        if (_lastHighlightedColumnIndex == -1)
            return;

        for (var i = 0; i < gridSize.y; i++)
        {
            _grid[columnIndex, i].SetHighlight(true);
        }

        HapticManager.GenerateHaptic(PresetType.MediumImpact);
    }

    public void OnCubeDestroyed(Cube cube)
    {
        StartCoroutine(MoveCubesDown(cube));
    }
    
    private IEnumerator MoveCubesDown(Cube cube)
    {
        var targetCell = _cells.FirstOrDefault(cell => cell.cube == cube);

        if (targetCell == null)
            yield break;
        
        yield return new WaitForSeconds(.5f);

        targetCell.cube = null;

        for (var i = targetCell.gridPosition.y; i >= 1; i--)
        {
            if (_grid[targetCell.gridPosition.x, i - 1].cube != null)
            {
                var cell = _grid[targetCell.gridPosition.x, i];
                cell.cube = _grid[targetCell.gridPosition.x, i - 1].cube;
                cell.cube.name = cell.name;
                _grid[targetCell.gridPosition.x, i - 1].cube = null;

                cell.cube.transform.DOMove(cell.transform.position, 10f)
                    .SetSpeedBased(true).SetEase(Ease.OutCubic).OnComplete(() => { CheckForMatch(cell); });
            }
        }
    }

    public void OnCubeUpdated(Cube cube)
    {
        var targetCell = _cells.FirstOrDefault(cell => cell.cube == cube);

        if (targetCell == null)
            return;

        CheckForMatch(targetCell);
    }
}

public enum CubeComparisonType
{
    Top = 1,
    Bottom = 2,
    Left = 3,
    Right = 4
}