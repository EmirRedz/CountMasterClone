using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int itemCollected;
    public GameObject particlesOnCollect;
    public bool plusOrMinus;
    [SerializeField] Transform spawnPos;
    [SerializeField] bool spawnPickup;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (plusOrMinus)
            {
                itemCollected += 1;
            }
            else
            {
                itemCollected -= 1;
            }


            if (particlesOnCollect)
            {
                LeanPool.Spawn(particlesOnCollect, transform.position, Quaternion.identity);
            }

            gameObject.SetActive(false);
        }
    }

    public void SetParticleGO(GameObject go)
    {
        particlesOnCollect = go;
    }
}
