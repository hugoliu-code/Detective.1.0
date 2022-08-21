using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("Components")]
    Rigidbody2D rb;
    [Space(2)]
    [Header("Horizontal Movement")]
    [SerializeField] float runSpeed;
    [SerializeField] float rollTime;
    [SerializeField] float rollSpeed;
    [SerializeField] float rollDelay;
    private float nextAvailableRollTime = 0;
    [Space(2)]
    [Header("Vertical Movement")]
    [SerializeField] float jumpSpeed;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float lowJumpMultiplier = 2;
    [Space(2)]
    [Header("Conditions")]
    [SerializeField] Vector2 jumpColliderBottomOffset;
    [SerializeField] float jumpColliderRadius;
    private bool isTouchingGround = false;
    private bool isRolling = false;
    private bool isSwitching = false; //This might be turned public at some point
    [SerializeField] float switchTime = 1; //how long does it take to switch guns?
    [SerializeField] int firstGunID = 0;
    [SerializeField] int secondGunID = 1;
    [SerializeField] int currentGunID = 0;
    [SerializeField] GameObject[] gunList;
    [Header("LayerMasks")]
    [SerializeField] LayerMask groundLayer;

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
