using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UIset.util
{

    public class CheckboxDefaultON
    {
        private bool toggleState;

        public bool ToggleState
        {
            get { return toggleState; }
            set { toggleState = value; }
        }

        public void DrawCheckboxGUI(string label)
        {
            toggleState = GUILayout.Toggle(toggleState, label);
        }
    }
}
