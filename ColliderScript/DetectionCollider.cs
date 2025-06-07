using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways]
public class DetectionCollider : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider2D;

    private void OnValidate()
    {
        if (boxCollider2D == null)
            boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D == null)
            return;
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Jugador esta dentro del collider de detección: " + collision.gameObject.name);
    }
}
