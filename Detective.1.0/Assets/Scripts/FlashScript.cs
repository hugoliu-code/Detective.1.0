using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashScript : MonoBehaviour
{
    private Material prevMaterial; //the material before we set the new one
    public Material flashMaterial;
    [SerializeField] float flashDowntime; //How long inbetween shots does the flash need to wait? (to prevent infinite white)
    private float nextAvailableFlash = 0;
    // Start is called before the first frame update
    public void Flash(SpriteRenderer spriteRenderer, float time)
    {
        if(Time.time > nextAvailableFlash)
            StartCoroutine(FlashCoroutine(spriteRenderer, time));
    }
    private IEnumerator FlashCoroutine(SpriteRenderer sr, float time)
    {
        nextAvailableFlash = Time.time + time + flashDowntime; //current time plus the time for the flash, and then the waiting time
        prevMaterial = sr.material;
        sr.material = flashMaterial;
        yield return new WaitForSeconds(time);
        sr.material = prevMaterial;

    }
}
