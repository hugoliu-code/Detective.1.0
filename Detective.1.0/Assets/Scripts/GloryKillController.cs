using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloryKillController : MonoBehaviour
{    /*
     * This should be attached to main player object (the parent)
     * The script should take care of all events
     * related to a glory kill
     * 
     */
    #region Variables
    [Header("References")]
    [SerializeField] PlayerController playerScript;
    private SpriteRenderer sprite;
    private Animator anim;


    private bool isGloryKilling = false;
    //The idea is that when a glory kill starts, no other one should be happening at the same time.
    //This means we can just keep a current frame value, and pass it into the GloryKillFrameMove function

    #endregion
    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.clear;
        anim = GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        //need to make event system to find out if the enemy can be glory killed (health/stunned, etc)
        //need system to find what kind of enemy it is so i can find the proper kill
        if (collision.CompareTag("Enemy") && 
            Input.GetKeyDown(KeyCode.M) && 
            !isGloryKilling)
        {
            Debug.Log("GloryKilling");
            GloryKillIdentifier id = collision.gameObject.GetComponent<GloryKillIdentifier>();
            PlayGloryKill(id.Identifier, id.gameObject);
        }
    }
    private void PlayGloryKill(string killName, GameObject enemyObject)
    {
        sprite.color = Color.white;
        enemyObject.SetActive(false);
        playerScript.isGloryKilling = true;
        anim.Play(killName);
    }

    public void EndGloryKill()
    {
        sprite.color = Color.clear;
        playerScript.isGloryKilling = false;
        anim.Play("Empty");
    }

    

}
