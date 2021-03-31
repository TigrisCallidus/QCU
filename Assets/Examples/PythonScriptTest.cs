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
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PythonScriptTest : MonoBehaviour {
    const string pythonEXE = @"/.q/python.exe";
    const string exchange = @"Exchange/";
    const string pythonFileName = "testfile.py";
    const string pythonScripts = @"/PythonScripts/";

    public bool RunThreaded;


    PythonJob myJob;


    // Start is called before the first frame update
    void Start() {
        if (RunThreaded) {
            prepareAndStartJob();
        } else {
            RunPythonFileDirectly();
        }
    }

    void Update() {
        if (myJob != null) {
            if (myJob.Update()) {
                // Alternative to the OnFinished callback
                finishJob();
                myJob = null;
            } else {
                doWaiting();
            }
        }
    }



    //gets called when the job is finished
    void finishJob() {
        Debug.Log("Job finished with output" + myJob.Output);
    }

    //gets called each frame while the job is running. This normally is not needed but you coud do things here.
    void doWaiting() {

    }

    void finishedJobCallback(string output) {
        Debug.Log("Finished Job Callbacl was called with output:" + output);
    }

    //Doing the work. Creates a new job and runs it.
    void prepareAndStartJob() {

        string pythonPath = Application.streamingAssetsPath + pythonEXE;
        string filePath = Application.streamingAssetsPath + pythonScripts + exchange + pythonFileName;

        myJob = new PythonJob();
        myJob.PythonPath = pythonPath;
        myJob.FilePath = filePath;

        myJob.Start();
    }


    //if you want to run a pythonfile directly, not in a thread
    public string RunPythonFileDirectly() {

        Process process = new Process();
        string pythonPath = Application.streamingAssetsPath + pythonEXE;
        string filePath = Application.streamingAssetsPath + pythonScripts + exchange + pythonFileName;

        Debug.Log("reading pythonfile from: " + pythonPath);


        process.StartInfo.FileName = pythonPath;
        process.StartInfo.Arguments = filePath;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.StartInfo.CreateNoWindow = true;

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;

        process.Start();

        StreamReader reader = process.StandardOutput;
        string output = reader.ReadToEnd();

        Debug.Log(output);

        process.WaitForExit();
        Debug.Log(process.ExitCode);
        process.Close();
        return output;
    }
}
