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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using QuantumImage;
using Qiskit;
using UnityEngine.UI;

public class ReverseImageConstructor : MonoBehaviour {

    public bool UseSingleFile = false;

    public TextAsset SingleFile;

    public TextAsset[] MultipleFiles;

    public Texture2D Output;

    public int Dimension = 256;

    public RawImage TargetImage;

    public int Normalization = 15;
    public bool RecalculateNormalization = true;



    void Start() {
        if (UseSingleFile) {
            ConstructOutput();
        } else {
            ConstructOutputCombined();
        }
    }

    public void ConstructOutput() {

        //We need a QiskitSimulator since it can not only run qiskit but also interpret the output from qiskit
        QiskitSimulator simulator = new QiskitSimulator();
        //Preparing the array where the probabilities are stored on
        double[] returnValue = new double[MathHelper.IntegerPower(2, 16)];

        //Here we use the ability from the simulator to read the proabilities from the qiskit output string into our double array
        simulator.ReadProbabilities(ref returnValue, SingleFile.text, Normalization, RecalculateNormalization);

        //We now interpret the probabilities as an image to visualize them
        Output = QuantumImageCreator.GetGreyTextureDirect(returnValue, Dimension, Dimension);
        TargetImage.texture = Output;
    }

    public void ConstructOutputCombined() {

        //Preparing an array of strings to give the simulator
        string[] files = new string[MultipleFiles.Length];
        for (int i = 0; i < MultipleFiles.Length; i++) {
            files[i] = MultipleFiles[i].text;
        }

        //We need a QiskitSimulator since it can not only run qiskit but also interpret the output from qiskit
        QiskitSimulator simulator = new QiskitSimulator();
        //Preparing the array where the probabilities are stored on
        double[] returnValue = new double[MathHelper.IntegerPower(2, 16)];

        //Here we use the ability from the simulator to read the proabilities from the qiskit output strings into our double array
        simulator.ReadProbabilities(ref returnValue, files, Normalization, RecalculateNormalization);


        //We now interpret the probabilities as an image to visualize them
        Output = QuantumImageCreator.GetGreyTextureDirect(returnValue, Dimension, Dimension);
        TargetImage.texture = Output;
    }

}
