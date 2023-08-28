# ECSHelper
ECSHelper is an easy and efficient GUI tool designed to help you test the connection/configuration of custom Echo servers.

It allows you to emulate both WebSocket servers (Config, Login, Transaction and Matchmaking) and UDP servers (Game Server).

You will find a compiled version of the project in the **Releases** section.
This allows you to use it directly. 
Alternatively, you can clone the project to make more in-depth changes to the servers.

## Screenshots

| ![Matchmaking Server Running](Screenshots/MatchmakingServerRunning.png) |
|:----------------------------------------------------------------------------------------------------:| 
|                                     *Matchmaking Server Running*                                     |

| ![UDP Server Running](Screenshots/UDPServerRunning.png) |
|:----------------------------------------------------------------------------:| 
|                     *UDP Server Running (Gamer Server)*                      |

## Server Configurations
For every server, you will have these common settings:
- **Listening Port:** The port on which the game client will send data to
- **Show sent data in logs:** Should the data returned by the server appear in the logs?
- **Show Hex data as Text:** Should any Hex data be printed as a Text representation?

For some servers, you will have different settings to play with:
- **(WS) Matchmaking Server**
   - **Targeted game server:** Address and port of the supposed game server. This is the `IP:Port` the game client will try to connect to when matchmaking
   - **Reply only once:** Should the Matchmaking server only answer once to the game client?
- **(UDP) UDP Server (aka. Game Server)**
   - **Send received data back (echo):** Should the server send back the exact data it received?
   - **Send back custom payload:** This allows you to specify any hex string to be sent to the game client when the server receives data