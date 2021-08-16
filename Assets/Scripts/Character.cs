using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public float speed;
    public float turnSmoothTime;
    float turnSmoothVelocity;
    public Transform cam;

    [Header("Sounds")]
    public AudioClip itemPick;
    public AudioClip hurt;
    AudioSource audioSource;


    [Header("Atrribute")]
    public int playerLife;
    public Image healthBar;

    Animator animator;
    StageManager stageManager;
    public bool hasDied = false;

    //we are getting the animator and audiosource component from the player character gameobject
    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        stageManager = FindObjectOfType<StageManager>(); //we are finding the stageManager class in game scene, so we can use it later

        healthBar.fillAmount = playerLife / 100f; //update healthbar to full on game start
    }
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(-horizontal, 0f, -vertical).normalized;


        //calculate player direction and smoothly turn and walk towards the direction, camera is facing. we are using the atan2 formula to achieve this
        if (!hasDied && stageManager.timerIsRunning)
        {
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) / Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                //move player forward, and switch to walk animation
                Vector3 moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
                animator.SetBool("walk", true);
                controller.Move(moveDirection.normalized * speed * Time.deltaTime);

                //if we detect the shift button held on, then switch to run animation and double player speed, else switch to walk animation.
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    animator.SetBool("run", true);
                    speed = 6f;
                }
                else
                {
                    speed = 3f;
                    animator.SetBool("run", false);
                }
            }
            else
            {
                //if the wasd button isn't pressed, then switch to idle animation, and reset player speed
                speed = 3f;
                animator.SetBool("run", false);
                animator.SetBool("walk", false);
            }


            if (playerLife == 0)
            {
                controller.detectCollisions = false;
                hasDied = true;
            }
        } else if (!stageManager.timerIsRunning)
        {
            animator.SetBool("run", false);
            animator.SetBool("walk", false);
        } else if (hasDied)
        {
            animator.SetTrigger("death");
        }

    }

    //this function reduces player health by the integer value passed, as well as updates the healthbar fill UI
    public void DealDamage(int amount)
    {
        playerLife -= amount;
        healthBar.fillAmount = playerLife / 100f;
    }

    //We are detecting orb item, enemies and obstacles hit, by entering trigger collider attached to them
    private void OnTriggerEnter(Collider other)
    {
        //if collider is a orb, we increment by number of keys collected by 1
        if (other.CompareTag("Orb"))
        {
            stageManager.noOfOrbsCollected += 1;
            stageManager.UpdateUI();
            Destroy(other.gameObject);
            audioSource.PlayOneShot(itemPick);
        }

        //if we collide with an enemy, we are reducing player health by 20, and updating the healthbar fill UI
        else if (other.CompareTag("Enemy"))
        {
            DealDamage(20);
            audioSource.PlayOneShot(hurt);
        }

        //if we collide with an obstacle, we are reducing player health by 5, and updating the healthbar fill UI
        else if (other.CompareTag("Obstacle"))
        {
            DealDamage(10);
            audioSource.PlayOneShot(hurt);

        //if we collider with door trigger, open if all orbs have been collected, else remain closed
        } else if (other.CompareTag("Door"))
        {
            if (stageManager.noOfOrbsCollected == stageManager.noOfOrbs)
            {
                other.GetComponent<Animator>().enabled = true;
                stageManager.musicSource.volume = 0.3f;
                other.GetComponent<AudioSource>().Play();
                FindObjectOfType<CameraController>().Target = null;
            } else
            {
                stageManager.source.PlayOneShot(stageManager.doorLocked);
                stageManager.extraTip.gameObject.SetActive(true);
                stageManager.extraTip.text = "COLLECT ALL ORBS TO UNLOCK DOOR";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            if (stageManager.noOfOrbsCollected != stageManager.noOfOrbs)
            {
                stageManager.extraTip.gameObject.SetActive(false);
                stageManager.extraTip.text = "";
            } else
            {
                stageManager.musicSource.Stop();
                stageManager.source.PlayOneShot(stageManager.complete);
                Invoke(nameof(LoadGameWin), 3f);
            }
        }
    }

    //misc function
    void LoadGameWin()
    {
        stageManager.LoadGameWin();
    }
}
