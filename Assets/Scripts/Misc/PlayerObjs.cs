using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * holder of all player related game objects
 */
public class PlayerObjs : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private Camera playerCam;
    [SerializeField] private GameObject playerCanvas;
    public TextMeshProUGUI waveText;

    public static PlayerObjs i { get; private set; }

    private void Awake()
    {
        if (i != null)
        {
            Destroy(gameObject);
        }
        else
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>(
            FindObjectsInactive.Include);
    }

    public void SetUpPlayerObjs()
    {
        playerController.gameObject.SetActive(true);
        playerCam.gameObject.SetActive(true);
        playerCanvas.SetActive(true);
    }
}
