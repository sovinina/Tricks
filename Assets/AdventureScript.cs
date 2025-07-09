using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdventureScript : MonoBehaviour
{
    public Vector3 mousePos;
    public Camera mainCamera;
    public Vector3 mousePosWorld;
    public Vector2 mousePosWorld2D;
    RaycastHit2D hit;

    public GameObject player;
    public GameObject foxie;
    public GameObject stick;
    public GameObject stickMenu;
    public GameObject stone;
    public GameObject stoneMenu;
    public GameObject trap;
    public GameObject trap2;
    public GameObject answer;
    public GameObject menu;
    public GameObject cross;
    public GameObject ending;
    public GameObject owl;

    public AudioClip cry;
    public AudioClip whimper;
    public AudioClip yay;
    public AudioClip footsteps;

    AudioSource audioFoxie;
    AudioSource audioPlayer;
    AudioSource audioOwl;

    public Vector2 targetPos;
    public float speed;

    public bool isMoving;
    public bool isStick;
    public bool isStone;
    public bool isFoxie;

    private bool isTrap = false;
    private bool isTrapShown = false;

    private bool flipHint;
    private bool flipFoxie;

    Vector2 mouse;
    public Texture2D cursor;

    private string currAnimation;

    Animator playerAnimator;
    Animator foxieAnimator;
    Animator owlAnimator;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = player.GetComponent<Animator>();
        foxieAnimator = foxie.GetComponent<Animator>();
        owlAnimator= owl.GetComponent<Animator>();
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        audioFoxie = foxie.GetComponent<AudioSource>();
        audioPlayer = player.GetComponent<AudioSource>();
        audioPlayer.Stop();
        audioOwl = owl.GetComponent<AudioSource>();
        audioOwl.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;

            //print(isFoxie);

            mousePosWorld = mainCamera.ScreenToWorldPoint(mousePos);

            print(mousePosWorld);

            mousePosWorld2D = new Vector2(mousePosWorld.x, mousePosWorld.y);

            ContactFilter2D filter2D = new ContactFilter2D();
            filter2D.NoFilter();

            List<RaycastHit2D> hits = new();

            var hitCount = Physics2D.Raycast(mousePosWorld2D, Vector2.zero, filter2D, hits);


            bool hasFoxie = false;
            bool hasMoving = true;
            flipHint = false;
            flipFoxie = false;

            foreach (var hit in hits)
            {
                print(hit.collider.gameObject.tag);

                //print(hit.point);

                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                }
                else if (hit.collider.gameObject.CompareTag("CutScene"))
                {
                    hasMoving = false;
                }
                else if (hit.collider.gameObject.CompareTag("Stick"))
                {
                    isStick = true;
                }
                else if (hit.collider.gameObject.CompareTag("Stone"))
                {
                    isStone = true;
                }
                else if (hit.collider.gameObject.CompareTag("Foxie"))
                {
                    hasMoving = false;
                    flipFoxie = true;
                    audioPlayer.Play();
                    Moving(new Vector2(-6.68f, -2.85f));

                    if (isTrapShown == true)
                    {
                        hasFoxie = true;
                    }
                    else
                    {
                        isTrapShown = true;
                        isTrap = true;
                    }
                }
                else if (hit.collider.gameObject.CompareTag("StickMenu"))
                {
                    cross.SetActive(true);
                    hasMoving = false;
                    async Task IncorrectItem()
                    {
                        audioFoxie.Stop();
                        audioFoxie.clip = cry;
                        audioFoxie.Play();

                        await Task.Delay(1230);

                        audioFoxie.Stop();
                        audioFoxie.clip = whimper;
                        audioFoxie.Play();

                    }
                    _ = IncorrectItem();
                }
                else if (hit.collider.gameObject.CompareTag("StoneMenu"))
                {
                    foxieAnimator.Play("wii");
                    hasMoving = false;

                    audioFoxie.Stop();
                    audioFoxie.clip = yay;
                    audioFoxie.Play();

                    async Task EndingSceneLoad()
                    {
                        await Task.Delay(2000);

                        SceneManager.LoadScene("Ending");
                       
                    }
                    _ = EndingSceneLoad();

                }
                else if (hit.collider.gameObject.CompareTag("hint"))
                {
                    hasMoving = false;
                    owlAnimator.Play("speaking");
                    audioPlayer.Play();
                    Moving(new Vector2(11.42f, -4.02f));
                    flipHint = true;                        
                }
            }

            if (hasMoving == true)
            {
                audioPlayer.Play();
                Moving(hits[0].point);
            }

            isFoxie = hasFoxie;

        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
        
    }

    private void FixedUpdate()
    {
        if(isMoving)
        {
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPos, speed);

            ChangeAnimation("walk");

            if (!isFoxie)
            {
                menu.SetActive(false);
            }

            if (player.transform.position.x == targetPos.x && player.transform.position.y == targetPos.y)
            {
                isMoving = false;
                audioPlayer.Stop();

                ChangeAnimation("idle");


                if (isStick)
                {
                    stick.SetActive(false);
                    stickMenu.SetActive(true);
                }

                if (isStone)
                {
                    stone.SetActive(false);
                    stoneMenu.SetActive(true);
                }

                if (isFoxie)
                {
                    menu.SetActive(true);
                    player.GetComponent<SpriteRenderer>().flipX = false;
                }

                if (isTrap)
                {
                    trap.SetActive(true);

                    async Task TrapDialog()
                    {
                        await Task.Delay(3500);

                        trap.SetActive(false);

                    }
                    _ = TrapDialog();

                    isTrap = false;
                }

                if (flipHint)
                {
                    player.GetComponent<SpriteRenderer>().flipX = true;

                    trap2.SetActive(true);

                    async Task TrapDialog()
                    {
                        await Task.Delay(1500);

                        trap2.SetActive(false);

                        await Task.Delay(2500);

                        answer.SetActive(true);

                        await Task.Delay(2000);

                        answer.SetActive(false);

                    }
                    _ = TrapDialog();

                    audioOwl.Play();

                    isTrap = false;
                }

                if (flipFoxie)
                {
                    player.GetComponent<SpriteRenderer>().flipX = false;
                }
            }
        }
        

    }

    void Moving(Vector2 point)
    {
        targetPos = point;
        isMoving = true;
        CheckSpriteFlip();
    }
    void ChangeAnimation(string animation)
    {
        if (currAnimation == animation) return;

        playerAnimator.Play(animation);
        currAnimation = animation;
    }
    void CheckSpriteFlip()
    {
        if(player.transform.position.x > targetPos.x)
        {
            player.GetComponent<SpriteRenderer>().flipX= false;
        }
        else
        {
            player.GetComponent<SpriteRenderer>().flipX= true;
        }
    }
}
