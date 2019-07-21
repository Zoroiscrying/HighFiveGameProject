using System;
using System.Collections;
using System.Collections.Generic;
using Game.Const;
using Game.Model.SceneSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            SceneMgr.LoadScene(SceneName.testScene,
                () => print("Test_OnLeaveScene"),
                () => print("Test_OnEnterNewScene"));

    }
}
