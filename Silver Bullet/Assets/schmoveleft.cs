using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class schmoveleft : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float schmoveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = Vector2.left * schmoveSpeed;
    }
}
