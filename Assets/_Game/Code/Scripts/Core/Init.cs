using System;
using UnityEngine;

namespace VinhLB
{
    public class Init : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}