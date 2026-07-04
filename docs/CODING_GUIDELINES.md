# CODING_GUIDELINES.md

## 1. Objetivo

Este documento define as regras de implementação do projeto para humanos e para o ChatGPT Codex.

O objetivo é manter o código:

```text
modular
legível
testável
mobile-first
orientado a dados
seguro para save/economia
fácil de expandir
```

---

## 2. Regras gerais

```text
Usar C#.
Usar Unity 6.3 LTS.
Evitar lógica de jogo em MonoBehaviours de UI.
Preferir serviços e classes puras para regras de domínio.
Usar ScriptableObjects para dados estáticos.
Usar classes serializáveis para estado salvo.
Salvar IDs, não referências Unity.
Evitar singletons globais não controlados.
Evitar código específico de plataforma fora de Platform/.
Toda feature econômica deve atualizar save imediatamente.
Toda alteração em save deve considerar migração futura.
```

---

## 3. Convenções C#

### 3.1 Naming

```text
Classes: PascalCase
Interfaces: I + PascalCase
Methods: PascalCase
Properties: PascalCase
Private fields: _camelCase
Local variables: camelCase
Constants: PascalCase ou UPPER_SNAKE_CASE, manter consistência por contexto
ScriptableObject IDs: lower_snake_case
IAP product IDs: reverse DNS
```

### 3.2 Exemplos

```csharp
public interface IDamageResolver
{
    DamageResult Resolve(DamageRequest request);
}
```

```csharp
public sealed class DamageResolver : IDamageResolver
{
    private readonly IRandomService _randomService;

    public DamageResolver(IRandomService randomService)
    {
        _randomService = randomService;
    }

    public DamageResult Resolve(DamageRequest request)
    {
        // Implementation here.
    }
}
```

---

## 4. Arquitetura

### 4.1 Separação obrigatória

```text
Data Definition: ScriptableObjects com dados editáveis.
Runtime State: classes serializáveis com estado atual.
Domain Services: lógica de jogo pura.
Presentation: MonoBehaviours, UI e animações.
External Services: UGS, Ads, IAP, plataforma.
Persistence: save local, cloud save e migração.
```

### 4.2 Regra para MonoBehaviour

MonoBehaviours podem:

```text
receber input
atualizar visual
executar animações
reagir a eventos
chamar serviços
```

MonoBehaviours não devem:

```text
calcular dano diretamente
sortear loot diretamente
alterar economia sem serviço
salvar cloud diretamente
resolver regras de boss/resting site diretamente
conter lógica de progressão complexa
```

---

## 5. ScriptableObjects

### 5.1 Uso correto

Usar ScriptableObjects para:

```text
classes de heróis
habilidades
inimigos
bosses
itens
equipamentos
status effects
temas de dungeon
tabelas de encontro
tabelas de loja
produtos econômicos
```

### 5.2 Regra crítica

Nunca modificar dados de ScriptableObject em runtime como se fossem estado do jogador.

Errado:

```csharp
heroClassDefinition.CurrentHp -= damage;
```

Correto:

```csharp
heroInstanceState.CurrentHp -= damage;
```

---

## 6. Estado e save

### 6.1 Regras

```text
Todo estado persistente deve estar em SaveSnapshot.
Todo objeto salvo deve ter ID estável.
Todo save deve ter saveVersion.
Toda nova versão deve ter migração.
Toda compra concluída deve salvar local e tentar cloud sync.
Toda concessão econômica deve passar por Economy/PurchaseGrantService.
```

### 6.2 Proibido

```text
Salvar referências diretas a GameObject.
Salvar referências diretas a ScriptableObject.
Salvar estado apenas em PlayerPrefs.
Alterar gems diretamente fora do CurrencyService.
Conceder IAP diretamente na UI.
```

---

## 7. Async e serviços externos

### 7.1 Regras

```text
Usar async/await para UGS, IAP, Cloud Save e Auth.
Toda chamada externa deve tratar erro.
Toda chamada externa deve ter feedback de UI quando relevante.
Nunca bloquear a main thread esperando rede.
Serviços externos devem expor resultado tipado, não lançar exceção para fluxo comum.
```

### 7.2 Exemplo de resultado

```csharp
public readonly struct ServiceResult<T>
{
    public bool Success { get; }
    public T Value { get; }
    public string ErrorCode { get; }
    public string ErrorMessage { get; }
}
```

---

## 8. Eventos

Usar EventBus ou sinais centralizados para desacoplar sistemas.

Eventos sugeridos:

```text
CombatStarted
TurnStarted
SkillUsed
DamageApplied
StatusEffectApplied
CombatEnded
FloorCompleted
BossDefeated
RestingSiteEntered
CurrencyChanged
InventoryChanged
HeroHired
SaveCompleted
CloudSyncCompleted
PurchaseCompleted
RewardedAdCompleted
```

Regra:

```text
Eventos comunicam fatos que já aconteceram.
Comandos solicitam ações.
Não misturar evento com comando.
```

---

## 9. UI

### 9.1 Regras

```text
UI deve chamar comandos de serviços.
UI deve renderizar estado recebido.
UI não deve ser fonte de verdade do jogo.
Toda tela complexa deve ter ViewModel.
Popups devem ser reutilizáveis.
Animações não devem bloquear save/economia.
```

### 9.2 Botões de compra e ads

Todo botão de compra/ad deve:

```text
exibir recompensa claramente
validar disponibilidade
prevenir duplo clique
mostrar loading
tratar cancelamento
tratar falha
conceder recompensa apenas após confirmação
```

---

## 10. Combate

### 10.1 Regras

```text
Combate deve ser determinístico quando usar mesma seed.
TargetingRulesService valida alvos.
DamageResolver calcula dano.
StatusEffectResolver aplica efeitos.
TurnManager controla ordem e fim de turno.
CombatController orquestra, mas não deve concentrar todas as regras.
```

### 10.2 Testes mínimos

```text
ordem de turno por speed
skill só pode ser usada de ranks válidos
skill só pode atingir targets válidos
dano reduz HP corretamente
morte remove combatente da ordem
stun pula turno
poison aplica dano por turno
vitória detectada quando todos inimigos morrem
derrota detectada quando party morre
```

---

## 11. Dungeon generation

### 11.1 Regras

```text
Toda run tem seed.
FloorGenerator recebe seed + floor + theme.
Boss obrigatório a cada 5 andares.
Resting site após boss a cada 10 andares.
Mudança de tema a cada 20 andares.
Tabela de encontros controla variação.
```

### 11.2 Testes mínimos

```text
andar 5 retorna boss
andar 10 retorna boss + resting site
andar 20 retorna boss + resting site + theme transition
mesma seed gera mesmos andares
pesos de encounter respeitam filtros de tema/andar
```

---

## 12. Performance

### 12.1 Regras mobile

```text
Evitar allocations por frame.
Evitar FindObjectOfType em runtime crítico.
Evitar LINQ em loops de combate, se gerar GC.
Usar pooling para VFX e floating texts.
Usar Sprite Atlas.
Compactar texturas corretamente.
Testar em aparelho real.
```

### 12.2 Update loops

Evitar `Update()` desnecessário. Preferir eventos, coroutines controladas ou timers centralizados.

---

## 13. Git e versionamento

### 13.1 Branches

```text
main: estável
 develop: integração
feature/<nome>: novas features
fix/<nome>: correções
release/<versao>: preparação de release
```

### 13.2 Commits

Formato recomendado:

```text
feat: add combat turn manager
fix: prevent duplicate iap grants
refactor: split damage resolver from combat controller
test: add floor generation tests
docs: update save conflict rules
```

### 13.3 Git LFS

Usar Git LFS para:

```text
.psd
.aseprite
.png grandes
.wav
.mp3
.fbx, se algum dia houver
arquivos binários grandes
```

---

## 14. Regras para Codex

### 14.1 Como pedir tarefas

Sempre pedir alterações pequenas, com escopo fechado.

Bom:

```text
Implemente TargetingRulesService para validar ranks de usuário e alvo usando SkillDefinition. Inclua testes EditMode para skills válidas e inválidas. Não altere UI.
```

Ruim:

```text
Crie todo o combate do jogo.
```

### 14.2 Instruções padrão para Codex

Usar este bloco ao abrir tarefas grandes:

```text
Siga CODING_GUIDELINES.md.
Não altere arquitetura sem explicar.
Não coloque regra de domínio em UI.
Use ScriptableObjects apenas para dados estáticos.
Use estado serializável para runtime/save.
Inclua testes quando alterar regra de domínio.
Não implemente serviços externos reais sem interface e mock.
```

### 14.3 Checklist antes de aceitar código do Codex

```text
Compila?
Quebrou alguma cena?
Criou dependência circular?
Alterou IDs existentes?
Afeta save?
Precisa de migração?
Afeta economia?
Precisa de teste?
Tem lógica de domínio na UI?
Tem chamadas externas sem tratamento de erro?
```

---

## 15. Pull request checklist

```text
[ ] Código compila sem erros.
[ ] Testes relevantes passam.
[ ] Não há referências quebradas em prefabs/cenas.
[ ] Não há lógica de domínio nova dentro de UI.
[ ] Novos ScriptableObjects têm IDs estáveis.
[ ] Mudanças em save incluem migração, se necessário.
[ ] Mudanças econômicas passam por serviços apropriados.
[ ] Eventos de analytics foram adicionados, se necessário.
[ ] Nenhum asset sem licença foi adicionado.
[ ] Funciona em tela mobile.
```

---

## 16. Definição de pronto

Uma feature está pronta quando:

```text
funciona no editor
funciona em build Android ou iOS quando aplicável
tem teste se envolve regra de domínio
tem tratamento de erro se envolve rede/serviço externo
tem analytics se é ação relevante do jogador
tem save se altera progresso
tem documentação atualizada se muda arquitetura
```

