using UnityEngine;
using UnityEngine.UI;

public class ChatLogView : MonoBehaviour {

    /// <summary>
    /// The cell view prefab.
    /// </summary>
    [SerializeField] ChatLogCellView cellViewPrefab = null;
    /// <summary>
    /// The chat log scroll.
    /// </summary>
    [SerializeField] ScrollRect chatLogScroll = null;
    /// <summary>
    /// The input field.
    /// </summary>
    [SerializeField] public InputField inputField = null;

    void Start() {
        /*
        inputField.onEndEdit.AddListener((log) => {
            AddChatLog(log);
            inputField.text = string.Empty;
        });
        */
    }

    public void AddChatLog (string message) {
        if (string.IsNullOrEmpty (message))
            return;

        ChatLogCellView cellView = Instantiate(cellViewPrefab) as ChatLogCellView;
        cellView.transform.SetParent(chatLogScroll.content, false);
        cellView.transform.localScale = Vector3.one;
        cellView.SetLogText(message);
    }
}
