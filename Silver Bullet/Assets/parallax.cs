using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallax : MonoBehaviour
{
    [SerializeField] float depth = 5f;
    public GameObject player;
    public Vector2 playerPos;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x / depth, player.transform.position.y / depth, 0);
    }
}
