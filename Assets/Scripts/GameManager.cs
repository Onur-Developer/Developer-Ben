using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<ComminationBen> cmBen = new List<ComminationBen>();
    private List<ComminationBen> _choosableBen = new List<ComminationBen>();
    public List<EnemySpawner> _enemySpawners = new List<EnemySpawner>();
    public CameraShake cameraShake;
    [SerializeField] private Terminal gameManagerTerminal;
    [SerializeField] private Terminal menuTerminal;
    [SerializeField] private Color comingInputColor;
    [SerializeField] private ComminationBen writeRandomFunc;
    [SerializeField] private ComminationBen medkit;
    [SerializeField] private GameObject[] lights;
    [SerializeField] private Sprite player1;
    [SerializeField] private Sprite player2;
    private bool _isStart;
    private bool _isSpawnEnemy;
    private bool _terminalControl = true;
    [HideInInspector] public bool terminalAnimation;
    private GameObject _mud1, _mud2;
    private int _addedFunctionCount;
    private float _graphicCount;
    private float _mechanicCount;
    private const string MUSIC_VOLUME = "set.MusicVolume";
    private const string SFX_VOLUME = "set.SFXVolume";
    private Terminal _terminal;

    delegate void GameStarting();

    private event GameStarting GameStartingEvent;
    private string _laterValue;
    private ComminationBen _formerBen;
    private int _enemyCount;
    public int enemySpawnerCount;
    [HideInInspector] public bool isDash;


    [HideInInspector] public bool isHealthSystem;

    [HideInInspector] public bool isMedKit;
    [HideInInspector] public bool isLight;
    [HideInInspector] public bool isCameraShake;
    [HideInInspector] public bool isAnimation;
    [HideInInspector] public bool isParticle;
    [HideInInspector] public bool isSound;
    private ComminationBen[] _randomBen = new ComminationBen[3];
    List<ComminationBen> _mechanicFuncs = new List<ComminationBen>();
    List<ComminationBen> _graphicFuncs = new List<ComminationBen>();

    [Header("UI Elements")] [SerializeField]
    private Transform winPanel;

    [SerializeField] private TextMeshProUGUI clicktoWin;
    [SerializeField] private Transform weaponPanel;
    [SerializeField] private GameObject weaponInfoPanel;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private GameObject genderPanel;
    [SerializeField] private GameObject benUI;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject thickButton;
    [SerializeField] private GameObject tabbuttons;
    [SerializeField] private Button menuTab;
    [SerializeField] private Button gameManagerTab;

    [Header("Objects")] [SerializeField] private GameObject player;
    [SerializeField] private GameObject flag;
    [SerializeField] private Transform playerWeapons;
    [SerializeField] private GameObject enemySpawner;
    [SerializeField] private Transform enemySpawnerParent;
    [SerializeField] private GameObject drone;
    public HeadCount headCount;
    [SerializeField] private GameObject mudInstance;
    [SerializeField] private BackgroundMusic backgroundMusic;
    [SerializeField] private GameObject gasLamb;

    [Header("Sounds")] [SerializeField] private AudioSource otherSource;
    [SerializeField] private AudioClip characterClip;
    [SerializeField] private Settings settings;
    [SerializeField] private AudioClip winSound;

    public int EnemyCount
    {
        set
        {
            _enemyCount += value;
            flag.SetActive(EnemyCount <= 0);
        }
        get { return _enemyCount; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        for (int i = 10; i < 18; i++)
        {
            _graphicFuncs.Add(cmBen[i]);
        }

        for (int i = 18; i < cmBen.Count; i++)
        {
            _mechanicFuncs.Add(cmBen[i]);
        }
    }

    private void Start()
    {
        cmBen[0].writeValues = cmBen[0].values;
        menuTerminal.gameObject.SetActive(true);
        tabbuttons.SetActive(true);
        menuTab.interactable = false;
        StartCoroutine(menuTerminal.WriteTextLine(cmBen[0]));
    }

    #region Special Functions

    void AddWinCondition()
    {
        flag.SetActive(true);
        StartCoroutine(EndClickText("Click the Flag to Win."));
        StartCoroutine(nameof(FakeUpdate));
    }

    void AddPlayer()
    {
        GameStartingEvent -= AddWinCondition;
        flag.SetActive(enemySpawnerParent.childCount == 0);
        player.gameObject.SetActive(true);
    }

    void AddMove()
    {
        if (!player.GetComponent<PlayerInput>().enabled)
            StartCoroutine(EndClickText("Press W,A,S,D or Arrow Keys to move and collide with the flag."));
        player.GetComponent<PlayerInput>().enabled = true;
    }

    void AddDagger()
    {
        GameStartingEvent += OpenHeadSlider;
        if (!playerWeapons.GetChild(0).GetComponent<Weapon>().isPick)
            StartCoroutine(EndClickText("Press Left Mouse button for the attack."));
        playerWeapons.GetChild(0).gameObject.SetActive(true);
        playerWeapons.GetChild(0).GetComponent<Weapon>().isPick = true;
        weaponInfoPanel.SetActive(true);
        player.GetComponent<Player>().ChangeWeaponinCode(1);
    }

    void AddBasicEnemy()
    {
        GameStartingEvent -= AddBasicEnemy;
        GameStartingEvent += EnemySpawnerStart;
        GameStartingEvent += IncreaseEnemyCount;
        GameStartingEvent += ReloadAllWeapon;
        GameObject spawner = Instantiate(enemySpawner, enemySpawnerParent);
        EnemySpawner eSpawner = spawner.GetComponent<EnemySpawner>();
        eSpawner.enemies.Add(eSpawner.enemyPool[0]);
        _enemySpawners.Add(eSpawner);
        IncreaseEnemyCount();
        EnemySpawnerStart();
    }

    void AddBow()
    {
        playerWeapons.GetChild(3).GetComponent<Weapon>().isPick = true;
        weaponPanel.GetChild(3).GetChild(0).gameObject.SetActive(true);
        GameStartingEvent -= AddBow;
        GameStartingEvent += IncreaseEnemyCount;
        GameObject spawner = Instantiate(enemySpawner, enemySpawnerParent);
        EnemySpawner eSpawner = spawner.GetComponent<EnemySpawner>();
        eSpawner.enemies.Add(eSpawner.enemyPool[0]);
        _enemySpawners.Add(eSpawner);
        IncreaseEnemyCount();
        EnemySpawnerStart();
        StartCoroutine(EndClickText("Press 4 to open the Bow."));
    }

    void AddPistol()
    {
        playerWeapons.GetChild(1).GetComponent<Weapon>().isPick = true;
        weaponPanel.GetChild(1).GetChild(0).gameObject.SetActive(true);
        GameStartingEvent -= AddPistol;
        GameStartingEvent += IncreaseEnemyCount;
        GameObject spawner = Instantiate(enemySpawner, enemySpawnerParent);
        EnemySpawner eSpawner = spawner.GetComponent<EnemySpawner>();
        eSpawner.enemies.Add(eSpawner.enemyPool[0]);
        _enemySpawners.Add(eSpawner);
        IncreaseEnemyCount();
        EnemySpawnerStart();
        StartCoroutine(EndClickText("Press 2 to open the Pistol."));
    }

    void AddMachineGun()
    {
        playerWeapons.GetChild(2).GetComponent<Weapon>().isPick = true;
        weaponPanel.GetChild(2).GetChild(0).gameObject.SetActive(true);
        GameStartingEvent -= AddMachineGun;
        GameStartingEvent += IncreaseEnemyCount;
        GameObject spawner = Instantiate(enemySpawner, enemySpawnerParent);
        EnemySpawner eSpawner = spawner.GetComponent<EnemySpawner>();
        eSpawner.enemies.Add(eSpawner.enemyPool[0]);
        _enemySpawners.Add(eSpawner);
        IncreaseEnemyCount();
        EnemySpawnerStart();
        StartCoroutine(EndClickText("Press 3 to open the Machine Gun."));
    }

    void AddBomb()
    {
        playerWeapons.GetChild(7).GetComponent<Weapon>().isPick = true;
        weaponPanel.GetChild(7).GetChild(0).gameObject.SetActive(true);
        GameStartingEvent -= AddBomb;
        GameStartingEvent += IncreaseEnemyCount;
        GameObject spawner = Instantiate(enemySpawner, enemySpawnerParent);
        EnemySpawner eSpawner = spawner.GetComponent<EnemySpawner>();
        eSpawner.enemies.Add(eSpawner.enemyPool[0]);
        _enemySpawners.Add(eSpawner);
        IncreaseEnemyCount();
        EnemySpawnerStart();
        StartCoroutine(EndClickText("Press 8 to open the Bomb."));
    }

    void AddIceBomb()
    {
        playerWeapons.GetChild(8).GetComponent<Weapon>().isPick = true;
        weaponPanel.GetChild(8).GetChild(0).gameObject.SetActive(true);
        GameStartingEvent -= AddIceBomb;
        GameStartingEvent += IncreaseEnemyCount;
        GameObject spawner = Instantiate(enemySpawner, enemySpawnerParent);
        EnemySpawner eSpawner = spawner.GetComponent<EnemySpawner>();
        eSpawner.enemies.Add(eSpawner.enemyPool[0]);
        _enemySpawners.Add(eSpawner);
        IncreaseEnemyCount();
        EnemySpawnerStart();
        StartCoroutine(EndClickText("Press 9 to open the Ice Bomb."));
    }

    void AddMiniGun()
    {
        playerWeapons.GetChild(4).GetComponent<Weapon>().isPick = true;
        weaponPanel.GetChild(4).GetChild(0).gameObject.SetActive(true);
        GameStartingEvent -= AddMiniGun;
        GameStartingEvent += IncreaseEnemyCount;
        GameObject spawner = Instantiate(enemySpawner, enemySpawnerParent);
        EnemySpawner eSpawner = spawner.GetComponent<EnemySpawner>();
        eSpawner.enemies.Add(eSpawner.enemyPool[0]);
        _enemySpawners.Add(eSpawner);
        IncreaseEnemyCount();
        EnemySpawnerStart();
        StartCoroutine(EndClickText("Press 5 to open the Mini Gun."));
    }

    void AddFlameGun()
    {
        playerWeapons.GetChild(5).GetComponent<Weapon>().isPick = true;
        weaponPanel.GetChild(5).GetChild(0).gameObject.SetActive(true);
        GameStartingEvent -= AddFlameGun;
        GameStartingEvent += IncreaseEnemyCount;
        GameObject spawner = Instantiate(enemySpawner, enemySpawnerParent);
        EnemySpawner eSpawner = spawner.GetComponent<EnemySpawner>();
        eSpawner.enemies.Add(eSpawner.enemyPool[0]);
        _enemySpawners.Add(eSpawner);
        IncreaseEnemyCount();
        EnemySpawnerStart();
        StartCoroutine(EndClickText("Press 6 to open the Flame Gun."));
    }

    void AddLaserGun()
    {
        playerWeapons.GetChild(6).GetComponent<Weapon>().isPick = true;
        weaponPanel.GetChild(6).GetChild(0).gameObject.SetActive(true);
        GameStartingEvent -= AddLaserGun;
        GameStartingEvent += IncreaseEnemyCount;
        GameObject spawner = Instantiate(enemySpawner, enemySpawnerParent);
        EnemySpawner eSpawner = spawner.GetComponent<EnemySpawner>();
        eSpawner.enemies.Add(eSpawner.enemyPool[0]);
        _enemySpawners.Add(eSpawner);
        IncreaseEnemyCount();
        EnemySpawnerStart();
        StartCoroutine(EndClickText("Press 7 to open the Laser Gun."));
    }

    void AddSwordEnemy()
    {
        AddEnemy(1, AddSwordEnemy);
    }

    void AddShieldEnemy()
    {
        AddEnemy(2, AddShieldEnemy);
    }

    void AddBowEnemy()
    {
        AddEnemy(3, AddBowEnemy);
    }

    void AddRunnerEnemy()
    {
        AddEnemy(4, AddRunnerEnemy);
    }

    void AddBossEnemy()
    {
        AddEnemy(5, AddBossEnemy);
    }

    void AddDrone()
    {
        drone.SetActive(true);
    }

    void AddTrap()
    {
        GameStartingEvent -= AddTrap;
        GameStartingEvent += MudControl;
        _mud1 = Instantiate(mudInstance);
        _mud2 = Instantiate(mudInstance);
        MudControl();
    }

    void AddColor()
    {
        GameStartingEvent -= AddColor;
        if (isLight)
            globalLight.intensity = .35f;
        else
            globalLight.intensity = 1f;
    }

    void AddSpeed()
    {
        GameStartingEvent -= AddSpeed;
        player.GetComponent<Player>().speed = 6.5f;
    }

    void AddDash()
    {
        GameStartingEvent -= AddDash;
        player.GetComponent<Player>().ısDash = true;
        StartCoroutine(EndClickText("Press Space or X to do dash."));
    }

    void AddChangePlayer()
    {
        player.GetComponent<PlayerInput>().enabled = false;
        genderPanel.SetActive(true);
        Time.timeScale = 0;
    }

    void AddHealthSystem()
    {
        GameStartingEvent -= AddHealthSystem;
        player.GetComponent<Player>().isHealthSystem = true;
        isHealthSystem = true;
        cmBen.Add(medkit);
    }

    void AddMedKit()
    {
        GameStartingEvent -= AddMedKit;
        isMedKit = true;
    }

    void AddLight()
    {
        GameStartingEvent -= AddLight;
        isLight = true;
        gasLamb.SetActive(true);
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(true);
        }

        if (globalLight.intensity <= .75f)
            globalLight.intensity = .25f;
        else
            globalLight.intensity = .35f;
    }

    void AddCameraShake()
    {
        GameStartingEvent -= AddCameraShake;
        isCameraShake = true;
        player.GetComponent<Player>().isCameraShake = true;
        for (int i = 0; i < playerWeapons.childCount; i++)
        {
            Weapon wp = playerWeapons.GetChild(i).GetComponent<Weapon>();
            wp.isCameraShake = true;
        }
    }

    void AddAnimation()
    {
        GameStartingEvent -= AddAnimation;
        player.GetComponent<Animator>().enabled = true;
        flag.transform.GetChild(1).GetComponent<Animator>().enabled = true;
        drone.GetComponent<Animator>().enabled = true;
        gasLamb.GetComponent<Animator>().enabled = true;
        isAnimation = true;
    }

    void AddParticle()
    {
        GameStartingEvent -= AddParticle;
        isParticle = true;
    }

    void AddMusic()
    {
        GameStartingEvent -= AddMusic;
        backgroundMusic.isMusic = true;
    }

    void AddSoundEffect()
    {
        GameStartingEvent -= AddSoundEffect;
        isSound = true;
        player.GetComponent<Player>().isSound = true;
        drone.GetComponent<Drone>().isSound = true;
        for (int i = 0; i < playerWeapons.childCount; i++)
        {
            Weapon weapon = playerWeapons.GetChild(i).GetComponent<Weapon>();
            weapon.isSoundWeapon = true;
        }
    }

    #endregion

    void GameStarted()
    {
        _terminalControl = !_terminalControl;
        terminalAnimation = _terminalControl;
        gameManagerTerminal.gameObject.SetActive(_terminalControl);
        tabbuttons.SetActive(false);
        backgroundMusic.ChangeVolume(_terminalControl);
        if (gameManagerTerminal.gameObject.activeSelf)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            WriteOutput(_formerBen, _laterValue, _terminal);
            if (_enemySpawners.Count > 0)
                EnemySpawnerControl();
        }
    }


    public void PlayerDied()
    {
        EnemyCount = -EnemyCount;
        playerWeapons.gameObject.SetActive(false);
        weaponInfoPanel.SetActive(false);
        flag.SetActive(false);
        drone.SetActive(false);
        headCount.gameObject.SetActive(false);
        player.GetComponent<PlayerInput>().enabled = false;
        if (_enemySpawners.Count > 0)
            EnemySpawnerControl();
        StartCoroutine(nameof(PlayAgain));
    }

    void AddEnemy(int index, GameStarting function)
    {
        GameStartingEvent -= function;
        GameStartingEvent += IncreaseEnemyCount;
        GameObject spawner = Instantiate(enemySpawner, enemySpawnerParent);
        EnemySpawner eSpawner = spawner.GetComponent<EnemySpawner>();
        eSpawner.enemies.Add(eSpawner.enemyPool[0]);
        _enemySpawners.Add(eSpawner);
        IncreaseEnemyCount();
        AddNewEnemy(index);
        EnemySpawnerStart();
    }

    public void ResumeButton()
    {
        pausePanel.SetActive(false);
        player.GetComponent<PlayerInput>().enabled = true;
        Time.timeScale = 1;
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void ChooseGender(int gender)
    {
        otherSource.PlayOneShot(characterClip);
        Player playerScript = player.GetComponent<Player>();
        switch (gender)
        {
            case 0:
                playerScript.animator.runtimeAnimatorController = playerScript.player1Anim;
                player.GetComponent<SpriteRenderer>().sprite = player1;
                break;
            case 1:
                playerScript.animator.runtimeAnimatorController = playerScript.player2Anim;
                player.GetComponent<SpriteRenderer>().sprite = player2;
                break;
        }

        genderPanel.SetActive(false);
        Time.timeScale = 1;
        StartCoroutine(nameof(PlayerChoosed));
    }

    public void Tab(int index)
    {
        switch (index)
        {
            case 0:
                menuTerminal.gameObject.SetActive(false);
                gameManagerTerminal.gameObject.SetActive(true);
                gameManagerTab.interactable = false;
                menuTab.interactable = true;
                break;
            case 1:
                menuTerminal.gameObject.SetActive(true);
                gameManagerTerminal.gameObject.SetActive(false);
                gameManagerTab.interactable = true;
                menuTab.interactable = false;
                break;
        }
    }

    void EnemySpawnerStart()
    {
        _isSpawnEnemy = false;
        EnemySpawnerControl();
    }

    void MudControl()
    {
        Vector2 randomPos1 = new Vector2(Random.Range(-18f, 18f), Random.Range(-10f, 10f));
        Vector2 randomPos2 = new Vector2(Random.Range(-18f, 18f), Random.Range(-10f, 10f));
        _mud1.transform.position = randomPos1;
        _mud2.transform.position = randomPos2;
    }

    void OpenHeadSlider()
    {
        headCount.gameObject.SetActive(true);
    }

    void EnemySpawnerControl()
    {
        _isSpawnEnemy = !_isSpawnEnemy;
        for (int i = 0; i < _enemySpawners.Count; i++)
        {
            _enemySpawners[i].gameObject.SetActive(_isSpawnEnemy);
        }
    }

    void AddNewEnemy(int newEnemy)
    {
        for (int i = 0; i < _enemySpawners.Count; i++)
        {
            EnemySpawner enemySpawnerNow = _enemySpawners[i].GetComponent<EnemySpawner>();
            enemySpawnerNow.enemies.Add(enemySpawnerNow.enemyPool[newEnemy]);
        }
    }

    void ReloadAllWeapon()
    {
        for (int i = 0; i < playerWeapons.childCount; i++)
        {
            playerWeapons.GetChild(i).GetComponent<Weapon>().ReloadAgain();
        }
    }

    void IncreaseEnemyCount()
    {
        headCount.gameObject.GetComponent<Slider>().maxValue += 5;
        EnemyCount = 5;
        enemySpawnerCount += 5;
    }

    public IEnumerator YouWin()
    {
        thickButton.SetActive(true);
        flag.SetActive(false);
        winPanel.gameObject.SetActive(true);
        player.SetActive(false);
        drone.SetActive(false);
        headCount.gameObject.SetActive(false);
        benUI.SetActive(false);
        player.transform.position = Vector2.zero;
        otherSource.PlayOneShot(winSound);
        yield return new WaitForSeconds(2f);
        winPanel.gameObject.SetActive(false);
        GameStarted();
    }

    IEnumerator FakeUpdate()
    {
        while (!winPanel.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Flag"))
                        StartCoroutine(nameof(YouWin));
                }
            }

            yield return null;
        }
    }

    IEnumerator PlayAgain()
    {
        yield return new WaitForSeconds(3f);
        if (GameStartingEvent != null)
            GameStartingEvent.Invoke();
        playerWeapons.gameObject.SetActive(true);
        weaponInfoPanel.SetActive(true);
        player.GetComponent<PlayerInput>().enabled = true;
        if (player.GetComponent<Animator>().enabled)
            player.GetComponent<Animator>().Play("PlayerIdle");
        else
        {
            player.GetComponent<Player>().ChangeSprite();
        }

        enemySpawnerCount = EnemyCount;
    }

    IEnumerator PlayerChoosed()
    {
        yield return new WaitForSeconds(.1f);
        player.GetComponent<PlayerInput>().enabled = true;
    }

    IEnumerator EndClickText(string sValue)
    {
        benUI.SetActive(true);
        clicktoWin.text = sValue;
        yield return new WaitForSeconds(4f);
        benUI.GetComponent<Animator>().Play("BenOut");
        yield return new WaitForSeconds(1f);
        benUI.SetActive(false);
    }


    void StartEvent(string value, ComminationBen comingBen)
    {
        _laterValue = value;
        _formerBen = comingBen;
        string functionName = comingBen.functionID;
        MethodInfo method = GetType().GetMethod(functionName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
            GameStartingEvent += (GameStarting)Delegate.CreateDelegate(typeof(GameStarting), this, method);
        else
            print("method is null");

        if (GameStartingEvent != null)
            GameStartingEvent.Invoke();
        if (method != null)
            GameStarted();
    }


    public void InputFieldControl(string value)
    {
        if (_addedFunctionCount >= 25)
        {
            PlayerPrefs.SetFloat("Graphic", _graphicCount);
            PlayerPrefs.SetFloat("Mechanic", _mechanicCount);
            PlayerPrefs.SetString("GameName", value);
            SceneManager.LoadScene("ReviewScene");
        }

        string controlValue = value;
        ComminationBen activatedBen = null;
        if (value == "Quit()")
            Application.Quit();
        for (int i = 0; i < cmBen.Count; i++)
        {
            if (controlValue == cmBen[i].nameID)
            {
                activatedBen = cmBen[i];
                break;
            }
        }

        if (activatedBen == null)
            activatedBen = cmBen[3];
        activatedBen.writeValues = activatedBen.values;
        string firstEightChars = value.Substring(0, Mathf.Min(15, value.Length));
        string secondEightChars = value.Substring(0, Mathf.Min(13, value.Length));
        switch (activatedBen.id)
        {
            case "Start" when menuTerminal.gameObject.activeSelf:
                _isStart = true;
                if (activatedBen.isWriting)
                    activatedBen.writeValues = activatedBen.wrongValues;
                _choosableBen.Add(cmBen[4]);
                _terminal = menuTerminal;
                break;
            case "Settings" when menuTerminal.gameObject.activeSelf:
                activatedBen.isWriting = false;
                _terminal = menuTerminal;
                break;
            case "Quit" when menuTerminal.gameObject.activeSelf:
                Application.Quit();
                break;
            case "Function" when _isStart:
                if (menuTerminal.gameObject.activeSelf)
                    goto default;
                bool control = activatedBen.isRandom ? ControlRandomFunc(activatedBen) : ControlFunction(activatedBen);
                if (!control)
                    goto default;
                if (!activatedBen.isWriting)
                {
                    StartEvent(value, activatedBen);
                    _addedFunctionCount++;
                }
                else
                    activatedBen.writeValues = activatedBen.wrongValues;

                _terminal = gameManagerTerminal;
                break;
            default:
                activatedBen = cmBen[3];
                _terminal = menuTerminal.gameObject.activeSelf ? menuTerminal : gameManagerTerminal;
                break;
        }

        if (activatedBen.id == "Function" && !activatedBen.isWriting)
        {
            activatedBen.isWriting = true;
            return;
        }

        if (firstEightChars == MUSIC_VOLUME || secondEightChars == SFX_VOLUME && menuTerminal.gameObject.activeSelf)
        {
            string whichSound = firstEightChars == MUSIC_VOLUME ? MUSIC_VOLUME : SFX_VOLUME;
            activatedBen = cmBen[2];
            settings.ChangeVolume(whichSound, value);
        }

        activatedBen.isWriting = true;
        WriteOutput(activatedBen, value, _terminal);
    }

    bool ControlFunction(ComminationBen activatedBen)
    {
        switch (activatedBen.functionID)
        {
            case "AddWinCondition":
                _choosableBen.Add(cmBen[5]);
                break;
            case "AddPlayer":
                if (cmBen[4].isWriting)
                    _choosableBen.Add(cmBen[6]);
                break;
            case "AddMove":
                if (cmBen[5].isWriting)
                    _choosableBen.Add(cmBen[7]);
                break;
            case "AddDagger":
                if (cmBen[6].isWriting)
                    _choosableBen.Add(cmBen[8]);
                break;
        }

        for (int i = 0; i < _choosableBen.Count; i++)
        {
            if (activatedBen.functionID == _choosableBen[i].functionID)
            {
                return true;
            }
        }

        return false;
    }

    void WriteOutput(ComminationBen comingBen, string value, Terminal terminal)
    {
        thickButton.SetActive(false);
        terminal.playerInputField.text = "";
        terminal.context.GetChild(terminal.writeThisLine).GetChild(1).GetComponent<TextMeshProUGUI>().color =
            comingInputColor;
        terminal.context.GetChild(terminal.writeThisLine).GetChild(1).GetComponent<TextMeshProUGUI>().text =
            "Coming Input: " + value;
        terminal.writeThisLine++;
        if (value == "Start()")
        {
            terminal.AddNewLine();
            terminal.playerInputField.transform.SetParent(terminal.context.GetChild(terminal.writeThisLine).transform);
            terminal.playerInputField.transform.position = terminal.playerInputField.transform.parent.position;
            menuTerminal.gameObject.SetActive(false);
            gameManagerTerminal.gameObject.SetActive(true);
            terminal = gameManagerTerminal;
            terminal.writeThisLine++;
            menuTab.interactable = false;
            gameManagerTab.interactable = false;
        }

        string[] cmControl = comingBen.writeValues != null ? comingBen.writeValues : new[] { "" };
        string[] values = cmControl;
        for (int i = 0; i <= values.Length; i++)
        {
            terminal.AddNewLine();
        }

        StartCoroutine(terminal.WriteTextLine(comingBen));
    }

    public void ControlisRandom(ComminationBen comBen)
    {
        int index = 0;
        for (int i = 0; i < cmBen.Count; i++)
        {
            if (comBen.functionID == cmBen[i].functionID)
            {
                index = i;
                break;
            }
        }

        if (_addedFunctionCount >= 25)
        {
            for (int i = 0; i <= cmBen[9].writeValues.Length; i++)
            {
                gameManagerTerminal.AddNewLine();
            }

            StartCoroutine(gameManagerTerminal.WriteTextLine(cmBen[9]));
            return;
        }


        if (index >= 8)
        {
            AddRandomFunc();
            for (int i = 0; i < writeRandomFunc.values.Length; i++)
            {
                gameManagerTerminal.AddNewLine();
            }

            StartCoroutine(gameManagerTerminal.WriteTextLine(writeRandomFunc));
        }
    }

    void AddRandomFunc()
    {
        List<ComminationBen> randomGrap = new List<ComminationBen>(_graphicFuncs);
        List<ComminationBen> randomMec = new List<ComminationBen>(_mechanicFuncs);
        for (int i = 0; i < _randomBen.Length; i++)
        {
            if (i == 0 && _graphicFuncs.Count > 0)
            {
                int randomGrapIndex = Random.Range(0, randomGrap.Count);
                _randomBen[i] = randomGrap[randomGrapIndex];
                randomGrap.Remove(randomGrap[randomGrapIndex]);
            }
            else
            {
                int randomMecIndex = Random.Range(0, randomMec.Count);
                _randomBen[i] = randomMec[randomMecIndex];
                randomMec.Remove(randomMec[randomMecIndex]);
            }
        }

        for (int i = 1; i < 4; i++)
        {
            writeRandomFunc.values[i] = _randomBen[i - 1].functionID + "()";
        }

        writeRandomFunc.writeValues = writeRandomFunc.values;
    }

    bool ControlRandomFunc(ComminationBen activatedBen)
    {
        if (_randomBen[0] == null)
            return false;
        if (activatedBen.isWriting)
            return true;

        ComminationBen inputBen = null;

        for (int i = 0; i < _randomBen.Length; i++)
        {
            if (activatedBen.functionID == _randomBen[i].functionID)
                inputBen = activatedBen;
        }

        for (int i = 0; i < _graphicFuncs.Count; i++)
        {
            if (inputBen != null && inputBen.functionID == _graphicFuncs[i].functionID)
            {
                _graphicCount += Random.Range(1f,1.26f);
                _graphicFuncs.Remove(activatedBen);
                return true;
            }
        }

        for (int i = 0; i < _mechanicFuncs.Count; i++)
        {
            if (inputBen != null && inputBen.functionID == _mechanicFuncs[i].functionID)
            {
                _mechanicCount += Random.Range(.5f,.81f);
                _mechanicFuncs.Remove(activatedBen);
                return true;
            }
        }

        return false;
    }
}