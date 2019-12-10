using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
   
    bool touchingWater = false;
    bool isGrounded = false;
 

    public bool getGrounded()
    {
        return isGrounded;
    }

    public bool getWater()
    {
        return touchingWater;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Water")
        {
            touchingWater = true;
        }

        if(other.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            touchingWater = true;
        }

        if (other.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            touchingWater = false;
        }

        if (other.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

}
