using UnityEngine;
using Random = UnityEngine.Random;

public class BatUIFly : MonoBehaviour
{
    [SerializeField] private RectTransform spriteTransform;
    [SerializeField] private float speed;
    private Animator _animator;
    private Camera _cam;
    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;
    private Vector2 _direction;
    private Vector2 _startPosition;
    private float _delay;


    private void Awake()
    {
        _cam = Camera.main;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        var rect = spriteTransform.rect;
        _minX = _cam.pixelWidth / -2f - rect.width / 2f;
        _minY = _cam.pixelHeight / -2f - rect.height / 2f;
        _maxX = _cam.pixelWidth / 2f + rect.width / 2f; 
        _maxY = _cam.pixelHeight / 2f + rect.height / 2f;
        _delay = Random.Range(1f, 5f);
        SetStartPosition();
    }

    private void Update()
    {
        if (_delay >= -1f)
            _delay -= Time.deltaTime;
        
        if (_delay < 0f)
            spriteTransform.anchoredPosition += _direction * speed * Time.deltaTime;
        
        if (OutOfLimits())
        {
            SetStartPosition();
        }
    }

    private void LateUpdate()
    {
        _animator.SetFloat("Delay", _delay);
    }

    private bool OutOfLimits()
    {
        var position = spriteTransform.anchoredPosition;
        return (position.x < _minX || position.x > _maxX || position.y < _minY || position.y > _maxY);
    }
    
    private void SetStartPosition()
    {
        _direction = new Vector2(Random.Range(-2f, 2f), Random.Range(-1f, 1f)).normalized;
        spriteTransform.localEulerAngles = Vector3.zero;
        if (_direction.x > 0)
            spriteTransform.localEulerAngles = new Vector3(0f, 180f, 0f);

        _startPosition = new Vector2(_direction.x > 0 ? _minX : _maxX,
            _direction.y > 0 ? _direction.y * _minY : _direction.y * _maxY);

        spriteTransform.anchoredPosition = _startPosition;
    }
}
