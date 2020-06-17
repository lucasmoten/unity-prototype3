using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /*
     * Customizations
     * 
     * Changed the player to a Rat character downloaded from asset store.
     * Tuned animations for rat to setup and transition running, jumping, and dying.
     * Created a foreground using models from another free asset pack. 
     * Speed of foreground, ground, and background adjusted for retro depth race effect. 
     * Spawns obstacles in different 'lanes'. Added ability to change lanes via arrow keys.
     * Ground now looks like a path to run along.  
     * Scoring system based on time, jumping and changing lanes.
     * Use of player preferences to store and retrieve high score value.
     * UI elements for name, progress through level, score, game over and level done.
     * Ability to restart to try for another high score.
     * 
     */


    private Rigidbody playerRb;
    public float jumpForce;
    public float gravityModifier;
    public bool isOnGround = true;
    public bool gameOver;
    private Animator playerAnim;
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;
    public AudioClip jumpSound;
    public AudioClip crashSound;
    private AudioSource playerAudio;
    private float wantedZ;
    private int score;
    private int level;
    private int hiScore;
    private int hiLevel;
    public GameObject scoreText;
    public GameObject playerProgress;
    public GameObject DeadPanel;
    public GameObject LevelDonePanel;
    public GameObject personalBestScoreText;

    private float maxZ = 3.25f;
    private float minZ = -2.50f;
    private float laneDampening = 5.0f;
    private int scoreTiming = 15;
    private int scoreJumping = 50;
    private int scoreLaneChange = 25;
    private System.Random rnd;
    private float progressMinX = -325;
    private float progressMaxX = 325;
    private float levelDuration = 60f;
    public bool levelDone;
    private float timePlayStarted = 0f;


    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        Physics.gravity *= gravityModifier;
        rnd = new System.Random();
        hiScore = PlayerPrefs.GetInt("HiScore", 0);
        hiLevel = PlayerPrefs.GetInt("HiLevel", 1);
        personalBestScoreText.GetComponent<Text>().text = hiLevel.ToString() + ":" + hiScore.ToString();
        ResetGame();
        InvokeRepeating("ScoreByTime", 0, .5f);
    }

    public void ResetGame()
    {
        timePlayStarted = Time.timeSinceLevelLoad;
        score = 0;
        level = 1;
        wantedZ = 0;
        DeadPanel.SetActive(false);
        LevelDonePanel.SetActive(false);
        gameOver = false;
        levelDone = false;
        playerAnim.SetBool("Dead", false);
        levelDuration = 10f;
        clearObstacles();
    }

    private void clearObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles) Destroy(obstacle);
    }

    public void NextLevel()
    {
        score += 1000;
        level++;
        if (level > hiLevel)
        {
            hiLevel = level;
            PlayerPrefs.SetInt("HiLevel", hiLevel);
        }
        LevelDonePanel.SetActive(false);
        timePlayStarted = Time.timeSinceLevelLoad;
        levelDuration += 10f;
        levelDone = false;
        clearObstacles();
    }

    void ScoreByTime()
    {
        if (!gameOver && !levelDone)
        {
            float percentComplete = ((Time.timeSinceLevelLoad - timePlayStarted) / levelDuration);
            if (percentComplete > 1.0f)
            {
                percentComplete = 1.0f;
                levelDone = true;
                //gameOver = true;
                LevelDonePanel.SetActive(true);
                LevelDonePanel.GetComponentInChildren<Button>().Select();
            }
            float progressLength = progressMaxX - progressMinX;
            float progressX = progressMinX + (progressLength * percentComplete);
            playerProgress.transform.localPosition = new Vector3(progressX, -17.4f, 0);
            score += rnd.Next(scoreTiming);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnGround && !gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                score += rnd.Next(scoreJumping);
                playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isOnGround = false;
                dirtParticle.Stop();
                playerAnim.SetBool("Grounded", false);
                playerAnim.SetTrigger("Jump_trig");
                playerAudio.PlayOneShot(jumpSound, 1.0f);
            }

            // Lane changing
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                score += rnd.Next(scoreLaneChange);
                wantedZ = ((wantedZ < 0) ? 0 : maxZ);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                score += rnd.Next(scoreLaneChange);
                wantedZ = ((wantedZ > 0) ? 0 : minZ);
            }
            float currentZ = transform.position.z;
            currentZ = Mathf.Lerp(currentZ, wantedZ, laneDampening * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);
        }
    }

    private void FixedUpdate()
    {
        //score += Time.timeSinceLevelLoad
        scoreText.GetComponent<Text>().text = level.ToString() + ":" + score.ToString();

        if(score > hiScore)
        {
            hiScore = score;
            PlayerPrefs.SetInt("HiScore", hiScore);
            personalBestScoreText.GetComponent<Text>().text = hiLevel.ToString() + ":" + hiScore.ToString();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            playerAnim.SetBool("Grounded", true);
            if(!gameOver) dirtParticle.Play();
        } else if (collision.gameObject.CompareTag("Obstacle"))
        {
            dirtParticle.Stop();
            explosionParticle.Play();
            playerAnim.SetBool("Dead", true);
            playerAudio.PlayOneShot(crashSound, 1.0f);
            gameOver = true;
            DeadPanel.SetActive(true);
            DeadPanel.GetComponentInChildren<Button>().Select();
            Debug.Log("Game Over!");
        }
        
    }
}
