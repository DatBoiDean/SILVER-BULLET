using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//hate i have to add a comment and re-save things to get it to recognize
//THE SHIT THATS BEEN IN HERE
public class ButtonTemplate : MonoBehaviour
{
    public GameObject player;
    [SerializeField] float useRange;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(player.transform.position, transform.position);

        if (dist <= useRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Button Pressed");
            }
        }
    }
}
