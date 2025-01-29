using UnityEngine;

public class TouchController : MonoBehaviour
{
    private GridController _gridController;
    
    private void Start()
    {
        _gridController = GameManager.Instance.levelManager.currentLevel.gridController;
        
        GameManager.Instance.touchManager.OnTouchBegin += OnTouchBegin;
        GameManager.Instance.touchManager.OnTouchMoveWorld += OnTouchMove;
        GameManager.Instance.touchManager.OnTouchEnd += OnTouchEnd;
    }

    private void OnTouchBegin(Vector2 position)
    {
        GameManager.Instance.touchManager.TouchDistance = Camera.main.transform.position.magnitude;
    }
    
    private void OnTouchMove(Vector3 position)
    {
        _gridController.UpdateBlockPosition(position);
    }
    
    private void OnTouchEnd(Vector2 position)
    {
        _gridController.DropBlock();
    } 
}