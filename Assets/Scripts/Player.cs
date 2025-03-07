using UnityEngine;
using System.IO;

public class Player : MonoBehaviour
{
    enum Colour {
        UNDEFINED = 0,
        BLUE,
        RED,
    };

    [SerializeField]
    public bool enableControls;
    public string fileName = "test.json";
    
    [Tooltip("Drag the ice rink game object inside the rink prefab here.")]
    public GameObject rink;
    public GameObject jerseyPicker;
    public float movementSpeed = 3.5f;
    
    [Tooltip("Untoggle = Blue | Toggle = Red")]
    public bool team;

    private Colour col = Colour.UNDEFINED;
    private SpriteRenderer ren;
    private Vector2 playerPosition;
    private Vector2 waypoint;
    private Collider2D skateArea;
    private int playerID = 0;

    void Awake()
    {
        FetchData();
        transform.Translate(playerPosition);
        Debug.Log("Player ID:" + playerID + " spawned in " + playerPosition);
        skateArea = rink.GetComponent<PolygonCollider2D>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ren = jerseyPicker.GetComponent<SpriteRenderer>();
        col = team ? Colour.RED : Colour.BLUE;
        
        if ((int)col == 0)
        {
            return;
        }

        ren.enabled = false;
        SetTeamJersey(team ? "Red" : "Blue");
    }

    void SetTeamJersey(string colour)
    {
        ren = jerseyPicker.transform.Find(colour).GetComponent<SpriteRenderer>();
        ren.enabled = true;
    }

    void Update()
    {
        // TODO: Collisions with walls.
        if (!enableControls)
        {
            Vector2 movement = waypoint * movementSpeed * Time.deltaTime;
            transform.Translate(movement);
        } else {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector2 movement = new Vector2(horizontalInput, verticalInput) * movementSpeed * Time.deltaTime;
            transform.Translate(movement);
        }
    }

    void FetchData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileName.Replace(".json", ""));

        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found: " + fileName);
            return;
        }

        PlayerAction data = JsonUtility.FromJson<PlayerAction>(jsonFile.text);
        playerID = data.id;
        // Apply position to GameObject
        waypoint = new Vector2(data.x, data.y);
    }
}
