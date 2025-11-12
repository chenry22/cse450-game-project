using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSimulationController : MonoBehaviour
{
    TestSimulationController instance;

    void Awake() {
        instance = this;
    }
}
