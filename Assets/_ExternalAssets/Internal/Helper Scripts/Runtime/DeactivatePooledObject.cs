using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class DeactivatePooledObject : MonoBehaviour
{
    [SerializeField] float timeToDeactivate;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(Deactivate());
    }

    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(timeToDeactivate);
       LeanPool.Despawn(gameObject);
    }

}
