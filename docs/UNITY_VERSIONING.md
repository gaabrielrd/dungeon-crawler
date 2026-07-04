# Unity versioning conventions

## Versioned project files

The Unity project lives at the repository root. Commit project source files under `Assets/`, `Packages/`, and `ProjectSettings/`, including `.meta` files. Keep all first-party game content under `Assets/Game`.

Do not commit generated local folders or logs: `Library`, `Temp`, `Obj`, `Build`, `Builds`, `Logs`, `UserSettings`, IDE project files, or `*.log`.

## Git LFS

Binary and large asset formats are tracked through Git LFS in `.gitattributes`:

- Images and sources: `*.png`, `*.jpg`, `*.jpeg`, `*.psd`, `*.aseprite`
- Audio: `*.wav`, `*.mp3`, `*.ogg`
- Models, animation, and video: `*.fbx`, `*.anim`, `*.mp4`, `*.mov`

Unity YAML files such as scenes, prefabs, `.asset`, `.meta`, materials, and controllers stay text-mergeable unless listed above for LFS.

## Folder layout

Use this structure for game-owned content:

- `Assets/Game/Scripts`: C# runtime and editor scripts
- `Assets/Game/Data`: ScriptableObjects and authored gameplay data
- `Assets/Game/Art`: sprites, pixel art sources, models, and visual source files
- `Assets/Game/Audio`: music, SFX, voice, and audio source files
- `Assets/Game/Prefabs`: reusable Unity prefabs
- `Assets/Game/Scenes`: game scenes
- `Assets/Game/UI`: UI assets, prefabs, styles, and layouts
- `Assets/Game/Tests`: edit mode and play mode tests

## Naming

- C# scripts: `PascalCase.cs`
- ScriptableObjects: `SO_<Category>_<Name>.asset`
- Prefabs: `PF_<Category>_<Name>.prefab`
- Sprites: `SPR_<Category>_<Name>.png`
- Scenes: `SCN_<Name>.unity`

Use clear category names such as `Hero`, `Enemy`, `Dungeon`, `Combat`, `UI`, or `Item`. Rename Unity assets through the Unity Editor when references already exist.
