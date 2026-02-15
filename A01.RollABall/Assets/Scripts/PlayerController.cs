using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    
    public float speed = 0; // Change pubclic to display on UI
    
    // UI text component to display count of "PickUp" objects collected.
    public TextMeshProUGUI countText;
    // UI object to display winning text.
    public GameObject winTextObject;

    // Rigidbody of the player.
    private Rigidbody rb; 
    // Count number of collectibles
    private int count;
    // Movement along X and Y axes.
    private float movementX;
    private float movementY;
    
    private float jumpForce = 6f;

    // Autdio
    public AudioClip pickupSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         // Get and store the Rigidbody component attached to the player
        rb = GetComponent<Rigidbody>();
        count = 0;
        
        SetCountText();

        // Initially set the win text to be inactive.
        winTextObject.SetActive(false);
    }
    // OnMove function is called when a move input is detected.
    void OnMove(InputValue movementValue)
    {
         // Convert the input value into a Vector2 for movement.
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // Called by New Input System action "Jump" (bind to Space)
    void OnJump(InputValue jumpValue)
    {
        // only jump when key is pressed (not released) and player is grounded
        if (jumpValue.isPressed)
        {
            // Make jump consistent by clearing vertical velocity
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if(count >= 13)
        {
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy")); 
        }
    }

    // FixedUpdate is called once per fixed frame-rate frame.
    void FixedUpdate()
    {
        // Create a 3D movement vector using the X and Y inputs.
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        // Apply force to the Rigidbody to move the player.
        rb.AddForce(movement * speed);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            // Destroy the current object
            Destroy(gameObject); 
            // Update the winText to display "You Lose!"
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object the player collided with has the "PickUp" tag.
        if(other.gameObject.CompareTag("PickUp"))
        {
            // Deactivate the collided object (making it disappear).
            other.gameObject.SetActive(false);
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            // Update Count when collide with collectibles
            count += 1;
            SetCountText();
        }

    }
}
