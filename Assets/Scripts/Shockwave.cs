using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    Vector3 ShockwaveScale;
    int destroyTimer = 120;

    // Start is called before the first frame update
    void Start()
    {
        ShockwaveScale = this.transform.localScale;
        destroyTimer = 120;
    }

    // Update is called once per frame
    void Update()
    {
        destroyTimer--;
        if ( destroyTimer <= 0 )
        {
            Destroy(this.gameObject);
        }
        ShockwaveScale = new Vector3(ShockwaveScale.x + 0.1f, ShockwaveScale.y + 0.1f, ShockwaveScale.z + 0.1f);

        this.transform.localScale = ShockwaveScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Asteroid>() != null)
        {
            Game.AsteroidDestroyed(collision.gameObject.transform.position);
            Destroy(collision.gameObject);
        }
    }
}
