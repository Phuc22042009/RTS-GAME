# RTS Autonomous Combat Simulation (TFT-Inspired)

An independent software engineering prototype exploring scalable gameplay architecture, state-driven combat behaviors, and memory-conscious systems in Unity (C#). Inspired by the tactical auto-battler dynamics of *Teamfight Tactics* (TFT).

![Combat Simulation Demo](demo.gif)

---

## Key Engineering Highlights

* **Autonomous Combat Simulation:** Autonomous entities with dynamic nearest-enemy targeting, attack cadence calculated via attack speed timers, and a TFT-style mana-ability cycle (mana gain on attack & damage taken, casting empowered ability at 100% mana).
* **5-State FSM with Hysteresis Buffer:** Robust state machine (`Idle`, `Walk`, `Attack`, `CastAbility`, `Death`) implementing a $0.25\,\text{m}$ hysteresis deadband margin to completely eliminate boundary state-thrashing and jitter between pursuit and attack modes.
* **0 B Garbage Collection Spatial Registry:** Centralized $O(N)$ active entity registry (`UnitRegistry`) replacing scene-wide queries (`FindObjectsOfType`), achieving zero heap allocations in the core combat update loop.
* **Decoupled Event-Driven Architecture:** Complete separation of concerns between gameplay logic, physics, and visual presentation (world-space floating health/mana bars and floating combat text) via C# action events, enabling sub-millisecond round resets and state rollback without scene reloading.

---

## Architectural Breakdown

```
                    ┌──────────────────────────────────────────────┐
                    │               BattleManager                  │
                    │   (Round lifecycle, win/loss, rollback key)  │
                    └──────────────────────┬───────────────────────┘
                                           │
                                           ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                       Unit                                              │
│                        (Central Coordinator / Controller)                               │
├───────────────┬─────────────────┬──────────────────┬─────────────────┬──────────────────┤
│ UnitMovement  │ UnitPerception  │   UnitCombat     │  HealthSystem   │    UnitVisual    │
│  (Locomotion, │  (Targeting,    │ (Cadence timing, │ (Durability,    │ (Hit flash,      │
│  rotation)    │  team filtering)│ mana & ability)  │  IDamageable)   │  floating bars)  │
└───────┬───────┴────────┬────────┴────────┬─────────┴────────┬────────┴────────┬─────────┘
        │                │                 │                  │                 │
        └────────────────┴────────┬────────┴──────────────────┴─────────────────┘
                                  ▼
                   ┌──────────────────────────────┐
                   │         StateMachine         │
                   │   ┌──────────────────────┐   │
                   │   │      IdleState       │   │
                   │   ├──────────────────────┤   │
                   │   │      WalkState       │   │
                   │   ├──────────────────────┤   │
                   │   │     AttackState      │   │ (Hysteresis Buffer 0.25m)
                   │   ├──────────────────────┤   │
                   │   │  CastAbilityState    │   │ (Full Mana Ability Trigger)
                   │   ├──────────────────────┤   │
                   │   │      DeathState      │   │
                   │   └──────────────────────┘   │
                   └──────────────────────────────┘
```

### Component-Based Separation of Concerns
1. **`UnitMovement`**: Encapsulates translation and smooth angular orientation facing targets with a rotation rate of $720^\circ/\text{s}$.
2. **`UnitPerception`**: Senses living hostile units by querying `UnitRegistry`, discarding friendly fire and dead entities.
3. **`UnitCombat`**: Regulates attack intervals via cooldown cadence ($\text{cooldown} = 1.0 / \text{attackSpeed}$), charges mana on attack ($+15$), and unleashes critical ability strikes ($2.5\times$ damage multiplier) when mana reaches $100\%$.
4. **`HealthSystem`**: Implements `IDamageable`, processes damage mitigation, charges mana on receiving hits ($+10$), and dispatches `OnHealthChanged`, `OnTakeDamage`, and `OnDeath` events.
5. **`UnitVisual` & `UnitFloatingBarUI`**: Pure observers listening to combat events to display white damage flashes, procedural attack punches, in-world floating HP/Mana bars, and floating combat text (`DamagePopup`).

---

## Getting Started

### Prerequisites
* Unity **2022.3 LTS** (or compatible Unity 2022.3+ version)
* Universal 2D or Built-in Render Pipeline

### Running the Prototype
1. Clone the repository:
   ```bash
   git clone <your-repo-url>
   ```
2. Open the project in Unity Editor (version 2022.3.62f2 or newer).
3. Open the main tactical scene:
   ```
   Assets/Scenes/Tactical.unity
   ```
4. Enter **Play Mode**:
   * Units will automatically detect opponents, maneuver into attack range, and execute combat.
   * Press **`R`** at any point to trigger an instant in-memory round reset (rollback to initial transforms, full health, and zero mana without reloading the scene).
