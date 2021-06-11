using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerMove player;
    private BlockDrag[] blocks;
    private Rigidbody2D draggingBlock = null;
    private Vector2 dragOffset;
    private Vector2 rotateAnchor;
    private float baseRotation;
    //private float rotateDampen = 20;

    private bool dragging = false;
    private bool rotating = false;

    private void Start()
    {
        player = FindObjectOfType<PlayerMove>();
        blocks = FindObjectsOfType<BlockDrag>();
    }

    private void Update()
    {
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
                //exit ROTATE MODE
                rotating = false;
                //rotate draggingBlock to nearest 90 degree angle
                print(draggingBlock.rotation);
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
                    draggingBlock.constraints = RigidbodyConstraints2D.FreezeAll;
                    draggingBlock = null;
                }
            }
            else
			{
                //in ROTATE MODE- rotate dragging block

                float rotateSpeed = 3;
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
                draggingBlock.constraints = RigidbodyConstraints2D.FreezeAll;
                draggingBlock = null;
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
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 1, LayerMask.GetMask("Block"));
                if (hit.collider != null)
                {
                    draggingBlock = hit.collider.GetComponent<Rigidbody2D>();
                    //TODO deactivate player and parent it to the block it's in
                    draggingBlock.constraints = RigidbodyConstraints2D.FreezeRotation;
                    dragOffset = new Vector2(draggingBlock.position.x - hit.point.x, draggingBlock.position.y - hit.point.y);
                    dragging = true;
                }
            }
            if (Input.GetButtonDown("MouseRight"))
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 1, LayerMask.GetMask("Block"));
                if (hit.collider != null)
                {
                    draggingBlock = hit.collider.GetComponent<Rigidbody2D>();
                    //TODO deactivate player and parent it to the block it's in
                    draggingBlock.constraints = RigidbodyConstraints2D.FreezePosition;
                    rotateAnchor = mousePos;
                    baseRotation = draggingBlock.rotation;
                    rotating = true;
                }
            }
        }


        /*if (draggingBlock != null)
		{
            if (Input.GetButtonUp("MouseLeft"))
			{
                //drop the block
                //snap the block to the connecting orifice if appropriate
                //go back to playing mode
                //(exit rotation mode too, just to be safe)
                //recenter the camera (on the average of all blocks?)

                draggingBlock.constraints = RigidbodyConstraints2D.FreezeAll;
                draggingBlock = null;
            }
            else if (Input.GetButtonDown("MouseRight"))
            {
                //enter ROTATION MODE
                draggingBlock.constraints = RigidbodyConstraints2D.FreezePosition;
                rotateAnchor = mousePos;
                baseRotation = draggingBlock.rotation;
			}
            else if (Input.GetButtonUp("MouseRight"))
            {
                //exit ROTATION MODE
                draggingBlock.constraints = RigidbodyConstraints2D.FreezeRotation;
                dragOffset = new Vector2(draggingBlock.position.x - mousePos.x, draggingBlock.position.y - mousePos.y);
                //TODO rotate dbrb to nearest 90 degree angle
            }
            else if (Input.GetButton("MouseRight"))
			{
                //in ROTATE MODE- rotate dragging block

                float rotateSpeed = 3;
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
            else
			{
                //move dragging block with cursor
                draggingBlock.MovePosition(mousePos + dragOffset);
			}
        }
        else
		{
            if (Input.GetButtonDown("MouseLeft"))
			{
                //if one of the blocks is under mouse:
                //go into dragging mode
                //drag that block
                //deactivate player and parent it to the block it's in
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 1, LayerMask.GetMask("Block"));
                if (hit.collider != null)
                {
                    draggingBlock = hit.collider.GetComponent<Rigidbody2D>();
                    dragOffset = new Vector2(draggingBlock.position.x - hit.point.x, draggingBlock.position.y - hit.point.y);
                    draggingBlock.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
			}
        }*/
    }
}
