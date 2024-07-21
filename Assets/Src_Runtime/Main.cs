using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    bool isHumanWDown;
    bool isAIWDown;

    void Start() {

    }

    void Update() {

        // Process Input
        // 人类的输入
        if (Input.GetKey(KeyCode.W)) {
            isHumanWDown = true;
        } else {
            isHumanWDown = false;
        }

        if (Time.time % 2 == 0) {
            isAIWDown = true;
        } else {
            isAIWDown = false;
        }

        // DoLogic
        if (isHumanWDown) {
            Debug.Log("人类前进");
        }

        if (isAIWDown) {
            Debug.Log("AI前进");
        }

    }

}
