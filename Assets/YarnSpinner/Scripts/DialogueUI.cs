/*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using TMPro;

namespace Yarn.Unity
{
    /// Displays dialogue lines to the player, and sends
    /// user choices back to the dialogue system.

    public class DialogueUI : Yarn.Unity.DialogueUIBehaviour
    {

        /// The object that contains the dialogue and the options.
        /** This object will be enabled when conversation starts, and
         * disabled when it ends.
         */
        public GameObject dialogueContainer;

        /// How quickly to show the text, in seconds per character
        [Tooltip("How quickly to show the text, in seconds per character")]
        public float textSpeed = 0.025f;
        public float nextLineSeconds = 3f;
        public float waitForOptionsTimer = 5f;

        /// The buttons that let the user choose an option
        public List<Button> optionButtons;
        public TextMeshProUGUI lineText;

        // When true, the user has indicated that they want to proceed to
        // the next line.
        private bool userRequestedNextLine = false;

        // The method that we should call when the user has chosen an
        // option. Externally provided by the DialogueRunner.
        private System.Action<int> currentOptionSelectionHandler;

        // When true, the DialogueRunner is waiting for the user to press
        // one of the option buttons.
        private bool waitingForOptionSelection = false;

        public UnityEngine.Events.UnityEvent onDialogueStart;

        public UnityEngine.Events.UnityEvent onDialogueEnd;

        public UnityEngine.Events.UnityEvent onLineStart;
        public UnityEngine.Events.UnityEvent onLineFinishDisplaying;
        public DialogueRunner.StringUnityEvent onLineUpdate;
        public UnityEngine.Events.UnityEvent onLineEnd;

        public UnityEngine.Events.UnityEvent onOptionsStart;
        public UnityEngine.Events.UnityEvent onOptionsEnd;

        public DialogueRunner.StringUnityEvent onCommand;

        void Awake()
        {
            // Start by hiding the container
            if (dialogueContainer != null)
                dialogueContainer.SetActive(false);

            foreach (var button in optionButtons)
            {
                button.gameObject.SetActive(false);
            }
        }

        public override Dialogue.HandlerExecutionType RunLine(Yarn.Line line, IDictionary<string, string> strings, System.Action onComplete)
        {
            // Start displaying the line; it will call onComplete later
            // which will tell the dialogue to continue
            StartCoroutine(DoRunLine(line, strings, onComplete));
            return Dialogue.HandlerExecutionType.PauseExecution;
        }

        /// Show a line of dialogue, gradually        
        private IEnumerator DoRunLine(Yarn.Line line, IDictionary<string, string> strings, System.Action onComplete)
        {
            onLineStart?.Invoke();

            userRequestedNextLine = false;

            if (strings.TryGetValue(line.ID, out var text) == false)
            {
                Debug.LogWarning($"Line {line.ID} doesn't have any localised text.");
                text = line.Text;
            }

            Voice.Registry.TryGetValue(GetNameFromLine(text), out Voice voice);

            if (voice == null)
                lineText.color = Color.black;
            else
                lineText.color = voice.voiceColor;

            if (textSpeed > 0.0f)
            {
                // Display the line one character at a time
                var stringBuilder = new StringBuilder();

                foreach (char c in text)
                {

                    // play a voice if character is a vowel
                    if ("aeiouAEIOU".IndexOf(c) >= 0)
                    {
                        // get the voice event and play it as a oneshot
                        voice?.Talk();
                    }

                    stringBuilder.Append(c);
                    onLineUpdate?.Invoke(stringBuilder.ToString());
                    if (userRequestedNextLine)
                    {
                        // We've requested a skip of the entire line.
                        // Display all of the text immediately.
                        onLineUpdate?.Invoke(text);
                        break;
                    }
                    yield return new WaitForSeconds(textSpeed);
                }
            }
            else
            {
                // Display the entire line immediately if textSpeed <= 0
                onLineUpdate?.Invoke(text);
            }

            // We're now waiting for the player to move on to the next line
            userRequestedNextLine = false;

            // Indicate to the rest of the game that the line has finished being delivered
            onLineFinishDisplaying?.Invoke();

            //while (userRequestedNextLine == false) {
            //    yield return null;
            //}
            yield return new WaitForSeconds(nextLineSeconds);

            //// Avoid skipping lines if textSpeed == 0
            //yield return new WaitForEndOfFrame();

            // Hide the text and prompt
            onLineEnd?.Invoke();

            onComplete();

        }

        public override void RunOptions(Yarn.OptionSet optionsCollection, IDictionary<string, string> strings, System.Action<int> selectOption)
        {
            StartCoroutine(DoRunOptions(optionsCollection, strings, selectOption));
        }

        /// Show a list of options, and wait for the player to make a
        /// selection.
        public IEnumerator DoRunOptions(Yarn.OptionSet optionsCollection, IDictionary<string, string> strings, System.Action<int> selectOption)
        {
            // Display each option in a button, and make it visible
            int i = 0;

            waitingForOptionSelection = true;

            currentOptionSelectionHandler = selectOption;

            foreach (var optionString in optionsCollection.Options)
            {

                // dont show options if no buttons are available
                if (i >= optionButtons.Count - 1)
                    continue;
                // dont show the last option ever
                if (i >= optionsCollection.Options.Length - 1)
                    continue;

                optionButtons[i].gameObject.SetActive(true);

                // When the button is selected, tell the dialogue about it
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => SelectOption(optionString.ID));

                if (strings.TryGetValue(optionString.Line.ID, out var optionText) == false)
                {
                    Debug.LogWarning($"Option {optionString.Line.ID} doesn't have any localised text");
                    optionText = optionString.Line.ID;
                }

                var unityText = optionButtons[i].GetComponentInChildren<Text>();
                if (unityText != null)
                {
                    unityText.text = optionText;
                }

                var textMeshProText = optionButtons[i].GetComponentInChildren<TMPro.TMP_Text>();
                if (textMeshProText != null)
                {
                    textMeshProText.text = optionText;
                }

                i++;
            }

            onOptionsStart?.Invoke();

            // Wait until the chooser has been used and then removed 
            float timer = 0f;
            while (waitingForOptionSelection)
            {
                timer += Time.deltaTime;

                // after some time always choose the last option
                if (timer > waitForOptionsTimer)
                {
                    SelectOption(optionsCollection.Options.Length - 1);
                }

                yield return null;
            }


            // Hide all the buttons
            foreach (var button in optionButtons)
            {
                button.gameObject.SetActive(false);
            }

            onOptionsEnd?.Invoke();

        }

        /// Run an internal command.
        public override Dialogue.HandlerExecutionType RunCommand(Yarn.Command command, System.Action onComplete)
        {
            StartCoroutine(DoRunCommand(command, onComplete));
            return Dialogue.HandlerExecutionType.ContinueExecution;
        }

        public IEnumerator DoRunCommand(Yarn.Command command, System.Action onComplete)
        {
            // "Perform" the command
            Debug.Log("Command: " + command.Text);
            onComplete?.Invoke();
            yield break;
        }

        /// Called when the dialogue system has started running.
        public override void DialogueStarted()
        {
            // Enable the dialogue controls.
            if (dialogueContainer != null)
                dialogueContainer.SetActive(true);

            onDialogueStart?.Invoke();
        }

        /// Called when the dialogue system has finished running.
        public override void DialogueComplete()
        {
            onDialogueEnd?.Invoke();

            // Hide the dialogue interface.
            if (dialogueContainer != null)
                dialogueContainer.SetActive(false);

        }

        public void MarkLineComplete()
        {
            userRequestedNextLine = true;
        }

        public void SelectOption(int index)
        {
            if (waitingForOptionSelection == false)
            {
                Debug.LogWarning("An option was selected, but the dialogue UI was not expecting it.");
                return;
            }
            waitingForOptionSelection = false;
            currentOptionSelectionHandler?.Invoke(index);
        }

        protected string GetNameFromLine(string line)
        {
            if (line.Length == 0)
                return "";
            int indexOfPoints = line.IndexOf(':');


            if (indexOfPoints <= 0)
            {
                Debug.LogWarning("No name for this line!");
                return "";
            }
            else
                return line.Substring(0, indexOfPoints);

        }

        protected string GetLineFromLineWithoutName(string line)
        {
            if (line.Length == 0)
                return "";
            int indexOfPoints = line.IndexOf(':');

            if (indexOfPoints <= 0)
            {
                Debug.LogWarning("No name for this line!");
                return "";
            }
            else
                return line.Substring(indexOfPoints + 2);
        }

    }

}
