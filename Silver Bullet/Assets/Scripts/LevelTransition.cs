using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement; 

public class LevelTransition : MonoBehaviour
{
    [SerializeField] int level;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        //Letting it detect the player so the player prefab doesnt get gunked up. 
        {
            SceneManager.LoadScene(level);
        }
    }
}
