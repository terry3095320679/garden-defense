# Project 1 Submission Notes

## Repository

GitHub: https://github.com/terry3095320679/garden-defense

## Playable Build

Upload the separately tested `GardenDefense_Windows.zip` archive to Canvas. The archive must contain the complete Unity Windows build folder, not only the `.exe` file.

## Eight Requirements (copy-ready)

1. **Scenes:** MainMenu (index 0) starts Gameplay, and gameplay pause/game-over UI returns to MainMenu.
2. **Input System:** PlayerControls.inputactions drives Rigidbody2D player movement through the new Input System.
3. **Tilemap:** Gameplay uses a dirt/grass arena Tilemap with Tilemap Collider 2D boundaries.
4. **Pooling:** SimplePool prewarms and visibly reuses Bullet prefab instances instead of instantiating every shot.
5. **Triggers/layers:** Bullet hits, enemy contact, and diamond pickups use OnTriggerEnter2D and configured gameplay layers.
6. **Pause:** The Canvas pause menu stops time, resumes correctly, and includes a working Main Menu option.
7. **PlayerPrefs:** Coins, diamonds, and permanent upgrades persist between sessions.
8. **Lighting/material:** Gameplay visibly uses URP Light2D components, a pulsing light, lit material, and glowing diamond pickup.

See `README.md` for the complete script summary, scene/game-object description, controls, setup, and third-party resource disclosure required by the rubric.
