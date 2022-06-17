using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public Transform shotDirection;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(shotDirection.position, shotDirection.transform.forward * 10);
    }

    public void CanShoot()
    {
        GameState.canShot = true;
    }

    public void Shooting()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(shotDirection.transform.position, shotDirection.transform.forward, out hitInfo, 300))
        {
            if (hitInfo.collider.gameObject.GetComponent<ZombiCon>() != null)
            {
                ZombiCon hitzonbi = hitInfo.collider.gameObject.GetComponent<ZombiCon>();

                hitzonbi.ZonbiDeath();
            }
        }
    }
}
