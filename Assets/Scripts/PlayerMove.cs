using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    private float speed = 8;
    private float jumpForce = 10;
    private float gravityForce = 20;

    private Rigidbody2D rb;
    private EdgeCollider2D ec;
    private bool jumpQueued = false;

    private Heart tetheredHeart = null;
    private LineRenderer tetherRenderer;

    public GameObject insideBlock;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        ec = gameObject.GetComponent<EdgeCollider2D>();
        tetherRenderer = gameObject.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Restart"))
		{
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        float xInput = Input.GetAxis("Horizontal");
        float xVel = xInput * speed;

        Vector2 startPoint = rb.position + ec.points[4] + Vector2.down * 0.1f;
        Vector2 endPoint = rb.position + ec.points[3] + Vector2.down * 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, endPoint - startPoint, Vector2.Distance(startPoint, endPoint), LayerMask.GetMask("Tiles"));
        bool onGround = hit.collider != null;
        float yVel = onGround ? 0 : rb.velocity.y - gravityForce * Time.fixedDeltaTime;

        if (jumpQueued)
        {
            jumpQueued = false;
            if (onGround)
            {
                //TODO play jump sound
                yVel = jumpForce;
            }
        }

        Vector2 vel = new Vector2(xVel, yVel);
        rb.velocity = vel;
        rb.MovePosition(rb.position + vel * Time.fixedDeltaTime);
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
