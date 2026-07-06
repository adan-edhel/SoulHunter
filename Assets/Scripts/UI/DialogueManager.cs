using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

using SoulHunter.Player;
using UnityEngine.EventSystems;

namespace SoulHunter.Dialogue
{
    public class DialogueManager : MonoBehaviour // Mort
    {
        // Singleton Instance
        public static DialogueManager Instance { get; private set; }

        // Dialogue state
        public static bool inDialogue;

        // Queue of dialogue sentences
        private Queue<string> sentences;

        // Currently triggered dialogue
        private Dialogue currentDialogue;
        private DialogueTrigger currentTrigger;

        private bool nonTriggerable;
        private int currentExchange;

        Animator animator;

        [Header("Fonts")] // Font variables
        [SerializeField] TMP_FontAsset dialogueFont;
        [SerializeField] TMP_FontAsset normalFont;

        [Header("Objects")] // Object variables
        [SerializeField] GameObject dialogueBox;
        [SerializeField] Button ContinueButton;

        [Header("Images")] // Image variables
        [SerializeField] Image portraitImage;
        [SerializeField] Image portraitFrame;

        [Header("Texts")] // Text variables
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI dialogueText;

        void Start()
        {
            Instance = this;
            inDialogue = false;
            sentences = new Queue<string>();

            animator = dialogueBox.GetComponent<Animator>();
        }

        /// <summary>
        /// Initializes dialogue and sets respective text colors
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="dialogue"></param>
        /// <param name="_nameColor"></param>
        /// <param name="_dialogueColor"></param>
        public void StartDialogue(DialogueTrigger trigger, Dialogue dialogue)
        {
            ContinueButton.Select();

            // Set current exchange index
            currentExchange = 0;

            // Set current trigger & dialogue references
            currentTrigger = trigger;
            currentDialogue = dialogue;

            // Change dialogue state
            inDialogue = true;
            // Pause player input
            PlayerBase.isPaused = true;

            // Update animator parameters
            animator.SetBool("inDialogue", inDialogue);

            // Clear the queue of sentences
            sentences.Clear();

            PrepareExchange(dialogue.exchanges[currentExchange]);

            // Play dialogue start sound effect
            AudioManager.PlaySound(AudioManager.Sound.StartDialogue,
                currentTrigger.transform.position);
        }

        /// <summary>
        /// Prepares the current exchange and initiates it
        /// </summary>
        /// <param name="exchange"></param>
        public void PrepareExchange(DialogueExchange exchange)
        {
            UpdateDialogueElements();

            foreach (string sentence in exchange.sentences)
            {
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }

        /// <summary>
        /// Types the next sentence in the dialogue box
        /// </summary>
        public void DisplayNextSentence()
        {
            // At the start of each sentence, select continue button
            ContinueButton.Select();

            HandleNPCAnimation();

            // If the dialogue isn't triggerable
            if (nonTriggerable)
            {
                inDialogue = false;
                animator.SetBool("inDialogue", inDialogue);
                PlayerBase.isPaused = false;
                nonTriggerable = false;
            }
            else
            {
                // If dialogue sentences are finished
                if (sentences.Count == 0)
                {
                    // If there are more exchanges in the dialogue
                    if (currentDialogue.exchanges.Length > currentExchange + 1)
                    {
                        // Increase exchange index by 1
                        currentExchange++;
                        // Prepare next exchange
                        PrepareExchange(currentDialogue.exchanges[currentExchange]);
                        return;
                    }
                    
                    // Reset dialogue elements & values, and end dialogue.
                    dialogueText.fontStyle = FontStyles.Italic;
                    dialogueText.color = Color.white;
                    dialogueText.font = normalFont;
                    dialogueText.fontSize = 20;
                    dialogueText.text = "";
                    currentDialogue = null;
                    EndDialogue();
                    return;
                }

                dialogueText.fontStyle = FontStyles.Normal;
                dialogueText.font = dialogueFont;
                dialogueText.fontSize = 36;
                string sentence = sentences.Dequeue();
                StopAllCoroutines();
                StartCoroutine(TypewriteSentence(sentence));
            }
        }

        /// <summary>
        /// Indicates end of dialogue and handles the termination of current dialogue
        /// </summary>
        public void EndDialogue()
        {
            StopAllCoroutines();
            UpdateDialogueElements();
            inDialogue = false;
            PlayerBase.isPaused = false;
            nonTriggerable = true;

            // Update animator parameters
            animator.SetBool("inDialogue", inDialogue);

            // Handle trigger activation
            currentTrigger.HandleTriggerActivation(false);

            // Deselect all UI elements
            EventSystem.current.SetSelectedGameObject(null);

            // Destroy NPC
            if (currentTrigger.NPC)
            {
                Destroy(currentTrigger.NPC, 1);
            }

            // Play dialogue end sound
            AudioManager.PlaySound(AudioManager.Sound.EndDialogue,
                currentTrigger.transform.position);
        }

        /// <summary>
        /// Types dialogue in a typewriter style
        /// </summary>
        /// <param name="_sentence"></param>
        /// <returns></returns>
        IEnumerator TypewriteSentence(string _sentence)
        {
            dialogueText.text = "";
            foreach (char letter in _sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(GameSettings.dialogueSpeed / 10);
            }
        }

        /// <summary>
        /// Updates elements in the dialogue UI in their 
        /// respective context
        /// </summary>
        void UpdateDialogueElements()
        {
            CharacterData person = currentDialogue?.
                exchanges[currentExchange].character;

            if (person)
            {
                nameText.text = person.name;
                nameText.color = person.nameColor;
                dialogueText.color = person.dialogueColor;

                if (person.portrait)
                {
                    portraitImage.color = Color.white;
                    portraitFrame.color = Color.white;
                    portraitImage.sprite = person.portrait;
                    portraitImage.preserveAspect = true;
                }
                else
                {
                    portraitImage.color = Color.clear;
                    portraitFrame.color = Color.clear;
                }
            }
            else
            {
                nameText.text = "";
                nameText.color = Color.white;
                dialogueText.color = Color.white;

                portraitImage.color = Color.clear;
                portraitFrame.color = Color.clear;
            }
        }

        /// <summary>
        /// Handles NPC animation according to exchange being played
        /// </summary>
        void HandleNPCAnimation()
        {
            if (!currentTrigger.NPC) return;

            if (currentDialogue.exchanges[currentExchange].isPointing)
            {
                currentTrigger.NPC?.GetComponentInChildren<Animator>().SetBool("isPointing", true);
            }
            else
            {
                currentTrigger.NPC?.GetComponentInChildren<Animator>().SetBool("isPointing", false);
            }
        }

        /// <summary>
        /// Pauses dialogue when game is paused
        /// </summary>
        public void PauseCheck()
        {
            if (GameManager.gameIsPaused)
            {
                animator.SetBool("GameIsPaused", true);
            }
            else
            {
                animator.SetBool("GameIsPaused", false);
            }
        }

        private void Update()
        {
            // When using mouse, reselects continue button
            // to avoid losing the cursor and getting stuck
            if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (inDialogue)
                {
                    ContinueButton.Select();
                }
            }
        }
    }

    [System.Serializable]
    public class Dialogue
    {
        public DialogueExchange[] exchanges;
    }

    [System.Serializable]
    public class DialogueExchange
    {
        public bool isPointing;

        public CharacterData character;

        [TextArea(3, 10)]
        public string[] sentences;
    }
}