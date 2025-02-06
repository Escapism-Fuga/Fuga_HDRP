using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using extOSC;
using Treegen;

public class MyOSC : MonoBehaviour
{
    public GameObject arbreTest;
    public extOSC.OSCReceiver oscReceiver;
    public Treegen.TreegenTreeGenerator treegenTreeGenerator;

    private float targetLightMath = 0.0010f;  // Valeur initiale pour l'échelle
    private float currentLightMath = 0.0010f;
    private float velocityLight = 0f; // Vélocité pour SmoothDamp

    
    private float targetLeavesMath = 0f;  // Valeur cible pour l'échelle des feuilles
    private float currentLeavesMath = 0f;
    private float velocityLeaves = 0f;

    private float targetLeavesOffMath = 0f;  // Valeur cible pour l'offset des feuilles
    private float currentLeavesOffMath = 0f;
    private float velocityLeavesOff = 0f;
    
    public float smoothTime = 0.2f; // Plus petit = plus rapide, plus grand = plus fluide

    private bool arbreActif;

    void Start()
    {
        oscReceiver.Bind("/vkb_midi/9/note/38", message => MessageVolume(message));
        oscReceiver.Bind("/vkb_midi/9/note/39", message => MessageFrequance(message));
        arbreActif = true;
        StartCoroutine("CoOsc");
    }

    void MessageVolume(OSCMessage oscMessage)
    {
        float value = 0;

        if (oscMessage.Values[0].Type == OSCValueType.Int)
            value = oscMessage.Values[0].IntValue;
        else if (oscMessage.Values[0].Type == OSCValueType.Float)
            value = oscMessage.Values[0].FloatValue;
        else
            return;
       
        Debug.Log("Volume: " + value);

        // Mettre à jour les valeurs cibles
        targetLightMath = Mathf.Lerp(0.0010f, 0.0030f, Mathf.InverseLerp(0f, 4095f, value));
        targetLeavesMath = Mathf.Lerp(0f, 12f, Mathf.InverseLerp(0f, 4095f, value));

        targetLeavesOffMath = Mathf.Lerp(5f, 20f, Mathf.InverseLerp(0f, 4095f, value));
        
    }

    void MessageFrequance(OSCMessage oscMessage)
    {
        float value = 0;

        if (oscMessage.Values[0].Type == OSCValueType.Int)
        {
            value = oscMessage.Values[0].IntValue;
        }
        else if (oscMessage.Values[0].Type == OSCValueType.Float)
        {
            value = oscMessage.Values[0].FloatValue;
        }
        else
        {
            return;
        }
        
        Debug.Log("Volume: " + value);

    }

    int counter;

    void Update()
    {
        /// Utiliser Time.smoothDeltaTime pour un ajustement plus fluide en cas de chutes de framerate
        float smoothDeltaTime = Time.smoothDeltaTime;

        // Lissage des valeurs
        currentLightMath = Mathf.SmoothDamp(currentLightMath, targetLightMath, ref velocityLight, smoothTime, Mathf.Infinity, smoothDeltaTime);
        currentLeavesMath = Mathf.SmoothDamp(currentLeavesMath, targetLeavesMath, ref velocityLeaves, smoothTime, Mathf.Infinity, smoothDeltaTime);
        currentLeavesOffMath = Mathf.SmoothDamp(currentLeavesOffMath, targetLeavesOffMath, ref velocityLeavesOff, smoothTime, Mathf.Infinity, smoothDeltaTime);

        // Appliquer les valeurs lissées
        treegenTreeGenerator.transform.localScale = new Vector3(currentLightMath, currentLightMath, currentLightMath);
        treegenTreeGenerator.LeavesScale = Vector3.one * currentLeavesMath;
        treegenTreeGenerator.LeavesOffset = currentLeavesOffMath;

        if ( counter > 2 )
        {
            treegenTreeGenerator.NewGen();
            counter = 0;
        }
  

        counter++;
    }


    


    public IEnumerator CoOsc()
    {
        while (arbreActif == true)
        {
         


            yield return new WaitForSeconds(0.03f);
        }

    }


}
