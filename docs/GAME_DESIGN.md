# GAME_DESIGN.md

## 1. Visão do jogo

**Formato:** jogo mobile 2D, pixel art, dungeon crawler, turn-based RPG, free-to-play com ads e compras internas.

**Plataformas-alvo:** Android, iPhone e iPad.

**Engine escolhida:** Unity 6.3 LTS.

**Premissa de design:** o jogador monta e evolui uma party de aventureiros que avança por andares procedurais de uma dungeon. Cada andar pode conter combate, evento, recompensa, loja, boss ou descanso. O jogo usa uma estrutura de combate por turnos com posições, inspirado em RPGs táticos de party, com forte ênfase em composição de grupo, risco/recompensa, gerenciamento de recursos e progressão incremental.

> Referência de estilo: inspirado em jogos como Darkest Dungeon, mas sem copiar personagens, classes, interface, nomes, arte, lore, mecânicas proprietárias ou apresentação audiovisual específica.

---

## 2. Pilares de design

### 2.1 Decisões táticas simples, consequências relevantes

Cada turno deve ter poucas ações possíveis, mas cada decisão deve importar. O jogador deve avaliar:

- qual inimigo eliminar primeiro;
- qual personagem proteger;
- quando curar;
- quando gastar consumíveis;
- quando usar habilidades com cooldown;
- quando arriscar continuar ou priorizar sobrevivência.

### 2.2 Progressão curta por sessão, longa por conta

Cada sessão deve entregar avanço perceptível, mesmo quando o jogador perde uma run. O jogo deve combinar:

- progresso dentro da run;
- evolução permanente da conta;
- desbloqueio de personagens;
- desbloqueio de habilidades;
- economia persistente;
- upgrades adquiridos nos resting sites.

### 2.3 Conteúdo modular e expansível

O jogo deve ser construído de forma data-driven para facilitar criação de:

- novos heróis;
- novas classes;
- novas habilidades;
- novos inimigos;
- novos bosses;
- novos temas de dungeon;
- novas lojas;
- novos eventos;
- novas recompensas.

### 2.4 Mobile-first

O jogo deve funcionar bem em sessões curtas, com interface legível em celular e tablet. As interações devem exigir poucos toques e evitar menus profundos durante o combate.

---

## 3. Loop principal

### 3.1 Loop de sessão

```text
Abrir jogo
→ Sincronizar save
→ Coletar recompensa disponível
→ Gerenciar party
→ Entrar na dungeon
→ Resolver andar
→ Receber recompensa
→ Avançar para próximo andar
→ Encontrar boss/resting site/novo tema em marcos específicos
→ Encerrar sessão ou continuar
→ Salvar localmente e em nuvem
```

### 3.2 Loop de combate

```text
Iniciar encontro
→ Determinar ordem dos turnos
→ Jogador escolhe ação do personagem ativo
→ Validar posições e alvos
→ Resolver dano, cura, buffs, debuffs e efeitos
→ Inimigo executa ação
→ Atualizar status e cooldowns
→ Verificar vitória/derrota
→ Conceder recompensa ou encerrar run
```

### 3.3 Loop de progressão

```text
Ganhar XP, moedas e itens
→ Evoluir personagens
→ Desbloquear habilidades
→ Melhorar equipamentos
→ Contratar novos personagens
→ Aumentar profundidade alcançada
→ Desbloquear novos temas e inimigos
```

---

## 4. Estrutura da dungeon

### 4.1 Regras de andar

O jogo usa andares sequenciais com marcos fixos.

| Regra | Frequência | Descrição |
|---|---:|---|
| Andar comum | Maioria dos andares | Combate, evento, recompensa ou encontro especial. |
| Boss | A cada 5 andares | Encontro mais difícil, com inimigo único ou composição especial. |
| Safe Resting Site | A cada 10 andares | Local seguro após desafio importante. Permite descansar, comprar, aprender, contratar e melhorar. |
| Mudança de tema | A cada 20 andares | Novo bioma/tema visual, novos inimigos, novas recompensas e bosses. |

### 4.2 Resolução de conflito entre regras

Quando múltiplas regras caírem no mesmo andar, aplicar esta prioridade:

```text
1. Boss
2. Safe Resting Site após o boss
3. Mudança de tema após o descanso, quando aplicável
```

Exemplos:

```text
Andar 5  = boss intermediário
Andar 10 = boss + resting site
Andar 15 = boss intermediário
Andar 20 = boss + resting site + mudança de tema
Andar 25 = boss intermediário
Andar 30 = boss + resting site
```

### 4.3 Temas iniciais sugeridos

| Faixa de andares | Tema | Inimigos sugeridos |
|---:|---|---|
| 1–20 | Forgotten Crypt | esqueletos, ratos gigantes, cultistas, armaduras assombradas |
| 21–40 | Fungal Depths | fungos, insetos, criaturas tóxicas, druidas corrompidos |
| 41–60 | Ember Mines | goblins, construtos, elementais de fogo, mineradores amaldiçoados |
| 61–80 | Sunken Temple | anfíbios, sacerdotes antigos, monstros aquáticos |
| 81–100 | Astral Abyss | aberrações, sombras, entidades arcanas |

---

## 5. Combate

### 5.1 Modelo de formação

O combate usa fileiras/posições em vez de grid aberto.

```text
Party:  [4] [3] [2] [1]  vs  [1] [2] [3] [4] :Inimigos
```

- Posição 1 é a linha de frente.
- Posição 4 é a retaguarda.
- Habilidades definem de quais posições podem ser usadas.
- Habilidades definem quais posições podem ser alvo.
- Alguns efeitos empurram, puxam ou trocam posições.

### 5.2 Participantes

Cada combatente possui:

```text
id
nome
classe ou tipo
posição atual
vida atual e máxima
velocidade/iniciativa
atributos
resistências
lista de habilidades
status effects
cooldowns
estado: vivo, morto, incapacitado, fugiu
```

### 5.3 Atributos sugeridos

| Atributo | Função |
|---|---|
| HP | Vida máxima. |
| Attack | Base para dano físico. |
| Magic | Base para dano mágico ou habilidades especiais. |
| Defense | Redução de dano físico. |
| Resistance | Redução de dano mágico/status. |
| Speed | Ordem de turno e chance de agir primeiro. |
| Accuracy | Chance de acertar. |
| Evasion | Chance de evitar ataques. |
| Critical | Chance de acerto crítico. |

### 5.4 Tipos de ação

```text
Ataque básico
Habilidade ofensiva
Habilidade defensiva
Cura
Buff
Debuff
Mover posição
Usar item
Passar turno
Fugir, se permitido
```

### 5.5 Status effects iniciais

| Status | Efeito |
|---|---|
| Bleed | Dano físico por turno. |
| Poison | Dano verdadeiro ou mágico por turno. |
| Burn | Dano por turno e possível redução de defesa. |
| Stun | Perde próximo turno. |
| Guard | Protege aliado ou reduz dano recebido. |
| Mark | Aumenta dano recebido de certas habilidades. |
| Vulnerable | Aumenta dano recebido. |
| Weak | Reduz dano causado. |
| Haste | Aumenta velocidade. |
| Slow | Reduz velocidade. |

---

## 6. Party e personagens

### 6.1 Tamanho da party

MVP recomendado:

```text
Party ativa: até 4 personagens
Roster persistente: múltiplos personagens desbloqueados/contratados
```

### 6.2 Classes iniciais sugeridas

| Classe | Função | Posições ideais |
|---|---|---|
| Guardian | Tanque/proteção | 1–2 |
| Duelist | Dano físico e mobilidade | 1–3 |
| Ranger | Dano à distância | 3–4 |
| Cleric | Cura e suporte | 2–4 |
| Occultist | Debuff e dano mágico | 3–4 |
| Alchemist | Poison, burn e consumíveis | 2–4 |

### 6.3 Progressão de personagem

Personagens devem evoluir por:

```text
Nível
XP
Habilidades aprendidas
Upgrade de habilidades
Equipamentos
Raridade ou rank
Traits positivos/negativos, opcional pós-MVP
```

### 6.4 Contratação

Nos resting sites, o jogador pode contratar novos personagens. A seleção deve ser procedural com base em:

```text
profundidade alcançada
tema atual
raridade
classe
nível médio da conta
configuração remota de balanceamento
```

---

## 7. Resting site

A cada 10 andares, após o boss, o jogador chega a um safe resting site.

### 7.1 Funções do resting site

```text
Curar party
Remover ou reduzir status negativos
Comprar itens
Vender itens, opcional
Aprender novas habilidades
Melhorar habilidades
Contratar novos personagens
Aplicar upgrades permanentes ou temporários
Reroll da loja
Salvar progresso da run
```

### 7.2 Monetização aceitável no resting site

O resting site pode incluir ações com rewarded ads ou moeda premium, desde que não bloqueiem o progresso essencial.

Exemplos:

```text
Rewarded ad para reroll grátis da loja
Rewarded ad para bônus pequeno de gold
Moeda premium para contratar personagem raro
Moeda premium para comprar cosmético
Compra interna para pacote de recursos
```

Evitar:

```text
Pagar para continuar obrigatoriamente
Pagar para vencer boss específico
Bloquear cura básica atrás de anúncio
Forçar anúncio para avançar
```

---

## 8. Procedural generation

### 8.1 Filosofia

A geração procedural deve criar variação controlada, não caos. O jogador deve sentir surpresa, mas o designer precisa manter controle de dificuldade.

### 8.2 Dados de entrada

```text
seed da run
andar atual
tema atual
nível médio da party
histórico recente de encontros
estado de HP da party
número de consumíveis disponíveis
profundidade máxima já alcançada
```

### 8.3 Tipos de andar

| Tipo | Descrição |
|---|---|
| Combat | Encontro regular. |
| Elite Combat | Combate mais difícil, maior recompensa. |
| Event | Escolha de risco/recompensa. |
| Treasure | Recompensa sem combate ou com custo. |
| Shop | Loja menor fora de resting site, opcional. |
| Boss | Marco obrigatório. |
| Resting Site | Marco seguro. |

### 8.4 Encounter generation

Cada tema deve ter tabelas de encontro:

```text
common encounters
rare encounters
elite encounters
boss encounters
special encounters
```

Cada encontro deve ter:

```text
id
peso
andar mínimo
andar máximo
tema
composição de inimigos
nível base
modificadores opcionais
recompensa base
```

---

## 9. Economia e recompensas

### 9.1 Moedas

MVP recomendado:

```text
Gold: moeda comum, obtida jogando.
Gems: moeda premium, obtida por compra, eventos ou recompensas limitadas.
```

### 9.2 Recompensas pós-combate

```text
gold
xp
itens consumíveis
equipamentos
materiais de upgrade
chance de moeda premium em quantidades pequenas, limitada
```

### 9.3 Loot

O loot deve considerar:

```text
tema atual
profundidade
raridade
boss ou combate comum
classe da party
histórico recente de drops
```

---

## 10. MVP recomendado

### 10.1 Escopo jogável mínimo

```text
1 tema de dungeon
20 andares
4 classes jogáveis
8 a 12 inimigos comuns
4 bosses
1 resting site funcional
party de até 4 personagens
combate por turnos com ranks
save local
cloud save
login Google/Apple
rewarded ads
compras internas básicas
```

### 10.2 Escopo fora do MVP

```text
PvP
guildas
chat
multiplayer síncrono
skins complexas
battle pass
eventos sazonais
mundo aberto
cutscenes longas
editor de dungeon
sistema social avançado
```

---

## 11. Direção visual

### 11.1 Estilo

```text
2D pixel art
fantasia sombria estilizada
contraste forte
silhuetas legíveis
animações curtas e impactantes
interface limpa para mobile
```

### 11.2 Tamanhos iniciais sugeridos

```text
Personagens: 64x64 ou 96x96
Inimigos pequenos: 64x64
Inimigos médios: 96x96
Bosses: 128x128 ou 192x192
Ícones: 32x32 ou 64x64
Tiles: 16x16 ou 32x32
```

### 11.3 Princípio visual

Legibilidade mobile é mais importante que detalhe. Um sprite bonito que não comunica função em tela pequena deve ser simplificado.

---

## 12. Retenção

### 12.1 Ganchos de curto prazo

```text
recompensa diária
missões simples
objetivos por run
bosses frequentes
loot visível pós-combate
novas habilidades a cada marco
```

### 12.2 Ganchos de médio prazo

```text
desbloqueio de classes
melhoria da base/roster
coleção de equipamentos
progressão de temas
metas de profundidade
```

### 12.3 Ganchos de longo prazo

```text
eventos sazonais
novos temas de dungeon
novos bosses
novas classes
leaderboards, opcional
season pass, opcional
```

---

## 13. Métricas de sucesso

Métricas iniciais:

```text
D1 retention
D7 retention
tempo médio de sessão
profundidade média alcançada
taxa de conclusão do tutorial
taxa de derrota por andar
taxa de uso de rewarded ads
conversão de IAP
ARPU
ARPDAU
percentual de jogadores com conta vinculada
```

---

## 14. Decisões abertas

- O jogo terá morte permanente de personagens?
- O jogador poderá abandonar uma run sem penalidade?
- Haverá energia/stamina para entrar na dungeon?
- Equipamentos serão permanentes ou perdidos na run?
- Bosses serão fixos por andar ou selecionados por tema?
- Personagens terão raridade?
- Haverá narrativa ou apenas progressão mecânica?

