﻿using UnityEngine;
using UnityEngine.UI;

public class ChatLogCellView : MonoBehaviour {

    [SerializeField] Text label = null;

    public void SetLogText (string logMessage) {
        label.text = logMessage;
    }
}
