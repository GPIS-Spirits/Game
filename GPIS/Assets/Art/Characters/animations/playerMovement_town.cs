using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    //public TextMeshProUGUI collectibleText;
    //public TextMeshProUGUI timerText;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb2d;

    public float speed;

    // int collectibleCount = 0;
    // float timer = 0;

    //house
    bool nearDoor = false; //is player near door
    Door currentDoor = null; //referece to the door that player is near
    public TextMeshProUGUI interactText; // Assign in Inspector

    //NPC
    bool nearNPC = false; //is player near NPC
    NPC currentNPC = null; //reference to the NPC that player is near
    public TextMeshProUGUI dialogueText; // Assign in Inspector - shows NPC dialogue

    public Sprite left, right, up, downleft, downright;

    private bool lookingLeft = false;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // timer += Time.deltaTime % 60;
        // timerText.text = timer.ToString("0");

        Move();
        Interact();

        if (nearNPC && currentNPC != null && Input.GetKeyDown(KeyCode.T))
        {
            ShowNPCPreviewText();
        }
    }

    void Interact()
    {
        if (nearDoor && currentDoor != null && Input.GetKeyDown(KeyCode.E))
        {
            EnterDoor();
        }
    }

    void EnterDoor()
    {
        SceneManager.LoadScene(currentDoor.sceneToLoad);
    }

    void TalkToNPC()
    {
        if (dialogueText == null || currentNPC == null)
            return;

        // Open or close NPC dialogue WITHOUT touching interactText
        if (!dialogueText.gameObject.activeSelf)
        {
            dialogueText.text = currentNPC.dialogueText;
            dialogueText.gameObject.SetActive(true);
        }
        else
        {
            dialogueText.gameObject.SetActive(false);
        }
    }



    void Move()
    {
        Vector2 move = Vector2.zero;

        if (Input.GetKey(KeyCode.A)) move.x = -speed;
        else if (Input.GetKey(KeyCode.D)) move.x = speed;
        else move.x = 0;

        if (Input.GetKey(KeyCode.W)) move.y = speed;
        else if (Input.GetKey(KeyCode.S)) move.y = -speed;
        else move.y = 0;

        if (move.x < 0)
        {
            spriteRenderer.sprite = left;
            lookingLeft = true;
        }
        else if (move.x > 0)
        {
            spriteRenderer.sprite = right;
            lookingLeft = false;
        }

        if (move.y > 0) // moving UP
        {
            spriteRenderer.sprite = up;
        }
        else if (move.y < 0)
        {
            spriteRenderer.sprite = (lookingLeft) ? downleft : downright;
        }

        rb2d.velocity = move;
    }

    void OnTriggerEnter2D(Collider2D other)
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
                nearNPC = false;
                currentNPC = null;

                if (dialogueText != null)
                    dialogueText.gameObject.SetActive(false);

                interactText.text = "Press E to Enter";
                interactText.gameObject.SetActive(true);
            }
        }

        if (other.CompareTag("NPC"))
        {
            currentNPC = other.GetComponent<NPC>();

            if (currentNPC != null)
            {
                nearNPC = true;
                nearDoor = false;
                currentDoor = null;

                interactText.text = "Press T to Talk";
                interactText.gameObject.SetActive(true);
            }
        }


    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Door"))
        {
            nearDoor = false;
            currentDoor = null;

            if (!nearNPC)
                interactText.gameObject.SetActive(false);
        }

        if (other.CompareTag("NPC"))
        {
            nearNPC = false;
            currentNPC = null;

            if (dialogueText != null)
                dialogueText.gameObject.SetActive(false);

            interactText.text = "Press T to Talk"; // reset it
            interactText.gameObject.SetActive(false); // hide it since we're leaving
        }

    }

    void ShowNPCPreviewText()
    {
        if (interactText == null || currentNPC == null)
            return;

        // Show the NPC's text as the prompt temporarily
        interactText.text = currentNPC.dialogueText;
        interactText.gameObject.SetActive(true);
    }

}