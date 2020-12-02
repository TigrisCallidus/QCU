from qiskit import(
  QuantumCircuit,
  execute,
  Aer)

simulator = Aer.get_backend('qasm_simulator')

qc = QuantumCircuit(16,16)
qc.rx(0, 0)
qc.rx(0, 1)
qc.rx(0, 2)
qc.rx(0, 3)
qc.rx(0, 4)
qc.rx(0, 5)
qc.rx(0, 6)
qc.rx(0, 7)
qc.rx(0, 8)
qc.rx(0, 9)
qc.rx(0, 10)
qc.rx(0, 11)
qc.rx(0, 12)
qc.rx(0, 13)
qc.rx(0, 14)
qc.rx(0, 15)
qc.measure([0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15], [0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15])

job = execute(qc, simulator, shots=1000)

result = job.result()
counts = result.get_counts(qc)
print(counts)