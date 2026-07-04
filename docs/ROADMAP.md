# ROADMAP.md

## 1. Visão geral

Este roadmap organiza o desenvolvimento do jogo em sprints incrementais. O objetivo é chegar rapidamente a um protótipo jogável, depois integrar save, monetização, conteúdo e distribuição mobile.

**Stack assumida:**

```text
Unity 6.3 LTS
C#
Unity Gaming Services
AdMob
Unity IAP
Unity Cloud Save
Unity Authentication
GitHub + Codex
```

---

## 2. Milestones

| Milestone | Objetivo |
|---|---|
| Sprint 0 | Fundação técnica e builds mobile. |
| Sprint 1 | Protótipo de combate. |
| Sprint 2 | Dungeon run procedural. |
| Sprint 3 | Sistemas RPG. |
| Sprint 4 | Conta, save local e cloud save. |
| Sprint 5 | Monetização com ads e IAP. |
| Sprint 6 | Conteúdo inicial, balanceamento e soft launch. |

---

## 3. Sprint 0 — Fundação técnica

### Objetivo

Criar o projeto Unity com estrutura profissional, versionamento, builds Android/iOS funcionando e arquitetura inicial pronta para desenvolvimento assistido por Codex.

### Entregas

```text
Projeto Unity 6.3 LTS criado
Repositório GitHub criado
Git LFS configurado
Estrutura de pastas criada
Assembly definitions criados
Cena Bootstrap criada
Cena MainMenu criada
Build Android local funcionando
Build iOS gerada e abrindo no Xcode
CODING_GUIDELINES.md aplicado ao repositório
ScriptableObject base criado
EventBus inicial criado
ServiceRegistry inicial criado
```

### Tarefas

```text
Configurar Unity Hub e versão LTS.
Criar projeto 2D URP ou 2D Core.
Configurar Android Build Support.
Configurar iOS Build Support.
Configurar Git e .gitignore para Unity.
Configurar Git LFS.
Criar estrutura Assets/Game.
Criar assemblies.
Criar Bootstrap scene.
Criar MainMenu scene.
Criar sistema simples de navegação.
Criar primeiro teste EditMode.
Validar build Android.
Validar export iOS no Mac.
```

### Critérios de aceite

```text
Projeto abre sem erro.
Build Android instala em aparelho ou emulador.
Export iOS abre no Xcode.
MainMenu carrega a partir do Bootstrap.
Primeiro teste automatizado passa.
Codex consegue navegar pela estrutura do projeto.
```

---

## 4. Sprint 1 — Combat prototype

### Objetivo

Criar o primeiro combate funcional com party, inimigos, turnos, habilidades e vitória/derrota.

### Entregas

```text
Party com 4 posições
Inimigos com 1 a 4 posições
TurnManager
CombatController
SkillDefinition
TargetingRulesService
DamageResolver
StatusEffectResolver básico
UI básica de combate
1 herói jogável
1 inimigo comum
Vitória e derrota
Testes de regras principais
```

### Tarefas

```text
Criar CombatantState.
Criar HeroClassDefinition.
Criar EnemyDefinition.
Criar SkillDefinition.
Implementar ranks 1-4.
Implementar TargetingRulesService.
Implementar TurnManager.
Implementar DamageResolver.
Implementar morte de combatente.
Implementar fluxo de vitória/derrota.
Criar UI temporária de combate.
Criar botão de skill.
Criar seleção de alvo.
Criar testes EditMode.
```

### Critérios de aceite

```text
Jogador inicia combate.
Jogador escolhe habilidade.
Sistema valida alvo por rank.
Inimigo age automaticamente.
HP é reduzido corretamente.
Combate termina com vitória ou derrota.
Testes de TurnManager e TargetingRulesService passam.
```

---

## 5. Sprint 2 — Dungeon run procedural

### Objetivo

Transformar combates isolados em uma run com andares, geração procedural, bosses, resting sites e mudança de tema.

### Entregas

```text
DungeonRunService
FloorGenerator
EncounterGenerator
DungeonThemeDefinition
EncounterTableDefinition
Boss a cada 5 andares
Resting site a cada 10 andares
Mudança de tema a cada 20 andares
Recompensa pós-combate
Tela simples de resultado
Primeiro fluxo de avançar andar
```

### Tarefas

```text
Criar RunState.
Criar FloorState.
Criar DungeonThemeDefinition.
Criar EncounterDefinition.
Criar EncounterTableDefinition.
Implementar seed de run.
Implementar regra floor % 5 == boss.
Implementar regra floor % 10 == resting site pós-boss.
Implementar regra floor % 20 == theme transition.
Criar recompensa pós-combate.
Criar tela Combat Result.
Criar transição de Combat para próximo andar.
Criar tela Resting Site placeholder.
Criar testes de geração procedural.
```

### Critérios de aceite

```text
Run inicia no andar 1.
Andares comuns geram combates.
Andar 5 gera boss.
Andar 10 gera boss e depois resting site.
Andar 20 troca tema após boss/resting site.
Mesma seed gera sequência reproduzível.
```

---

## 6. Sprint 3 — RPG systems

### Objetivo

Adicionar progressão de personagens, inventário, equipamentos, habilidades desbloqueáveis e contratação.

### Entregas

```text
RosterService
PartyService
HeroProgressionService
InventoryService
EquipmentService
SkillUnlockService
ShopService inicial
Contratação no resting site
Level up
Itens consumíveis
Equipamentos básicos
Status effects expandidos
```

### Tarefas

```text
Criar HeroInstanceState.
Criar PartyState.
Criar RosterState.
Criar ItemDefinition.
Criar EquipmentDefinition.
Criar InventoryState.
Implementar XP e level up.
Implementar aprendizado de habilidades.
Implementar consumível de cura.
Implementar equipamento simples.
Implementar contratação de herói.
Implementar loja do resting site.
Implementar custos em gold.
Criar UI básica de party management.
Criar UI básica de hero details.
```

### Critérios de aceite

```text
Personagens ganham XP.
Personagens sobem de nível.
Jogador aprende pelo menos uma habilidade nova.
Jogador equipa item.
Jogador usa consumível.
Jogador contrata personagem no resting site.
Party ativa pode ser alterada fora do combate.
```

---

## 7. Sprint 4 — Conta, save local e cloud save

### Objetivo

Implementar persistência robusta local e em nuvem, com autenticação Google/Apple e sincronização entre dispositivos.

### Entregas

```text
SaveSnapshot v1
LocalSaveService
SaveMigrationService
Unity Authentication
Login anônimo
Account linking Google
Account linking Apple
Unity Cloud Save
CloudConflictResolver
Tela de conta
Tela de conflito de save
Testes de save/migração
```

### Tarefas

```text
Criar SaveSnapshot.
Criar LocalSaveService.
Criar SaveMigrationService.
Criar SaveReason.
Integrar Unity Services Initialization.
Integrar Unity Authentication.
Implementar login anônimo.
Implementar login/link Google.
Implementar login/link Apple.
Integrar Unity Cloud Save.
Criar regra de sync.
Criar conflito local vs cloud.
Criar UI de resolução de conflito.
Salvar após combate.
Salvar após compra/contratação.
Salvar ao pausar app.
Criar testes de migração.
Criar testes de conflito.
```

### Critérios de aceite

```text
Progresso persiste após fechar app.
Jogador pode vincular conta.
Save sincroniza na nuvem.
Novo dispositivo consegue recuperar save.
Conflito é detectado e resolvido explicitamente.
Compras permanentes não são perdidas em conflito.
```

---

## 8. Sprint 5 — Monetização

### Objetivo

Implementar monetização inicial com rewarded ads, compras internas, remove ads e loja.

### Entregas

```text
Unity IAP integrado
Produtos configurados
PurchaseService
PurchaseGrantService
Restore purchases
Google Mobile Ads integrado
Rewarded ads
Remove Ads
Starter Pack
Gem packs
Analytics de monetização
Remote Config para parâmetros econômicos
```

### Tarefas

```text
Configurar produtos no Google Play Console.
Configurar produtos no App Store Connect.
Integrar Unity IAP.
Criar ProductDefinition.
Criar PurchaseService.
Criar PurchaseGrantService.
Implementar compra de gems.
Implementar starter pack.
Implementar remove ads.
Implementar restore purchases.
Integrar Google Mobile Ads.
Criar RewardedAdService.
Criar recompensa por ad pós-combate.
Criar reroll de loja por ad.
Criar limites anti-abuso.
Criar eventos analytics.
Criar Remote Config inicial.
```

### Critérios de aceite

```text
Compra fake/sandbox funciona no Android.
Compra sandbox funciona no iOS.
Remove Ads é salvo e restaurável.
Rewarded ad concede recompensa apenas após conclusão.
Recompensa de ad é salva local/cloud.
Eventos de monetização são registrados.
```

---

## 9. Sprint 6 — Conteúdo inicial e soft launch

### Objetivo

Construir o primeiro bloco completo de conteúdo, balancear a experiência inicial e preparar publicação em teste fechado.

### Entregas

```text
Primeiro tema: Forgotten Crypt
20 andares jogáveis
4 classes jogáveis
8 a 12 inimigos comuns
4 bosses
1 resting site funcional
Itens iniciais
Equipamentos iniciais
Economia inicial
Tutorial básico
Tela de configurações
Build de teste fechado Android
Build TestFlight iOS
```

### Tarefas

```text
Criar tiles/background do primeiro tema.
Criar sprites de 4 classes.
Criar sprites de inimigos.
Criar sprites de bosses.
Criar ícones de habilidades.
Criar 20 andares balanceados.
Criar encounter tables.
Criar boss tables.
Criar loot tables.
Criar tutorial inicial.
Criar onboarding de account linking.
Criar tela de configurações.
Configurar analytics dashboard.
Configurar builds de teste.
Executar playtests.
Ajustar dificuldade.
Ajustar economia.
Corrigir bugs críticos.
```

### Critérios de aceite

```text
Jogador novo entende como jogar.
Jogador completa pelo menos os primeiros 5 andares sem instrução externa.
Primeiro boss é vencível com party inicial.
Resting site é compreensível.
Save cloud funciona em dispositivo real.
Rewarded ads funcionam em ambiente de teste.
IAP sandbox funciona.
Build Android entra em teste fechado.
Build iOS entra no TestFlight.
```

---

## 10. Backlog pós-MVP

```text
Novos temas de dungeon
Novas classes
Novos bosses
Eventos especiais
Missões diárias
Recompensa diária
Cosméticos
Season pass
Cloud Code para validação econômica
Server-side receipt validation
Leaderboard
Achievements
Push notifications
Balanceamento por segmentação
A/B testing
Localization en-US
```

---

## 11. Riscos e mitigação

| Risco | Impacto | Mitigação |
|---|---|---|
| Escopo crescer demais | Alto | Manter MVP em 20 andares e 4 classes. |
| Save cloud gerar perda de progresso | Alto | Conflict resolver explícito e testes automatizados. |
| Monetização atrasar publicação | Médio | Implementar IAP/ads em sandbox cedo. |
| Assets inconsistentes | Médio | Definir paleta, resolução e pipeline. |
| Codex alterar arquivos demais | Médio | Tarefas pequenas e checklist de PR. |
| Dificuldade mal balanceada | Alto | Analytics por andar e Remote Config. |
| App Store rejeitar login/compra | Alto | Implementar Apple Sign-In/restore purchases corretamente. |

---

## 12. Ordem prática de execução

Recomendação operacional:

```text
1. Sprint 0 completa.
2. Sprint 1 até o combate ficar divertido mesmo com placeholder.
3. Sprint 2 para validar estrutura de dungeon.
4. Sprint 4 parcialmente: save local antes de muito conteúdo.
5. Sprint 3 para progressão.
6. Sprint 4 completo: cloud/account linking.
7. Sprint 5 monetização.
8. Sprint 6 conteúdo e soft launch.
```

Observação: save local deve começar antes da Sprint 4 completa. Assim, os sistemas de RPG já nascem persistíveis.

---

## 13. Definition of Done global

Uma sprint só deve ser considerada concluída quando:

```text
build compila
fluxo principal funciona em aparelho real, quando aplicável
testes críticos passam
documentação foi atualizada
não há erros críticos no console
save não foi quebrado
economia não pode duplicar recurso facilmente
UI é utilizável em tela mobile
```

---

## 14. Primeiro prompt recomendado para Codex

```text
Leia os arquivos GAME_DESIGN.md, TECH_DESIGN.md, CODING_GUIDELINES.md, SAVE_SYSTEM.md, MONETIZATION_DESIGN.md, CONTENT_PIPELINE.md e ROADMAP.md.

Crie a estrutura inicial de pastas em Assets/Game conforme TECH_DESIGN.md.
Crie os assembly definitions principais.
Crie uma cena Bootstrap e uma cena MainMenu simples.
Crie um GameBootstrap.cs que inicializa um ServiceRegistry mínimo.
Crie um primeiro teste EditMode validando que o ServiceRegistry registra e resolve um serviço mock.

Não implemente combate ainda.
Não implemente Unity Gaming Services ainda.
Não crie lógica de UI complexa.
Siga CODING_GUIDELINES.md.
```

