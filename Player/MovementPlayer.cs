using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

/* este archivo tiene como propositio el control del movimiento horizontal
 * y vertical del personaje, incluyendo mecanicas como el salto y el dash
 */

public class MovementPlayer : MonoBehaviour
{
    //Variables y/o configuraciones editables
    [Header("Movement:")]
    [SerializeField, Range(0, 1000)] private int moveMultiplicator;
    [SerializeField, Range(0, 1)] private float smoothMove;
    [Space]

    [Header("Jump:")]
    [SerializeField, Range(0, 200)] private float jumpForce;
    [SerializeField] private GameObject ground;
    [SerializeField] private Vector2 dimensionsBox;
    [SerializeField] private LayerMask layerGround;
    [Space]

    [Header("Dash:")]
    [SerializeField, Range(0, 25)] private int framesToStartDash;
    [SerializeField, Range(0, 50)] private int invulnerableFrames;
    [SerializeField, Range(0, 50)] private int endDash;
    [SerializeField, Range(0, 10)] private int framesCooldownDash;
    [SerializeField, Range(0, 1000)] private float dashForce;

    //variables no editables
    private float framesInDash;
    private bool grounded;
    private int numJumpings = 0;
    private float originalScaleX;
    private float originalGravity;
    private bool inDash;
    private bool canDash = true;
    private bool canJump = true;
    Vector2 velocidad = Vector2.zero;

    private double lastTime;


    //Componentes internos del pj
    private Rigidbody2D rigidbody2DPlayer;
    private Animator animator;
    private PlayerInput playerInput;

    void Start()
    {
        framesInDash = framesToStartDash + invulnerableFrames + endDash;
        //Secuencia para instanciar dentro del codigo codigos externos
        originalScaleX = transform.localScale.x;
        rigidbody2DPlayer = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        originalGravity = rigidbody2DPlayer.gravityScale;
    }

    void FixedUpdate()
    {
        //movimiento del personaje, donde se le da una velocidad al personaje,
        //por lo que siempre estara ligada la velocidad horizontal a este parametro
        if (!inDash)
        {
            Move((playerInput.actions["Move"].ReadValue<Vector2>().x * moveMultiplicator) * Time.fixedDeltaTime);


            //ejecución de acciones a traves del buffer
            //Acciones de movimiento
            InputAction actionMovement = GameManager.Instance.GetLastInputMovement();
            if (actionMovement != null)
            {
                ActionMovementBuffer(actionMovement);
            }
        }
    }
    private void Update()
    {
        //Deteccion del suelo
        grounded = Physics2D.OverlapBox(ground.transform.position, dimensionsBox, 0, layerGround);
        if (grounded && numJumpings >= 1) numJumpings = 0;

        //envios al animator del jugador

        animator.SetBool("Move" , playerInput.actions["Move"].ReadValue<Vector2>().x != 0);
        animator.SetBool("Grounded", grounded);

        //giro de la escala del personaje
        float scaleToMove = ((int) playerInput.actions["Move"].ReadValue<Vector2>().x * 30) + (int) rigidbody2DPlayer.velocity.x;
        switch (scaleToMove)
        {
            case > 0:
                transform.localScale = new(originalScaleX, transform.localScale.y);
                break;
            case < 0:
                transform.localScale = new(-originalScaleX, transform.localScale.y);
                break;
            case 0:
                break;
        }
    }
    private void Move(float move)
    {
        Vector2 newVelocity = new(move, rigidbody2DPlayer.velocity.y);
        rigidbody2DPlayer.velocity = Vector2.SmoothDamp(rigidbody2DPlayer.velocity, newVelocity, ref velocidad, smoothMove);
    }

    //Metodos ejecutados desde el inputSystem en unity

    public void RequestJump(InputAction.CallbackContext callbackContext) //Solicita el salto
    {
        if (callbackContext.performed && canJump)
        {
            GameManager.Instance.AddBufferMovement(callbackContext.action); //envio de la accion al buffer
            numJumpings++;
        }
        //Suma de cuantos saltos se han dado
        if (callbackContext.canceled)
        {
            animator.ResetTrigger("Jump");
        }
    }
    public void RequestDash(InputAction.CallbackContext callbackContext) //Solicita un dash
    {
        if (callbackContext.performed && canDash)
        {
            GameManager.Instance.AddBufferMovement(callbackContext.action);
            StartCoroutine(CoolDownDash());
            lastTime = callbackContext.time;
        }

    }

    //verifica cualquier acction en cola del buffer
    private void ActionMovementBuffer(InputAction input)
    {
        switch (input.name)
        {
            case "Jump":

                switch (numJumpings)
                {
                    case 0:

                        //ejecucion del primer salto
                        rigidbody2DPlayer.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        animator.SetTrigger("Jump");

                        break;
                    case 1:

                        //ejecucion del segundo salto con menor potencia
                        rigidbody2DPlayer.velocity = new Vector2(rigidbody2DPlayer.velocity.x, 0);
                        rigidbody2DPlayer.AddForce(Vector2.up * (float)(jumpForce * 0.8), ForceMode2D.Impulse);
                        animator.SetTrigger("Jump");

                        break;
                }
                break;
            case "Dash":

                StartCoroutine(Dash());

                break;
        }
    }
    private IEnumerator CoolDownDash()
    {
        canDash = false;

        yield return new WaitForSeconds((framesCooldownDash + framesInDash) * Time.fixedDeltaTime);
        yield return new WaitWhile(() => !grounded);

        canDash = true;
    }
    private IEnumerator Dash()
    {
        inDash = true;
        canJump = false;
        animator.SetTrigger("Dash");
        
        yield return new WaitForSeconds(framesToStartDash * Time.fixedDeltaTime);

        rigidbody2DPlayer.velocity = Vector2.zero;
        rigidbody2DPlayer.gravityScale = 0;
        rigidbody2DPlayer.AddForce(new Vector2(transform.localScale.x * dashForce, 0), ForceMode2D.Impulse);

        yield return new WaitForSeconds(invulnerableFrames * Time.fixedDeltaTime);

        canJump = true;

        yield return new WaitForSeconds(endDash * Time.fixedDeltaTime);

        rigidbody2DPlayer.gravityScale = originalGravity;
        rigidbody2DPlayer.velocity = Vector2.zero;
        inDash = false;

    }
    //Metodo para dibujar el espacio usado para detectar el suelo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(ground.transform.position, dimensionsBox);

    }
}
