using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;


public class Terminal : MonoBehaviour
{
    [SerializeField] private Image codeLine;
    public TMP_InputField playerInputField;
    [SerializeField] private AudioSource openTerminalSource;
    [SerializeField] private AudioSource otherSource;
    [SerializeField] private AudioSource benSource;
    [SerializeField] private AudioClip playerKeyboard;
    [SerializeField] private AudioClip playerEnterKeyboard;
    [SerializeField] private byte newLine;
    [SerializeField] private GameObject tabButtons;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Button otherButton;
    public Transform context;
    private RectTransform _contextRect;
    private int _lineCount;
    public int writeThisLine;
    private bool _isAnimation;

    private void Awake()
    {
        context = transform.GetChild(1).GetChild(0).transform;
        _contextRect = context.GetComponent<RectTransform>();
        playerInputField.gameObject.SetActive(false);
        for (int i = 0; i < newLine; i++)
        {
            AddNewLine();
        }
    }

    private void OnEnable()
    {
        bool terminalContr = GameManager.instance.terminalAnimation;
        if (terminalContr)
        {
            openTerminalSource.Play();
            ComingAnimation();
        }
        playerInputField.ActivateInputField();
        if(context.childCount < 7)
            return;
        RectTransform elementToCenter = context.GetChild(context.childCount-7).GetComponent<RectTransform>();
        RectTransform content = context.GetComponent<RectTransform>();
        float targetPosition = -elementToCenter.localPosition.y - elementToCenter.rect.height / 2f -100;
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetPosition);
    }


    void ComingAnimation()
    {
        _isAnimation = true;
        transform.DOScale(Vector3.zero, .5f)
            .SetEase(Ease.Linear).From()
            .OnComplete((() =>
            {
                RectTransform elementToCenter = context.GetChild(context.childCount-7).GetComponent<RectTransform>();
                RectTransform content = context.GetComponent<RectTransform>();
                float targetPosition = -elementToCenter.localPosition.y - elementToCenter.rect.height / 2f -100;
                content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetPosition);
                tabButtons.SetActive(true);
                GameManager.instance.terminalAnimation = false;
            }));
    }

    #region InputActions

    public void OnMouseClick()
    {
        playerInputField.ActivateInputField();
    }

    #endregion


    public void AddNewLine()
    {
        Image newLine = Instantiate(codeLine, context);
        string writeLine = _lineCount < 10 ? "0" + _lineCount : _lineCount.ToString();
        newLine.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = writeLine;
        _lineCount++;
        _contextRect.sizeDelta = new Vector2(_contextRect.sizeDelta.x,
            20 * (_lineCount + 1));
    }

    public void TakeInputField()
    {
        if (playerInputField.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            otherSource.PlayOneShot(playerEnterKeyboard);
            GameManager.instance.InputFieldControl(playerInputField.text);
        }
    }

    public void ClickSound()
    {
        audioSource.pitch = Random.Range(.9f, 1.1f);
        audioSource.PlayOneShot(playerKeyboard);
    }

    public IEnumerator WriteTextLine(ComminationBen benObj)
    {
        if (_isAnimation)
            yield return new WaitForSeconds(.5f);
        otherButton.interactable = false;
        _isAnimation = false;
        audioSource.Play();
        playerInputField.gameObject.SetActive(false);
        playerInputField.text = "";
        string[] values = benObj.writeValues;
        List<TextMeshProUGUI> writeLines = new List<TextMeshProUGUI>();
        for (int i = 0; i < values.Length; i++)
        {
            writeLines.Add(context.GetChild(writeThisLine).GetChild(1).GetComponent<TextMeshProUGUI>());
            writeThisLine++;
        }

        if (benObj.benSounds.Length > 0 && !benObj.isSound)
            StartCoroutine(TalkBen(benObj));
        for (int j = 0; j < values.Length; j++)
        {
            for (int i = 0; i < values[j].Length; i++)
            {
                writeLines[j].text += values[j][i];
                yield return new WaitForSeconds(0.065f);
            }
        }

        playerInputField.gameObject.SetActive(true);
        playerInputField.ActivateInputField();
        playerInputField.transform.SetParent(context.GetChild(writeThisLine).transform);
        playerInputField.transform.position = playerInputField.transform.parent.position;
        otherButton.interactable = true;
        if (benObj.writeValues == benObj.values)
            GameManager.instance.ControlisRandom(benObj);
        if (playerInputField.gameObject.activeSelf)
            audioSource.Stop();
    }

    IEnumerator TalkBen(ComminationBen comminationBen)
    {
        int i = -1;
        while (i < comminationBen.benSounds.Length)
        {
            if (!benSource.isPlaying)
            {
                i++;
                if (i >= comminationBen.benSounds.Length)
                    break;
                benSource.PlayOneShot(comminationBen.benSounds[i]);
                benSource.Play();
            }

            yield return null;
        }

        if (comminationBen.id != "Settings")
            comminationBen.isSound = true;
    }
}