# Steam-Riptide-Defaults
 A base project used to make Client-Host (Listen Server) games using Riptide.

 <ins>Includes</ins>:
    - A minimalist system for connecting and spawning clients using the Riptide-SteamTransport.
    - A Rigidbody rollback system to allow synced physics.
    - A basic Client-Side Prediction implementation using the Rigidbody rollback system.


 Important:

 - Server and Client Physics objects should be set to ignore collisions with one another. This can be achieved with the collision matrix.
 - Auto Simulation must be disabled in the project. This is done by default in the NetworkBody class.
 - Enhanced Determinism allows for deterministic physics. This is crucial! Even though enhanced determinism is worse for performance, the amount of reconciliations that will not occur due to it far outweighs the performance defecit.