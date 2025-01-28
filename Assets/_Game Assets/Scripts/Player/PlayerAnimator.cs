using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerController _playerController;
    private Animator _animator;
    
    public PlayerAnimator Initialize(PlayerController playerController)
    {
        _playerController = playerController;
        
        _animator = GetComponent<Animator>();

        return this;
    }

    private void Update()
    {
        UpdateBlendSpeed();
    }

    private Vector3 _lastPosition;
    private float _blendTreeSpeed = 0;
    private void UpdateBlendSpeed()
    {
        var position = transform.position;
        
        _blendTreeSpeed = Mathf.Lerp(_blendTreeSpeed, Mathf.Clamp01((_lastPosition - position).magnitude * 10f), 10 * Time.deltaTime);
        _animator.SetFloat(AnimatorParameterKey.Speed, _blendTreeSpeed);
        _lastPosition = position;
    }
    
    private struct AnimatorParameterKey
    {
        public static readonly int Speed = Animator.StringToHash("Speed");
    }
}
