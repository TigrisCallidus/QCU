# QCU
Quantum Computing Unity allows to make a call to a areal quantum computer from within unity


## Setup

In order to run QCU you need to first setup a python environment with qiskit installed.
One way to do this is, is by using anaconda with the following commands

conda create -n qiskitEnvironment python=3

conda activate qiskitEnvironment

pip install qiskit



For more info: https://qiskit.org/documentation/install.html

After you have created this environment, you can find it in your anaconda installation: https://docs.anaconda.com/anaconda/user-guide/faq/

In that folder there will be a folder "env" and inside there will be a folder "qiskitEnvironment" (if you named it like this).
Copy all the content of the folder "qiskitEnvironment" into your local copy of QCU into the folder Assets/StreamingAssets/.q 
(The folder will be empty except a PASTE_YOUR_FILES_HERE.txt)

## Using QCU

### Example

Under Assets/Examples there is the Scene QiskitExample.unity. In the Scene there is an Object QiskitExample with the Script RealQiskitBlurExample.cs on it.
That script contains example code on how to use QCU to run code on a real quantum computer.

To try it you can change the settings on the QiskitExample object. NOTE if you want to use a real quantum device instead of the qiskit simulator, you need to Provide your AuthentificationToken. You can copy paste it into the "Token" field of the "QiskitExample" Gameobject.

When you press play the script will automatically run. This example transforms an image (Texture) into a quantum circuit and applies a quantum blur to it.
Then it calculates the probabilities (using qiskit simulator or a real backend) and from the probabilities it generates the blurred image.

### General Usage

As you can see in the example there are these steps which needs to be done:

1. Create a new SimulateJob
2. Create a new QuantumCircuit with the needed number of qubits
  - If possible use no initialisation for the amplitudes, since this is slow on real devices
  - Add gates to the circuit (X, H, RX, CX, CRX gates are supported at the moment)
3. Create a new QiskitSimulator
  - Set the numberOfShots (number of times the circuit is run)
  - Set useRealMachine to true if you want to use a real device (or false if not)
  - Set the authToken to your authentification token for the IBM backend (if useRealMachine)
  - Specify the backend used (if you want a specific one)
4. Prepare the SimulateJob by setting the circuit and the simulator.
5. Start the SimulateJob
6. Wait for the SimulateJob to finish (in an Update)
7. When the Job is finished get the calculated probabilities from your job to further useage.
