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
// that they have been altered from the originals.using System;

using Qiskit;
using System.Collections.Generic;
using UnityEngine;
using Qiskit.Float;

namespace QuantumImage {
    /// <summary>
    /// Enum used to pick channels in some functions.
    /// </summary>
    public enum ColorChannel {
        R,
        G,
        B,
        A
    }

    /// <summary>
    /// Class for creating quantum image effects.Contains transformations (image to circuit, and vice versa) 
    /// as well as the fully ready effects QuantumBlur (which uses rotation on the qubits to Blur the image)
    /// and Teleportation which kinda mixes two different images. Most things are provided for grey and colored seperately.
    /// </summary>
    public class QuantumImageCreator {




        /// <summary>
        /// Initialisation only needed for effects, which use the python files.
        /// Is relatively slow, so should be only called once.
        /// </summary>
        public QuantumImageCreator() {

        }







        //This region contains effecs, which do not need python, so no initialization needed and just static functions.
        #region Direct Effects (without Python)


        /// <summary>
        /// Getting a quantum circuit representation of a color channel directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="inputTexture">The image which should be converted to a circuit</param>
        /// <param name="colorChannel">Which color channel is converted</param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns></returns>
        public static QuantumCircuit GetCircuitDirect(Texture2D inputTexture, ColorChannel colorChannel = ColorChannel.R, bool useLog = false) {
            //TODO logarithmic encoding
            //TODO no need to go over double array unneeded copying
            double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(inputTexture, colorChannel);

            return GetCircuitDirect(imageData, useLog);
        }

        /// <summary>
        /// Getting a quantum circuit representation of a 2d array of data. (Most likely an image but can be other things). Not using python
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="imageData">The data (of the image) as a 2d double array. For image data floats would be more than sufficient!</param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns></returns>
        public static QuantumCircuit GetCircuitDirect(double[,] imageData, bool useLog = false) {
            return QuantumImageHelper.HeightToCircuit(imageData);
        }

        /// <summary>
        /// Getting a quantum circuit representation of a 2d array of data. (Most likely an image but can be other things). Not using python
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="imageData">The data (of the image) as a 2d double array. For image data floats is more than sufficient!</param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns></returns>
        public static QuantumCircuitFloat GetCircuitDirect(float[,] imageData, bool useLog = false) {
            return QuantumImageHelper.HeightToCircuit(imageData);
        }


        /// <summary>
        /// Getting a quantum circuit representation of each color channel of an image directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="inputTexture">The image which should be converted into quantum circuits representing the channels</param>
        /// <param name="redChannel">Returns the quantum circuit for the red channel of the image.</param>
        /// <param name="greenChannel">Returns the quantum circuit for the green channel of the image.</param>
        /// <param name="blueChannel">Returns the quantum circuit for the blue channel of the image.</param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        public static void GetCircuitDirectPerChannel(Texture2D inputTexture, out QuantumCircuit redChannel, out QuantumCircuit greenChannel, out QuantumCircuit blueChannel, bool useLog = false) {
            QuantumImageHelper.TextureToColorCircuit(inputTexture, out redChannel, out greenChannel, out blueChannel, useLog);
        }

        /// <summary>
        /// Getting a quantum circuit representation of each color channel of an image directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="inputTexture">The image which should be converted into quantum circuits representing the channels</param>
        /// <param name="redChannel">Returns the quantum circuit for the red channel of the image.</param>
        /// <param name="greenChannel">Returns the quantum circuit for the green channel of the image.</param>
        /// <param name="blueChannel">Returns the quantum circuit for the blue channel of the image.</param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        public static void GetCircuitDirectPerChannel(Texture2D inputTexture, out QuantumCircuitFloat redChannel, out QuantumCircuitFloat greenChannel, out QuantumCircuitFloat blueChannel, bool useLog = false) {
            QuantumImageHelper.TextureToColorCircuit(inputTexture, out redChannel, out greenChannel, out blueChannel, useLog);
        }

        /// <summary>
        /// Getting a grey scale texture for a given quantum circuit (which should represent an image) directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="quantumCircuit">The quantum circuit with the grey scale image representation</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="renormalize">If the image (colors) should be renormalized. (Giving it the highest possible saturation / becomes most light) </param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns>A texture showing the encoded image.</returns>
        public static Texture2D GetGreyTextureDirect(QuantumCircuit quantumCircuit, int width, int height, bool renormalize = false, bool useLog = false, SimulatorBase simulator = null) {
            //TODO Make version with only floats (being faster needing less memory)
            double[,] imageData = QuantumImageHelper.CircuitToHeight2D(quantumCircuit, width, height, renormalize, simulator);

            return QuantumImageHelper.CalculateGreyTexture(imageData);
        }

        public static Texture2D GetGreyTextureDirect(double[] probabilities, int width, int height, double normalization=1) {
            //TODO Make version with only floats (being faster needing less memory)
            double[,] imageData = QuantumImageHelper.ProbabilitiesToHeight2D(probabilities, width, height, normalization);

            return QuantumImageHelper.CalculateGreyTexture(imageData);
        }


        /// <summary>
        /// Getting a colored texture for given quantum circuits (each one representing 1 color channel of an image) directly without using python.
        /// Fast version is a lot faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="redCircuit">The quantum circuit which represents the red channel of the image.</param>
        /// <param name="greenCircuit">The quantum circuit which represents the green channel of the image.</param>
        /// <param name="blueCircuit">The quantum circuit which represents the blue channel of the image.</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="renormalize">If the image (colors) should be renormalized. (Giving it the highest possible saturation / becomes most light) </param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns>A texture showing the encoded image.</returns>
        public static Texture2D GetColoreTextureDirectFast(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, int width, int height, bool renormalize = false, bool useLog = false) {
            return QuantumImageHelper.CalculateColorTexture(redCircuit, greenCircuit, blueCircuit, width, height, renormalize);
        }

        /// <summary>
        /// Getting a colored texture for given quantum circuits (each one representing 1 color channel of an image) directly without using python.
        /// Fast version is a lot faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="redCircuit">The quantum circuit which represents the red channel of the image.</param>
        /// <param name="greenCircuit">The quantum circuit which represents the green channel of the image.</param>
        /// <param name="blueCircuit">The quantum circuit which represents the blue channel of the image.</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="renormalize">If the image (colors) should be renormalized. (Giving it the highest possible saturation / becomes most light) </param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns>A texture showing the encoded image.</returns>
        public static Texture2D GetColoreTextureDirectFast(QuantumCircuitFloat redCircuit, QuantumCircuitFloat greenCircuit, QuantumCircuitFloat blueCircuit, int width, int height, bool renormalize = false, bool useLog = false) {
            return QuantumImageHelper.CalculateColorTexture(redCircuit, greenCircuit, blueCircuit, width, height, renormalize);
        }

        #endregion


        //Helper functions, which need python code, thats why they are not exported to QuantumImageHelper
        #region Internal Helper Functions


        /// <summary>
        /// OLD VERSION use the faster version instead. 
        /// Getting a colored texture for given quantum circuits (each one representing 1 color channel of an image) directly without using python.
        /// Is faster than python versions but does not support logarithmic encoding yet and may still contain some errors.
        /// </summary>
        /// <param name="redCircuit">The quantum circuit which represents the red channel of the image.</param>
        /// <param name="greenCircuit">The quantum circuit which represents the green channel of the image.</param>
        /// <param name="blueCircuit">The quantum circuit which represents the blue channel of the image.</param>
        /// <param name="width">The width of the image</param>
        /// <param name="height">The height of the image</param>
        /// <param name="renormalize">If the image (colors) should be renormalized. (Giving it the highest possible saturation / becomes most light) </param>
        /// <param name="useLog">If logarithmic encoding is chosen DOES NOTHING (at the moment)</param>
        /// <returns>A texture showing the encoded image.</returns>
        public static Texture2D GetColoreTextureDirect(QuantumCircuit redCircuit, QuantumCircuit greenCircuit, QuantumCircuit blueCircuit, int width, int height,
            bool renormalize = false, bool useLog = false, SimulatorBase simulator = null) {
            double[,] redData = QuantumImageHelper.CircuitToHeight2D(redCircuit, width, height, renormalize, simulator);
            double[,] greenData = QuantumImageHelper.CircuitToHeight2D(greenCircuit, width, height, renormalize, simulator);
            double[,] blueData = QuantumImageHelper.CircuitToHeight2D(blueCircuit, width, height, renormalize, simulator);

            return QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);
        }

        #endregion
    }

}