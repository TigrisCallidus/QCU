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

//Class to run simple threaded jobs. Look at SimulateJob on how to use it.
public class ThreadedJob {
    private bool m_IsDone = false;
    private object m_Handle = new object();
    private System.Threading.Thread m_Thread = null;
    public bool IsDone {
        get {
            bool tmp;
            lock (m_Handle) {
                tmp = m_IsDone;
            }
            return tmp;
        }
        set {
            lock (m_Handle) {
                m_IsDone = value;
            }
        }
    }

    public virtual void Start() {
        m_Thread = new System.Threading.Thread(Run);
        m_Thread.Start();
    }
    public virtual void Abort() {
        m_Thread.Abort();
    }

    protected virtual void ThreadFunction() { }

    protected virtual void OnFinished() { }

    public virtual bool Update() {
        if (IsDone) {
            OnFinished();
            return true;
        }
        return false;
    }
    public IEnumerator WaitFor() {
        while (!Update()) {
            yield return null;
        }
    }
    private void Run() {
        ThreadFunction();
        IsDone = true;
    }
}