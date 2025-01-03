using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private static SceneManager _instance;

    public static SceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("SceneManager");
                _instance = obj.AddComponent<SceneManager>();
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && IsGameOverScene())
        {
            LoadScene("InGame");
        }
    }

    public bool IsGameOverScene()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game Over";
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}