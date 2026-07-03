using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SoulHunter.Enemy;

namespace SoulHunter.Base
{
    public class DataManager : MonoBehaviour // Mort
    {
        // Singleton Instance
        public static DataManager Instance;

        [Header("Game Statistics")]
        public int soulsCollected;
        public int velocityScore;
        public float durationInAir;

        [Header("Input Statistics")]
        public int timesJumped;
        public int timesMissedGrapple;
        public int timesHitGrapple;
        public int timesMissedAttack;
        public int timesHitAttack;

        void Start()
        {
            Instance = this;
        }
    }
}