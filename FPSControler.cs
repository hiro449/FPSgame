using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSControler : MonoBehaviour
{

    float x, z;

    float speed = 0.1f;

    public GameObject cam;
    Quaternion cameraRot, characterRot;

    float Xsensivility = 3f, Ysensivility = 3f;

    bool cursorLock = true;

    float minX = -90f, maxX = 90f;

    public Animator animater;

    int ammunition = 50, maxAmmunition = 50, ammouclip = 10, maxAmmouclip = 10;

    int playerHP = 100, maxPlayerHP = 100;

    public Slider HPslider;
    public Text ammoText;

    public GameObject mainCamera, subCamera;




    // Start is called before the first frame update
    void Start()
    {
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;

        GameState.canShot = true;

        HPslider.value = playerHP;
        ammoText.text = ammouclip + "/" + ammunition;
    }

    // Update is called once per frame
    void Update()
    {
        float xRot = Input.GetAxis("Mouse X") * Ysensivility;
        float yRot = Input.GetAxis("Mouse Y") * Xsensivility;

        cameraRot *= Quaternion.Euler(-yRot,0,0);
        characterRot *= Quaternion.Euler(0, xRot, 0);

        cameraRot = ClampRotation(cameraRot);

        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;

        UpDateLock();

        if (Input.GetMouseButton(0) && GameState.canShot)
        {
            if (ammouclip > 0)
            {
            animater.SetTrigger("Fire");
            GameState.canShot = false;
                ammouclip--;
                ammoText.text = ammouclip + "/" + ammunition;
            }
            else
            {
                Debug.Log("’e‚ª‚È‚¢");
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            int ammouNeed = maxAmmouclip - ammouclip;
            int ammouAvailable = ammouNeed < ammunition ? ammouNeed : ammunition;

            if (ammouNeed != 0 && ammunition != 0)
            {
            animater.SetTrigger("Reload");
                ammunition -= ammouAvailable;
                ammouclip += ammouAvailable;
                ammoText.text = ammouclip + "/" + ammunition;
            }

        }

        if (Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0)
        {
            if (!animater.GetBool("Walk"))
            {
                animater.SetBool("Walk", true);
            }
        } else if (animater.GetBool("Walk"))
        {
            animater.SetBool("Walk", false);
        }


        if (z > 0 && Input.GetKey(KeyCode.LeftShift))
        {
            if (!animater.GetBool("Run"))
            {
                animater.SetBool("Run", true);
                speed = 0.25f;
            }
        }
        else if (animater.GetBool("Run"))
        {
            animater.SetBool("Run", false);
            speed = 0.1f;
        }

        if (Input.GetMouseButton(1))
        {
            subCamera.SetActive(true);
            mainCamera.GetComponent<Camera>().enabled = false;
        } else if (subCamera.activeSelf)
        {
            subCamera.SetActive(false);
            mainCamera.GetComponent<Camera>().enabled = true;
        }

    }

    private void FixedUpdate()//0.02•b‚²‚Æ‚É
    {
        x = 0;
        z = 0;

        x = Input.GetAxisRaw("Horizontal") * speed;
        z = Input.GetAxisRaw("Vertical") * speed;

        // transform.position += new Vector3(x, 0, z);
        transform.position += cam.transform.forward * z + cam.transform.right * x;
    }

    public void UpDateLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        } else if (Input.GetMouseButton(0))
        {
            cursorLock = true;
        }

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;   //show
        }
    }

    public Quaternion ClampRotation(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;

        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);
        return q;
    }

    public void TakeHP(float damage)
    {
        playerHP = (int)Mathf.Clamp(playerHP - damage, 0, playerHP);
        HPslider.value = playerHP;

        if (playerHP <= 0 && !GameState.GameOver)
        {
            GameState.GameOver = true;
        }
    }
}
