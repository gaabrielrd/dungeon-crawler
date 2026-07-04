# SAVE_SYSTEM.md

## 1. Objetivo

O sistema de save deve permitir que o jogador:

```text
jogue offline
salve progresso localmente
sincronize progresso entre Android, iPhone e iPad
vincule conta Google e/ou Apple
recupere compras permanentes
resolva conflitos entre dispositivos
mantenha compatibilidade com versões futuras do jogo
```

---

## 2. Serviços recomendados

```text
Unity Authentication
Unity Cloud Save
Local file save
SaveMigrationService
AccountLinkingService
CloudConflictResolver
```

### 2.1 Estratégia

```text
Save local sempre existe.
Cloud save sincroniza snapshots versionados.
Login anônimo é criado no primeiro acesso.
Google/Apple são vinculados depois para persistência cross-device.
O jogador pode vincular mais de um provedor à mesma conta.
```

---

## 3. Fluxo de autenticação

### 3.1 Primeiro acesso

```text
Abrir jogo
→ Inicializar Unity Services
→ Criar login anônimo
→ Criar PlayerId
→ Criar save local vazio
→ Iniciar tutorial
```

### 3.2 Vinculação de conta

```text
Jogador abre tela de conta
→ Escolhe Google ou Apple
→ Login externo é concluído
→ AccountLinkingService vincula provedor ao PlayerId atual
→ Cloud save é enviado/mesclado
→ UI confirma conta protegida
```

### 3.3 Novo dispositivo

```text
Jogador instala em outro dispositivo
→ Login anônimo temporário
→ Jogador entra com Google/Apple
→ Sistema encontra cloud save existente
→ Baixa snapshot
→ Compara com save local
→ Resolve conflito se necessário
→ Aplica save escolhido/mesclado
```

---

## 4. Modelo de save

### 4.1 Regras

```text
Salvar IDs, não referências Unity.
Salvar snapshot completo, não apenas deltas.
Versionar todo save.
Registrar timestamp UTC.
Registrar deviceId do último update.
Registrar buildVersion.
Separar estado persistente, estado da run e estado econômico.
```

### 4.2 SaveSnapshot v1

```json
{
  "saveVersion": 1,
  "schemaVersion": "1.0.0",
  "playerId": "unity_player_id",
  "createdAtUtc": "2026-07-04T00:00:00Z",
  "lastUpdatedUtc": "2026-07-04T00:00:00Z",
  "lastDeviceId": "device_guid",
  "buildVersion": "0.1.0",
  "profile": {
    "displayName": "Player",
    "accountLevel": 1,
    "accountXp": 0,
    "maxFloorReached": 0,
    "tutorialCompleted": false,
    "linkedProviders": ["anonymous"]
  },
  "economy": {
    "gold": 0,
    "gems": 0,
    "materials": [],
    "ownedProducts": [],
    "purchaseHistory": []
  },
  "roster": [],
  "party": {
    "activeHeroInstanceIds": []
  },
  "inventory": {
    "items": [],
    "equipment": []
  },
  "run": {
    "active": false,
    "runId": null,
    "seed": 0,
    "currentFloor": 0,
    "currentThemeId": null,
    "visitedFloors": []
  },
  "settings": {
    "musicVolume": 1.0,
    "sfxVolume": 1.0,
    "language": "pt-BR",
    "notificationsEnabled": false
  },
  "meta": {
    "totalRunsStarted": 0,
    "totalRunsWon": 0,
    "totalRunsLost": 0,
    "totalBossesDefeated": 0,
    "totalAdsWatched": 0,
    "totalPurchasesCompleted": 0
  }
}
```

---

## 5. Estruturas de estado

### 5.1 HeroInstanceState

```json
{
  "instanceId": "hero_abc123",
  "classId": "guardian",
  "displayName": "Brann",
  "level": 1,
  "xp": 0,
  "currentHp": 30,
  "maxHp": 30,
  "rank": 1,
  "rarity": "common",
  "learnedSkillIds": ["guardian_strike"],
  "equippedItemInstanceIds": [],
  "statusEffects": [],
  "createdAtUtc": "2026-07-04T00:00:00Z"
}
```

### 5.2 ItemInstanceState

```json
{
  "instanceId": "item_abc123",
  "definitionId": "health_potion_small",
  "quantity": 3,
  "acquiredAtUtc": "2026-07-04T00:00:00Z"
}
```

### 5.3 EquipmentInstanceState

```json
{
  "instanceId": "equip_abc123",
  "definitionId": "rusty_sword",
  "level": 1,
  "rarity": "common",
  "affixIds": [],
  "equippedByHeroInstanceId": null
}
```

### 5.4 RunState

```json
{
  "active": true,
  "runId": "run_abc123",
  "seed": 918273,
  "currentFloor": 7,
  "currentThemeId": "forgotten_crypt",
  "partySnapshot": [],
  "visitedFloors": [
    {
      "floor": 1,
      "type": "combat",
      "encounterId": "crypt_common_001",
      "completed": true,
      "rewardClaimed": true
    }
  ]
}
```

---

## 6. Quando salvar

### 6.1 Save local

Salvar localmente em:

```text
fim de combate
entrada em novo andar
entrada no resting site
compra de item
contratação de personagem
mudança de equipamento
level up
compra interna concluída
rewarded ad recompensado
app pausado
app fechado, quando possível
```

### 6.2 Cloud save

Sincronizar cloud em:

```text
fim de combate
fim de boss
entrada no resting site
compra interna concluída
account linking concluído
app pausado, se houve alteração relevante
intervalo periódico seguro, se necessário
```

Evitar cloud save em cada microação para reduzir latência, custo e risco de conflito.

---

## 7. Estratégia offline

O jogo deve funcionar offline, exceto para:

```text
login social
cloud sync
IAP
ads
remote config atualizado
```

Se offline:

```text
permitir jogar com save local
marcar save como dirty
não exibir ações que dependem de ads/IAP
sincronizar quando conexão voltar
resolver conflito se cloud save mudou em outro dispositivo
```

---

## 8. Resolução de conflitos

### 8.1 Quando há conflito

Conflito ocorre quando:

```text
save local e cloud têm timestamps diferentes
ambos foram modificados depois do último sync conhecido
PlayerId é o mesmo, mas deviceId difere
progresso relevante diverge
```

### 8.2 Estratégia recomendada

Não fazer merge automático complexo no MVP. Usar escolha explícita quando houver divergência importante.

UI de conflito:

```text
Save neste dispositivo
- último update
- andar atual
- nível da conta
- gold/gems
- profundidade máxima

Save na nuvem
- último update
- andar atual
- nível da conta
- gold/gems
- profundidade máxima

Escolher: usar este dispositivo / usar nuvem
```

### 8.3 Merge automático permitido

Pode fazer merge automático apenas para dados seguros:

```text
settings locais
flags de tutorial se uma versão completou
compras permanentes confirmadas
maior maxFloorReached
maior totalBossesDefeated
```

Não fazer merge automático de:

```text
moeda premium
inventário
estado de run ativa
histórico de compras
personagens contratados
```

---

## 9. Compras e save

### 9.1 Regra crítica

Compra interna concluída deve ser persistida imediatamente local e cloud.

Fluxo:

```text
IAP confirmado
→ Validar produto
→ Conceder recompensa
→ Atualizar save local
→ Tentar cloud sync
→ Registrar pendência se cloud falhar
→ Reconciliar depois
```

### 9.2 Pending grants

Se a compra for confirmada, mas o cloud sync falhar:

```json
{
  "pendingGrants": [
    {
      "grantId": "grant_abc123",
      "source": "iap",
      "productId": "com.company.dungeoncrawler.gems_small",
      "amount": 300,
      "createdAtUtc": "2026-07-04T00:00:00Z",
      "synced": false
    }
  ]
}
```

---

## 10. Migração de save

### 10.1 SaveMigrationService

Responsável por transformar saves antigos para a versão atual.

```text
v1 -> v2
v2 -> v3
v3 -> atual
```

Nunca pular versões sem função explícita.

### 10.2 Exemplo

```csharp
public interface ISaveMigration
{
    int FromVersion { get; }
    int ToVersion { get; }
    SaveSnapshot Migrate(SaveSnapshot oldSave);
}
```

### 10.3 Regras

```text
backup antes da migração
migração determinística
não perder compras
não perder moeda premium
registrar falhas
bloquear upload cloud se migração falhar
```

---

## 11. Segurança e integridade

### 11.1 Ameaças

```text
edição manual do save local
duplicação de moeda
rollback de save antigo
compra não concedida
compra duplicada
conflito entre dispositivos
```

### 11.2 Mitigações MVP

```text
checksum simples do save local
save local ofuscado, se necessário
validação de produto IAP
histórico de purchase transaction IDs
cloud save após eventos econômicos
limitar concessões de rewarded ads
não confiar apenas no client para eventos críticos pós-MVP
```

### 11.3 Pós-MVP

```text
Cloud Code para grants sensíveis
validação server-side de recibos
economia mais server-authoritative
anti-cheat básico
histórico auditável de transações
```

---

## 12. API interna sugerida

```csharp
public interface ISaveService
{
    SaveSnapshot Current { get; }
    bool HasUnsyncedChanges { get; }

    Task InitializeAsync();
    Task SaveLocalAsync(SaveReason reason);
    Task SyncCloudAsync(SaveReason reason);
    Task<CloudConflictResult> CheckConflictAsync();
    Task ResolveConflictAsync(ConflictResolutionChoice choice);
}
```

```csharp
public enum SaveReason
{
    Manual,
    CombatCompleted,
    BossCompleted,
    FloorChanged,
    RestingSiteEntered,
    PurchaseCompleted,
    RewardedAdCompleted,
    AccountLinked,
    AppPaused,
    AppQuit
}
```

---

## 13. Testes obrigatórios

```text
criar save novo
salvar e carregar local
migrar save v1 para atual
detectar conflito local/cloud
resolver conflito usando local
resolver conflito usando cloud
não perder compra permanente em conflito
não duplicar gems ao repetir sync
salvar após compra IAP simulada
restaurar remove ads
continuar run ativa em outro dispositivo
jogar offline e sincronizar depois
```

---

## 14. Decisões abertas

- O jogador poderá jogar sem vincular conta indefinidamente?
- Haverá botão manual de sync?
- Haverá limite de dispositivos?
- O save será único por conta ou múltiplos slots?
- Run ativa pode ser continuada em outro dispositivo ou apenas progresso permanente?
- Haverá exclusão de conta/save dentro do app?

Recomendação inicial:

```text
1 slot por conta
jogo offline permitido
sync automático + botão manual
run ativa sincronizável
account linking fortemente incentivado após tutorial
```

