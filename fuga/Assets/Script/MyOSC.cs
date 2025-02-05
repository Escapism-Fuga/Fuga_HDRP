using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using extOSC;
using UnityEditor;
using Treegen;


public class MyOSC : MonoBehaviour
{
    // public TreegenTreeGenerator treegenTreeGenerator;
    public extOSC.OSCReceiver oscReceiver;

    public Treegen.TreegenTreeGenerator treegenTreeGenerator;
    public float[] values = new float[3];

    public static float ScaleValue(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
    }

    void Start()
    {
        oscReceiver.Bind("/vkb_midi/9/note/36", message => MessageVolume(message));
        oscReceiver.Bind("/vkb_midi/9/note/37", message => MessageFrequance(message));
        oscReceiver.Bind("/btn1", message => MessageBtn1(message));
        oscReceiver.Bind("/btn2", message => MessageBtn2(message));
        oscReceiver.Bind("/btn3", message => MessageBtn3(message));
    }

    void MessageVolume(OSCMessage oscMessage)
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
        float lightMath = Mathf.Lerp(0f, 5f, value / 1000f);

        treegenTreeGenerator.TreeNoiseForce = lightMath;

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

        Debug.Log("Frequence: " + value);
        float lightMath = Mathf.Lerp(0f, 5f, value / 1000f);

        treegenTreeGenerator.LeavesNoiseForce = lightMath;

    }

    void MessageBtn1(OSCMessage oscMessage)
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

        Debug.Log("btn1: " + value);
        float lightMath = Mathf.Lerp(0f, 5f, value / 1000f);

    }

    void MessageBtn2(OSCMessage oscMessage)
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

        Debug.Log("btn2: " + value);
        float lightMath = Mathf.Lerp(0f, 5f, value / 1000f);
    }

    void MessageBtn3(OSCMessage oscMessage)
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

        Debug.Log("btn3: " + value);
        float lightMath = Mathf.Lerp(0f, 5f, value / 1000f);

    }
    void Update()
    {

        treegenTreeGenerator.NewGen();
    }
}