using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehaviour : MonoBehaviour {
    public float Life = 100;
    public GameObject Impact;

    public void Damage(float x, Vector3 point)
    {
        if (Life > 0)
        {
            Destroy(Instantiate(Impact, point, Quaternion.identity), .5f);
            Life -= x;
            if (Life <= 0)
                Kill();
        }
    }

    void Kill()
    {
        Destroy(gameObject);
    }
}
