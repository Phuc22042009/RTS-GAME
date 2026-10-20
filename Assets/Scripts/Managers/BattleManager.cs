using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("Win Assets")]
    [SerializeField] private Sprite p1WinsSprite;
    [SerializeField] private Sprite p2WinsSprite;
    [SerializeField] private bool showDebugHUD = false;

    public enum BattleState
    {
        InCombat,
        BattleOver
    }

    public BattleState CurrentState { get; private set; } = BattleState.InCombat;

    public event Action<int> OnTeamWon;
    public event Action OnBattleReset;

    private readonly List<UnitInitialData> initialUnits = new List<UnitInitialData>();
    private int winningTeam = -1;
    private GameObject winBannerObj;

    private struct UnitInitialData
    {
        public Unit unit;
        public Vector3 position;
        public Quaternion rotation;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInitialize()
    {
        if (Instance == null && FindObjectOfType<BattleManager>() == null)
        {
            if (FindObjectOfType<Unit>() != null)
            {
                GameObject bmObj = new GameObject("BattleManager");
                bmObj.AddComponent<BattleManager>();
            }
        }
    }

    private void Start()
    {
        foreach (Unit u in UnitRegistry.UnitList)
        {
            if (u != null)
            {
                initialUnits.Add(new UnitInitialData
                {
                    unit = u,
                    position = u.transform.position,
                    rotation = u.transform.rotation
                });
            }
        }

        UnitRegistry.OnUnitUnregistered += CheckBattleCondition;
    }

    private void OnDestroy()
    {
        UnitRegistry.OnUnitUnregistered -= CheckBattleCondition;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBattle();
        }
    }

    private void CheckBattleCondition(Unit unregisteredUnit)
    {
        if (CurrentState == BattleState.BattleOver)
            return;

        StartCoroutine(EvaluateVictoryNextFrame());
    }

    private IEnumerator EvaluateVictoryNextFrame()
    {
        yield return null;

        List<Unit> redAlive = UnitRegistry.GetAliveUnits(0);
        List<Unit> blueAlive = UnitRegistry.GetAliveUnits(1);

        if (redAlive.Count == 0 && blueAlive.Count > 0)
        {
            EndBattle(1);
        }
        else if (blueAlive.Count == 0 && redAlive.Count > 0)
        {
            EndBattle(0);
        }
        else if (redAlive.Count == 0 && blueAlive.Count == 0)
        {
            EndBattle(-1);
        }
    }

    private void EndBattle(int winnerTeam)
    {
        CurrentState = BattleState.BattleOver;
        winningTeam = winnerTeam;
        OnTeamWon?.Invoke(winnerTeam);

        ShowVictoryBanner(winnerTeam);
    }

    private void ShowVictoryBanner(int winnerTeam)
    {
        if (winBannerObj == null)
        {
            winBannerObj = new GameObject("VictoryBanner");
            winBannerObj.transform.position = new Vector3(0f, 0f, -1f);
            winBannerObj.transform.localScale = new Vector3(1.6f, 1.6f, 1f);
            SpriteRenderer sr = winBannerObj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 200;

            Sprite bannerSprite = null;
            if (winnerTeam == 1 && p1WinsSprite != null)
            {
                bannerSprite = p1WinsSprite;
            }
            else if (winnerTeam == 0 && p2WinsSprite != null)
            {
                bannerSprite = p2WinsSprite;
            }

            if (bannerSprite != null)
            {
                sr.sprite = bannerSprite;
            }
        }
        winBannerObj.SetActive(true);
    }

    public void ResetBattle()
    {
        if (winBannerObj != null)
        {
            winBannerObj.SetActive(false);
        }

        CurrentState = BattleState.InCombat;
        winningTeam = -1;

        foreach (var data in initialUnits)
        {
            if (data.unit != null)
            {
                data.unit.ResetUnit(data.position, data.rotation);
            }
        }

        OnBattleReset?.Invoke();
    }

    private void OnGUI()
    {
        if (!showDebugHUD)
            return;

        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontSize = 16;
        boxStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 20;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle contentStyle = new GUIStyle(GUI.skin.label);
        contentStyle.fontSize = 16;
        contentStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.BeginArea(new Rect(16, 16, 380, 145));
        GUILayout.BeginVertical(boxStyle);

        int redCount = UnitRegistry.GetAliveUnits(0).Count;
        int blueCount = UnitRegistry.GetAliveUnits(1).Count;

        GUILayout.Label($"<b>TFT RTS Combat Simulation</b>", titleStyle);
        GUILayout.Space(4);
        GUILayout.Label($"Red: <color=red>{redCount}</color> alive  |  Blue: <color=cyan>{blueCount}</color> alive", contentStyle);

        if (CurrentState == BattleState.BattleOver)
        {
            string winnerText = winningTeam == 1 ? "<color=cyan>BLUE TEAM WINS!</color>" :
                               winningTeam == 0 ? "<color=red>RED TEAM WINS!</color>" : "DRAW!";
            GUILayout.Space(2);
            GUILayout.Label($"<b>{winnerText}</b>", contentStyle);
        }

        GUILayout.Space(4);
        GUILayout.Label($"Press <b>[R]</b> to Restart Round", contentStyle);
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
