using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCol : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, 0.1f);
    }
}
