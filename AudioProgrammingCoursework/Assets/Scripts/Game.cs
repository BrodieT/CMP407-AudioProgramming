using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Game : MonoBehaviour
{
    //Get campfire game object in order to handle the burnout and affect the music accordingly
    public GameObject campfire;
    public GameObject fireLight;
    
    //Game timer for campfire burnout
    float timer = 10.0f;

    //Tracks if the game is finished
    bool gameOver = false;

    bool spawnEnemy = false;

    public GameObject enemy;
    public GameObject player;

    //Raycast variables to find the enemy
    public LayerMask enemyMask;
    RaycastHit hit;

    //Store all the game manager's audio sources
    AudioSource[] audioSources;

    //Starting variables for the restart function
    Vector3 enemySpawnPos = new Vector3();
    Vector3 playerSpawnPos = new Vector3();
    float startFireMinIntensity = new float();
    float startFireMaxIntensity = new float();

    // Start is called before the first frame update
    void Start()
    {
        //Hide the cursor from view
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //get all audio sources on the game manager 
        audioSources = GetComponents<AudioSource>();

        //Generate synthesised audio
        AudioManager.GenerateAudio();

        //assign the background music to the first two audio sources
        audioSources[0].clip = (AudioManager.getAccomp());
        audioSources[1].clip = AudioManager.getBass();
        audioSources[0].volume = 0.002f;
        audioSources[1].volume = 0.002f;

        //Store starting values
        startFireMinIntensity = fireLight.GetComponentInChildren<AnimatedFire>().MinIntensity;
        startFireMaxIntensity = fireLight.GetComponentInChildren<AnimatedFire>().MaxIntensity;
        enemySpawnPos = enemy.transform.position;
        playerSpawnPos = player.transform.position;

        //Init UI
        UI.SetActive(true);
        menu.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {      
        //decrement game timer
        timer -= Time.deltaTime;

        //Proceed with the burnout of the fire if the enemy hasnt been spawned in yet
        if (!spawnEnemy)
        {
            //when the timer is less than 0, reset the timer and decrement the intensity of the fire visuals
            if (timer <= 0.0f)
            {
                if (fireLight.GetComponentInChildren<AnimatedFire>().MaxIntensity > 0)
                {
                    timer = 5.0f;
                    fireLight.GetComponentInChildren<AnimatedFire>().MinIntensity = Mathf.Lerp(fireLight.GetComponentInChildren<AnimatedFire>().MinIntensity, 0.0f, 0.5f);

                    fireLight.GetComponentInChildren<AnimatedFire>().MaxIntensity = Mathf.Lerp(fireLight.GetComponentInChildren<AnimatedFire>().MaxIntensity, 0.0f, 0.5f);

                    player.GetComponent<CharacterController>().SetHB(0.3f, 0.05f, false);
                }
            }

            //Spawn in the enemy, deactivate the fire, and start playing the creepy background music
            if(fireLight.GetComponentInChildren<AnimatedFire>().MaxIntensity <= 0.1f)
            {
                campfire.GetComponent<AudioSource>().volume = 0;
                fireLight.SetActive(false);
                StartCoroutine("LoopSound");
                enemy.SetActive(true);
                spawnEnemy = true;
            }
        }
        
        //If the player fires the gun, check to see if it hits the enemy and if so play the win sfx and set gameover to true
        
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100, enemyMask))
        {
            if (Input.GetButtonDown("Fire1") && player.GetComponent<CharacterController>().ammo > 0)
            {

                audioSources[0].Stop();
                audioSources[1].Stop();

                enemy.GetComponent<EnemyController>().isDead = true;

                if (!audioSources[2].isPlaying && !gameOver)
                {
                    gameOver = true;
                    player.GetComponent<CharacterController>().SetGameOver(gameOver);
                    gOverTxt = "You Win!";

                    //Ragdoll enemy
                    enemy.transform.Rotate(new Vector3(90, 0, 0));
                    audioSources[2].clip = AudioManager.getWin();
                    audioSources[2].Play();
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            audioSources[2].clip = AudioManager.getWin();
            audioSources[2].Play();

        }
        //Handles if the enemy catches the player and the lose state
        if (enemy.GetComponent<EnemyController>().caught && !gameOver)
        {
            audioSources[2].clip = AudioManager.getLose();

            audioSources[0].Stop();
            audioSources[1].Stop();

            if (!audioSources[2].isPlaying && !gameOver)
            {
                gameOver = true;
                player.GetComponent<CharacterController>().SetGameOver(gameOver);
                gOverTxt = "You Lose!";
                audioSources[2].Play();
            }
        }

        if(gameOver && !audioSources[2].isPlaying)
        {
            UI.SetActive(false);
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gOverTxtObj.text = gOverTxt;
        }

    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        campfire.GetComponent<AudioSource>().volume = 1;
        fireLight.SetActive(true);

        fireLight.GetComponentInChildren<AnimatedFire>().MinIntensity = startFireMinIntensity;
        fireLight.GetComponentInChildren<AnimatedFire>().MaxIntensity = startFireMaxIntensity;
        enemy.transform.SetPositionAndRotation(enemySpawnPos, Quaternion.Euler(0,0,0));
        enemy.GetComponent<Animator>().speed = 1;
        player.GetComponent<CharacterController>().Gun.Stop();
        player.transform.position = playerSpawnPos;
        enemy.GetComponent<EnemyController>().caught = false;
        enemy.GetComponent<EnemyController>().isDead = false;

        gameOver = false;
        spawnEnemy = false;
        timer = 5.0f;
        enemy.SetActive(false);
        UI.SetActive(true);
        menu.SetActive(false);
        player.GetComponent<CharacterController>().SetGameOver(gameOver);
        player.GetComponent<CharacterController>().SetHB(1, 1, true);
        player.GetComponent<CharacterController>().ammo = 2;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    public GameObject UI;
    public GameObject menu;
    public Text gOverTxtObj;
    string gOverTxt;

    IEnumerator LoopSound()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        int i = 0;

        while (i < 1)
        {
            if(!audioSources[0].isPlaying)
            {
                audioSources[0].Play();

            }
            if (!audioSources[1].isPlaying)
            {
                audioSources[1].Play();
            }

            i++;

            yield return null;
        }
    }
}
