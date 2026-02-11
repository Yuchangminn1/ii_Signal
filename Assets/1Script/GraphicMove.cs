using UnityEngine;
using UnityEngine.Events;

public class GraphicMove : MonoBehaviour
{
    public Vector2 startPos;
    public Vector2 targetPos;
    public float speed = 2f;

    public SequenceScript SequenceScript;

    public UnityEvent onMoveEnd;

    bool _isMoving = false;





    [Header("설정")]
    private float duration = 1.0f;

    // 1. 컴포넌트 캐싱
    private RectTransform _rectTransform;
    // 2. 트윈 객체 캐싱 (제어용)
    Vector2 velocity = Vector2.zero;
    float smoothTime = 0.3f;
    void Awake()
    {
        // 미리 참조를 캐싱하여 호출 비용 절감
        _rectTransform = GetComponent<RectTransform>();

        startPos = _rectTransform.localPosition;
    }

    void OnEnable()
    {
        if (_rectTransform != null)
        {
            _rectTransform.localPosition = startPos;
        }


    }

    void FixedUpdate()
    {
        if (_isMoving)

            _rectTransform.localPosition = Vector2.SmoothDamp(
                _rectTransform.localPosition,
                targetPos,
                ref velocity,
                smoothTime   // 0.2 ~ 0.4 추천
            );
        if (Vector2.Distance(_rectTransform.localPosition, targetPos) < 0.1f)
        {
            _rectTransform.localPosition = targetPos;
            _isMoving = false;
            SequenceScript?.TriggerOn();
            onMoveEnd?.Invoke();
        }

    }

    public void Reset()
    {
        if (_rectTransform != null)
            _rectTransform.localPosition = startPos;
    }
    public void MoveGraphic()
    {
        _isMoving = true;
    }

    public bool MoveGraphicBool()
    {
        if (Vector2.Distance(_rectTransform.localPosition, targetPos) < 0.1f)
            return false;
        _isMoving = true;
        return true;
    }

    // 오브젝트가 비활성화될 때 트윈 정리 (메모리 누수 방지)
    void OnDisable()
    {

    }
}
