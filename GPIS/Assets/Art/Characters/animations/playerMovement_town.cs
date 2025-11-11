
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class playerMovement_town : MonoBehaviour
{
    //public TextMeshProUGUI collectibleText;
    //public TextMeshProUGUI timerText;

    Animator animator;
    SpriteRenderer spriteRenderer;
    public InputAction movement;
    public float moveSpeed;
   // int collectibleCount = 0;
   // float timer = 0;

    Vector3 pos;
    Rigidbody2D cat;

    //house
    private bool nearDoor = false; //is player near door
    private Door currentDoor = null; //referece to the door that player is near


    public TextMeshProUGUI interactText; // Assign in Inspector

    // Start is called before the first frame update
    void Start()
    {
        cat = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement.Enable();
        moveSpeed += 100;
    }

    void Update()
    {
        // timer += Time.deltaTime % 60;
        // timerText.text = timer.ToString("0");

        if (nearDoor && currentDoor != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(currentDoor.sceneToLoad);
        }
    }

    void FixedUpdate()
    {
        var moveDirection = movement.ReadValue<Vector2>();
        float x = moveDirection.x * moveSpeed * Time.deltaTime;
        float y = moveDirection.y * moveSpeed * Time.deltaTime;
        pos = new Vector3(x, y, 0);

        animator.SetFloat("X", x);
        if (x < 0) spriteRenderer.flipX = true;
        else spriteRenderer.flipX = false;
        animator.SetFloat("Y", y);


        cat.velocity = pos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.CompareTag("Collectible"))
        //{
        //    other.gameObject.SetActive(false);
        //    collectibleCount++;
        //    collectibleText.text = ": " + collectibleCount;

        //    if (collectibleCount >= 8)
        //    {
        //        SceneManager.LoadScene("CreditScreen");
        //    }
        //}

        if (other.CompareTag("Door"))
        {

            currentDoor = other.GetComponent<Door>();
            if (currentDoor != null)
            {
                nearDoor = true;
                if (interactText != null)
                {
                    interactText.text = $"Press E to Enter";
                    interactText.gameObject.SetActive(true);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Door"))
        {
            nearDoor = false;
            currentDoor = null;
            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }
}


