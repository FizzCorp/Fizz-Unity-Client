using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Fizz;
using Fizz.Chat;
using Fizz.Common;

/*
 * Implement a basic multi-lingual chat client.
*/
public class ChatPanel : MonoBehaviour {
    class MessageData
    {
        public long id;
        public string from;
        public string nick;
        public string to;
        public string body;

        public MessageData(FizzChannelMessage message, string locale)
        {
            id = message.Id;
            from = message.From;
            nick = message.Nick;
            to = message.To;
            body = message.Body;

            if (message.Translations != null && message.Translations.ContainsKey(locale)) 
            {
                body = message.Translations[locale];
            }
        }

        public string SaveToString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    //Demo
    private static string APP_ID = "751326fc-305b-4aef-950a-074c9a21d461";
    private static string APP_SECRET = "5c963d03-64e6-439a-b2a9-31db60dd0b34";
    
    //Channel
    private static string CHANNEL_ID = "global-sample";

    [SerializeField] Dropdown userIdDropdown = null;
    [SerializeField] Dropdown languageDropdown = null;
    [SerializeField] Button connectButton = null;

    private IFizzClient _client = new FizzClient(APP_ID, APP_SECRET);
    private ChatLogView _logView;
    private ICollection<long> _receivedMessage = new HashSet<long>();

    public string UserId
    {
        get
        {
            return userIdDropdown.captionText.text;
        }
    }

    public IFizzLanguageCode Locale
    {
        get
        {
            return MapLocale(languageDropdown.captionText.text);
        }
    }

    private void Awake()
    {
        if (connectButton != null)
        {
            connectButton.onClick.AddListener(OnBtnConnect);
        }

        _logView = GetComponent<ChatLogView>();
        _logView.inputField.onEndEdit.AddListener(OnSendMessage);
        _logView.inputField.interactable = false;
    }

    private void Update()
    {
        _client.Update();
    }

    private IFizzLanguageCode MapLocale(string selection) 
    {
        switch (selection)
        {
            case "French":
                return FizzLanguageCodes.French;
            case "Spanish":
                return FizzLanguageCodes.Spanish;
            default:
                return FizzLanguageCodes.English;
        }
    }

    private void OnBtnConnect()
    {
        connectButton.interactable = false;
        _client.Open(UserId, Locale, FizzServices.All, ex => {
            if (ex != null)
            {
                Debug.LogError("Failed to connect: " + ex.Message);
            }
            else 
            {
                _client.Chat.Listener.OnConnected = OnFizzConnected;
                _client.Chat.Listener.OnMessagePublished = OnMessagePublished;
            }
        });
    }

    private void OnFizzConnected(bool syncRequired)
    {
        if (!syncRequired)
        {
            return;
        }

        _logView.inputField.interactable = true;
        _client.Chat.Subscribe(CHANNEL_ID, ex =>
        {
            if (ex != null)
            {
                Debug.LogError(ex.Message);
            }
            else
            {
                _client.Chat.QueryLatest(CHANNEL_ID, 5, OnMessageHistory);
            }
        });
    }

    private void OnMessageHistory(IList<FizzChannelMessage> messages, FizzException ex)
    {
        if (ex != null)
        {
            Debug.LogError(ex);
            return;
        }

        foreach (FizzChannelMessage message in messages)
        {
            _logView.AddChatLog(new MessageData(message, Locale.Code).SaveToString());
        }
    }

    private void OnMessagePublished(FizzChannelMessage message)
    {
        if (_receivedMessage.Contains(message.Id))
        {
            // duplicate message;
            return;
        }

        _receivedMessage.Add(message.Id);
        string json = new MessageData(message, Locale.Code).SaveToString();
        _logView.AddChatLog(json);
    }

    private void OnSendMessage(string text)
    {
        _client.Chat.PublishMessage(
            CHANNEL_ID, 
            UserId, 
            body: text, 
            data: null, 
            translate: true,
            filter: true,
            persist: true, 
            callback: ex => {
                if (ex != null)
                {
                    Debug.LogError(ex.Message);
                }
            }
        );

        _logView.inputField.text = string.Empty;
    }
}
