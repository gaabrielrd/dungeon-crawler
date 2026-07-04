# AGENTS.md

## Project Snapshot

This is a Unity 6000.5.2f1 mobile 2D dungeon crawler prototype in C#.

Current implemented focus:

- Core service bootstrap, service registry, event bus, scene loading, mock auth, and local save basics.
- Data definitions as ScriptableObjects under `Assets/Game/Scripts/Data/Definitions`.
- Pure combat domain logic under `Assets/Game/Scripts/Combat`.
- EditMode test coverage under `Assets/Game/Tests/EditMode/Editor`.
- uGUI-based prototype screens under `Assets/Game/Scripts/UI`.

Do not assume every system described in `Docs/TECH_DESIGN.md` or `Docs/ROADMAP.md` is implemented. Those files are partly target architecture.

## Key Docs

Read these first for most tasks:

- `Docs/CODING_GUIDELINES.md`: coding rules, architecture boundaries, and combat expectations.
- `Docs/TECH_DESIGN.md`: intended architecture, folder layout, service boundaries, and future systems.
- `Docs/BUILD.md`: Unity version, build scenes, Android/iOS build notes.
- `Docs/CHARACTERS_AND_ENEMIES.md`: current combat/content design reference.
- `Docs/CONTENT_PIPELINE.md`: ScriptableObject content, asset naming, licenses, and AI asset rules.

For persistence or monetization tasks, also read:

- `Docs/SAVE_SYSTEM.md`
- `Docs/MONETIZATION_DESIGN.md`

## Repository Structure

Important paths:

- `Assets/Game/Scripts/Combat`: pure combat domain code.
- `Assets/Game/Scripts/Core`: bootstrap, services, event bus, save snapshot.
- `Assets/Game/Scripts/Data/Definitions`: ScriptableObject definitions.
- `Assets/Game/Scripts/UI`: prototype UI screens and navigation.
- `Assets/Game/Scripts/Editor`: editor tooling.
- `Assets/Game/Scenes`: project scenes.
- `Assets/Game/Tests/EditMode/Editor`: EditMode tests.
- `Docs`: design, build, content, save, monetization, and roadmap docs.
- `Packages/manifest.json`: Unity package dependencies.
- `ProjectSettings/ProjectVersion.txt`: authoritative Unity editor version.

There are currently no `.asmdef` files. Although docs recommend them for the future, do not add assembly definitions unless the task explicitly asks for that architecture change.

## Architecture Rules

- Keep gameplay rules out of MonoBehaviours and UI classes.
- Prefer pure C# domain classes for combat, targeting, damage, progression, generation, and economy logic.
- Use ScriptableObjects only for static data definitions.
- Runtime state must live in serializable state classes and must not mutate ScriptableObjects.
- Save stable IDs instead of Unity object references.
- UI should render state and call controller/service commands; it should not become the source of truth.
- Use `IEventBus` for UI-observable facts such as combat start, turn changes, damage, and combat end.
- Avoid global singletons except the existing controlled `ServiceRegistry` pattern.

## Current Combat Model

Combat is currently implemented as pure domain code:

- `CombatantState`: runtime combatant state, side, rank, HP, stats, alive status.
- `CombatFormationState`: up to four combatants per side, unique ranks per side.
- `TurnManager`: speed-ordered turn selection, skips dead combatants.
- `CombatController`: starts combat, advances turns, handles victory/defeat, executes basic attacks, exposes targeting queries.
- `DamageResolver`: resolves basic attack damage with `max(1, attacker.Attack - target.Defense)`.
- `TargetingRulesService`: validates `SkillDefinition` user ranks, target ranks, target side/type, and living targets.
- `SkillDefinition`: already has `ValidUserRanks`, `ValidTargetRanks`, and `TargetType`.

Current combat target type mapping:

- `SkillTargetType.Enemy`: single enemy.
- `SkillTargetType.Ally`: single ally, excluding self.
- `SkillTargetType.Self`: self only.
- `SkillTargetType.AllEnemies`: all living enemies in valid target ranks.
- `SkillTargetType.AllAllies`: living allies in valid target ranks, including self.
- `SkillTargetType.Any`: any living side/rank allowed by rank rules.

Do not add complex skills, status effects, cooldowns, AI, or final UI behavior unless the task explicitly asks.

## Testing And Verification

Primary verification for C# gameplay/domain work is Unity EditMode tests.

If Unity MCP tools are available:

1. After script edits, refresh/import if needed.
2. Check `mcpforunity://editor/state` until Unity is not compiling.
3. Check console errors with `read_console`.
4. Run relevant EditMode tests with `run_tests`.
5. Poll with `get_test_job`.

Useful test classes:

- `CombatantStateTests`
- `CombatTurnFlowTests`
- `BasicAttackTests`
- `TargetingRulesServiceTests`
- `GameDefinitionTests`
- `ServiceRegistryTests`
- `AuthAndSaveServiceTests`
- `UIFlowNavigationTests`

`dotnet build` against the generated solution may fail without useful diagnostics outside Unity. Treat Unity compilation and Unity Test Framework results as the authoritative signal.

## Unity And Asset Notes

- Use Unity 6000.5.2f1.
- Keep build scenes aligned with `Docs/BUILD.md`.
- Do not commit artifacts under `Builds/`.
- Use `.meta` files for Unity assets/scripts.
- Follow Git LFS and asset naming guidance from `Docs/UNITY_VERSIONING.md` and `Docs/CONTENT_PIPELINE.md`.
- External assets need license records; AI-generated assets need logs as described in `CONTENT_PIPELINE.md`.

## Documentation Maintenance

When implementing systems that move from roadmap/design into code, update docs so they distinguish:

- Implemented behavior.
- Planned behavior.
- Out-of-scope behavior.
- Validation/test commands used.

After finishing any implementation task, check the corresponding status entries in `Docs/ROADMAP.md` and `Docs/TECH_DESIGN.md`. Update `Implemented`, `Implemented inicial`, `Implemented parcial`, or `Planned` markers in the same change when the task changes project status.

Prefer small, factual documentation updates tied to the changed subsystem.
