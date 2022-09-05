using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    //Native Components on the same object as this script =======================================
    [Header("Components")]
    Rigidbody2D rb;

    //Stats concerning horizontal movement ======================================================
    [Space(2)]
    [Header("Horizontal Movement")]
    [SerializeField] float runSpeed;
    [SerializeField] float rollTime;
    [SerializeField] float rollSpeed;
    [SerializeField] float rollDelay;
    private float nextAvailableRollTime = 0;

    //Stats concerning vertical movement ========================================================
    [Space(2)]
    [Header("Vertical Movement")]
    [SerializeField] float jumpSpeed;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2;

    //States the player might be in =============================================================
    [Space(2)]
    [Header("Conditions")]
    [SerializeField] Vector2 jumpColliderBottomOffset;
    [SerializeField] float jumpColliderRadius;
    private bool isTouchingGround = false;
    public bool isRolling = false;
    public bool isSwitching = false; //This might be turned public at some point

    //Variables concerning gun switching ========================================================
    [Space(2)]
    [Header("Gun Switching")]
    [SerializeField] float switchTime = 1; //how long does it take to switch guns?
    [SerializeField] int firstGunID = 0; //index location of the first gun
    [SerializeField] int secondGunID = 1; //index location of the second gun
    [SerializeField] int currentGunID = 0; //index location of the current gun to use
    [SerializeField] GameObject[] gunList;

    //Layermasks ================================================================================
    [Space(2)]
    [Header("LayerMasks")]
    [SerializeField] LayerMask groundLayer;

    //Children Objects ==========================================================================
    [Space(2)]
    [Header("Child Objects")]
    [SerializeField] Animator playerSprite;

    //Animation States ==========================================================================
    private string AnimIdle = "Idle";
    private string AnimRun = "Run";
    private string AnimRoll = "Roll";
    private string AnimJump = "Jump";
    private string AnimFall = "Fall";
    #endregion

    #region Native Unity Functions
    private void Start()
    {
        //Initializing Components
        rb = GetComponent<Rigidbody2D>();
        InitializeGuns();
    }
    private void Update()
    {
        ConditionsCheck();
        HorizontalMovement();
        RollMovement();
        VerticalMovement();
        SwitchGun();
        AnimationController();
    }
    #endregion

    #region GunSwitching

    
    private void InitializeGuns() 
    {
        //Activate the first gun gameobject, while setting all other guns as off
        if(gunList.Length <= firstGunID || gunList.Length <= secondGunID)
        {
            Debug.Log("ERROR: the ID of the first or second gun is too large");
        }
        for(int a = 0; a<gunList.Length; a++)
        {
            if (a != firstGunID)
                gunList[a].SetActive(false);
            else
                gunList[a].SetActive(true);
        }
    }
    private void SwitchGun()
    {
        //Check for conditions, and then start coroutine to switch guns
        if(!isSwitching && Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(SwitchGunCoroutine());
        }
    }
    IEnumerator SwitchGunCoroutine()
    {
        isSwitching = true;
        gunList[currentGunID].SetActive(false); //deactivates current gun
        //Here is where I would have to implement how the switching is visually shown
        yield return new WaitForSeconds(switchTime);
        if (firstGunID == currentGunID)
            currentGunID = secondGunID;
        else
            currentGunID = firstGunID;
        gunList[currentGunID].SetActive(true); //activates the new gun
        isSwitching = false;
    }
    #endregion 

    #region Horizontal Movement
    void HorizontalMovement()
    {
        if (isRolling)
        {
            return;
        }


        if (Input.GetKey(KeyCode.D))
        {
            transform.localScale = new Vector3(1, 1, 1);
            rb.velocity = new Vector2(runSpeed, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.localScale = new Vector3(-1, 1, 1);
            rb.velocity = new Vector2(-runSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
    void RollMovement()
    {
        if (Input.GetKeyDown(KeyCode.S) && Time.time > nextAvailableRollTime && isTouchingGround)
        {
            nextAvailableRollTime = Time.time + rollDelay + rollTime;
            StartCoroutine(Roll(rollTime, rollSpeed, (int)transform.localScale.x));
        }
    }
    IEnumerator Roll(float rollTime, float rollSpeed, int direction)
    {
        isRolling = true;
        rb.velocity = new Vector2(rollSpeed * direction, rb.velocity.y);
        yield return new WaitForSeconds(rollTime);

        isRolling = false;
    }
    
    #endregion

    #region Vertical Movement
    void VerticalMovement() //Note to self: might remove the fancy hold to jump higher thing
    {
        if (isTouchingGround)
        {
            Jump();
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.W))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }


        if (rb.velocity.y < -0.1f)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
    }

    #endregion

    void AnimationController()
    {
        if (isRolling)
        {
            playerSprite.Play(AnimRoll);
        }
        else if(!isTouchingGround && rb.velocity.y >= 0.1f)
        {
            playerSprite.Play(AnimJump);
        }
        else if(!isTouchingGround && rb.velocity.y <= -0.1f)
        {
            playerSprite.Play(AnimFall);
        }
        else if(Mathf.Abs(rb.velocity.x) >= 0.1f)
        {
            playerSprite.Play(AnimRun);
        }
        else
        {
            playerSprite.Play(AnimIdle);
        }
    }
    void ConditionsCheck()
    {
        //Checks Conditions regarding a state a player is in relation to its actions and the environment around it.
        isTouchingGround = Physics2D.OverlapCircle((Vector2)transform.position + jumpColliderBottomOffset, jumpColliderRadius, groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + jumpColliderBottomOffset, jumpColliderRadius);
    }
}
