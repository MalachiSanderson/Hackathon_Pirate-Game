using System.Collections.Generic;
using UnityEngine;

public class AutoParentWhenInBounds : MonoBehaviour
{
    public List<string> tags = new List<string>();
    private readonly HashSet<Transform> parentedTransforms = new HashSet<Transform>();
    private Transform rootParent = null;

    private void Start()
    {
        rootParent = GetRootParent(transform);
        transform.SetParent(null);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!tags.Contains(collider.tag)) return;

        var other = GetRootParent(collider.transform);
        parentedTransforms.Add(collider.transform);
        other.SetParent(rootParent);
    }

    private void OnTriggerExit(Collider collider)
    {
        if (!tags.Contains(collider.tag)) return;

        if (parentedTransforms.Contains(collider.transform))
        {
            parentedTransforms.Remove(collider.transform);
            collider.transform.SetParent(null);
        }
    }

    private void Update()
    {
        transform.position = rootParent.position;
        transform.rotation = rootParent.rotation;
        transform.localScale = rootParent.localScale;
    }

    private static Transform GetRootParent(Transform transform)
    {
        return transform.parent != null ? transform.parent : transform;
    }
}