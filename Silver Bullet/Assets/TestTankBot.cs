using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTankBot : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Transform player;
    [SerializeField] Rigidbody2D tankBotRB;

    public bool flip;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = transform.localScale;

        if (player.transform.position.x > transform.position.x) //check if player's position is greater than enemy's position
        {
            scale.x = Mathf.Abs(scale.x) * -1 * (flip? -1: 1);
            transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
        }
        else
        {
            scale.x = Mathf.Abs(scale.x) * (flip ? -1 : 1);
            transform.Translate(moveSpeed * Time.deltaTime * -1, 0, 0);
        }

        transform.localScale = scale;
    }
}
