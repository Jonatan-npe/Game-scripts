using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CollisionController : MonoBehaviour
{
    private CapsuleCollider2D[] capsuleCollider2D;
    private void Start()
    {
        // Get all capsule colliders in children and keep only those on layer 8 or 10.
        var all = GetComponentsInChildren<CapsuleCollider2D>();
        var keep = new List<CapsuleCollider2D>(all.Length);
        foreach (var c in all)
        {
            if (c == null) continue;
            int layer = c.gameObject.layer;
            if (layer == 8 || layer == 10)
            {
                keep.Add(c);
            }
            else
            {
                Destroy(c);
            }
        }
        capsuleCollider2D = keep.ToArray();
    }
}