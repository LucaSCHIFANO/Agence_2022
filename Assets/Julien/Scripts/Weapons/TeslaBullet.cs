using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaBullet : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private float delayExplosion;

    private void Start()
    {
        Invoke("Explode", delayExplosion);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward * velocity * Time.deltaTime;
    }

    void Explode()
    {
        Destroy(gameObject);
        
        // BOOOOOOMMMM ! Tu stun tout les gens autour
    }
}
