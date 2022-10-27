using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;
    public static readonly string[] levelNames = new string[] { "LevelOne", "LevelTwo" };

    [SerializeField] private GameObject closeOnLoadingLevel;
    [SerializeField] private GameObject showOnLoadingLevel;
    [SerializeField] private GameObject resultPagePrefab;

    private Slider progressBar;
    private Text progressText;
    private SavedRecord _saveRecord = null;

    public int chosenCharacterIndex { get; private set; } = 0;
    public SavedRecord savedRecord
    {
        get
        {
            SavedRecord temp = this._saveRecord;
            this._saveRecord = null;
            return temp;
        }
        set
        {
            this._saveRecord = value;
        }
    }


    private void Awake()
    {
        GameManager.Instance = this;
        DontDestroyOnLoad(this.gameObject);
        this.progressBar = this.showOnLoadingLevel.transform.Find("LoadingProgressBar").GetComponent<Slider>();
        this.progressText = this.showOnLoadingLevel.transform.Find("LoadingProgressText").GetComponent<Text>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Delete))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void StartNewGame(int chosenCharacterIndex)
    {
        this.chosenCharacterIndex = chosenCharacterIndex;
        StartCoroutine(this.LoadLevel(GameManager.levelNames[0]));
    }

    public void ResumeSavedGame(SavedRecord savedRecord)
    {
        this.savedRecord = savedRecord;
        this.chosenCharacterIndex = savedRecord.characterData.chosenCharacterIndex;
        StartCoroutine(this.LoadLevel(GameManager.levelNames[savedRecord.levelData.currentLevelIndex]));
    }

    public void GoToNextStage()
    {
        int currentSceneIndex = GameManager.levelNames.Select((name, index) => new { name, index }).First((each) => each.name == SceneManager.GetActiveScene().name).index;
        if (currentSceneIndex == GameManager.levelNames.Length - 1)
        {
            this.ShowGameResult(true);
        }
        else
        {
            StartCoroutine(this.LoadLevel(GameManager.levelNames[currentSceneIndex + 1]));
        }
    }

    public void BackToMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowGameResult(bool isWin)
    {
        ResultPage resultPage = Instantiate(this.resultPagePrefab, GameObject.Find("UICanvas").transform).GetComponent<ResultPage>();
        resultPage.ShowResult(isWin);
    }

    private IEnumerator LoadLevel(string levelName)
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            this.closeOnLoadingLevel.SetActive(false);
            this.showOnLoadingLevel.SetActive(true);
            this.progressText.text = "Loading... 0%";
            this.progressBar.value = 0;
            yield return new WaitForSeconds(1.0f);
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);

        if (SceneManager.GetActiveScene().name == "Main")
        {
            while (!operation.isDone)
            {
                float normalizedProgress = operation.progress / 0.9f;
                this.progressText.text = $"Loading... {(int)(normalizedProgress * 100)}%";
                this.progressBar.value = normalizedProgress;
                yield return null;
            }
        }
    }
}
