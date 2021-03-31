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
using System;
using System.Diagnostics;
using System.IO;

public class PythonJob : ThreadedJob {

    public string PythonPath;
    public string FilePath;

    public string Output;

    public Action<string> FinishedCallback;

    protected override void ThreadFunction() {
        // Do your threaded task. DON'T use the Unity API here
        Output=RunPythonFile();
    }
    protected override void OnFinished() {
        // This is executed by the Unity main thread when the job is finished
        if (FinishedCallback!=null) {
            FinishedCallback.Invoke(Output);
        }
    }


    public string RunPythonFile() { //, ref double[] probabilities) {
        UnityEngine.Debug.Log("Starting thread");


        Process process = new Process();
        string pythonPath = PythonPath;
        string filePath = FilePath;



        process.StartInfo.FileName = pythonPath;
        process.StartInfo.Arguments = filePath;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.CreateNoWindow = true;

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.Start();

        StreamReader reader = process.StandardOutput;
        string output = reader.ReadToEnd();

        //UnityEngine.Debug.Log(output);

        process.WaitForExit();
        UnityEngine.Debug.Log(process.ExitCode);
        UnityEngine.Debug.Log(process.StandardError.ReadToEnd());

        process.Close();
        //Debug.Log(output);
        return output;
        //ReadProbabilities(ref probabilities, output);
    }
}