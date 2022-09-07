using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TestDummyScript : MonoBehaviour
{
    [SerializeField] int timesHit;
    HitManager hm;
    void Start()
    {
        hm = GameObject.FindGameObjectWithTag("HitManager").GetComponent<HitManager>();
        hm.onHit += TestingHit;
    }
    private void TestingHit(AttackData data)
    {
        if (data.receiver.Equals(this.gameObject))
        {
            timesHit+= data.damage;
        }
    }

}
