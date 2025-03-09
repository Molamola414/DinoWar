using System;
using UnityEngine;

namespace Assets.MicroWar.Scripts.Common
{
    /// <summary>
    /// Attach it to camera to take screenshots in run mode, [S] key by default
    /// </summary>
    public class Screenshot : MonoBehaviour
    {
        /// <summary>
        /// Screenshot size multiplier
        /// </summary>
        public int SuperSize = 1;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                var filename = DateTime.Now.Ticks + ".png";

                ScreenCapture.CaptureScreenshot(filename, SuperSize);
                Debug.Log(filename);
            }
        }
    }
}