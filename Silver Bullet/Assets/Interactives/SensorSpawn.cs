using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditorInternal.VersionControl;
using UnityEngine;

public class SensorSpawn : MonoBehaviour
//DESPITE THIS SCRIPT'S NAME
//THIS CAN ALSO WORK FOR DOORS THAT GO UP AND DOWN
//HAVE FUN!
{
    public GameObject enemy;
    [SerializeField] GameObject target;
    [SerializeField] float cooldown;
    [SerializeField] int limit;
    public int limitCounter = 0;
    public bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (limitCounter >= limit)
        {
            active = false;
        }
        if (active == false)
        {
            if (limitCounter != 0)
            {
            StopAllCoroutines();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (active == false)
            {
            StartCoroutine(Horde());
            Debug.Log("Player Entered Zone");
            }
        }
    }

    IEnumerator Horde()
    {
        limitCounter++;
        active = true;
        Instantiate (enemy, target.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(cooldown);
        StartCoroutine(Horde());
    }
}
