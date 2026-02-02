using UnityEngine;
using System.Collections;

/// <summary>
/// Controla la lógica de movimiento del jugador.
/// La lógica de combate está centralizada en CombatPlayer.
/// </summary>
public class MainPlayer : MonoBehaviour
{
    // Eventos y declaraciones necesarias para los eventos de dirección
    public event System.Action<float> OnDirectionChanged;
    private float lastDirection = 1f;

    // Componentes internos del jugador
    private SpriteRenderer spriteRenderer;

    private Animator animator;
    private MovementPlayer movementPlayerScript;
    private Transform playerTransform;
    public Transform PlayerTransform
    {
        get { return playerTransform; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        movementPlayerScript = GetComponent<MovementPlayer>();
        playerTransform = GetComponent<Transform>();
    }

    public void DisableMovement()
    {
        movementPlayerScript.CanMove = false;
    }

    public void EnableMovement()
    {
        movementPlayerScript.CanMove = true;
    }

    public void NotifyDirectionChange(float newDirection)
    {
        if (newDirection != lastDirection)
        {
            lastDirection = newDirection;
            OnDirectionChanged?.Invoke(newDirection);
        }
    }
    public IEnumerator FrameWaiter(float duration)
    {
        for (int i = 0; i < duration; i++)
        {
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator InvulnerabilityFrames(float duration, CapsuleCollider2D colliderIgnored, Color invulnerabilityColor)
    {
        colliderIgnored.enabled = false;
        spriteRenderer.color = invulnerabilityColor;
        yield return FrameWaiter(duration);
        spriteRenderer.color = Color.white;
        colliderIgnored.enabled = true;
    }

}
