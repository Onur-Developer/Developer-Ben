using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameReview : MonoBehaviour
{
    [SerializeField] private ComminationBen[] functions;
    [SerializeField] private ComminationBen[] resetFunctions;
    [SerializeField] private TextMeshProUGUI graphicTM;
    [SerializeField] private TextMeshProUGUI mechanicTM;
    [SerializeField] private TextMeshProUGUI totalTM;
    [SerializeField] private TextMeshProUGUI gameNameTM;
    [SerializeField] private GameObject reviewTMInstance;
    [SerializeField] private Transform functionPanel;
    [SerializeField] private Transform combosPanel;
    [SerializeField] private GameObject panel1;
    [SerializeField] private GameObject panel2;


    private void Awake()
    {
        gameNameTM.maxVisibleCharacters = 29;
    }


    private void Start()
    {
        graphicTM.text = GraphicScore().ToString("F1") + "/10";
        mechanicTM.text = MechanicScore().ToString("F1") + "/10";
        totalTM.text = ((GraphicScore() + MechanicScore()) / 2).ToString("F1");
        gameNameTM.text = PlayerPrefs.GetString("GameName");
        WriteNotFunctions();
        PlayerPrefs.DeleteAll();
        ResetF();
    }

    float GraphicScore()
    {
        float graphic = PlayerPrefs.GetFloat("Graphic");
        return graphic;
    }

    float MechanicScore()
    {
        float mechanic = PlayerPrefs.GetFloat("Mechanic");
        if (mechanic >= 9.6f)
            mechanic = 10;
        return mechanic;
    }

    void WriteNotFunctions()
    {
        for (int i = 0; i < functions.Length; i++)
        {
            if (!functions[i].isWriting)
                CreateText(functions[i].nameID, functionPanel);
        }
    }

    void ResetF()
    {
        foreach (ComminationBen ben in resetFunctions)
        {
            ben.isWriting = false;
            ben.isSound = false;
        }
    }

    void CreateText(string value, Transform comingParent)
    {
        GameObject reviewTM = Instantiate(reviewTMInstance, comingParent);
        reviewTM.GetComponent<TextMeshProUGUI>().text = "*" + value;
    }

    public void NextButton()
    {
        panel1.SetActive(!panel1.activeSelf);
        panel2.SetActive(!panel2.activeSelf);
    }

    public void HomeButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}