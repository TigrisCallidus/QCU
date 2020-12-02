// -*- coding: utf-8 -*-

// This code is part of Qiskit.
//
// (C) Copyright IBM 2020.
//
// This code is licensed under the Apache License, Version 2.0. You may
// obtain a copy of this license in the LICENSE.txt file in the root directory
// of this source tree or at http://www.apache.org/licenses/LICENSE-2.0.
//
// Any modifications or derivative works of this code must retain this
// copyright notice, and modified files need to carry a notice indicating
// that they have been altered from the originals.
using Qiskit;
using QuantumImage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealQiskitBlurExample : MonoBehaviour {

    [Tooltip("The texture to test. The Read/Write must be enabled in the setting for it.")]
    public Texture2D Input;
    [Tooltip("The result. The new texture")]
    public Texture2D Output;
    [Tooltip("The rotation which is applied by the quantumblur to the texture")]

    public float Rotation = 0.25f;

    [Tooltip("How many shots are used (how often the circuit is run and measured) for simulator and the backend")]
    public int NumberOfShots = 1000;

    [Tooltip("Your token to access the IBM backend.")]
    public string Token = "";

    [Tooltip("IF a real device is used or not")]
    public bool UseReal = false;

    //Linking
    [HideInInspector]
    public RawImage TargetImage;
    [HideInInspector]
    public GameObject LoadingIndicator;


    SimulateJob myJob;

    //Gets called when you press run
    void Start() {
        TargetImage.texture = Input;
        prepareAndStartJob();
    }

    // Update is called once per frame and is needed here to check if the job is finished
    void Update() {
        if (myJob != null) {
            if (myJob.Update()) {
                // Alternative to the OnFinished callback
                finishJob();
                myJob = null;
            }
        }
    }

    //Doing the work. Creates a new job and runs it.
    void prepareAndStartJob() {

        //Showing Loading Indicator
        LoadingIndicator.SetActive(true);

        //First create a new simulatejob to run this on a seperate thread
        myJob = new SimulateJob();
        //Now set the "simulator" to the QiskitSimulator. (If UseReal is set to true, a real backend is used, and your Token needs to be provided).
        myJob.Simulator = new QiskitSimulator(1000, UseReal, Token);
        //Creating a circuit from the red channel of the provided texture (for black and white image any of the 3 color channels is ok).
        myJob.Circuit=QuantumImageCreator.GetCircuitDirect(Input, ColorChannel.R);
        //applying additional manipulation to the circuit
        applyPartialQ(myJob.Circuit, Rotation);
        //run the job, meaning start the simulation (or the call to the backend)
        myJob.Start();
    }

    //gets called when the job is finished
    void finishJob() {
        Debug.Log("Job finished");
        //Creating a texture with the calculated probabilities
        Output = QuantumImageCreator.GetGreyTextureDirect(myJob.Probabilities, Input.width, Input.height,myJob.Circuit.OriginalSum);
        //Setting the new texture to the image on screen
        TargetImage.texture = Output;

        //Hiding Loading Indicator
        LoadingIndicator.SetActive(false);

    }

    //A simple quantumBlur is applied to the circuit
    void applyPartialQ(QuantumCircuit circuit, float rotation) {
        for (int i = 0; i < circuit.NumberOfQubits; i++) {
            //Making a "blur" effect by applying a (small) rotation to each qubit.
            circuit.RX(i, rotation);
        }
    }

}
