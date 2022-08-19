using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V1BulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] LayerMask collidables; //objects, ground, enemies
    private Vector3 previousPosition; //position of the object the frame before
    private HitManager hm;
    private bool hasCollided = false;
    private void Start()
    {
        hm = GameObject.FindGameObjectWithTag("HitManager").GetComponent<HitManager>();
        previousPosition = transform.position;
    }
    private void Update()
    {
        if (hasCollided) { return; }
        RaycastHit2D collisionCheck = Physics2D.Raycast(previousPosition, transform.position-previousPosition, Vector2.Distance(previousPosition, transform.position), collidables);
        if (collisionCheck)
        {
            HandleCollision(collisionCheck);
            hasCollided = true;

        }
        previousPosition = transform.position;
    }

    void HandleCollision(RaycastHit2D collision)
    {
        transform.position = collision.point;
        AttackData attackData = ScriptableObject.CreateInstance<AttackData>();
        attackData.damage = 1;
        attackData.receiver = collision.collider.gameObject;
        hm.BroadcastHit(attackData);
        Invoke("CustomDestroy",0.01f);
    }
    void CustomDestroy()
    {
        Destroy(this.gameObject);
    }
}
