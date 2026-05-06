using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBackZone : MonoBehaviour
{
    [SerializeField] GameObject pushBackZone;
    [SerializeField] EnemyHealth1 enemyHealth1;
    // Start is called before the first frame update
    void Start()
    {
        pushBackZone.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {

        if (enemyHealth1.bossTookDamage)
        {
            StartCoroutine(PushPlayerOnDamage());
        }
    }

    IEnumerator PushPlayerOnDamage()
    {
        pushBackZone.SetActive(true);
        yield return new WaitForSeconds(1f);
        pushBackZone.SetActive(false);
        enemyHealth1.bossTookDamage = false;
    }
}
