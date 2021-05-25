using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyBinds", menuName = "KeyBinds")]
public class KeyBinds : ScriptableObject
{
    [System.Serializable]
   public class KeyBindCheck
    {
        public PlayerActions action;
        public KeyCode keyCode;
    }

    public KeyBindCheck[] keyBindChecks;
}
