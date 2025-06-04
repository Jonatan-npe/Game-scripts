using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    /*Este codigo va a ser el cerebro central de la ejecuci�n,
     * donde se va a acceder a otros codigos en ambos sentidos,
     * siendo este el nodo central*/

    public static GameManager Instance { get; private set; } //se instancia para poder acceder a el desde cualquier otro codigo
    [SerializeField] private InputBuffer inputBuffer;
    [SerializeField, Tooltip("Frame rate objetivo para el juego. Usa -1 para ilimitado.")]
    private int targetFrameRate = 60;
    public int TargetFrameRate
    {
        get => targetFrameRate;
        set
        {
            targetFrameRate = value;
            Application.targetFrameRate = targetFrameRate;
        }
    }


    private void Awake()
    {
        //linea para revisar si no existe ya una instancia de este objeto
        if (Instance == null)// si no existe se instancia el objeto actual
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else //si existe se destruye el objeto actual
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = targetFrameRate;
    }

    public void AddBufferMovement(InputAction inputAction)
    {
       inputBuffer.AddActionMove(inputAction);
    }
    public InputAction GetLastInputMovement()
    {
        return inputBuffer.GetLastInputMovement();
    }
    public void AddBufferAttack(InputAction inputAction)
    {
        inputBuffer.AddActionAttack(inputAction);
    }
    public InputAction GetLastInputAttack() 
    {
        return inputBuffer.GetLastInputAttack();
    }
}
