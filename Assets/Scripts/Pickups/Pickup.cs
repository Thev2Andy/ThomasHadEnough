using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public string Identifier;

    public void Collect()
    {
        Destroy(this.gameObject);
    }
}
