using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    //Jump/ movement variables
    bool isGrounded = false;
    bool touchingWater = false;
    public float moveSpeed = 5.0f;
    public float jumpHeight = 0.75f;
    public GameObject Harmonica;

    //First person Look at variables
    Vector2 mouseLook;
    Vector2 smoothV;
    float sensitivity = 5.0f;
    float smoothing = 2.0f;

    //Head bob variables
    float bobSpeed = 4.8f;
    float bobAmount = 0.1f;
    float timer = Mathf.PI / 2;
    float transitionSpeed = 20.0f;
    Vector3 restPos;
    Vector3 camPos;

    //Ammo count and UI element
    public int ammo = 6;
    public Text ammoCount;

    //Gameobject for the flashlight
    public GameObject flashlight;

    //Audio clips for the water and dirt footsteps
    public List<AudioClip> dirtFootsteps = new List<AudioClip>();
    public List<AudioClip> waterFootsteps = new List<AudioClip>();
   
    //Audio clips for the landing in dirt/water
    public AudioClip dirtLanding;
    public AudioClip waterLanding;

    public List<AudioClip> heartbeats = new List<AudioClip>();

    //Audio source for the footsteps and landing sounds
    public AudioSource feetSource;

    //Gunshot and shell casing audio clip + gun source
    public AudioClip gunshot;
    public AudioClip shellcasing;
    public AudioSource Gun;


    //Timer for determining if the next footstep should be played
    float StepTime = 0.5f;
    public float stepRate = 0.5f;

    float heartRate = 1.0f;
    float heartTime = 1;
    bool gameOver = false;

    public GameObject prompt;

    // Start is called before the first frame update
    void Start()
    {
        heartTime = heartRate;
        StepTime = stepRate;
        //Initialise the camera bob positions
        restPos = Camera.main.transform.localPosition;
        camPos = Camera.main.transform.localPosition;
        prompt.SetActive(false);

        for (int i = 0; i < waterFootsteps.Count; i++)
        {
            float[] samp = new float[waterFootsteps[i].samples];
            waterFootsteps[i].GetData(samp, 0);

            waterFootsteps[i] = AudioManager.CreateClip(AudioManager.LowPassFilter(new List<float>(samp)), "WaterFootstep" + i.ToString());
        }


    }

    void PlayHarmonica()
    {

        Harmonica.GetComponent<AudioSource>().clip = AudioManager.GenerateHarmonica(Random.Range(-21, 3));

        Harmonica.GetComponent<AudioSource>().Play();
       
    }
    public void SetGameOver(bool g)
    {
        gameOver = g;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            HandleInput();

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out interactable, 20.0f, InteractablesMask))
            {
                prompt.SetActive(true);
            }
            else
            {
                prompt.SetActive(false);
            }
        }
    }

    public LayerMask InteractablesMask;
    RaycastHit interactable;
    //This fuction handles player input
    void HandleInput()
    {
        //Apply a jump force when the player presses the jump  button while touching the ground
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * Mathf.Sqrt(2.0f * -jumpHeight * Physics.gravity.y), ForceMode.VelocityChange);
        }

        if (Input.GetButtonDown("Fire1") && ammo > 0)
        {
            //Shoot Gun Here
            //Play Gunshot sound here
            //Shell casing dropping sound 

            float[] gunshotSamples = new float[gunshot.samples * gunshot.channels];
            gunshot.GetData(gunshotSamples, 0);
            Gun.PlayOneShot(AudioManager.CreateClip(AudioManager.Reverb(new List<float>(gunshotSamples), 25000, 3), "GunshotEcho"));
            AudioSource.PlayClipAtPoint(shellcasing, transform.position + new Vector3(0, -2, 0));
            ammo--;
        }

        if(Input.GetButtonDown("Fire2"))
        {
            flashlight.SetActive(true);
        }

        if(Input.GetButtonUp("Fire2"))
        {
            flashlight.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out interactable, 20.0f, InteractablesMask))
            {
                PlayHarmonica();
            }
        }
        //Pressing escape on the keyboard will return the cursor to the screen
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        ammoCount.text = ammo.ToString();
    }

    public void SetHB(float vol, float rate, bool reset)
    {
        if (!reset)
        {
            hVol += vol;
            heartRate -= rate;
        }
        else
        {
            hVol = vol;
            heartRate = rate;
        }
    }
    float hVol = 1.0f;


    //This function handles physics calculations every cycle such as player movement
    private void FixedUpdate()
    {
        if (!gameOver)
        {
            heartTime -= Time.deltaTime;

            if (heartTime <= 0)
            {
                heartTime = heartRate;
                int hIndex = Random.Range(0, heartbeats.Count);
                AudioSource.PlayClipAtPoint(heartbeats[hIndex], Camera.main.transform.position, hVol);
            }

            //Apply gravity if the player is not grounded
            if (!isGrounded)
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * Physics.gravity.y);
            }

            //Apply a headbob to the camera if the player is moving 
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                timer += bobSpeed * Time.deltaTime;
                Vector3 newPos = new Vector3(Mathf.Cos(timer) * bobAmount, restPos.y + Mathf.Abs(Mathf.Sin(timer) * bobAmount), restPos.z);
                Camera.main.transform.localPosition = newPos;

                //decrement timer 
                StepTime -= Time.deltaTime;

                if (StepTime <= 0)
                {
                    StepTime = stepRate;

                    //Play footstep sounds here 
                    if (isGrounded)
                    {
                        int fIndex = 0;

                        //if the timer is less than 0 and it is time to play the next footstep sound
                        if (dirtFootsteps.Count > 0
                            && waterFootsteps.Count > 0)
                        {

                            //check if the player is in water or on land and play a random footstep sound from the appropriate list
                            if (touchingWater)
                            {
                                fIndex = Random.Range(0, waterFootsteps.Count);
                                //GetComponent<AudioSource>().PlayOneShot(waterFootsteps[fIndex]);
                                feetSource.clip = waterFootsteps[fIndex];
                                if (!feetSource.isPlaying)
                                {
                                    feetSource.Play();
                                }
                            }
                            else
                            {
                                fIndex = Random.Range(0, dirtFootsteps.Count);
                                feetSource.clip = dirtFootsteps[fIndex];
                                if (!feetSource.isPlaying)
                                {
                                    feetSource.Play();
                                }
                                // GetComponent<AudioSource>().PlayOneShot(dirtFootsteps[fIndex]);
                            }

                        }
                    }
                }
            }
            else
            {
                timer = Mathf.PI / 2;
                Vector3 newPos = new Vector3(Mathf.Lerp(camPos.x, restPos.x, transitionSpeed * Time.deltaTime), Mathf.Lerp(camPos.y, restPos.y, transitionSpeed * Time.deltaTime), Mathf.Lerp(camPos.z, restPos.z, transitionSpeed * Time.deltaTime));
                Camera.main.transform.localPosition = newPos;
            }

            if (timer > Mathf.PI * 2)
            {
                timer = 0;
            }

            //Turn and move the player if applicable
            Turn();
            Move(-Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"));
        }
    }

    //This function handles the rotation of the camera based on the mouse position for a first person controller
    void Turn()
    {
        Vector2 md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1.0f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1.0f / smoothing);

        mouseLook += smoothV;

        mouseLook.y = Mathf.Clamp(mouseLook.y, -65.0f, 65.0f);

        Camera.main.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        GetComponent<Rigidbody>().MoveRotation(Quaternion.AngleAxis(mouseLook.x, transform.up));
    }

    //This function handles the player movement foward/back, and left/right based on input
    void Move(float h, float v)
    {
        Vector3 movement = new Vector3(h, 0.0f, v);
        movement = Camera.main.transform.forward * (-movement.x) + Camera.main.transform.right * movement.z;
        movement.y = 0.0f;
        movement = movement.normalized * moveSpeed * Time.deltaTime;
        GetComponent<Rigidbody>().MovePosition(transform.position + movement);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            if (!isGrounded)
            {
                feetSource.clip = waterLanding;
                if(!feetSource.isPlaying)
                {
                    feetSource.Play();
                }
               // GetComponent<AudioSource>().PlayOneShot(waterLanding);
                touchingWater = true;
            }
        }

        if (other.gameObject.tag == "Ground")
        {
            if (!touchingWater)
            {
                feetSource.clip = dirtLanding;
                if (!feetSource.isPlaying)
                {
                    feetSource.Play();
                }
                //GetComponent<AudioSource>().PlayOneShot(dirtLanding);
            }
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
