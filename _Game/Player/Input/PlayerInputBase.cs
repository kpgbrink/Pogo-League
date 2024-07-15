using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerInputBase : PlayerObject
{
    public event Action Destroyed;

    public int? SpawnNum { get; set; } = null;

    public Camera splitScreenCamera;

    // This will basically be used to just move the objects in a respawn run a method on it or something after a respawn and then call it good.
    // Prevent the object from being destroyed and recreated.
    [SerializeField]
    bool respawnReusesGameObject = true;
    public bool RespawnReusesGameObject => respawnReusesGameObject;

    [SerializeField]
    bool destroyable = true;
    public bool Destroyable => destroyable;

    [SerializeField]
    bool makeOnRespawn = true;
    public bool MakeOnRespawn => makeOnRespawn;

    [SerializeField]
    bool controlActiveBeforeGameStart = false;
    public bool ControlActiveBeforeGameStart => controlActiveBeforeGameStart;

    [SerializeField]
    bool controlActiveAfterGameEnd = true;
    public bool ControlActiveAfterGameEnd => controlActiveAfterGameEnd;

    [SerializeField] 
    public Rect splitScreenRect = new Rect(0, 0, 1, 1);

    // Warning this isn't the only place where control is set to inactive
    // The player filters out things based on the 
    // controlActiveBeforeGameStart
    // controlActiveAfterGameEnd
    [SerializeField]
    bool controlActive = true;
    public bool ControlActive { get => controlActive; set => controlActive = value; }

    public Vector3 StartingLocalPosition { get; set; }

    private Dictionary<Transform, Vector3> initialLocalPositions = new Dictionary<Transform, Vector3>();

    void OnDestroy()
    {
        Destroyed?.Invoke();
    }

    void Awake()
    {
        StartingLocalPosition = transform.localPosition;
        StoreInitialLocalPositions();
    }

    private void StoreInitialLocalPositions()
    {
        foreach (Transform child in transform)
        {
            initialLocalPositions[child] = child.localPosition;
        }
    }

    public Vector3 GetInitialLocalPosition(Transform child)
    {
        return initialLocalPositions.TryGetValue(child, out var position) ? position : Vector3.zero;
    }

    public virtual void OnPlayerInputActionTriggered(InputAction.CallbackContext inputAction)
    {
    }

    public void Die(bool lowerLives)
    {
        Player.Die(spawnNum: SpawnNum, lowerLives: lowerLives);
    }

    public void Respawn()
    {
        Player.Respawn();
    }

    // This is only called on spawn if there is a camera set for the player.
    // THis is for split screen
    public void SetSplitScreenCamera()
    {
        UpdateSplitScreen();
    }

    /// <summary>
    /// If split-screen is enabled, then for each player in the game, adjust the player's <see cref="Camera.rect"/>
    /// to fit the player's split screen area according to the number of players currently in the game and the
    /// current split-screen configuration.
    /// </summary>
    private void UpdateSplitScreen()
    {
        // Nothing to do if there is no camera
        if (splitScreenCamera == null)
        {
            return;
        }
        // Set Player Split screen camera
        Player.PlayerInput.camera = splitScreenCamera;

        // Determine number of split-screens to create based on highest player index we have.
        var minSplitScreenCount = 0;
        foreach (var player in PlayerInput.all)
        {
            if (player.playerIndex >= minSplitScreenCount)
                minSplitScreenCount = player.playerIndex + 1;
        }

        // Determine divisions along X and Y. Usually, we have a square grid of split-screens so all we need to
        // do is make it large enough to fit all players.
        var numDivisionsX = Mathf.CeilToInt(Mathf.Sqrt(minSplitScreenCount));
        var numDivisionsY = numDivisionsX;
        var maintainAspectRatioInSplitScreen = false;
        if (!maintainAspectRatioInSplitScreen && numDivisionsX * (numDivisionsX - 1) >= minSplitScreenCount)
        {
            // We're allowed to produce split-screens with aspect ratios different from the screen meaning
            // that we always add one more column before finally adding an entirely new row.
            numDivisionsY -= 1;
        }

        // Assign split-screen area to each player.
        foreach (var player in PlayerInput.all)
        {
            // Make sure the player's splitScreenIndex isn't out of range.
            var splitScreenIndex = player.splitScreenIndex;
            if (splitScreenIndex >= numDivisionsX * numDivisionsY)
            {
                Debug.LogError(
                    $"Split-screen index of {splitScreenIndex} on player is out of range (have {numDivisionsX * numDivisionsY} screens); resetting to playerIndex",
                    player);
                //player.splitScreenIndex = player.playerIndex;
            }

            // Make sure we have a camera.
            var camera = player.camera;
            if (camera == null)
            {
                // I assume this will be fixed when all players are spawned.
                continue;
            }

            // Assign split-screen area based on m_SplitScreenRect.
            var column = splitScreenIndex % numDivisionsX;
            var row = splitScreenIndex / numDivisionsX;
            var rect = new Rect
            {
                width = splitScreenRect.width / numDivisionsX,
                height = splitScreenRect.height / numDivisionsY
            };
            rect.x = splitScreenRect.x + column * rect.width;
            // Y is bottom-to-top but we fill from top down.
            rect.y = splitScreenRect.y + splitScreenRect.height - (row + 1) * rect.height;
            camera.rect = rect;
        }
    }
}
