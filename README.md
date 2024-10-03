### 1. **Project Setup for Unity with URP**

- **URP Project Creation**: Start by creating a project using the URP template, ensuring cross-platform compatibility.
- **Global Volume**: Add a Global Volume with a Volume component for post-processing and global visual effects.
- **Graphics Quality & Cleanup**: Optimize settings by focusing on URP’s high fidelity mode, removing unnecessary quality levels, and cleaning up default URP settings.

### 2. **Development Settings**

- **Pause on Error**: Enable this option to catch errors during game testing.
- **Aspect Ratio**: Set the resolution to Full HD (1920x1080).
- **VSync**: Turn it on to optimize GPU performance during development.

### 3. **.editorconfig Setup**

- Create a `.editorconfig` file for consistent C# formatting. Example:
  ```ini
  CSharp_new_line_before_open_brace = none
  ```

### 4. **Camera Controls**

- Navigate using key combinations such as `Left Alt + Left Mouse Button` for rotating around objects and `Right Mouse Button + WASD` for moving around.

---

### 5. **Post-Processing Setup**

Ensure that post-processing is viewed and adjusted properly in **Game View** and is enabled in both the **Camera** and **URP Renderer** settings. 

#### Key Post-Processing Components:
- **Tonemapping**: Set to Neutral for a balanced scene.
- **Bloom**: Add for glow effects, with adjustments to **Threshold** and **Intensity**.
- **Vignette**: Adds subtle darkness at the edges of the screen.
- **Anti-Aliasing**: High-end smoothing of edges.
- **Ambient Occlusion**: Improves shadows where objects meet.
  
---

### 6. **Shaders & Shader Graph**

**Shader Graph** allows visual creation of shaders. For example, you can create custom effects like dissolving or outline transitions.

1. **Create a New Shader**: 
   - Right-click in the project -> Shader Graph -> URP Lit Shader.
2. **Add a Texture**: 
   - Use `Simple Texture2D` nodes to control shader inputs and outputs, including a `BaseMap` for your texture.
3. **Animate with Time**: 
   - Use a `Time` node to add dynamic effects, like scrolling textures, by multiplying it with a `Vector2` parameter for speed.
4. **Output**: 
   - Connect nodes to the UV input to animate the shader effect.

---

### 7. **Object Movement**

To move or rotate an object based on its facing direction:

```csharp
// Move in the direction the object is facing
transform.forward = <vector>;

// Rotate smoothly using interpolation
transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotationSpeed);
```

---

### 8. **Animation Setup**

1. Add an **Animator** component to an object.
2. Create an **Animator Controller** to manage different animations.
3. Record and loop animations.
4. **Transitions**: Add conditions to transition between states, with or without exit time.

---

### 9. **Cinemachine for Camera Control**

Cinemachine provides dynamic camera systems in Unity.

1. Install it via the **Package Manager**.
2. Add a **Virtual Camera** and adjust settings such as noise and follow/look at behavior for smooth movement.

---

### 10. **New Input System**

Unity’s **New Input System** offers better control for input management.

1. Install via the **Package Manager**.
2. Set up **Input Actions** for capturing player movements.
3. Retrieve values with `ReadValue<Vector2>` in scripts for handling input.

---

### 11. **Unity Shortcuts**

Learn key shortcuts like `W` for moving objects, `E` for rotating, and using `Shift` while rotating to snap to 15-degree increments.

---

### 12. **Singleton Pattern**

Singleton ensures a class only has one instance globally, often used for managers or game controllers.

1. **Setup Singleton**:
   ```csharp
   public static Player Instance { get; private set; }

   private void Awake()
   {
       if (Instance != null && Instance != this)
       {
           Destroy(gameObject);
           return;
       }
       Instance = this;
       DontDestroyOnLoad(gameObject);
   }
   ```

2. **Access Singleton**:
   ```csharp
   Player player = Player.Instance;
   ```

---

### 13. **Unity Events**

Events allow decoupled communication between objects, which enhances modularity.

1. **Declare an Event**:
   ```csharp
   public event EventHandler OnSelectedCounterChanged;
   ```

2. **Trigger Event**:
   ```csharp
   OnSelectedCounterChanged?.Invoke(this, EventArgs.Empty);
   ```

3. **Subscribe to Event**:
   ```csharp
   Player.Instance.OnSelectedCounterChanged += HandleSelectedCounterChanged;
   ```

4. **Unsubscribe** to prevent memory leaks when the object is destroyed:
   ```csharp
   Player.Instance.OnSelectedCounterChanged -= HandleSelectedCounterChanged;
   ```


---

# Netcode for Game Objects Setup Guide

## 1. Install the Netcode for Game Objects Package

1. Open Unity and navigate to the **Package Manager**.
2. In the top left corner, select **Unity Registry** to list all available packages.
3. Scroll down to find the package titled **Netcode for Game Objects**.

### Package Version Used in This Course

```plaintext
Package: com.unity.netcode.gameobjects  
Version: 1.2.0
```

> **Note:** It is highly recommended to install version 1.2.0 for this course to avoid potential issues due to changes in newer versions.

---

## 2. Install the Correct Version Manually

If the latest version differs from 1.2.0, follow these steps to install the exact version manually:

1. Click the **Plus (+)** icon in the top left of the Package Manager window.
2. Select **Add Package by Name**.
3. Enter the following details:

```plaintext
Name: com.unity.netcode.gameobjects  
Version: 1.2.0
```

4. Double-check for typos and click **Add**.
5. Unity will install Netcode 1.2.0 along with the required dependency:

```plaintext
Dependency: ND Transport 1.3.1
```

---

## 3. Verify Installation

1. Once installed, confirm that **Netcode for Game Objects** version **1.2.0** is listed, along with **ND Transport 1.3.1** as a dependency.
2. Close the Package Manager.

---

## 4. Future Version Considerations

Changes between versions may be minor (e.g., changes in the location of network prefabs), but it is important to follow this course with **Netcode 1.2.0** to avoid confusion.

---

## Network Manager Setup

1. Create a new **EmptyObject** and rename it `NetworkManager`.
2. Reset the transform of the object.
3. Add a new component called `NetworkManager` (Unity's built-in component).
4. Select **Unity Transport** as the transport mechanism to allow the game to send packets.
   - You will see the Unity Transport component added.
   - No need to modify anything, but ensure the address in **Unity Transport > Connect Data > Address** is set to `127.0.0.1`.

---

## Player Setup

1. In `NetworkManager`, add the **Player Prefab** (the object to instantiate when a player connects).
2. Add a **Network Object** component to the Player Prefab.
3. Modify the Player Script:
   - Change `MonoBehaviour` to `NetworkBehaviour`.

---

## Running the Game

1. In the `NetworkManager`, you will see options:
   - **Start Host** (Server + Client)
   - **Start Server**
   - **Start Client**

2. Press **Start Host** to spawn the player.

---

## UI for Starting Host and Client

1. Instead of controlling the start host/client from the Unity Editor, create a UI with two buttons: **Start Host** and **Start Client**.
2. Create a script to access these buttons and add listeners on click:
   - Use `NetworkManager.Singleton.StartHost();`.

---

## Build Settings

1. Ensure the **GameScene** is at the top of the build order (we don't need `Main` as the first scene for a multiplayer game).
2. Go to **Player Settings** > **Resolution**:
   - Set **FullScreen Mode** to **Windowed**.
   - Enable **Run in Background**.
   - Enable **Resizable Window**.
3. Set the **Company Name** and **Product Name** in Player Settings.

---

## Logs Location

To find logs for the game, navigate to:

```plaintext
C:\[User Profile]\AppData\LocalLow\[Company Name]\[App Name]
```

After building the game, open the log file to view the logs. Press `F5` to refresh the logs in real-time.

---

## Quantom Console Asset

While using log files is effective, the **Quantom Console Asset** can provide a better testing experience (paid asset).

---

## Synchronizing Player Movements

Ensure only the owning player can move their character by adding **IsOwner** conditions in the player script:

```csharp
if (!IsOwner) {
    return;
}
```

To synchronize movement, add **NetworkTransform** to the player prefab and adjust the sync settings, like excluding Y-direction movement or scaling.

For client-side synchronization, implement a `ClientNetworkTransform` as described in the [Unity Multiplayer Documentation](https://docs-multiplayer.unity3d.com/netcode/current/components/networktransform/#docusaurus_skipToContent_fallback):

```csharp
using Unity.Netcode.Components;
using UnityEngine;

namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
```

---

## Server-Side Movement Synchronization with RPC

To have the server control movement, use a Server RPC:

```csharp
[ServerRpc(RequireOwnership = false)]
```

---

## Networking Best Practices

- Add spawned objects to the **Network Prefab List** in the `NetworkManager`.
- Use **NetworkObjectReference** for complex RPC parameters:

```csharp
[ServerRpc(RequireOwnership = false)]
private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
    SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
}
```

---

## Lobby and Relay Setup

1. Install the Lobby package from the Unity Registry.
2. Use **LobbyService** for lobby operations and handle lobby heartbeat to keep it alive:

```csharp
private void HandleHeartBeat() {
    if (IsLobbyHost()) {
        heartBeatTimer -= Time.deltaTime;
        if (heartBeatTimer < 0f) {
            heartBeatTimer = 15f;
            LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
        }
    }
}
```

Fetch lobbies with:

```csharp
QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
```

For lobby relay code:

```csharp
joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
```

Enable Relay in Unity Cloud for seamless connections without port forwarding:

```csharp
Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYERS_ALLOWED - 1);
NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
```

---

## Debugging Multiplayer

Use **Debug Simulator** in **Unity Transport** to simulate latency, jitter, and packet drops. Use the **Multiplayer Tools Package** for network statistics and monitor them via **Profiler**.

--- 
