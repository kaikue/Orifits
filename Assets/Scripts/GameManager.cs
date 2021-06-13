using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const float connectionDistance = 0.1f;
    public const float snapDistance = 2.5f;
    public const float fadeTime = 0.6f;
    private const float rotateSpeed = 5;
    private const float levelTransitionTime = 2;
    private const float pitchVariation = 0.15f;
    public Color outsideColor;
    public bool levelComplete = false;

    private AudioSource audioSource;
    public AudioClip attachSound;
    public AudioClip separateSound;

    private PlayerMove player;
    private Orifice[] allOrifices;
    private BlockDrag[] allBlocks;
    private Rigidbody2D draggingBlock = null;
    private Vector2 dragOffset;
    private Vector2 rotateAnchor;
    private float baseRotation;
    //private float rotateDampen = 20;
    private Persistent persistent;

    private bool dragging = false;
    private bool rotating = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerMove>();
        allOrifices = FindObjectsOfType<Orifice>();
        allBlocks = FindObjectsOfType<BlockDrag>();
        audioSource = gameObject.GetComponent<AudioSource>();
        Persistent[] persistents = FindObjectsOfType<Persistent>();
        foreach (Persistent persistent in persistents)
        {
            if (!persistent.destroying)
            {
                this.persistent = persistent;
                break;
            }
        }
        CenterCamera();
    }

    private void CenterCamera()
	{
        Vector2 centerPos = allBlocks.Select(block => block.transform.position).Aggregate((x, y) => x + y) / allBlocks.Length;
        Camera.main.transform.position = new Vector3(centerPos.x, centerPos.y, Camera.main.transform.position.z);
    }

    public void PlaySound(AudioClip sound, bool randomizePitch = true)
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

    private void Update()
    {
        if (Input.GetButtonDown("Restart"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (rotating)
		{
            if (Input.GetButtonDown("MouseLeft"))
            {
                dragging = true;
            }
            if (Input.GetButtonUp("MouseLeft"))
			{
                dragging = false;
			}

            if (Input.GetButtonUp("MouseRight"))
            {
                rotating = false;
                //rotate draggingBlock to nearest 90 degree angle
                float roundedRot = Mathf.Round(draggingBlock.rotation / 90) * 90;
                draggingBlock.SetRotation(roundedRot);
                //draggingBlock.MoveRotation(roundedRot); //TODO lerp to it and wait a bit before freezing rotation?
                
                if (dragging)
                {
                    draggingBlock.constraints = RigidbodyConstraints2D.FreezeRotation;
                    dragOffset = new Vector2(draggingBlock.position.x - mousePos.x, draggingBlock.position.y - mousePos.y);
                }
                else
				{
                    ReleaseDraggingBlock();
                }
            }
            else
			{
                Vector2 mouseOffset = mousePos - rotateAnchor;
                float rotateDiff = -mouseOffset.x + mouseOffset.y;
                draggingBlock.MoveRotation(baseRotation + rotateDiff * rotateSpeed);

                //TODO better rotating method
                //TODO: this is giving wrong values?
                //float goalRotation = (Mathf.Rad2Deg * Mathf.Atan2(mousePos.y - rotateAnchor.y, mousePos.x - rotateAnchor.x) + 90 + baseRotation) % 360;
                //float effect = Vector2.Distance(mousePos, rotateAnchor) / rotateDampen;
                //float rot = Mathf.LerpAngle(baseRotation, goalRotation, effect) % 360;
                //print(goalRotation + " " + rot + " " + effect);
                //draggingBlock.MoveRotation(rot);
            }
        }
        else if (dragging)
		{
            if (Input.GetButtonDown("MouseRight"))
            {
                rotating = true;
                draggingBlock.constraints = RigidbodyConstraints2D.FreezePosition;
                rotateAnchor = mousePos;
                baseRotation = draggingBlock.rotation;

                if (Input.GetButtonUp("MouseLeft")) dragging = false;
            }
            else if (Input.GetButtonUp("MouseLeft"))
            {
                dragging = false;
                ReleaseDraggingBlock();
            }
            else
			{
                draggingBlock.MovePosition(mousePos + dragOffset);
            }
        }
        else
		{
            if (Input.GetButtonDown("MouseLeft"))
            {
                GrabDraggingBlock(mousePos);
                if (draggingBlock != null)
				{
                    draggingBlock.constraints = RigidbodyConstraints2D.FreezeRotation;
                    dragOffset = new Vector2(draggingBlock.position.x - mousePos.x, draggingBlock.position.y - mousePos.y);
                    dragging = true;
                }
            }
            if (Input.GetButtonDown("MouseRight"))
            {
                GrabDraggingBlock(mousePos);
                if (draggingBlock != null)
                {
                    draggingBlock.constraints = RigidbodyConstraints2D.FreezePosition;
                    rotateAnchor = mousePos;
                    baseRotation = draggingBlock.rotation;
                    rotating = true;
                }
            }
        }
    }

    private void GrabDraggingBlock(Vector2 mousePos)
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 1, LayerMask.GetMask("Block"));
        if (hit.collider != null)
        {
            draggingBlock = hit.collider.GetComponent<Rigidbody2D>();

            player.gameObject.SetActive(false);
            player.transform.parent = player.insideBlock.transform;

            foreach (Orifice orifice in allOrifices)
            {
                orifice.blockerRenderer.enabled = false;
                orifice.outsideRenderer.enabled = true;
            }

            Orifice[] draggingOrifices = draggingBlock.gameObject.GetComponentsInChildren<Orifice>();
            foreach (Orifice orifice in draggingOrifices)
            {
                orifice.Close();
            }

            foreach (BlockDrag block in allBlocks)
			{
                block.face.SetActive(true);
            }

            draggingBlock.gameObject.GetComponent<BlockDrag>().SetDragging(true);
            
            Camera.main.backgroundColor = outsideColor;

            persistent.SetInside(false);
        }
    }

    private void ReleaseDraggingBlock()
	{
        //snap dragging block to align orifices if nearby
        Orifice[] draggingOrifices = draggingBlock.gameObject.GetComponentsInChildren<Orifice>();
        foreach (Orifice orifice in allOrifices)
        {
            if (orifice.transform.parent == draggingBlock.transform) continue;

            bool done = false;
            foreach (Orifice draggingOrifice in draggingOrifices)
            {
                float distance = Vector3.Distance(orifice.transform.position, draggingOrifice.transform.position);
                if (distance < snapDistance)
				{
                    Vector3 offset = orifice.transform.position - draggingOrifice.transform.position;
                    draggingBlock.transform.position += offset;

                    //open all connecting orifices (may be more than just this one pair)
                    foreach (Orifice otherOrificeToOpen in allOrifices)
                    {
                        if (otherOrificeToOpen.transform.parent == draggingBlock.transform) continue;
                        foreach (Orifice myOrificeToOpen in draggingOrifices)
                        {
                            float d = Vector3.Distance(otherOrificeToOpen.transform.position, myOrificeToOpen.transform.position);
                            if (d < connectionDistance)
                            {
                                myOrificeToOpen.OpenWith(otherOrificeToOpen);
                            }
                        }
                    }

                    done = true;
                    break;
				}
            }
            if (done) break;
        }

        draggingBlock.gameObject.GetComponent<BlockDrag>().SetDragging(false);
        draggingBlock.constraints = RigidbodyConstraints2D.FreezeAll;
        draggingBlock = null;

        foreach (BlockDrag block in allBlocks)
        {
            block.face.SetActive(false);
        }

        foreach (Orifice orifice in allOrifices)
        {
            orifice.blockerRenderer.enabled = true;
            orifice.outsideRenderer.enabled = false;
        }

        CenterCamera();
        Camera.main.backgroundColor = Color.black;

        player.transform.parent = null;
        player.transform.rotation = Quaternion.identity;
        player.gameObject.SetActive(true);

        persistent.SetInside(true);
    }

    private IEnumerator WaitNextLevel()
	{
        yield return new WaitForSeconds(levelTransitionTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //TODO nicer transition
    }

    public void CompleteLevel()
	{
        levelComplete = true;
        StartCoroutine(WaitNextLevel());
    }
}
