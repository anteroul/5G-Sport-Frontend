public enum HockeyState
{
    Skating,
    Shooting,
    Passing,
    Defending
}

public class PlayerBehaviour : MonoBehaviour
{
    private Queue<HockeyAction> actionQueue = new Queue<HockeyAction>();
    private float actionTimer = 0f;
    private HockeyState currentState = HockeyState.Skating;
    
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LoadActionsFromJson();
        if (actionQueue.Count > 0)
            SetNextAction();
    }

    void Update()
    {
        if (actionQueue.Count == 0) return;

        actionTimer -= Time.deltaTime;
        if (actionTimer <= 0)
        {
            SetNextAction();
        }

        HandleState();
    }

    void LoadActionsFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("hockey_actions"); // Ensure file is in the Resources folder
        if (jsonFile == null)
        {
            Debug.LogError("Failed to load hockey_actions.json");
            return;
        }

        HockeyActionList actionDataList = JsonUtility.FromJson<HockeyActionList>(jsonFile.text);
        foreach (var action in actionDataList.actions)
        {
            actionQueue.Enqueue(action);
        }
    }

    void SetNextAction()
    {
        if (actionQueue.Count == 0) return;

        HockeyAction nextAction = actionQueue.Dequeue();
        actionTimer = nextAction.duration;
        currentState = ParseState(nextAction.actionType);
        Debug.Log($"Switched to state: {currentState} for {actionTimer} seconds");
    }

    HockeyState ParseState(string actionType)
    {
        return actionType.ToLower() switch
        {
            "skating" => HockeyState.Skating,
            "shooting" => HockeyState.Shooting,
            "passing" => HockeyState.Passing,
            "defending" => HockeyState.Defending,
            _ => HockeyState.Skating
        };
    }

    void HandleState()
    {
        switch (currentState)
        {
            case HockeyState.Skating:
                rb.velocity = new Vector2(3, 0); // Simulate skating forward
                break;
            case HockeyState.Shooting:
                rb.velocity = Vector2.zero;
                Debug.Log("Shoots the puck!");
                break;
            case HockeyState.Passing:
                rb.velocity = Vector2.zero;
                Debug.Log("Passes to a teammate.");
                break;
            case HockeyState.Defending:
                rb.velocity = new Vector2(-2, 0); // Skating backward (defending position)
                Debug.Log("Defending against the opponent.");
                break;
        }
    }
}
