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

using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Qiskit {


    /// <summary>
    /// Using real qiskit (python) for simulating OR (not yet implemented) to call a real quantum computer
    /// </summary>
    public class QiskitSimulator : SimulatorBase {

        bool useSimulator = true;
        int numberOfSimulations = 1000;
        string token;
        string backend;

        /// <summary>
        /// Initialises a new simulator
        /// </summary>
        /// <param name="numberOfShots">How many time the circuit is run</param>
        /// <param name="useRealMachine">If a real backend is used (instead of the qiskit simulator)</param>
        /// <param name="authToken">The authentification token needed for the backend</param>
        /// <param name="chosenBackend">The backend you want to use. Specify if you want something else than melbourne.</param>
        public QiskitSimulator(int numberOfShots=1000, bool useRealMachine = false, string authToken="", string chosenBackend= "ibmq_16_melbourne") {
            numberOfSimulations = numberOfShots;
            useSimulator = !useRealMachine;
            token = authToken;
            backend = chosenBackend;
        }


        public override double[] GetProbabilities(QuantumCircuit circuit) {
            if (circuit.AmplitudeLength==0) {
                circuit.AmplitudeLength = MathHelper.IntegerPower(2, circuit.NumberOfQubits);
            }

            double[] returnValue = new double[circuit.AmplitudeLength];

            WritePythonFile(circuit);
            string probString= RunPythonFile(circuit);

            ReadProbabilities(ref returnValue, probString);

            return returnValue;
        }


        public void WritePythonFile(QuantumCircuit circuit) {
            string filePath = Application.streamingAssetsPath + pythonScripts + exchange + pythonFileName;
            string fileContent="";
            if (useSimulator) {
                fileContent = pythonStartSimulator + circuit.GetQiskitString(true) + pythonEndSimulator + numberOfSimulations + pythonEnd;
            } else {
                fileContent = pythonBackendStart + circuit.GetQiskitString(true) + pythonBackendMid + token + pythonBackendEnd1 + backend + pythonBackendEnd2 + numberOfSimulations + pythonEnd;
            }

            Debug.Log("Writing python file to: " + filePath);


            File.WriteAllText(filePath, fileContent);
        }

        public string RunPythonFile(QuantumCircuit circuit){ //, ref double[] probabilities) {


            Process process = new Process();
            string pythonPath = Application.streamingAssetsPath + pythonEXE;
            string filePath = Application.streamingAssetsPath + pythonScripts + exchange + pythonFileName;

            Debug.Log("reading pythonfile from: " + pythonPath);
            //Debug.Log("filePath is: " + filePath);


            process.StartInfo.FileName = pythonPath;
            process.StartInfo.Arguments = filePath;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();

            StreamReader reader = process.StandardOutput;
            string output = reader.ReadToEnd();

            //UnityEngine.Debug.Log(output);

            process.WaitForExit();
            UnityEngine.Debug.Log(process.ExitCode);
            process.Close();
            return output;
            //ReadProbabilities(ref probabilities, output);
        }


        public void ReadProbabilities(ref double[] probabilities, string qiskitString) {
            int probabilityLength = probabilities.Length;
            //-4 because of the newline at the end (/r/n) counting as 2 elements
            string[] outputPairs = qiskitString.Substring(1, qiskitString.Length - 4).Split(new string[] { ", " }, StringSplitOptions.None);
            int pairLength = outputPairs.Length;
            double totalCount = 0;
            for (int i = 0; i < pairLength; i++) {
                string[] keyValue = outputPairs[i].Split(':');
                string key = keyValue[0].Substring(1, keyValue[0].Length - 2);
                string value = keyValue[1];
                //Debug.Log(key + " " + value);
                int position = Convert.ToInt32(key, 2);
                int count = Convert.ToInt32(value);
                if (position >= probabilityLength) {
                    Debug.LogError("Position is bigger than probabilities array " + position + " vs length: " + probabilityLength);
                } else {
                    probabilities[position] = count;
                    totalCount += count;
                }
            }

            for (int i = 0; i < probabilityLength; i++) {
                if (probabilities[i] > 0) {
                    probabilities[i] = probabilities[i] / totalCount;
                }
            }
            Debug.Log("Finished with probabilities");
        }


        #region Constant strings

        const string pythonFileName = "pythonfile.py";
        //const string outputFile = "outputfile.txt";


        const string pythonScripts = @"/PythonScripts/";
        const string exchange = @"Exchange/";
        const string pythonEXE = @"/.q/python.exe";


        const string pythonStartSimulator =
            @"from qiskit import(
  QuantumCircuit,
  execute,
  Aer)

simulator = Aer.get_backend('qasm_simulator')

";

        const string pythonBackendStart = @"from qiskit import(
  QuantumCircuit,
  execute,
  IBMQ)

";

        const string pythonEndSimulator = @"
job = execute(qc, simulator, shots=";

        const string pythonBackendMid = @"
IBMQ.enable_account('";
        const string pythonBackendEnd1 = @"')
provider = IBMQ.get_provider(hub = 'ibm-q')
device = provider.get_backend('";

        const string pythonBackendEnd2 = @"')
job = execute(qc, backend = device, shots= ";

        const string pythonEnd = @")

result = job.result()
counts = result.get_counts(qc)
print(counts)";

        #endregion

    }
}