using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 500;
    private GameObject focalPoint;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup

    public ParticleSystem smokeParticle;
    
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        StartCoroutine(PowerupCooldown());
    }

    void Update()
    {
        float posX = transform.position.x;
        float posZ = transform.position.z;

        if (posZ < -9.0f) {
            transform.position = new Vector3(posX, 0, -9.0f);
        } else if (posZ > 25.0f) {
            transform.position = new Vector3(posX, 0, 25.0f);
        } else if (posX < -19 ) {
            transform.position = new Vector3(-19, 0, posZ);
        } else if (posX > 19) {
            transform.position = new Vector3(19, 0, posZ);
        } else {
            // Add force to player in direction of the focal point (and camera)
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 playerPosition = focalPoint.transform.forward * verticalInput * speed * Time.deltaTime;

            playerRb.AddForce(playerPosition);

            // Set powerup indicator position to beneath player
            powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);

            // Set smokeParticle to always be where player is
            smokeParticle.transform.position = transform.position + new Vector3(0, -0.6f, 0);

            if (Input.GetKey(KeyCode.Space)) {
                smokeParticle.Play();
                playerRb.AddForce(playerPosition * 2, ForceMode.Impulse);
            }
        }

    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
        }
    }

    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position; 
           
            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }


        }
    }



}
