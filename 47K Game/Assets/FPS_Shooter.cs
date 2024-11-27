using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Shooter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Camera playerCamera; // Reference to the first-person camera
    [SerializeField] private float fireRate = 5f; // Shots per second
    [SerializeField] private float damage = 20f; // Damage per shot
    [SerializeField] private float maxRange = 50f; // Max range of the weapon

    [Header("UI")]
    [SerializeField] private RectTransform crosshair; // Crosshair UI element on Canvas

    [Header("Effects")]
    [SerializeField] private AudioClip fireSFX; // Firing sound

    [Header("Layers")]
    [SerializeField] private LayerMask enemyLayer; // LayerMask for enemies

    private float fireCooldown = 0f;
    private IDamageMethod damageMethod;

    private void Start()
    {
        // Attach a custom damage method to handle damage logic
        damageMethod = new CustomDamageMethod(damage, fireRate); // Custom damage method
    }

    private void Update()
    {
        // Handle shooting
        if (Input.GetButtonDown("Fire1") && fireCooldown <= 0f) // Left mouse button
        {
            Shoot();
            fireCooldown = 1f / fireRate; // Reset fire cooldown
        }

        // Reduce cooldown over time
        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    private void Shoot()
    {
        // Play shooting sound
        if (fireSFX) SoundManager.Instance.Play(fireSFX);

        // Get crosshair position in screen space
        Vector3 crosshairScreenPosition = crosshair.position;

        // Cast a ray from the camera through the crosshair's position
        Ray ray = playerCamera.ScreenPointToRay(crosshairScreenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRange, enemyLayer)) // Only hit objects in the enemyLayer
        {
            Debug.Log("Hit object: " + hit.collider.name); // Log the hit object

            // Check if the hit object is on the "Enemy" layer (ensuring to hit the parent if it's a child)
            GameObject hitObject = hit.collider.gameObject;

            // Check if the hit object (or its parent) is in the "Enemy" layer
            if (hitObject.layer == LayerMask.NameToLayer("Enemy") || hitObject.transform.parent != null && hitObject.transform.parent.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Debug.Log("Enemy hit: " + hitObject.name); // Log enemy hit

                // Try to get the Enemy component from the hit object's parent (in case it's on a child object like a mesh)
                Enemy target = hitObject.GetComponentInParent<Enemy>();
                if (target)
                {
                    // Replace ApplyDamage with the actual method defined in your damage method class
                    damageMethod.DamageTick(target); // Assuming 'DamageTick' is implemented in your damage method class

                    Debug.Log($"Dealt {damage} damage to {target.name}");

                    // Optionally, trigger additional effects like particle systems or sound effects here
                }
                else
                {
                    Debug.LogWarning("Hit object tagged 'Enemy' but no Enemy component found.");
                }
            }
            else
            {
                Debug.LogWarning("Hit object is not on the 'Enemy' layer.");
            }
        }
        else
        {
            Debug.LogWarning("No object hit by raycast.");
        }
    }
}

public class CustomDamageMethod : IDamageMethod
{
    private float damage;
    private float fireRate;
    private float lastFireTime;

    // Constructor to initialize damage and fire rate
    public CustomDamageMethod(float damage, float fireRate)
    {
        this.damage = damage;
        this.fireRate = fireRate;
        this.lastFireTime = 0f;
    }

    public void Init(float damage, float fireRate)
    {
        // Custom initialization if needed
        this.damage = damage;
        this.fireRate = fireRate;
    }

    // Implement DamageTick from IDamageMethod interface
    public void DamageTick(Enemy target)
    {
        // Check if the target is valid
        if (target == null) return;

        // Check if enough time has passed to fire based on fireRate
        if (Time.time - lastFireTime >= 1f / fireRate)
        {
            // Apply the damage directly to the enemy
            target.Health -= damage;

            // Update the last fire time to handle the fire rate
            lastFireTime = Time.time;

            // If the enemy's health is zero or below, trigger the death logic
            if (target.Health <= 0f)
            {
                target.Die(); // Call the Die method to handle death
                Debug.Log($"{target.name} has died.");
            }
        }
    }
}

