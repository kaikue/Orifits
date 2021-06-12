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

    private int keys = 0;

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

    private bool CheckSide(int point0, int point1, Vector2 direction)
	{
        Vector2 startPoint = rb.position + ec.points[point0] + direction * 0.1f;
        Vector2 endPoint = rb.position + ec.points[point1] + direction * 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, endPoint - startPoint, Vector2.Distance(startPoint, endPoint), LayerMask.GetMask("Blocker", "Tiles"));
        return hit.collider != null;
    }

    private void FixedUpdate()
    {
        float xInput = Input.GetAxis("Horizontal");
        float xVel = xInput * speed;

        bool onGround = CheckSide(4, 3, Vector2.down);
        bool onCeiling = CheckSide(1, 2, Vector2.up);
        float yVel = onGround ? 0 : rb.velocity.y - gravityForce * Time.fixedDeltaTime;
        if (onCeiling)
		{
            yVel = Mathf.Min(yVel, 0);
		}

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

	private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collider = collision.collider.gameObject;
        if (collider.CompareTag("Door"))
		{
            if (keys > 0)
			{
                keys--;
                //TODO play door open sound
                Destroy(collider);
            }
            else
			{
                //TODO play door locked sound
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
        GameObject collider = collision.gameObject;
        if (collider.CompareTag("BlockTrigger"))
		{
            insideBlock = collider;
        }

        if (collider.CompareTag("Key"))
        {
            keys++;
            //TODO play key collect sound
            Destroy(collider);
        }

        Heart heart = collider.GetComponent<Heart>();
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
