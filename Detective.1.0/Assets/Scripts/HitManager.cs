using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/*
 * This script should be present in every scene with damage or hitting
 * There should be an event here that will broadcast some damage information, as well as WHO is being attacked
 * So, those who subscribe to the event can check if the event pertains to them, and then can use the damage information
 * 
 * There should be a way for other scripts to call the damage event and pass parameters (preferably within a scriptable object)
 */
public class HitManager : MonoBehaviour
{
    public event onHitEventDelegate onHit;
    public delegate void onHitEventDelegate(AttackData data);

    public void BroadcastHit(AttackData data)
    {
        onHit?.Invoke(data);
    }
}
