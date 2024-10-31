using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChurningCock : MonoBehaviour
{
    public DickDescriptor dickDescriptor;
    public Transform ballScaler;
    public float baseBallScale;
    public InflatableCurve churnBounce;
    public InflatableCurve clenchCurve;
    public AudioSource ballGrumbleSource;
    private float churnIntensity = 1f;
    private Coroutine churnRoutine;
    private Coroutine clenchRoutine;

    void Start(){
        ballGrumbleSource.enabled = false;
    }

    void Update()
    {
        if(dickDescriptor.dicks[0].ballSizeInflater.GetSize() >= 3f && !ballGrumbleSource.enabled){
            ballGrumbleSource.enabled = true;
            churnRoutine = StartCoroutine(PulseBalls());
        }else if(dickDescriptor.dicks[0].ballSizeInflater.GetSize() < 3f && ballGrumbleSource.enabled){
            ballGrumbleSource.enabled = false;
            StopCoroutine(churnRoutine);
            churnRoutine = null;
        }

        if(dickDescriptor.isCumming() && clenchRoutine == null){
            clenchRoutine = StartCoroutine(ClenchBalls());
        }else if(!dickDescriptor.isCumming() && clenchRoutine != null){
            StopCoroutine(clenchRoutine);
            clenchRoutine = null;
        }

        churnIntensity = 1f + Mathf.Clamp(dickDescriptor.dicks[0].ballSizeInflater.GetSize() - 2f, 0, 3);
    }

    public IEnumerator ClenchBalls(){
        while (dickDescriptor.isCumming()){
            float startTime = Time.time;
            float endTime = startTime+clenchCurve.GetBounceDuration();
            while (Time.time < endTime) {
                float t = (Time.time - startTime) / clenchCurve.GetBounceDuration();
                float multiplier = clenchCurve.EvaluateCurve(t);
                ballScaler.localScale = new Vector3(multiplier, multiplier, multiplier) * baseBallScale;
                yield return null;
            }
        }

        clenchRoutine = null;
    }

    public IEnumerator PulseBalls(){
        while(ballGrumbleSource.enabled){
            if(!dickDescriptor.isCumming()){
                yield return new WaitForSeconds(Random.Range(2f, 5f));
                float startTime = Time.time;
                float endTime = startTime+churnBounce.GetBounceDuration();
                while (Time.time < endTime) {
                    float t = (Time.time - startTime) / churnBounce.GetBounceDuration();
                    float multiplier = Mathf.Pow(churnBounce.EvaluateCurve(t), churnIntensity);
                    ballScaler.localScale = new Vector3(multiplier, multiplier, multiplier) * baseBallScale;
                    yield return null;
                }
            }
            yield return null;
        }
    }
}
