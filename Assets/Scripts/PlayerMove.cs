using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private float speed = 8;
    private float jumpForce = 10;
    private float gravityForce = 20;
    private float pitchVariation = 0.15f;

    private Rigidbody2D rb;
    private EdgeCollider2D ec;
    private bool jumpQueued = false;

    private Heart tetheredHeart = null;
    private LineRenderer tetherRenderer;
    private GameManager gameManager;

    private int keys = 0;

    public GameObject insideBlock;

    private AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip bonkSound;
    public AudioClip heartSound;
    public AudioClip keySound;
    public AudioClip openSound;
    public AudioClip lockedSound;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        ec = gameObject.GetComponent<EdgeCollider2D>();
        tetherRenderer = gameObject.GetComponent<LineRenderer>();
        audioSource = gameObject.GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
		{
            jumpQueued = true;
        }

        if (tetherRenderer.enabled && !gameManager.levelComplete)
        {
            tetherRenderer.SetPosition(0, tetheredHeart.transform.position);
            tetherRenderer.SetPosition(1, transform.position);
        }
    }

    private void PlaySound(AudioClip sound, bool randomizePitch = true)
	{
        if (randomizePitch)
		{
            audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
		}
        else
		{
            audioSource.pitch = 1;
        }
        audioSource.PlayOneShot(sound);
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

        if (onGround && rb.velocity.y < 0)
		{
            PlaySound(bonkSound);
        }
        float yVel = onGround ? 0 : rb.velocity.y - gravityForce * Time.fixedDeltaTime;
        if (onCeiling && yVel > 0)
		{
            yVel = 0;
            PlaySound(bonkSound);
        }

        if (jumpQueued)
        {
            jumpQueued = false;
            if (onGround)
            {
                yVel = jumpForce;
                PlaySound(jumpSound);
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
                PlaySound(openSound);
                Destroy(collider);
            }
            else
			{
                PlaySound(lockedSound);
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
            PlaySound(keySound);
            Destroy(collider);
        }

        TriggerInstruction instruction = collider.GetComponent<TriggerInstruction>();
        if (instruction != null)
		{
            if (!instruction.isRestart || tetheredHeart == null)
            {
                instruction.TurnOn();
            }
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
                Color color = heart.gameObject.GetComponent<SpriteRenderer>().color;
                tetherRenderer.startColor = color;
                tetherRenderer.endColor = color;
                PlaySound(heartSound);
            }
            else if (heart != tetheredHeart)
			{
                gameManager.CompleteLevel();
                tetherRenderer.SetPosition(1, heart.transform.position);
                Color color = heart.gameObject.GetComponent<SpriteRenderer>().color;
                tetherRenderer.endColor = color;
			}
		}
	}
}
