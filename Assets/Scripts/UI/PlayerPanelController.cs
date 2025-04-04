using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Required for TextMeshPro


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

    public StatsPanelController statsPanel; // Reference to the Stats Panel
    private int selectedPlayerID = -1;

    void Start()
    {
        ShowTeam1(); // Default to Team 1 view

        team1Button.onClick.AddListener(ShowTeam1);
        team2Button.onClick.AddListener(ShowTeam2);

        InitializePlayerCards();
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
        {
            card.Initialize(this);
        }

        foreach (var card in team2Cards)
        {
            card.Initialize(this);
        }
    }

    public void SetSelectedPlayer(int playerID, string playerName, string team)
    {
        selectedPlayerID = playerID;
        statsPanel.UpdatePlayerInfo(playerID, playerName, team);
    }

    // Nested class for Player Cards (Manually Assigned)
    [System.Serializable]
    public class PlayerCardUI
    {
        public GameObject cardObject; // The actual card GameObject
        public TMP_Text playerIDText;
        public TMP_Text playerNameText;
        public string team;
        public Button selectButton;

        private PlayerPanelController panelController;
        private int playerID;

        public void Initialize(PlayerPanelController controller)
        {
            panelController = controller;

            // Read manually entered ID from Text
            if (int.TryParse(playerIDText.text, out int parsedID))
            {
                playerID = parsedID;
            }
            else
            {
                playerID = -1; // Invalid ID case
            }

            // Assign button functionality
            selectButton.onClick.AddListener(SelectPlayer);
        }

        void SelectPlayer()
        {
            if (playerID != -1)
            {
                panelController.SetSelectedPlayer(playerID, playerNameText.text, team);
            }
            else
            {
                Debug.LogWarning("Invalid Player ID! Please enter a valid number.");
            }
        }
    }
}
