using CrowdControl.Client.Unity;
using CrowdControl.Client.WebSocket;
using UnityEngine;

/// <summary>
/// Example implementation of a UnityGameStateManager that exposes a configurable game state.
/// </summary>
public class ExampleGameStateManager : UnityGameStateManager
{
    /// <summary>
    /// Current game state value, editable in the Inspector.
    /// </summary>
    [SerializeField]
    public GameState GameState = GameState.Ready;

    /// <summary>
    /// Returns the current <see cref="GameState"/>.
    /// </summary>
    public override GameState GetGameState() => GameState;
}