using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public bool IsReadyToMatch => gameObject.activeSelf && Time.time > _animationEndTime;
    
    [SerializeField] private GameObject cubeObject;

    [Header("Transform References")]
    [SerializeField] private Transform cube2X2;

    [SerializeField] private Transform cube2X1Left;
    [SerializeField] private Transform cube2X1Right;
    [SerializeField] private Transform cube2X1Top;
    [SerializeField] private Transform cube2X1Bottom;
    [SerializeField] private Transform cube1X1TopLeft;
    [SerializeField] private Transform cube1X1TopRight;
    [SerializeField] private Transform cube1X1BottomLeft;
    [SerializeField] private Transform cube1X1BottomRight;

    private Transform _graphicsParent;
    private float _animationEndTime;

    [Header("Debug Options")]
    [ShowInInspector] private ColorData[] _activeColors;

    [ShowInInspector] private GameObject[] _activeCubes;

    public void Initialize(ColorData[] colors)
    {
        if (colors.Length != 4)
        {
            throw new FormatException("Colors array must have 4 elements.");
        }

        _activeColors = new[] { colors[0], colors[1], colors[2], colors[3] };

        _graphicsParent = cubeObject.transform.parent;

        _activeCubes = new GameObject[4];

        switch (colors.Distinct().Count())
        {
            case 1:
                _activeCubes[0] = CreateCube(cube2X2, colors[0].color);
                _activeCubes[1] = _activeCubes[0];
                _activeCubes[2] = _activeCubes[0];
                _activeCubes[3] = _activeCubes[0];
                break;
            case 2:
                if (colors[0] == colors[1] && colors[2] == colors[3])
                {
                    _activeCubes[0] = CreateCube(cube2X1Top, colors[0].color);
                    _activeCubes[1] = _activeCubes[0];
                    _activeCubes[2] = CreateCube(cube2X1Bottom, colors[2].color);
                    _activeCubes[3] = _activeCubes[2];
                }
                else if (colors[0] == colors[2] && colors[1] == colors[3])
                {
                    _activeCubes[0] = CreateCube(cube2X1Left, colors[0].color);
                    _activeCubes[1] = CreateCube(cube2X1Right, colors[1].color);
                    _activeCubes[2] = _activeCubes[0];
                    _activeCubes[3] = _activeCubes[1];
                }
                else if (colors[0] == colors[1] && (colors[0] == colors[2] || colors[0] == colors[3]))
                {
                    _activeCubes[0] = CreateCube(cube2X1Top, colors[0].color);
                    _activeCubes[1] = _activeCubes[0];
                    _activeCubes[2] = CreateCube(cube1X1BottomLeft, colors[2].color);
                    _activeCubes[3] = CreateCube(cube1X1BottomRight, colors[3].color);
                }
                else if (colors[2] == colors[3] && (colors[1] == colors[2] || colors[0] == colors[2]))
                {
                    _activeCubes[0] = CreateCube(cube1X1TopLeft, colors[0].color);
                    _activeCubes[1] = CreateCube(cube1X1TopRight, colors[1].color);
                    _activeCubes[2] = CreateCube(cube2X1Bottom, colors[2].color);
                    _activeCubes[3] = _activeCubes[2];
                }
                else
                {
                    // Like : 0,1,1,0
                    goto case 4;
                }

                break;
            case 3:
                if (colors[0] == colors[1])
                {
                    _activeCubes[0] = CreateCube(cube2X1Top, colors[0].color);
                    _activeCubes[1] = _activeCubes[0];
                    _activeCubes[2] = CreateCube(cube1X1BottomLeft, colors[2].color);
                    _activeCubes[3] = CreateCube(cube1X1BottomRight, colors[3].color);
                }
                else if (colors[0] == colors[2])
                {
                    _activeCubes[0] = CreateCube(cube2X1Left, colors[0].color);
                    _activeCubes[1] = CreateCube(cube1X1TopRight, colors[1].color);
                    _activeCubes[2] = _activeCubes[0];
                    _activeCubes[3] = CreateCube(cube1X1BottomRight, colors[3].color);
                }
                else if (colors[1] == colors[3])
                {
                    _activeCubes[0] = CreateCube(cube1X1TopLeft, colors[0].color);
                    _activeCubes[1] = CreateCube(cube2X1Right, colors[1].color);
                    _activeCubes[2] = CreateCube(cube1X1BottomLeft, colors[2].color);
                    _activeCubes[3] = _activeCubes[1];
                }
                else if (colors[2] == colors[3])
                {
                    _activeCubes[0] = CreateCube(cube1X1TopLeft, colors[0].color);
                    _activeCubes[1] = CreateCube(cube1X1TopRight, colors[1].color);
                    _activeCubes[2] = CreateCube(cube2X1Bottom, colors[2].color);
                    _activeCubes[3] = _activeCubes[2];
                }
                else
                {
                    // Like : 0,1,1,2
                    goto case 4;
                }

                break;
            case 4:
                _activeCubes[0] = CreateCube(cube1X1TopLeft, colors[0].color);
                _activeCubes[1] = CreateCube(cube1X1TopRight, colors[1].color);
                _activeCubes[2] = CreateCube(cube1X1BottomLeft, colors[2].color);
                _activeCubes[3] = CreateCube(cube1X1BottomRight, colors[3].color);

                break;
        }
    }

    //TODO: convert tuple to class or struct
    [ShowInInspector, ReadOnly] private readonly List<Tuple<int, Cube, int>> _matchPairs = new();
    [ShowInInspector, ReadOnly] private readonly List<Tuple<int, Cube, int>> _matchedPairs = new();

    public List<ColorData> CompareColors(Cube cube, CubeComparisonType comparisonType)
    {
        var otherColors = cube._activeColors;

        if (otherColors.Length != 4)
        {
            throw new FormatException("Colors array must have 4 elements.");
        }

        var result = new List<ColorData>();

        switch (comparisonType)
        {
            case CubeComparisonType.Top:
                if (_activeColors[0] == otherColors[2])
                {
                    result.Add(_activeColors[0]);
                    _matchPairs.Add(new Tuple<int, Cube, int>(0, cube, 2));
                }

                if (_activeColors[1] == otherColors[3])
                {
                    result.Add(_activeColors[1]);
                    _matchPairs.Add(new Tuple<int, Cube, int>(1, cube, 3));
                }

                break;
            case CubeComparisonType.Bottom:
                if (_activeColors[2] == otherColors[0])
                {
                    result.Add(_activeColors[2]);
                    _matchPairs.Add(new Tuple<int, Cube, int>(2, cube, 0));
                }

                if (_activeColors[3] == otherColors[1])
                {
                    result.Add(_activeColors[3]);
                    _matchPairs.Add(new Tuple<int, Cube, int>(3, cube, 1));
                }

                break;
            case CubeComparisonType.Left:
                if (_activeColors[0] == otherColors[1])
                {
                    result.Add(_activeColors[0]);
                    _matchPairs.Add(new Tuple<int, Cube, int>(0, cube, 1));
                }

                if (_activeColors[2] == otherColors[3])
                {
                    result.Add(_activeColors[2]);
                    _matchPairs.Add(new Tuple<int, Cube, int>(2, cube, 3));
                }

                break;
            case CubeComparisonType.Right:
                if (_activeColors[1] == otherColors[0])
                {
                    result.Add(_activeColors[1]);
                    _matchPairs.Add(new Tuple<int, Cube, int>(1, cube, 0));
                }

                if (_activeColors[3] == otherColors[2])
                {
                    result.Add(_activeColors[3]);
                    _matchPairs.Add(new Tuple<int, Cube, int>(3, cube, 2));
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, null);
        }

        return result;
    }

    public void ApplyMatch()
    {
        if (_matchPairs.Count > 0)
        {
            UpdateCubes(_matchPairs.Select(x => x.Item1).ToList());

            var targetCubes = _matchPairs.Select(x => x.Item2).Distinct().ToList();
            targetCubes.ForEach(x => x.UpdateCubes(_matchPairs.Where(y => y.Item2 == x).Select(z => z.Item3).ToList()));
        }

        _matchedPairs.AddRange(_matchPairs);
        _matchPairs.Clear();
    }

    private void UpdateCubes(List<int> matchPairs)
    {
        foreach (var index in matchPairs)
        {
            var targetToDestroy =  _activeCubes[index];
            var targetColor = _activeColors[index];

            for (var i = 0; i < 4; i++)
            {
                if (_activeCubes[i] == targetToDestroy)
                {
                    _activeCubes[i] = null;
                    _activeColors[i] = null;
                }
            }

            if (targetToDestroy == null)
                continue;

            targetToDestroy.transform.DOScale(Vector3.zero, .2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(targetToDestroy));
            
            GameManager.Instance.levelManager.currentLevel.OnMatch(targetColor);
        }

        if (_activeCubes.All(x => x == null))
        {
            GameManager.Instance.levelManager.currentLevel.gridController.OnCubeDestroyed(this);

            transform.DOScale(Vector3.zero, .2f)
                .SetEase(Ease.InBack)
                .OnComplete(() => gameObject.SetActive(false));
            // Destroy(gameObject);

            return;
        }

        UpdateData();
    }

    [Button]
    private void UpdateData()
    {
        if (_activeCubes[0] != null && _activeCubes[1] == null && _activeCubes[2] == null &&
            _activeCubes[3] == null)
        {
            _activeCubes[1] = UpdateCube(_activeCubes[0], cube2X2);
            _activeCubes[2] = _activeCubes[0];
            _activeCubes[3] = _activeCubes[0];

            _activeColors[1] = _activeColors[0];
            _activeColors[2] = _activeColors[0];
            _activeColors[3] = _activeColors[0];
        }
        else if (_activeCubes[1] != null && _activeCubes[0] == null && _activeCubes[2] == null &&
                 _activeCubes[3] == null)
        {
            _activeCubes[0] = UpdateCube(_activeCubes[1], cube2X2);
            _activeCubes[2] = _activeCubes[1];
            _activeCubes[3] = _activeCubes[1];

            _activeColors[0] = _activeColors[1];
            _activeColors[2] = _activeColors[1];
            _activeColors[3] = _activeColors[1];
        }
        else if (_activeCubes[2] != null && _activeCubes[0] == null && _activeCubes[1] == null &&
                 _activeCubes[3] == null)
        {
            _activeCubes[0] = UpdateCube(_activeCubes[2], cube2X2);
            _activeCubes[1] = _activeCubes[2];
            _activeCubes[3] = _activeCubes[2];

            _activeColors[0] = _activeColors[2];
            _activeColors[1] = _activeColors[2];
            _activeColors[3] = _activeColors[2];
        }
        else if (_activeCubes[3] != null && _activeCubes[0] == null && _activeCubes[1] == null &&
                 _activeCubes[2] == null)
        {
            _activeCubes[0] = UpdateCube(_activeCubes[3], cube2X2);
            _activeCubes[1] = _activeCubes[3];
            _activeCubes[2] = _activeCubes[3];

            _activeColors[0] = _activeColors[3];
            _activeColors[1] = _activeColors[3];
            _activeColors[2] = _activeColors[3];
        }

        if (_activeCubes[0] == _activeCubes[1] && _activeCubes[2] == null && _activeCubes[3] == null)
        {
            _activeCubes[2] = UpdateCube(_activeCubes[0], cube2X2);
            _activeCubes[3] = _activeCubes[0];

            _activeColors[2] = _activeColors[0];
            _activeColors[3] = _activeColors[0];
        }
        else if (_activeCubes[0] == _activeCubes[2] && _activeCubes[1] == null && _activeCubes[3] == null)
        {
            _activeCubes[1] = UpdateCube(_activeCubes[0], cube2X2);
            _activeCubes[3] = _activeCubes[0];

            _activeColors[1] = _activeColors[0];
            _activeColors[3] = _activeColors[0];
        }
        else if (_activeCubes[1] == _activeCubes[3] && _activeCubes[0] == null && _activeCubes[2] == null)
        {
            _activeCubes[0] = UpdateCube(_activeCubes[1], cube2X2);
            _activeCubes[2] = _activeCubes[1];

            _activeColors[0] = _activeColors[1];
            _activeColors[2] = _activeColors[1];
        }
        else if (_activeCubes[2] == _activeCubes[3] && _activeCubes[0] == null && _activeCubes[1] == null)
        {
            _activeCubes[0] = UpdateCube(_activeCubes[2], cube2X2);
            _activeCubes[1] = _activeCubes[2];

            _activeColors[0] = _activeColors[2];
            _activeColors[1] = _activeColors[2];
        }

        if (_activeCubes[0] != null && _activeCubes[1] == null &&
            (_activeCubes[2] != null || _activeCubes[3] != null) && _activeCubes[0] != _activeCubes[2] &&
            _activeCubes[0] != _activeCubes[3])
        {
            _activeCubes[1] = UpdateCube(_activeCubes[0], cube2X1Top);
            _activeColors[1] = _activeColors[0];
        }

        if (_activeCubes[0] != null && _activeCubes[2] == null &&
            (_activeCubes[1] != null || _activeCubes[3] != null) && _activeCubes[0] != _activeCubes[1] &&
            _activeCubes[0] != _activeCubes[3])
        {
            _activeCubes[2] = UpdateCube(_activeCubes[0], cube2X1Left);
            _activeColors[2] = _activeColors[0];
        }

        if (_activeCubes[1] != null && _activeCubes[0] == null &&
            (_activeCubes[2] != null || _activeCubes[3] != null) && _activeCubes[1] != _activeCubes[2] &&
            _activeCubes[1] != _activeCubes[3])
        {
            _activeCubes[0] = UpdateCube(_activeCubes[1], cube2X1Top);
            _activeColors[0] = _activeColors[1];
        }

        if (_activeCubes[1] != null && _activeCubes[3] == null &&
            (_activeCubes[0] != null || _activeCubes[2] != null) && _activeCubes[1] != _activeCubes[0] &&
            _activeCubes[1] != _activeCubes[2])
        {
            _activeCubes[3] = UpdateCube(_activeCubes[1], cube2X1Right);
            _activeColors[3] = _activeColors[1];
        }

        if (_activeCubes[2] != null && _activeCubes[3] == null &&
            (_activeCubes[0] != null || _activeCubes[1] != null) && _activeCubes[2] != _activeCubes[0] &&
            _activeCubes[2] != _activeCubes[1])
        {
            _activeCubes[3] = UpdateCube(_activeCubes[2], cube2X1Bottom);
            _activeColors[3] = _activeColors[2];
        }

        if (_activeCubes[2] != null && _activeCubes[0] == null &&
            (_activeCubes[1] != null || _activeCubes[3] != null) && _activeCubes[2] != _activeCubes[1] &&
            _activeCubes[2] != _activeCubes[3])
        {
            _activeCubes[0] = UpdateCube(_activeCubes[2], cube2X1Left);
            _activeColors[0] = _activeColors[2];
        }

        if (_activeCubes[3] != null && _activeCubes[2] == null &&
            (_activeCubes[0] != null || _activeCubes[1] != null) && _activeCubes[3] != _activeCubes[0] &&
            _activeCubes[3] != _activeCubes[1])
        {
            _activeCubes[2] = UpdateCube(_activeCubes[3], cube2X1Bottom);
            _activeColors[2] = _activeColors[3];
        }

        if (_activeCubes[3] != null && _activeCubes[1] == null &&
            (_activeCubes[0] != null || _activeCubes[2] != null) && _activeCubes[3] != _activeCubes[0] &&
            _activeCubes[3] != _activeCubes[2])
        {
            _activeCubes[1] = UpdateCube(_activeCubes[3], cube2X1Right);
            _activeColors[1] = _activeColors[3];
        }

        GameManager.Instance.levelManager.currentLevel.gridController.OnCubeUpdated(this);
    }

    private GameObject CreateCube(Transform targetTransform, Color color)
    {
        var cube = Instantiate(cubeObject, _graphicsParent);
        cube.transform.localPosition = targetTransform.localPosition;
        cube.transform.localRotation = targetTransform.localRotation;

        cube.transform.localScale = Vector3.zero;
        cube.transform.DOScale(targetTransform.localScale, .25f).SetEase(Ease.OutBack);

        cube.gameObject.SetActive(true);
        cube.GetComponent<Renderer>().material.color = color;

        cube.name = targetTransform.name;

        _animationEndTime = Time.time + .2f;
        
        return cube;
    }

    private GameObject UpdateCube(GameObject cube, Transform targetTransform)
    {
        cube.transform.DOKill();
        cube.transform.DOLocalMove(targetTransform.localPosition, .2f).SetEase(Ease.OutBack).SetDelay(.25f);
        cube.transform.DOLocalRotateQuaternion(targetTransform.localRotation, .2f).SetEase(Ease.OutBack).SetDelay(.25f);
        cube.transform.DOScale(targetTransform.localScale, .2f).SetEase(Ease.OutBack).SetDelay(.25f);

        cube.name = targetTransform.name;

        _animationEndTime = Time.time + .5f;

        return cube;
    }
    
    public void SetReadyToMatch(bool state)
    {
        _animationEndTime = state ? 0 : Time.time + 10f;
    }
}