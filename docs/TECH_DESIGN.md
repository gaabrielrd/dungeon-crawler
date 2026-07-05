# TECH_DESIGN.md

## 1. Stack técnica

### 1.1 Engine

**Engine escolhida:** Unity 6.3 LTS.

Motivo: o projeto é um jogo mobile free-to-play com ads, compras internas, autenticação Google/Apple e save em nuvem. Unity oferece um ecossistema mais maduro para produção, distribuição, monetização e live operations mobile.

### 1.2 Linguagem

**Linguagem:** C#.

### 1.3 Plataformas

```text
Android
iOS
iPadOS
```

### 1.4 Ferramentas principais

```text
Unity Hub
Unity 6.3 LTS
Rider ou VS Code
Git
GitHub
Git LFS
ChatGPT Codex
Android SDK / Android Build Support
Xcode / iOS Build Support
Google Play Console
App Store Connect
TestFlight
```

### 1.5 Pacotes Unity recomendados

```text
2D Sprite
2D Pixel Perfect
Sprite Atlas
Addressables
Input System
Localization
Unity Authentication
Unity Cloud Save
Unity Economy
Unity IAP
Unity Analytics
Unity Remote Config
Unity Cloud Code
Google Mobile Ads Unity Plugin
```

### 1.6 Referências oficiais

- Unity 6 Releases & Support: https://unity.com/releases/unity-6/support
- Unity Cloud Save: https://docs.unity.com/en-us/cloud-save
- Unity Authentication: https://docs.unity.com/ugs/manual/authentication/manual/overview
- Unity Economy: https://docs.unity.com/en-us/economy
- Unity IAP: https://docs.unity.com/en-us/iap
- Unity Gaming Services Pricing: https://unity.com/products/gaming-services/pricing
- Google Play Console: https://developer.android.com/distribute/console
- Apple Developer Program: https://developer.apple.com/programs/

---

## 2. Arquitetura geral

O projeto deve seguir arquitetura modular, orientada a dados e com separação clara entre:

```text
Dados estáticos
Estado runtime
Serviços de domínio
Serviços externos
UI
Persistência
Configuração remota
```

### 2.1 Princípios

```text
Data-driven sempre que possível.
Lógica de jogo fora da UI.
UI apenas lê ViewModels e dispara comandos.
ScriptableObjects para definições estáticas.
Classes serializáveis para estado salvo.
Serviços com interfaces para facilitar testes.
Código específico de plataforma isolado.
Integrações externas encapsuladas.
```

Status atual:

| Área | Status |
|---|---|
| Dados estáticos via ScriptableObjects | Implemented |
| Estado runtime de combate | Implemented |
| Serviços de domínio de combate | Implemented parcial |
| Serviços externos, UGS, IAP, Ads, Remote Config | Planned |
| UI final e ViewModels completos | Planned |
| Persistência local | Implemented inicial |
| Cloud save e migração | Planned |

---

## 3. Estrutura de pastas

```text
Assets/
  Game/
    Scripts/
      Core/
        Bootstrap/
        Events/
        Services/
        Utilities/
      Auth/
      Save/
      Economy/
      Ads/
      IAP/
      Dungeon/
      Combat/
      Party/
      Heroes/
      Enemies/
      Items/
      Skills/
      StatusEffects/
      UI/
        Screens/
        Components/
        Popups/
        ViewModels/
      Platform/
        Android/
        iOS/
      Analytics/
      RemoteConfig/
      Tests/
        EditMode/
        PlayMode/
    Data/
      Heroes/
      Skills/
      Enemies/
      Bosses/
      Items/
      Equipment/
      StatusEffects/
      DungeonThemes/
      Encounters/
      Shops/
      Economy/
    Art/
      Characters/
      Enemies/
      Bosses/
      Tilesets/
      UI/
      Icons/
      Backgrounds/
    Audio/
      Music/
      SFX/
    Prefabs/
      Combatants/
      UI/
      VFX/
    Scenes/
      Bootstrap.unity
      MainMenu.unity
      Combat.unity
      RestingSite.unity
      Shop.unity
    Addressables/
    Resources/
      OnlyIfStrictlyNeeded/
```

---

## 4. Assembly definitions

Status: Planned.

Usar `.asmdef` para reduzir tempo de compilação e impor fronteiras arquiteturais.

Observação: o projeto ainda não usa `.asmdef`. Não adicionar assemblies sem uma tarefa explícita, porque isso altera fronteiras de compilação e referências Unity.

Sugestão:

```text
Game.Core
Game.Data
Game.Combat
Game.Dungeon
Game.Party
Game.Economy
Game.Save
Game.Auth
Game.UI
Game.Platform
Game.Analytics
Game.Tests
```

Dependências recomendadas:

```text
Game.Data -> Game.Core
Game.Combat -> Game.Core, Game.Data
Game.Dungeon -> Game.Core, Game.Data, Game.Combat
Game.Party -> Game.Core, Game.Data
Game.Economy -> Game.Core, Game.Data
Game.Save -> Game.Core, Game.Party, Game.Economy, Game.Dungeon
Game.UI -> Game.Core, Game.Combat, Game.Party, Game.Economy
```

Evitar dependência circular.

---

## 5. Cenas

### 5.1 Bootstrap

Cena inicial responsável por:

```text
inicializar serviços
carregar configurações locais
inicializar Unity Gaming Services
inicializar autenticação
carregar save local
sincronizar cloud save
rotear para main menu ou tutorial
```

### 5.2 MainMenu

Responsável por:

```text
exibir status da conta
mostrar party/roster
entrar na dungeon
abrir loja
abrir configurações
abrir gerenciamento de personagens
```

### 5.3 Combat

Responsável por:

```text
renderizar combate
capturar input do jogador
exibir timeline/turno atual
mostrar habilidades válidas
exibir feedback visual
notificar resultado do combate
```

### 5.4 RestingSite

Responsável por:

```text
curar party
comprar itens
aprender habilidades
contratar personagens
melhorar habilidades/equipamentos
continuar run
encerrar run, se permitido
```

---

## 6. Dados estáticos com ScriptableObjects

### 6.1 Definições principais

```text
HeroClassDefinition
HeroProgressionDefinition
SkillDefinition
EnemyDefinition
BossDefinition
ItemDefinition
EquipmentDefinition
StatusEffectDefinition
DungeonThemeDefinition
EncounterTableDefinition
EncounterDefinition
ShopTableDefinition
UpgradeDefinition
CurrencyDefinition
ProductDefinition
```

### 6.2 Exemplo: SkillDefinition

```csharp
public sealed class SkillDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public string Description;
    public Sprite Icon;

    public int[] ValidUserRanks;
    public int[] ValidTargetRanks;

    public SkillTargetType TargetType;
    public DamageType DamageType;
    public float DamageMultiplier;
    public int Cooldown;

    public StatusEffectApplication[] Effects;
}
```

### 6.3 Exemplo: DungeonThemeDefinition

```csharp
public sealed class DungeonThemeDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public int FirstFloor;
    public int LastFloor;

    public Sprite Background;
    public AudioClip Music;

    public EncounterTableDefinition CommonEncounters;
    public EncounterTableDefinition EliteEncounters;
    public EncounterTableDefinition BossEncounters;
    public ShopTableDefinition ShopTable;
}
```

---

## 7. Estado runtime

Nunca salvar referências diretas para ScriptableObjects. Salvar IDs.

Exemplo de estado runtime:

```csharp
[Serializable]
public sealed class HeroInstanceState
{
    public string InstanceId;
    public string ClassId;
    public int Level;
    public int CurrentXp;
    public int CurrentHp;
    public int MaxHp;
    public int Rank;
    public List<string> LearnedSkillIds;
    public List<string> EquippedItemInstanceIds;
    public List<RuntimeStatusEffectState> StatusEffects;
}
```

---

## 8. Serviços principais

### 8.1 Core

```text
GameBootstrap
ServiceRegistry
EventBus
SceneLoader
GameClock
RandomService
ConfigService
```

### 8.2 Combat

| Serviço | Status |
|---|---|
| CombatController | Implemented |
| TurnManager | Implemented |
| TargetingRulesService | Implemented |
| DamageResolver | Implemented |
| HealingResolver | Planned |
| StatusEffectResolver | Planned |
| CombatRewardResolver | Planned |
| CombatLogService | Planned |

### 8.3 Dungeon

```text
DungeonRunService
FloorGenerator
EncounterGenerator
DungeonThemeResolver
BossResolver
RestingSiteResolver
RunRewardService
```

### 8.4 Party

```text
PartyService
RosterService
HeroFactory
HeroProgressionService
SkillUnlockService
```

### 8.5 Economy

```text
CurrencyService
InventoryService
ShopService
LootService
UpgradeService
PurchaseGrantService
```

### 8.6 Platform/External

```text
AuthService
CloudSaveService
LocalSaveService
IAPService
AdsService
AnalyticsService
RemoteConfigService
```

---

## 9. Fluxo de combate técnico

| Etapa | Status |
|---|---|
| CombatScene recebe EncounterDefinition + PartyState | Planned |
| CombatController cria/recebe CombatantStates | Implemented parcial |
| TurnManager calcula ordem inicial | Implemented |
| UI renderiza estado | Implemented inicial |
| Player escolhe Skill + Target | Implemented inicial (ataque básico + seleção de alvo válido) |
| TargetingRulesService valida ação | Implemented |
| CombatController executa comando básico | Implemented |
| DamageResolver aplica dano básico | Implemented |
| StatusEffectResolver aplica efeitos | Planned |
| EventBus publica eventos de feedback, morte, vitória e derrota | Implemented |
| TurnManager avança turno | Implemented |
| AI decide ações dos inimigos | Implemented inicial (ataque básico em alvo vivo aleatório) |
| Verificar morte, vitória e derrota | Implemented |
| CombatRewardResolver gera recompensa | Planned |
| DungeonRunService avança estado da run | Planned |
| SaveService salva snapshot | Planned |

---

## 10. Procedural generation

### 10.1 Determinismo

Toda run deve ter uma seed. A seed deve permitir reproduzir geração de andares em testes e debug.

```csharp
public sealed class DungeonRunState
{
    public string RunId;
    public int Seed;
    public int CurrentFloor;
    public string CurrentThemeId;
    public List<ResolvedFloorState> VisitedFloors;
}
```

### 10.2 Resolução de andar

```text
Input: seed, currentFloor, theme, runState, playerPower
Output: ResolvedFloorState
```

Regras:

```text
floor % 5 == 0  -> boss
floor % 10 == 0 -> boss + resting site after victory
floor % 20 == 0 -> boss + resting site + theme transition
otherwise       -> weighted random floor type
```

---

## 11. Save e cloud

Usar:

```text
LocalSaveService para persistência offline
Unity Authentication para identidade
Unity Cloud Save para sincronização entre dispositivos
SaveMigrationService para versões futuras
```

Detalhes completos em `SAVE_SYSTEM.md`.

---

## 12. Monetização

Usar:

```text
Unity IAP para compras internas
Unity Economy para recursos, moedas e inventário econômico
Google Mobile Ads Plugin para AdMob
Remote Config para balanceamento de recompensas e preços virtuais
Cloud Code para concessões sensíveis, quando necessário
```

Detalhes completos em `MONETIZATION_DESIGN.md`.

---

## 13. UI

### 13.1 Escolha inicial

Usar **uGUI** para MVP.

Motivo:

```text
bom suporte mobile
mais exemplos práticos
integração madura com prefabs
mais simples para HUDs e popups de jogo
menor atrito com Codex
```

### 13.2 Estrutura UI

```text
ScreenController
PopupController
NavigationService
ViewModel por tela
Componentes reutilizáveis
```

### 13.3 Telas iniciais

```text
Loading
Main Menu
Party Management
Hero Details
Dungeon Entry
Combat
Combat Result
Resting Site
Shop
Settings
Account Linking
Cloud Conflict Resolution
IAP Store
Ad Reward Popup
```

---

## 14. Testes

### 14.1 EditMode tests

Prioridade alta:

| Teste | Status |
|---|---|
| TurnManager | Implemented |
| TargetingRulesService | Implemented |
| DamageResolver | Implemented |
| StatusEffectResolver | Planned |
| FloorGenerator | Planned |
| EncounterGenerator | Planned |
| SaveMigrationService | Planned |
| Cloud conflict resolver | Planned |
| Economy calculations | Planned |

### 14.2 PlayMode tests

Prioridade média:

```text
carregar Bootstrap
iniciar combate simples
resolver vitória
salvar e carregar snapshot
abrir resting site
executar compra fake
```

---

## 15. Analytics

Eventos mínimos:

```text
tutorial_started
tutorial_completed
run_started
floor_started
floor_completed
combat_started
combat_won
combat_lost
boss_started
boss_won
boss_lost
resting_site_entered
hero_hired
skill_unlocked
item_purchased_soft_currency
rewarded_ad_started
rewarded_ad_completed
iap_started
iap_completed
iap_failed
cloud_save_synced
cloud_save_conflict_detected
account_linked
```

---

## 16. Performance mobile

### 16.1 Regras

```text
usar sprite atlases
evitar allocations em combate
usar object pooling para VFX/textos flutuantes
limitar partículas
evitar overdraw excessivo na UI
testar em aparelho Android intermediário
testar em iPhone e iPad reais
```

### 16.2 Alvo inicial

```text
30 FPS estável no mínimo
60 FPS desejável
baixo consumo de bateria
carregamento inicial inferior a 10 segundos em aparelhos médios
```

---

## 17. Build e distribuição

### 17.1 Android

```text
Unity Android Build Support
Android SDK
Google Play Console
Internal testing track
Closed testing track
Production release
```

### 17.2 iOS/iPadOS

```text
Unity iOS Build Support
Mac com Xcode
Apple Developer Program
App Store Connect
TestFlight
Production release
```

---

## 18. Riscos técnicos

| Risco | Mitigação |
|---|---|
| Save cloud sobrescrever progresso | snapshot versionado + conflict resolver explícito |
| Integração de login Google/Apple atrasar | implementar login anônimo primeiro, depois account linking |
| Economia quebrada | Remote Config desde cedo |
| Ads prejudicarem retenção | priorizar rewarded ads, evitar interstitial agressivo |
| Assets inconsistentes | pipeline visual com paleta e tamanhos padronizados |
| Codex alterar arquitetura indevidamente | manter CODING_GUIDELINES.md e tarefas pequenas |
