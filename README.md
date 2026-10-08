# GARDEN DEFENSE

GARDEN DEFENSE is a small 2D arena shooter created for CPSC 386 Project 1. The player controls a sprout guardian on the left side of a garden arena, automatically fires toward enemies entering from the right, collects coins and diamonds, and chooses temporary weapon upgrades during each run. Coins and diamonds can also purchase permanent upgrades from the main menu.

## Project Information

- Unity version: **Unity 6.3 LTS (6000.3.23f1)**
- Render pipeline: **Universal Render Pipeline 2D**
- Target build: **Windows**
- Orientation: **16:9 landscape**

## Controls

- Move: `WASD` or arrow keys
- Fire: automatic
- Pause/resume: `Esc`
- UI: mouse

## Scenes and Key GameObjects

### MainMenu

- `Canvas`: title, Start, Upgrades, Quit, and permanent-upgrade UI.
- `MainMenuManager`: loads the Gameplay scene and quits the standalone build.
- `PermanentUpgradeMenu`: displays currency, prices, levels, maximum levels, and purchase results.

### Gameplay

- `Player`: Rigidbody2D/Collider2D movement, health, automatic shooting, and permanent stat bonuses.
- `BulletPool`: prewarms and reuses bullet prefab instances.
- `EnemySpawner`: spawns enemies from the right entrance and increases spawn frequency for the first two minutes.
- `DiamondSpawner`: occasionally spawns a collectible diamond outside the right edge so it drifts into the arena.
- `Grid/ArenaWalls`: Tilemap arena boundary with Tilemap Collider 2D.
- `Canvas`: health, experience, currency, timer, pause menu, game-over menu, and three-choice upgrade panel.
- `Global Light 2D` and pickup/portal lights: visible URP 2D lighting effects.

## Project 1 Requirement Checklist

| Requirement | Implementation |
| --- | --- |
| 1. Scenes and scene management | `MainMenu` is build index 0 and loads `Gameplay`; pause and game-over menus return to `MainMenu`. |
| 2. Input System player control | `PlayerControls.inputactions` supplies a Vector2 Move action to `PlayerMovement`; movement uses Rigidbody2D and Collider2D. |
| 3. Tilemap level | `Gameplay` contains a dirt/grass arena Tilemap with Tilemap Collider 2D boundaries. |
| 4. Prefab pooling | `SimplePool` prewarms 12 `Bullet` prefab instances and reuses inactive bullets after hits or lifetime expiration. |
| 5. Tags/layers/triggers | Bullet hits, enemy contact, and diamond collection are driven by `OnTriggerEnter2D`; configured physics layers separate gameplay object types. |
| 6. Pause menu | Canvas-based `PausePanel` sets `Time.timeScale` to 0, resumes correctly, and offers Main Menu. |
| 7. PlayerPrefs persistence | Coins, diamonds, and permanent Attack/Health/Fire Rate/Move Speed/Projectile upgrades are saved with PlayerPrefs. |
| 8. Lighting/material effect | The Gameplay scene uses URP Global Light 2D, a pulsing Light2D effect, a lit material, and a glowing diamond pickup. |

## Script Summary

| Script | Responsibility |
| --- | --- |
| `Bullet` | Configures movement, lifetime, collision, pooling, and normal/laser/cold slash/knockback/homing/explosive weapon behavior. |
| `CurrencyHUD` | Displays saved coin and diamond totals. |
| `DiamondPickup` | Moves the collectible left, handles trigger pickup, and awards saved diamonds. |
| `DiamondSpawner` | Performs low-probability timed diamond spawn checks. |
| `Enemy` | Scales health over time, moves toward the player, receives damage/status effects, and awards run progress. |
| `EnemySpawner` | Spawns enemies at the right entrance and increases spawn rate up to three times the initial rate. |
| `ExplosionFlash` | Displays and removes the short watermelon explosion visual. |
| `GameplayManager` | Returns from gameplay to the main menu. |
| `GameTimer` | Displays run time and keeps the final time visible while paused or defeated. |
| `MainMenuManager` | Connects Start/Quit buttons and loads Gameplay. |
| `PauseManager` | Opens, resumes, and exits the Canvas pause menu while managing `Time.timeScale`. |
| `PermanentUpgradeMenu` | Connects the permanent-upgrade UI to currency and upgrade data. |
| `PermanentUpgrades` | Stores upgrade caps, costs, purchases, and stat calculations. |
| `PlayerHealth` | Applies permanent maximum health, updates the health bar, and controls game over/retry. |
| `PlayerMovement` | Reads the Input System Move action and moves the Rigidbody2D player within the arena. |
| `PlayerProgress` | Saves and spends coins/diamonds with PlayerPrefs. |
| `PlayerShooter` | Automatically fires pooled bullets and applies permanent/run weapon upgrades. |
| `PulseLight2D` | Animates a visible URP Light2D intensity effect. |
| `RunProgress` | Tracks kills, experience thresholds, and earned upgrade choices. |
| `SimplePool` | Prewarms, returns, and reuses bullet prefab instances. |
| `UpgradeChoicePanel` | Randomly presents three weapon unlock/stat choices and pauses during selection. |
| `WeaponUpgradeTypes` | Defines shared weapon and upgrade-stat enums/data. |

## Persistent Progress and Balance

- Base player health: 50
- Base attack: 10
- Base enemy health: 50; health increases every 10 seconds
- Attack and Health permanent caps: 100 levels
- Fire Rate and Move Speed permanent caps: 10 levels
- Projectile count cap: 3 projectiles
- Run upgrade thresholds: 10 kills, then 15, 20, and so on

Unity regenerates `Library`, `Temp`, `Obj`, `Logs`, and `UserSettings`; these folders are intentionally excluded from Git.

## Building

1. Open **File > Build Profiles**.
2. Confirm `MainMenu` is scene index 0 and `Gameplay` is scene index 1.
3. Select Windows and build into an empty `Build/` folder.
4. Zip the entire Build folder, extract the archive elsewhere, and test the executable before Canvas submission.

