# dungeon-crawler

Unity mobile 2D dungeon crawler prototype built with Unity 6000.5.2f1 and C#.

## Current Status

Implemented foundations include:

- Core bootstrap/service registry/event bus basics.
- ScriptableObject data definitions for heroes, enemies, skills, stats, items, equipment, bosses, themes, encounters, shops, status effects, and upgrades.
- Pure C# combat domain model: combatants, ranks, formations, turn flow, basic attack damage, targeting validation, victory/defeat events.
- EditMode tests for definitions, services, UI navigation, combatants, turn flow, basic attacks, and targeting rules.
- Prototype UI scene/navigation flow.

Planned systems include dungeon runs, advanced skills/status effects, enemy AI, progression, economy, cloud save, monetization, final combat UI, and content production.

## Key Documentation

- [AGENTS.md](AGENTS.md): optimized operating notes for coding agents.
- [Docs/CODING_GUIDELINES.md](Docs/CODING_GUIDELINES.md): architecture and coding rules.
- [Docs/TECH_DESIGN.md](Docs/TECH_DESIGN.md): technical architecture with implemented/planned status markers.
- [Docs/ROADMAP.md](Docs/ROADMAP.md): sprint roadmap with implemented/planned status markers.
- [Docs/CHARACTERS_AND_ENEMIES.md](Docs/CHARACTERS_AND_ENEMIES.md): class, enemy, boss, rank, and skill design reference.
- [Docs/CONTENT_PIPELINE.md](Docs/CONTENT_PIPELINE.md): content, ScriptableObject, asset, license, and AI asset pipeline.
- [Docs/UNITY_VERSIONING.md](Docs/UNITY_VERSIONING.md): project versioning, Git LFS rules, folder layout, and asset naming conventions.
- [Docs/BUILD.md](Docs/BUILD.md): Android and iOS/iPadOS build steps.

## Important Paths

- `Assets/Game/Scripts/Combat`: pure combat domain code.
- `Assets/Game/Scripts/Core`: bootstrap, services, event bus, and save basics.
- `Assets/Game/Scripts/Data/Definitions`: ScriptableObject definitions.
- `Assets/Game/Scripts/UI`: prototype UI screens and navigation.
- `Assets/Game/Tests/EditMode/Editor`: Unity EditMode tests.
- `Assets/Game/Scenes`: Unity scenes.

## Validation

Use Unity EditMode tests as the primary validation path for gameplay/domain changes. If Unity MCP is available, run the relevant EditMode test classes through the Unity Test Framework and check the Unity console for errors.

`dotnet build` on the generated solution may fail without useful diagnostics outside Unity, so Unity compilation and Unity Test Framework results are the authoritative signal.
