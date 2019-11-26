using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestTools : UnityEngine.MonoBehaviour
    {
        public Animation Animation;
        public KeyCode keyToPlay = KeyCode.P;

        private void Update()
        {
            if (Animation && Input.GetKeyDown(keyToPlay))
                print( Animation.Play());
        }
    }
}