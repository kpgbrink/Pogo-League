using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoLevelMenu.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using Assets.Scripts;
using Assets.Scripts._Game.Player.Damage;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    string playerName = null;
    public string PlayerName
    {
        get
        {
            if (playerName != null) return playerName;
            // Find the number of players
            return $"Player {PlayerNumber + 1}";
        }
        set
        {
            playerName = value;
        }
    }

    public int Lives { get; private set; } = 0;

    public int? PlayerDeathFixedUpdateTime { get; private set; }

    public PlayerSpar playerSpar;

    public PlayerSparData playerSparData;


    [SerializeField]
    GameEvent playerDieEvent;

    [SerializeField]
    GameEvent playerCompletelyDieEvent;

    PlayerSpawnManager spawnManager;
    FixedUpdateClock fixedUpdateClock;
    [NonSerialized]
    PlayerInput playerInput;

    public PlayerInput PlayerInput
    {
        get
        {
            if (playerInput != null) return playerInput;
            // Find the number of players
            playerInput = GetComponent<PlayerInput>();
            return playerInput;

        }
        set
        {
            playerInput = value;
        }
    }

    public PlayerSpawnManager SpawnManager
    {
        get
        {
            if (spawnManager != null)
                return spawnManager;
            spawnManager = GameObject.Find("/PlayerSpawnManager").GetComponent<PlayerSpawnManager>();
            return spawnManager;
        }
    }

    public FixedUpdateClock FixedUpdateClock
    {
        get
        {
            if (fixedUpdateClock != null)
                return fixedUpdateClock;
            fixedUpdateClock = GameObject.Find("/FixedUpdateClock")?.GetComponent<FixedUpdateClock>();
            return fixedUpdateClock;
        }
    }

    bool? GameGoing()
    {
        if (FixedUpdateClock == null) return null;
        return FixedUpdateClock.GameStarted;
    }

    bool? GamePaused()
    {
        if (FixedUpdateClock == null) return null;
        return FixedUpdateClock.GamePaused;
    }

    bool? GameBeforeStart()
    {
        if (FixedUpdateClock == null) return null;
        return FixedUpdateClock.ResetValuesOnSceneInit.GameBeforeStart;
    }

    bool? GameAfterEnd()
    {
        if (FixedUpdateClock == null) return null;
        return FixedUpdateClock.ResetValuesOnSceneInit.GameAfterEnd;
    }

    public int PlayerNumber => playerSparData.PlayerSpars.IndexOf(playerSpar);

    public class ResetableValuesOnSceneInit
    {
        public int SpawnNum { get; set; } = 0;
        public bool ControlEnabled { get; set; }
        public bool CompletelyDead => DeathFinalTime != null;
        public int? DeathFinalTime { get; set; } = null;
        public float TotalDamagesTaken => DamagesTaken.Sum(o => o.Value);
        public float TotalDamagesGiven => DamagesGiven.Sum(o => o.Value);

        public Dictionary<int, float> DamagesTaken = new Dictionary<int, float>();

        public Dictionary<int, float> DamagesGiven = new Dictionary<int, float>();
        public List<(int? playerNumber, int clock)> KillsDone { get; set; } = new List<(int?, int)>();

        public List<(int? playerNumber, int clock)> DeathsDone { get; set; } = new List<(int? playerNumber, int clock)>();

        static readonly int playerDamagerRememberQueueAmountDefault = 10;
        // Make scenes able to change this. For now just use the default.
        public int PlayerDamagerRememberQueueAmount { get; set; } = playerDamagerRememberQueueAmountDefault;

        public Queue<(string damageName, int? time, float damage)> DamageRemembererQueue { get; set; } = new Queue<(string damageName, int? time, float damage)>();
    }

    public void AddDamageQueue(string damageName, float damage)
    {
        // Add to damage queue
        if (ResetValuesOnSceneInit.DamageRemembererQueue.Count >= ResetValuesOnSceneInit.PlayerDamagerRememberQueueAmount)
            ResetValuesOnSceneInit.DamageRemembererQueue.Dequeue();
        int? time = null;
        if (FixedUpdateClock != null)
        {
            time = FixedUpdateClock.Clock;
        }
        ResetValuesOnSceneInit.DamageRemembererQueue.Enqueue((damageName, time, damage));
    }

    public ResetableValuesOnSceneInit ResetValuesOnSceneInit { get; set; }

    // Adds damage data for either DamagesTaken or DamagesGiven
    public void AddDamageData(Dictionary<int, float> damageDictionary, int playerNumber, float damageDone)
    {
        try
        {
            damageDictionary.TryGetValue(playerNumber, out var damageDoneStored);
            damageDictionary[playerNumber] = damageDoneStored + damageDone;
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log(e);
            damageDictionary.Add(playerNumber, damageDone);
        }
    }

    public void AddKillData(int playerNumber)
    {
        ResetValuesOnSceneInit.KillsDone.Add((playerNumber, FixedUpdateClock.Clock));
    }

    public void AddDeathData(DamageGiver damageGiver)
    {
        if (damageGiver == null) return;
        var playerNumber = FuncUtil.Invoke<int?>(() =>
        {
            if (damageGiver.Player == null) return null;
            return damageGiver.Player.PlayerNumber;

        });
        ResetValuesOnSceneInit.DeathsDone.Add((playerNumber, FixedUpdateClock.Clock));
    }

    public List<PlayerInputBase> PlayerControlledObjects { get; set; } = new List<PlayerInputBase>();

    public List<PlayerInputBase> PlayerControlledObjectsControlActive =>
        PlayerControlledObjects.Where(o =>
        {
            //Debug.Log("SHow all of the bools that are below" + (!o.ControlActiveBeforeGameStart && GameBeforeStart().GetValueOrDefault()) + (!o.ControlActiveOnGamePause && GamePaused().GetValueOrDefault()) + (!o.ControlActiveAfterGameEnd && GameAfterEnd().GetValueOrDefault()));
            if (!o.ControlActiveBeforeGameStart && GameBeforeStart().GetValueOrDefault()) return false;
            // stop controls if game is not going
            if (!o.ControlActiveOnGamePause && GamePaused().GetValueOrDefault()) return false;
            if (!o.ControlActiveAfterGameEnd && GameAfterEnd().GetValueOrDefault()) return false;
            return o.ControlActive;
        }).ToList();

    GameObject StartingObjects => GameObject.Find("/PlayerStartingObjects");

    // When respawn timer is above zero it starts to count down
    // Respawn happens after the counter has moved the value to 0
    [SerializeField]
    CountDownTimer respawnTimer;

    void Start()
    {
        respawnTimer.StopTimer();
        DontDestroyOnLoad(this.gameObject);
        PlayerSceneInitialize();
        SceneManager.activeSceneChanged += ChangedActiveScene;
        EventSubscription();
    }

    void EventSubscription()
    {
        PlayerInput.onActionTriggered += OnPlayerInputActionTriggered;
        foreach (var actionEvent in playerInput.actionEvents)
        {
            //Debug.Log("Available Action Event " + actionEvent.actionName);
        }
        foreach (var action in playerInput.actions)
        {
            //Debug.Log("Available Action Event " + action.name);
        }
    }

    /// <summary>
    /// PlayerInput
    /// </summary>
    /// <param name="inputAction"></param>
    public void OnPlayerInputActionTriggered(InputAction.CallbackContext inputAction)
    {
        //Debug.Log(inputAction.action.name);
        PlayerControlledObjectsControlActive.ForEach(p =>
        {
            p.OnPlayerInputActionTriggered(inputAction);
        });
    }

    public void OnEnable()
    {
        playerSparData.PlayerSpars.Add(playerSpar);
        //Debug.Log("Player count " + playerSparManager.PlayerSpars.Count);
    }

    public void OnDestroy()
    {
        playerSparData.PlayerSpars.Remove(playerSpar);
        //Debug.Log("Player count after remove " + playerSparManager.PlayerSpars.Count);
        // Delete all of the player controlled objects
        DestroyAllPlayerControlledObjects();
    }

    public void AddPlayerControlledGameObject(Transform transform)
    {
        // Make this work with more than one thing.
        var playerInputObject = (PlayerInputBase)transform.GetComponent(typeof(PlayerInputBase));
        playerInputObject.PlayerInputTransform = this.transform;
        playerInputObject.Destroyed += () => PlayerControlledObjects.Remove(playerInputObject);
        playerInputObject.SpawnNum = ResetValuesOnSceneInit.SpawnNum;
        PlayerControlledObjects.Add(playerInputObject);
        playerInputObject.SetSplitScreenCamera();
    }

    public void Spawn(bool firstSpawn = false)
    {
        if (SpawnManager == null)
        {
            Debug.LogError("SpawnManager is null");
        }
        // Set lives if first spawn
        if (firstSpawn)
        {
            Lives = SpawnManager.Lives;
        }
        var spawnTransform = SpawnManager.GetSpawnPoint(playerSpar, PlayerNumber, firstSpawn);
        // Add the spawn number
        ResetValuesOnSceneInit.SpawnNum++;
        // Add the objects
        foreach (Transform playerStartingObjectsChild in StartingObjects.transform)
        {
            foreach (Transform child in playerStartingObjectsChild)
            {
                // prevent respawning object that does not respawn
                if (!firstSpawn)
                {
                    var playerInputBase = child.GetComponent<PlayerInputBase>();
                    if (playerInputBase != null && !playerInputBase.MakeOnRespawn)
                    {
                        continue;
                    }
                }
                var obj = UnityEngine.Object.Instantiate(child, child.transform.position + spawnTransform.position, spawnTransform.rotation);
                // I'm pretty sure this makes no sense
                // it's not adding a PlayerObject to anything
                // This makes no sense
                var playerObjs = obj.GetComponentsInChildren<IPlayerObject>();
                foreach (var playerObj in playerObjs)
                {
                    playerObj.PlayerInputTransform = transform;
                }
                AddPlayerControlledGameObject(obj);
            }
        }
        //// Set the transform positions and rotations of the player object
        //PlayerControlledObjects.ForEach(p =>
        //{
        //    if (ResetValuesOnSceneInit.SpawnNum != p.SpawnNum) return;
        //    p.transform.SetPositionAndRotation(p.StartingLocalPosition + spawnTransform.position, spawnTransform.rotation);
        //});
        SpawnEventsOnPreviousSpawned();
    }

    private void SpawnEventsOnPreviousSpawned()
    {
        foreach (var playerControlledObject in PlayerControlledObjects.ToList())
        {
            var playerObjTransform = playerControlledObject.transform;
            if (playerObjTransform != null)
            {
                //Debug.Log(playerObjTransform);
                var playerObjInputBase = (PlayerInputBase)playerObjTransform.GetComponent(typeof(PlayerInputBase));
                if (ResetValuesOnSceneInit.SpawnNum - 1 == playerObjInputBase.SpawnNum)
                {
                    playerObjTransform.GetComponents<IOnNextRespawn>().Cast<IOnNextRespawn>().ToList().ForEach(o =>
                        o.NextRespawn()
                    );
                }
            }
        }
    }

    void PlayerSceneInitialize()
    {
        ResetValuesOnSceneInit = new ResetableValuesOnSceneInit();
        Spawn(firstSpawn: true);
    }

    void ChangedActiveScene(Scene current, Scene next)
    {
        PlayerSceneInitialize();
    }

    void FixedUpdate()
    {
        RespawnCountdown();
    }

    void RespawnCountdown()
    {
        if (!respawnTimer.CheckFinished()) return;
        respawnTimer.CountDown();
        //Debug.Log(respawnTimer.Value);
        respawnTimer.Going = false;
        // Respawn
        Respawn();
    }

    public void Respawn()
    {
        Debug.Log("RESPAWN");
        Spawn(firstSpawn: false);
    }

    public void DestroyAllPlayerControlledObjects()
    {
        // Iterate over copy because the list of objects will be modified because they
        // are removed upon being destroyed.
        foreach (var playerControlledObject in PlayerControlledObjects.ToList())
        {
            Destroy(playerControlledObject.transform.gameObject);
        }
    }

    public void OnLeave()
    {
        Destroy(this.gameObject);
    }

    public void OnDeviceRegained()
    {
        Debug.Log("Device regained");
    }

    public void OnDeviceLost()
    {
        Debug.Log("Device is lost");
        // Don't really do anything but delete the player if they don't join in the next spar
    }

    public void Die(
        int? spawnNum,
        float? damagedAmount = null,
        Collision collision = null,
        DamageGiver damageGiver = null,
        DamageTaker damageTaker = null,
        bool lowerLives = true
        )
    {
        AddDeathData(damageGiver);
        foreach (var playerControlledObject in PlayerControlledObjects.ToList())
        {
            var playerObjTransform = playerControlledObject.transform;
            if (playerObjTransform != null)
            {
                //Debug.Log(playerObjTransform);
                var playerObjInputBase = (PlayerInputBase)playerObjTransform.GetComponent(typeof(PlayerInputBase));
                if (spawnNum == null || spawnNum == playerObjInputBase.SpawnNum)
                {
                    var playerObjOnDestroy = playerObjTransform.GetComponent<IOnPlayerDie>();
                    playerObjTransform.GetComponents<IOnPlayerDie>().Cast<IOnPlayerDie>().ToList().ForEach(o =>
                        o.OnPlayerDie(
                        damagedAmount,
                        collision,
                        damageGiver,
                        damageTaker
                        )
                    );
                }
            }
        }
        // Lower my stock count
        if (lowerLives)
        {
            Debug.Log("lower lives");
            Lives--;
        }
        if (Lives > 0 || SpawnManager.InfiniteLives)
        {
            // Start the respawnTimer
            if (respawnTimer.CheckFinished() || !respawnTimer.Going)
            {
                respawnTimer.Value = SpawnManager.RespawnFixedFramesStart;
                respawnTimer.Going = true;
            }
        }
        else
        {
            Debug.Log("player dies forever");
            // Player dies forever
            ResetValuesOnSceneInit.DeathFinalTime = FixedUpdateClock.Clock;

            // Set fixed death timer
            if (FixedUpdateClock != null)
            {
                PlayerDeathFixedUpdateTime = FixedUpdateClock.Clock;
            }
            playerCompletelyDieEvent.Raise();
        }
        playerDieEvent.Raise(); // TODO change this to player completely die.
    }
}
