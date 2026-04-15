using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonShuffler : MonoBehaviour
//DESPITE THIS SCRIPT'S NAME
//THIS CAN ALSO WORK FOR DOORS THAT GO UP AND DOWN
//HAVE FUN!
//What the fuck do you mean "this cant be found" my brother in christ its right HERE
//FUCKING DUMBASS ENGINE I FUCKING HATE VIBE CODERS I FUCKING HATE VIBE CODERS I FUCKING HATE VIBE CODERS
{
    public GameObject player;
    [SerializeField] float useRange = 2;
    [SerializeField] GameObject target;
    [SerializeField] Vector3 horizTarget;
    [SerializeField] float moveSpeed;
    //KEEP THIS SHIT L O W
        //LIKE, BELOW 0.1 LOW  
    [SerializeField] bool RightInstead;
    public bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetLocation = target.transform.position;
        float dist = Vector3.Distance(player.transform.position, transform.position);
        //Measures distance between player object and this object

        if (dist <= useRange)
        //if player is in range
        {
            if (Input.GetKeyDown(KeyCode.E))
            //and interact key is pressed
            {
                Debug.Log("Button Pressed");
                //confirm in console
                active = true;
                //and set it to be active
            }
        }

        if(active == true)
        {
            if (RightInstead == false)
            //If this lowers instead of raises, follow these rules
            {
           if(target.transform.position.x <= horizTarget.x)
           //If the vertical position of the platform is equal or lower than its target point...
            {
                active = false;
                //Stop moving
            }
            else
            {
                //If this goes down
                target.transform.position = target.transform.position + new Vector3(moveSpeed * -1, 0f, 0f);
                //Gradually lowers based off of MoveSpee
            }
            }
            else
            //If it instead raises up
            {
                if(target.transform.position.x >= horizTarget.x)
           //If the vertical position of the platform is equal or greater than its target point...
            {
                active = false;
                //Stop moving
            }
            else
            {
                //If this goes down
                target.transform.position = target.transform.position + new Vector3(moveSpeed, 0f, 0f);
                //Gradually raises based off of MoveSpee
            }
            }
        }
    }
}
