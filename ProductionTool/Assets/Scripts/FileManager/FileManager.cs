using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SFB;

public class FileManager : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    private VisualElement root;

    private Button importButton;
    private Button saveButton;
    private Button saveAsButton;

    private TextField inputText;

    private string currentPath;
    private Label currentPathlabel;    
    
    private string currentFileName;
    private Label currentFileNamelabel;
    
    private void Awake()
    {
        if(document == null) { Debug.LogError($"No UI document assigned to: {gameObject.name}, {name}"); }

        root = document.rootVisualElement;
    }

    private void OnEnable()
    {
        importButton = root.Q<Button>("ImportButton");
        importButton.clicked += OnImportButtonClicked;

        saveButton = root.Q<Button>("SaveButton");
        saveButton.clicked += OnSaveButtonClicked;

        saveAsButton = root.Q<Button>("SaveAsButton");
        saveAsButton.clicked += OnSaveAsButtonClicked;

        inputText = root.Q<TextField>("InputText");
        inputText.verticalScrollerVisibility = ScrollerVisibility.Auto;

        currentPathlabel = root.Q<Label>("PathLabel");
        currentFileNamelabel = root.Q<Label>("FileNameLabel");
    }

    private void OnDisable()
    {
        importButton.clicked -= OnImportButtonClicked;
        saveButton.clicked -= OnSaveButtonClicked;
        saveAsButton.clicked -= OnSaveAsButtonClicked;
    }

    private void OnImportButtonClicked()
    {
        Debug.Log($"Import clicked");
        Debug.LogWarning("Sorry! does nothing yet ;-;");

        var extensions = new[] {
            new ExtensionFilter("Text files", "txt")
        };
        var url = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        Debug.Log($"Path selected: {url}");

        if(url.Length == 0) { Debug.LogWarning($"No url selected"); return; }
        if (url[0] == null || url[0] == "") { Debug.LogWarning($"No url selected"); return; }
        OnImport(url[0]);
    }

    private void OnImport(string url)
    {
        currentPath = url;
        currentPathlabel.text = currentPath;

        currentFileName = Path.GetFileName(currentPath);
        currentFileNamelabel.text = currentFileName;

        StreamReader streamReader = new StreamReader(currentPath);

        string fileContents = streamReader.ReadToEnd();
        streamReader.Close();

        inputText.value = fileContents;
    }

    private void OnSaveButtonClicked()
    {
        Debug.Log($"Save clicked");

        string url = null;
        if(currentPath == null || currentPath == "") 
        {
            url = GetWindowsBrowserUrl();
            if (url == null || url == "") { Debug.LogWarning($"No url selected"); return; }
        }
        else
        {
            if (File.Exists(currentPath))
            {
                url = currentPath;
            }
            else
            {
                url = GetWindowsBrowserUrl();
                if (url == null || url == "") { Debug.LogWarning($"No url selected"); return; }
            }
        }

        CreateTxtFile(inputText.text, url);
    }
    private void OnSaveAsButtonClicked()
    {
        string url = GetWindowsBrowserUrl();
        if( url == null || url == "" ) { Debug.LogWarning($"No url selected"); return; }

        CreateTxtFile(inputText.text, url);
        Debug.Log($"Save as clicked");
    }

    private string GetWindowsBrowserUrl()
    {
        var extentions = new[] {
            new ExtensionFilter("Text files", "txt")
        };
        string url = StandaloneFileBrowser.SaveFilePanel("Save file", Application.dataPath, "NewTxt", extentions);

        return url;
    }

    private void CreateTxtFile(string fileContents, string url)
    {
        currentPath = url;
        currentPathlabel.text = url;

        currentFileName = Path.GetFileName(currentPath);
        currentFileNamelabel.text = currentFileName;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(fileContents);

        StreamWriter streamWriter = new StreamWriter(url);

        streamWriter.Write(sb);
        streamWriter.Close();
        streamWriter.Dispose();

        AssetDatabase.Refresh();
    }
}
