# Riptide Steam Client Side Prediction
 A base project used to make Client-Host (Listen Server) games using Riptide.

### <u>Contents</u>:
    - A minimalist system for connecting and spawning clients using the Riptide-SteamTransport.
    - A Rigidbody rollback system to allow synced physics.
    - A basic Client-Side Prediction implementation using the Rigidbody rollback system. 


 Important:

 - Server and Client Physics objects should be set to ignore collisions with one another. This can be achieved with the collision matrix.
 - Auto Simulation must be disabled in the project. This is done by default in the NetworkBody class.
 - Enhanced Determinism allows for (almost) deterministic physics. This is crucial! Even though enhanced determinism is worse for performance, the amount of reconciliations that will not occur due to it far outweighs the performance defecit.
 - Due to timestep inconsistencies with collisions, generic bodies are not deterministic between clients. I am working on a fix for this.
