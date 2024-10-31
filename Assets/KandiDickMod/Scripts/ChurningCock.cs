using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChurningCock : MonoBehaviour
{
    public DickDescriptor dickDescriptor;
    public Transform ballScaler;
    public float baseBallScale;
    public InflatableCurve churnBounce;
    public AudioPack ballGrumbles;
    public AudioSource ballGrumbleSource;
    private float churnIntensity = 1f;
    private WaitForSeconds waitTime;
    private Coroutine routine;

    void Start(){
        ballGrumbleSource.enabled = false;
        ballGrumbleSource.loop = false;
        ballGrumbleSource.playOnAwake = false;
        if(churnBounce.GetBounceDuration() > 1){
            waitTime = new WaitForSeconds(0);
        }else{
            waitTime = new WaitForSeconds(1f - churnBounce.GetBounceDuration());
        }
    }

    void Update()
    {
        if(dickDescriptor.dicks[0].ballSizeInflater.GetSize() >= 3f && !ballGrumbleSource.enabled){
            ballGrumbleSource.enabled = true;
            routine = StartCoroutine(PlayGurgles());
        }else if(dickDescriptor.dicks[0].ballSizeInflater.GetSize() < 3f && ballGrumbleSource.enabled){
            ballGrumbleSource.enabled = false;
            StopCoroutine(routine);
        }

        churnIntensity = 1f + Mathf.Clamp((dickDescriptor.dicks[0].ballSizeInflater.GetSize() - 2f), 0, 3);
    }

    private IEnumerator PlayGurgles() {
        while (ballGrumbleSource.enabled) {
            if (!ballGrumbleSource.isPlaying) {
                yield return StartCoroutine(PulseBalls());
                yield return waitTime;
                ballGrumbles.Play(ballGrumbleSource);
                ballGrumbleSource.volume = 1;
            }
            yield return null;
        }

        routine = null;
    }

    public IEnumerator PulseBalls(){
        float startTime = Time.time;
        float endTime = startTime+churnBounce.GetBounceDuration();
        while (Time.time < endTime) {
            float t = (Time.time - startTime) / churnBounce.GetBounceDuration();
            float multiplier = Mathf.Pow(churnBounce.EvaluateCurve(t), churnIntensity);
            ballScaler.localScale = new Vector3(multiplier, multiplier, multiplier) * baseBallScale;
            yield return null;
        }
    }
}
