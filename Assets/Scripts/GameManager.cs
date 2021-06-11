using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerMove player;
    private BlockDrag[] blocks;
    private BlockDrag draggingBlock = null;

    private void Start()
    {
        player = FindObjectOfType<PlayerMove>();
        blocks = FindObjectsOfType<BlockDrag>();
    }

    private void Update()
    {
        if (draggingBlock != null)
		{
            if (Input.GetButtonUp("MouseLeft"))
			{
                //drop the block
                //snap the block to the connecting orifice if appropriate
                //go back to playing mode
                //(exit rotation mode too, just to be safe)
            }
            else if (Input.GetButtonDown("MouseRight"))
            {
                //enter ROTATION MODE
			}
            else if (Input.GetButtonUp("MouseRight"))
            {
                //exit ROTATION MODE
            }
            else if (Input.GetButton("MouseRight"))
			{
                //rotate by difference with original coords
			}
            else
			{
                //move dragging block with cursor
			}
        }
        else
		{
            if (Input.GetButtonDown("MouseLeft"))
			{
                //if one of the blocks is under mouse:
                //go into dragging mode
                //drag that block
                //deactivate player
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    print("Target Position: " + hit.collider.gameObject.transform.position);
                }
                else
				{
                    print("nothing clicked");
				}
			}
        }
    }
}
