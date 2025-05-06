using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerPanelController : MonoBehaviour
{
    [Header("Team Viewports")]
    public GameObject team1Viewport;
    public GameObject team2Viewport;

    [Header("Player Cards (Manually Assigned)")]
    public List<PlayerCardUI> team1Cards = new List<PlayerCardUI>();
    public List<PlayerCardUI> team2Cards = new List<PlayerCardUI>();

    [Header("UI Buttons")]
    public Button team1Button;
    public Button team2Button;

    [Header("Detail Panel")]
    public StatsPanelController statsPanel; // Reference to the Stats Panel

    [Header("ECG Wave Object")]
    public GameObject ecgWave;

    void Start()
    {
        ecgWave.SetActive(false);
        ShowTeam1();
        team1Button.onClick.AddListener(ShowTeam1);
        team2Button.onClick.AddListener(ShowTeam2);
        InitializePlayerCards();
    }

    // Expose this instance method
    public void ShowECGWave(bool show)
    {
        ecgWave.SetActive(show);  // Valid: this.ecgWave exists here :contentReference[oaicite:2]{index=2}
    }

    void ShowTeam1()
    {
        team1Viewport.SetActive(true);
        team2Viewport.SetActive(false);
    }

    void ShowTeam2()
    {
        team1Viewport.SetActive(false);
        team2Viewport.SetActive(true);
    }

    void InitializePlayerCards()
    {
        foreach (var card in team1Cards)
            card.Initialize(this);
        foreach (var card in team2Cards)
            card.Initialize(this);
    }

    /// <summary>
    /// Called by a PlayerCardUI when its Select button is clicked.
    /// </summary>
    public void SetSelectedPlayer(int playerID, string playerName, string team)
    {
        // 1) Turn off all fake-emitters
        foreach (var c in team1Cards) c.SetEmitting(false);
        foreach (var c in team2Cards) c.SetEmitting(false);

        // 2) Find & enable only the chosen card
        PlayerCardUI sel =
            team1Cards.Find(c => c.PlayerID == playerID)
          ?? team2Cards.Find(c => c.PlayerID == playerID);

        if (sel != null)
            sel.SetEmitting(true);

        // 3) Update the detail panel afterwards
        statsPanel.UpdatePlayerInfo(playerID, playerName, team);
    }

    [System.Serializable]
    public class PlayerCardUI
    {
        public GameObject cardObject;   // The root of the card prefab
        public TMP_Text playerIDText;
        public TMP_Text playerNameText;
        public string team;
        public Button selectButton;

        PlayerPanelController panelController;
        FakeDataController fakeCtrl;
        public int PlayerID { get; private set; }

        /// <summary>
        /// Called once on Start(); caches FakeDataController and wires the button.
        /// </summary>
        public void Initialize(PlayerPanelController controller)
        {
            panelController = controller;

            // Parse the PlayerID from the text
            if (!int.TryParse(playerIDText.text, out var id))
                id = -1;
            PlayerID = id;

            // Cache reference to the FakeDataController on this card
            fakeCtrl = cardObject.GetComponent<FakeDataController>();
            if (fakeCtrl == null)
                Debug.LogWarning($"No FakeDataController on cardObject for ID {PlayerID}");

            // Wire up the Select button
            selectButton.onClick.AddListener(OnSelectPressed);
        }

        void OnSelectPressed()
        {
            if (PlayerID < 0)
            {
                Debug.LogWarning("Invalid PlayerID");
                return;
            }
            panelController.SetSelectedPlayer(PlayerID, playerNameText.text, team);
            //ecgWave.SetActive(true);
            panelController.ShowECGWave(true);
        }

        /// <summary>
        /// Toggles the cached FakeDataController’s isEmitting flag.
        /// </summary>
        public void SetEmitting(bool emit)
        {
            if (fakeCtrl != null)
                fakeCtrl.isEmitting = emit;
        }
    }
}
