using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public CharacterController controller;

    public float maxSpeed = 10.0f;
    public float gravity = -30.0f;
    public float jumpHeight = 3.0f;

    public Transform groundCheck;
    public float groundRadius = 0.5f;
    public LayerMask groundMask;

    public Vector3 velocity;
    public bool isGrounded;
    public bool gameIsPaused;

    public GameObject gameOverPanel;

    [Header("HealthBar")]
    public HealthBarScreenSpaceController healthBar;

    [Header("Player Abilities")]
    [Range(0, 100)]
    public int health = 100;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame - once every 16.6666ms

    void Update()
    {
        #region player Control
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }

        if (isGrounded && controller.velocity.magnitude > 0)
        {
           // FindObjectOfType<AudioManager>().Play("Footstep");
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * maxSpeed * Time.deltaTime);
        
        if (Input.GetButton("Jump")) // && isGrounded
        {

            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            FindObjectOfType<AudioManager>().Play("Footstep");

        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        #endregion


        #region StopResume
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Main");
            gameOverPanel.SetActive(false);
            FindObjectOfType<GameManager>().Restart();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (gameIsPaused)
            {
                gameIsPaused = !gameIsPaused;
                PauseGame();
            }
            else if (!gameIsPaused)
            {
                gameIsPaused = !gameIsPaused;
                ResumeGame();
            }
        }
        #endregion

        if (health <= 0)
        {
            FindObjectOfType<GameManager>().EndGame();
            gameOverPanel.SetActive(true);
        }
       
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.TakeDamage(damage);
        if (health < 0)
        {
            health = 0;
        }
    }


    void PauseGame()
    {
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
    }


}
