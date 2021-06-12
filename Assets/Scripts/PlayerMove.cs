using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    private float speed = 8;
    private float startBoost = 4;
    private float jumpForce = 10;

    private Rigidbody2D rb;
    private bool leftPressed = false;
    private bool rightPressed = false;
    private bool startLeft = false;
    private bool startRight = false;
    private bool stopLeft = false;
    private bool stopRight = false;
    private bool jumpQueued = false;

    private Heart tetheredHeart = null;
    private LineRenderer tetherRenderer;

    public GameObject insideBlock;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        tetherRenderer = gameObject.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Restart"))
		{
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetAxisRaw("Horizontal") < 0 && !leftPressed)
        {
            leftPressed = true;
            rightPressed = false;
            startLeft = true;
        }
        if (Input.GetAxisRaw("Horizontal") > 0 && !rightPressed)
        {
            rightPressed = true;
            leftPressed = false;
            startRight = true;
        }
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            if (leftPressed)
            {
                leftPressed = false;
                stopLeft = true;
            }
            if (rightPressed)
            {
                rightPressed = false;
                stopRight = true;
            }
        }

        if (Input.GetButtonDown("Jump"))
		{
            jumpQueued = true;
        }

        if (tetherRenderer.enabled)
        {
            tetherRenderer.SetPosition(0, tetheredHeart.transform.position);
            tetherRenderer.SetPosition(1, transform.position);
        }
    }

    private void FixedUpdate()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float xVel = xInput * speed;
        Vector2 velocity = new Vector2(xVel, 0);
        rb.AddForce(velocity, ForceMode2D.Force);

        if (startLeft)
        {
            startLeft = false;
            rb.AddForce(Vector2.left * startBoost, ForceMode2D.Impulse);
        }
        if (startRight)
        {
            startRight = false;
            rb.AddForce(Vector2.right * startBoost, ForceMode2D.Impulse);
        }
        if (stopLeft)
        {
            stopLeft = false;
            rb.AddForce(new Vector2(-rb.velocity.x, 0), ForceMode2D.Impulse);
        }
        if (stopRight)
        {
            stopRight = false;
            rb.AddForce(new Vector2(-rb.velocity.x, 0), ForceMode2D.Impulse);
        }

        if (jumpQueued)
		{
            //TODO check for collision on feet
            //TODO play jump sound
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpQueued = false;
        }
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.gameObject.CompareTag("BlockTrigger"))
		{
            insideBlock = collision.gameObject;
		}

        Heart heart = collision.gameObject.GetComponent<Heart>();
        if (heart != null)
		{
            if (tetheredHeart == null)
			{
                tetheredHeart = heart;
                tetherRenderer.enabled = true;
                tetherRenderer.SetPosition(0, tetheredHeart.transform.position);
                tetherRenderer.SetPosition(1, transform.position);
            }
            else if (heart != tetheredHeart)
			{
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //TODO nicer transition
			}
		}
	}
}
