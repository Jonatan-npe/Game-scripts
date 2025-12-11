using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float scaleMove;

    private PlayerInput playerInput;
    private MainPlayer mainPlayer;
    private Vector2 originalPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        mainPlayer = GetComponentInParent<MainPlayer>();
        originalPosition = transform.localPosition;

        // Events subscription
        mainPlayer.OnDirectionChanged += OnPlayerDirectionChanged;
    }

    void OnDestroy()
    {
        // Events unsubscription
        if (mainPlayer != null)
            mainPlayer.OnDirectionChanged -= OnPlayerDirectionChanged;
    }

    private void OnPlayerDirectionChanged(float direction)
    {
        // It should move the camera from the 0 to originalPosition.x
        transform.DOMove(new Vector2(0, transform.localPosition.y), 0.25f).SetEase(Ease.Linear);
        transform.DOMove(new Vector2(originalPosition.x, transform.localPosition.y), 0.5f).SetEase(Ease.Linear);
    }
    // Update is called once per frame
    void Update()
    {
        if (playerInput.actions["Look"].ReadValue<Vector2>().y != 0)
        {
            transform.localPosition = new Vector2(originalPosition.x, originalPosition.y + (playerInput.actions["Look"].ReadValue<Vector2>().y*scaleMove));
        }
    }
}
