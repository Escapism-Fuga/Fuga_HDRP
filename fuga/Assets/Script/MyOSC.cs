using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using extOSC;
using Treegen;
using UnityEngine.Rendering.HighDefinition;

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

    private int targetBranchesMath = 0;  // Valeur cible pour les branches
    private int currentBranchesMath = 0;
    private float velocityBranches = 0f;

    private float targetLeavesOffMath = 0f;  // Valeur cible pour l'offset des feuilles
    private float currentLeavesOffMath = 0f;
    private float velocityLeavesOff = 0f;

    public float smoothTime = 0.2f; // Plus petit = plus rapide, plus grand = plus fluide

    void Start()
    {
        oscReceiver.Bind("/vkb_midi/9/note/36", message => MessageVolume(message));
        oscReceiver.Bind("/vkb_midi/9/note/40", message => MessageFrequance(message));
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
        targetLightMath = Mathf.Lerp(0.0010f, 0.0030f, Mathf.InverseLerp(0f, 200f, value));
        targetLeavesMath = Mathf.Lerp(0f, 12f, Mathf.InverseLerp(0f, 200f, value));
        targetBranchesMath = Mathf.RoundToInt(Mathf.Lerp(0f, 5f, Mathf.InverseLerp(0f, 200f, value)));
        targetLeavesOffMath = Mathf.Lerp(0f, 20f, Mathf.InverseLerp(0f, 200f, value));
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

        var material = new Material(Shader.Find("HDRP/Nature/SpeedTree8"));

        material.SetColor("_ColorTint", new Color(0, 0, value));
    }
    void Update()
    {
        // Lissage des valeurs
        currentLightMath = Mathf.SmoothDamp(currentLightMath, targetLightMath, ref velocityLight, smoothTime);
        currentLeavesMath = Mathf.SmoothDamp(currentLeavesMath, targetLeavesMath, ref velocityLeaves, smoothTime);
        currentBranchesMath = Mathf.RoundToInt(Mathf.SmoothDamp(currentBranchesMath, targetBranchesMath, ref velocityBranches, smoothTime));
        currentLeavesOffMath = Mathf.SmoothDamp(currentLeavesOffMath, targetLeavesOffMath, ref velocityLeavesOff, smoothTime);

        // Appliquer les valeurs lissées
        treegenTreeGenerator.transform.localScale = new Vector3(currentLightMath, currentLightMath, currentLightMath);
        treegenTreeGenerator.LeavesScale = Vector3.one * currentLeavesMath;
        treegenTreeGenerator.BranchSegments = currentBranchesMath;
        treegenTreeGenerator.LeavesOffset = currentLeavesOffMath;

        treegenTreeGenerator.NewGen();
    }
}
