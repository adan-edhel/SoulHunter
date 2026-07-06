using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using SoulHunter.Player;

namespace SoulHunter.UI
{
    public class GameUI : MonoBehaviour
    {
        private int playerHealth;
        public List<GameObject> healthList;

        public GameObject healthPanel;

        private void Start()
        {
            // Gets player health
            playerHealth = FindObjectOfType<PlayerBase>().Health;
            // Sets reference inside UIManager
            CanvasHandler.Instance.GameCanvas = gameObject;
            // Disables cursor
            Cursor.visible = false;

            for (int i = 0; i < healthPanel.transform.childCount; i++)
            {
                healthList.Add(healthPanel.transform.GetChild(i).gameObject);
            }
        }

        public void UpdateHealthPanel(int health)
        {
            for (int i = 0; i < healthList.Count; i++)
            {
                healthList[i].SetActive(false);
            }

            for (int i = 0; i < health; i++)
            {
                healthList[i].SetActive(true);
            }
        }
    }
}