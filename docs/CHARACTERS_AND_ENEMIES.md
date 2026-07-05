# Characters and Enemies

Documento de referência para classes jogáveis, personagens, inimigos, bosses e composições de encontro do primeiro bioma do jogo.

Este arquivo deve servir como base para criação de `HeroClassDefinition`, `SkillDefinition`, `EnemyDefinition`, `BossDefinition`, `EncounterDefinition` e `EncounterTableDefinition` no Unity.

---

## 1. Escopo

### Bioma inicial

```text
Nome: Cripta Esquecida
ID sugerido: forgotten_crypt
Andares: 1-20
Tema: catacumbas antigas, ossários, mortos-vivos, cultistas, relíquias profanadas
Tom visual: pixel art sombrio, baixa saturação, contraste alto, luz de velas, pedra fria, ossos e runas antigas
```

### Regras principais do bioma

```text
Andar 5: boss intermediário
Andar 10: boss + safe resting site
Andar 15: boss intermediário
Andar 20: boss final do bioma + safe resting site + transição de tema
```

### Modelo de ranks

```text
Party ranks: 1, 2, 3, 4
Enemy ranks: 1, 2, 3, 4

Rank 1: frente
Rank 4: fundo
```

---

## 2. Classes jogáveis e sistema de skills

Para o MVP, implementar primeiro as quatro classes principais:

1. Guardião Sepulcral
2. Lâmina Velada
3. Acólita da Vela
4. Arcanista do Túmulo

As classes Caçador de Relíquias e Exorcista Errante podem entrar como desbloqueáveis após os primeiros bosses.

---

## 2.0 Regras globais de skills

```text
Cada personagem começa com 2 skills iniciais.
Cada personagem pode ter no máximo 4 skills equipadas ao mesmo tempo.
Cada classe possui 6 skills disponíveis no total.
Novas skills são aprendidas no resting site.
Skills aprendidas podem ser melhoradas no resting site até o nível 3.
Skills ofensivas usam a curva Fibonacci de dano.
```

### Tipos de skill

```text
Ofensiva: causa dano direto ou dano em área.
Defensiva: cura, proteção, escudo, guard ou redução de dano.
Buff: melhora atributos, precisão, evasão, velocidade, dano ou economia.
Debuff: reduz atributos inimigos, aplica mark, vulnerable, slow, weak ou outros efeitos negativos.
```

### Regra de dano para skills ofensivas

A potência de uma skill ofensiva usa a curva Fibonacci como multiplicador de nível de skill.

| Nível da skill | Unidade Fibonacci | Multiplicador sobre dano médio do personagem |
|---:|---:|---:|
| 1 | 3 | 1.00x |
| 2 | 5 | 1.67x |
| 3 | 8 | 2.67x |

Regra sugerida para implementação:

```text
baseDamage = HeroProgressionDefinition.GetAverageDamage(heroLevel)
skillLevelMultiplier = fibonacciSkillPower[skillLevel] / 3
skillDamage = baseDamage * skillLevelMultiplier * skillSpecificMultiplier
```

Multiplicadores específicos sugeridos:

```text
Dano leve: 0.65x
Dano médio: 1.00x
Dano alto: 1.25x
Dano em área: 0.50x por alvo
Dano com forte controle/debuff: 0.75x
Dano multi-hit: dividir o multiplicador total entre os hits
```

### Aprendizado e melhoria no resting site

```text
Aprender nova skill: adiciona a skill à lista de skills conhecidas do personagem.
Melhorar skill: aumenta o nível da skill de 1 para 2 ou de 2 para 3.
Equipar skill: seleciona até 4 skills conhecidas para uso em combate.
Desequipar skill: remove a skill da lista ativa, sem apagar progresso.
```

Regras recomendadas:

```text
Skill level 1: efeito base.
Skill level 2: aumenta potência, duração, chance ou reduz cooldown.
Skill level 3: melhora fortemente a identidade da skill, sem mudar sua função principal.
```

---

# 2.1 Aldren Voss — Guardião Sepulcral

```text
Classe: Guardião Sepulcral
ID sugerido: guardian
Personagem base: Aldren Voss
Função: tanque, proteção, controle de frontline
Ranks ideais: 1-2
Dificuldade: baixa
```

## Equipamentos permitidos

```text
Armas:
- Espada curta
- Espada longa
- Maça
- Martelo de guerra

Defesa:
- Escudo leve
- Escudo pesado
- Armadura média
- Armadura pesada

Acessórios:
- Amuleto
- Anel defensivo
- Relíquia sagrada
```

## Skills da classe

Skills iniciais:

```text
- Golpe de Guarda
- Postura de Ferro
```

| Skill | ID sugerido | Tipo | Inicial | Aprendizado sugerido | Ranks de uso | Alvos válidos |
|---|---|---|---|---|---|---|
| Golpe de Guarda | `skill.guardian.guard_strike` | Ofensiva | Sim | Inicial | 1-2 | Inimigos 1-2 |
| Postura de Ferro | `skill.guardian.iron_stance` | Defensiva | Sim | Inicial | 1-2 | Self |
| Investida de Escudo | `skill.guardian.shield_charge` | Ofensiva/Debuff | Não | Resting site após nível 2 | 1-2 | Inimigos 1-2 |
| Proteger Aliado | `skill.guardian.protect_ally` | Defensiva | Não | Resting site após nível 4 | 1-2 | Aliado único |
| Quebra-Ossos | `skill.guardian.bone_breaker` | Ofensiva/Debuff | Não | Resting site após nível 6 | 1-2 | Inimigos 1-2 |
| Última Muralha | `skill.guardian.last_wall` | Defensiva/Buff | Não | Resting site após nível 8 | 1-2 | Todos os aliados |

## Efeitos por nível de skill

| Skill | Nível 1 | Nível 2 | Nível 3 |
|---|---|---|---|
| Golpe de Guarda | Dano médio em inimigo frontal. | Dano médio aumentado pela curva Fibonacci e +1 turno de guard leve em si. | Dano médio aumentado pela curva Fibonacci e aplica guard leve em si e no aliado mais ferido por 1 turno. |
| Postura de Ferro | Aumenta Defense própria por 2 turnos. | Aumenta Defense e Resistance por 2 turnos. | Aumenta Defense e Resistance por 3 turnos e reduz dano crítico recebido. |
| Investida de Escudo | Dano leve e pequena chance de stun. | Dano leve aumentado pela curva Fibonacci e chance média de stun. | Dano leve aumentado pela curva Fibonacci, chance média de stun e aplica Slow se stun falhar. |
| Proteger Aliado | Redireciona parte do dano de um aliado para o Guardião por 1 turno. | Redireciona dano por 2 turnos e reduz parte do dano recebido. | Redireciona dano por 2 turnos, reduz dano recebido e concede shield pequeno ao aliado protegido. |
| Quebra-Ossos | Dano médio e aplica Vulnerable por 1 turno. | Dano médio aumentado pela curva Fibonacci e aplica Vulnerable por 2 turnos. | Dano alto pela curva Fibonacci, aplica Vulnerable por 2 turnos e reduz Defense do alvo. |
| Última Muralha | Concede shield pequeno para todos os aliados. | Concede shield médio e aumenta Defense da party por 1 turno. | Concede shield alto, aumenta Defense da party por 2 turnos e remove Mark dos aliados. |

## Perfil de gameplay

O Guardião é a opção mais estável para o início do jogo. Ele protege a linha de trás, segura dano e oferece controle básico com stun. Deve ser o personagem mais tolerante a erro para jogadores iniciantes.

---

# 2.2 Neria Vale — Lâmina Velada

```text
Classe: Lâmina Velada
ID sugerido: rogue
Personagem base: Neria Vale
Função: dano físico, mobilidade, execução
Ranks ideais: 2-3
Dificuldade: média
```

## Equipamentos permitidos

```text
Armas:
- Adaga
- Par de adagas
- Espada curta
- Besta leve

Defesa:
- Armadura leve
- Capa reforçada

Acessórios:
- Anel ofensivo
- Talismã de agilidade
- Veneno alquímico
```

## Skills da classe

Skills iniciais:

```text
- Corte Rápido
- Passo Sombrio
```

| Skill | ID sugerido | Tipo | Inicial | Aprendizado sugerido | Ranks de uso | Alvos válidos |
|---|---|---|---|---|---|---|
| Corte Rápido | `skill.rogue.quick_cut` | Ofensiva | Sim | Inicial | 1-3 | Inimigos 1-2 |
| Passo Sombrio | `skill.rogue.shadow_step` | Buff/Defensiva | Sim | Inicial | 1-3 | Self |
| Punhalada Exposta | `skill.rogue.exposed_stab` | Ofensiva | Não | Resting site após nível 2 | 1-3 | Inimigos 1-3 |
| Lâmina Envenenada | `skill.rogue.poisoned_blade` | Ofensiva/Debuff | Não | Resting site após nível 4 | 1-3 | Inimigos 1-2 |
| Execução Silenciosa | `skill.rogue.silent_execution` | Ofensiva | Não | Resting site após nível 6 | 1-3 | Inimigos 1-3 |
| Marca da Morte | `skill.rogue.death_mark` | Debuff | Não | Resting site após nível 8 | 2-3 | Inimigos 1-4 |

## Efeitos por nível de skill

| Skill | Nível 1 | Nível 2 | Nível 3 |
|---|---|---|---|
| Corte Rápido | Dano médio com bônus pequeno de Speed na próxima rodada. | Dano médio aumentado pela curva Fibonacci e bônus maior de Speed. | Dano médio aumentado pela curva Fibonacci e chance de agir mais cedo no próximo turno. |
| Passo Sombrio | Move 1 rank para trás e ganha Evasion por 1 turno. | Move 1 rank para trás, ganha Evasion maior e remove Mark. | Move 1 rank para trás, ganha Evasion alta e aplica Haste por 1 turno. |
| Punhalada Exposta | Dano médio; dano bônus contra alvo com debuff. | Dano médio aumentado pela curva Fibonacci; bônus maior contra alvo com debuff. | Dano alto pela curva Fibonacci contra alvo com debuff e pequena chance de crítico adicional. |
| Lâmina Envenenada | Dano leve e aplica Poison por 2 turnos. | Dano leve aumentado pela curva Fibonacci e Poison por 3 turnos. | Dano médio pela curva Fibonacci, Poison por 3 turnos e aplica Vulnerable se o alvo já estiver envenenado. |
| Execução Silenciosa | Dano alto contra alvo abaixo de 35% de HP. | Dano alto aumentado pela curva Fibonacci contra alvo abaixo de 45% de HP. | Dano alto aumentado pela curva Fibonacci; se derrotar o alvo, ganha Evasion por 1 turno. |
| Marca da Morte | Aplica Mark por 2 turnos. | Aplica Mark e reduz Evasion do alvo por 2 turnos. | Aplica Mark, reduz Evasion e aumenta dano crítico recebido pelo alvo por 2 turnos. |

## Perfil de gameplay

A Lâmina Velada elimina ameaças rapidamente e se beneficia de debuffs aplicados por outras classes. Deve ser eficiente contra alvos frágeis de backline e inimigos já enfraquecidos.

---

# 2.3 Mirella Thorne — Acólita da Vela

```text
Classe: Acólita da Vela
ID sugerido: acolyte
Personagem base: Mirella Thorne
Função: cura, suporte, purificação
Ranks ideais: 3-4
Dificuldade: baixa/média
```

## Equipamentos permitidos

```text
Armas:
- Cajado
- Sino ritual
- Punhal cerimonial

Defesa:
- Manto leve
- Vestes consagradas

Acessórios:
- Relíquia sagrada
- Rosário
- Livro de preces
- Amuleto de cura
```

## Skills da classe

Skills iniciais:

```text
- Prece Menor
- Luz da Vela
```

| Skill | ID sugerido | Tipo | Inicial | Aprendizado sugerido | Ranks de uso | Alvos válidos |
|---|---|---|---|---|---|---|
| Prece Menor | `skill.acolyte.minor_prayer` | Defensiva | Sim | Inicial | 3-4 | Aliado único |
| Luz da Vela | `skill.acolyte.candlelight` | Ofensiva | Sim | Inicial | 3-4 | Inimigos 1-3 |
| Bênção Frágil | `skill.acolyte.fragile_blessing` | Buff | Não | Resting site após nível 2 | 3-4 | Aliado único |
| Purificar Feridas | `skill.acolyte.cleanse_wounds` | Defensiva | Não | Resting site após nível 4 | 3-4 | Aliado único |
| Círculo de Cinzas | `skill.acolyte.ash_circle` | Defensiva/Buff | Não | Resting site após nível 6 | 3-4 | Todos os aliados |
| Chama Consagrada | `skill.acolyte.consecrated_flame` | Defensiva/Buff | Não | Resting site após nível 8 | 3-4 | Todos os aliados |

## Efeitos por nível de skill

| Skill | Nível 1 | Nível 2 | Nível 3 |
|---|---|---|---|
| Prece Menor | Cura leve em um aliado. | Cura moderada e remove Mark. | Cura moderada, remove Mark e concede shield pequeno. |
| Luz da Vela | Dano leve sagrado; bônus contra mortos-vivos. | Dano leve aumentado pela curva Fibonacci; bônus maior contra mortos-vivos. | Dano médio pela curva Fibonacci contra mortos-vivos e aplica Weak por 1 turno. |
| Bênção Frágil | Aumenta Defense de um aliado por 2 turnos. | Aumenta Defense e Resistance por 2 turnos. | Aumenta Defense, Resistance e cura recebida por 2 turnos. |
| Purificar Feridas | Cura leve e remove Poison ou Bleed. | Cura moderada e remove até 1 status negativo comum. | Cura moderada, remove até 2 status negativos comuns e concede Resistance por 1 turno. |
| Círculo de Cinzas | Reduz dano recebido pela party por 1 turno. | Reduz dano recebido pela party por 2 turnos. | Reduz dano recebido, aumenta Resistance da party e remove Weak. |
| Chama Consagrada | Cura leve em todos os aliados. | Cura moderada em todos os aliados e remove 1 debuff. | Cura moderada em todos os aliados, remove 1 debuff e aplica regeneração leve por 2 turnos. |

## Perfil de gameplay

A Acólita é central para runs longas. Ela reduz o desgaste entre combates e aumenta a chance de sobrevivência até os resting sites.

---

# 2.4 Cael Morvain — Arcanista do Túmulo

```text
Classe: Arcanista do Túmulo
ID sugerido: arcanist
Personagem base: Cael Morvain
Função: dano mágico, controle, dano em área
Ranks ideais: 3-4
Dificuldade: média/alta
```

## Equipamentos permitidos

```text
Armas:
- Grimório
- Varinha
- Foco arcano
- Cajado ritual

Defesa:
- Manto arcano
- Vestes leves

Acessórios:
- Cristal arcano
- Anel místico
- Relíquia profana
- Fragmento rúnico
```

## Skills da classe

Skills iniciais:

```text
- Faísca Profana
- Selo de Lentidão
```

| Skill | ID sugerido | Tipo | Inicial | Aprendizado sugerido | Ranks de uso | Alvos válidos |
|---|---|---|---|---|---|---|
| Faísca Profana | `skill.arcanist.profane_spark` | Ofensiva | Sim | Inicial | 3-4 | Inimigos 1-4 |
| Selo de Lentidão | `skill.arcanist.slowing_seal` | Debuff | Sim | Inicial | 3-4 | Inimigos 1-4 |
| Estilhaço Arcano | `skill.arcanist.arcane_shard` | Ofensiva | Não | Resting site após nível 2 | 3-4 | Inimigos 2-4 |
| Névoa do Túmulo | `skill.arcanist.grave_mist` | Debuff | Não | Resting site após nível 4 | 3-4 | Todos os inimigos |
| Explosão Rúnica | `skill.arcanist.runic_burst` | Ofensiva | Não | Resting site após nível 6 | 3-4 | Todos os inimigos |
| Cometa Sepulcral | `skill.arcanist.sepulchral_comet` | Ofensiva/Debuff | Não | Resting site após nível 8 | 3-4 | Inimigo único + adjacentes |

## Efeitos por nível de skill

| Skill | Nível 1 | Nível 2 | Nível 3 |
|---|---|---|---|
| Faísca Profana | Dano mágico médio em um alvo. | Dano mágico médio aumentado pela curva Fibonacci. | Dano mágico alto pela curva Fibonacci e pequena chance de aplicar Vulnerable. |
| Selo de Lentidão | Aplica Slow por 2 turnos. | Aplica Slow maior por 2 turnos e reduz Evasion. | Aplica Slow maior por 3 turnos e reduz Evasion e Accuracy. |
| Estilhaço Arcano | Dano médio contra backline. | Dano médio aumentado pela curva Fibonacci e ignora pequena parte de Resistance. | Dano alto pela curva Fibonacci e ignora parte maior de Resistance. |
| Névoa do Túmulo | Reduz Accuracy de todos os inimigos por 1 turno. | Reduz Accuracy por 2 turnos. | Reduz Accuracy e Critical por 2 turnos. |
| Explosão Rúnica | Dano leve em todos os inimigos. | Dano em área aumentado pela curva Fibonacci. | Dano em área aumentado pela curva Fibonacci e aplica Weak por 1 turno em inimigos atingidos. |
| Cometa Sepulcral | Dano alto em um alvo e dano leve aos adjacentes. | Dano alto aumentado pela curva Fibonacci e maior dano adjacente. | Dano alto aumentado pela curva Fibonacci, dano adjacente e aplica Vulnerable no alvo principal. |

## Perfil de gameplay

O Arcanista é frágil, mas resolve grupos e controla ameaças. Precisa ser protegido por Guardião ou Acólita.

---

# 2.5 Bram Halvek — Caçador de Relíquias

```text
Classe: Caçador de Relíquias
ID sugerido: relic_hunter
Personagem base: Bram Halvek
Função: dano à distância, utilidade, loot
Ranks ideais: 2-4
Dificuldade: média
Disponibilidade sugerida: desbloqueável após o andar 5
```

## Equipamentos permitidos

```text
Armas:
- Besta leve
- Besta pesada
- Pistola antiga
- Adaga

Defesa:
- Armadura leve
- Casaco reforçado

Acessórios:
- Bolsa de ferramentas
- Amuleto de sorte
- Lente antiga
- Mapa rasgado
```

## Skills da classe

Skills iniciais:

```text
- Disparo Preciso
- Inspecionar Fraqueza
```

| Skill | ID sugerido | Tipo | Inicial | Aprendizado sugerido | Ranks de uso | Alvos válidos |
|---|---|---|---|---|---|---|
| Disparo Preciso | `skill.relic_hunter.precise_shot` | Ofensiva | Sim | Inicial | 2-4 | Inimigos 2-4 |
| Inspecionar Fraqueza | `skill.relic_hunter.inspect_weakness` | Debuff | Sim | Inicial | 2-4 | Inimigos 1-4 |
| Tiro de Cobertura | `skill.relic_hunter.covering_shot` | Ofensiva/Buff | Não | Resting site após nível 2 | 2-4 | Inimigo 1-3 + aliado único |
| Armadilha de Ossos | `skill.relic_hunter.bone_trap` | Debuff | Não | Resting site após nível 4 | 2-4 | Inimigo 1 |
| Flecha Perfurante | `skill.relic_hunter.piercing_bolt` | Ofensiva | Não | Resting site após nível 6 | 2-4 | Inimigos alinhados |
| Relíquia Instável | `skill.relic_hunter.unstable_relic` | Ofensiva/Debuff | Não | Resting site após nível 8 | 2-4 | Inimigos 1-4 |

## Efeitos por nível de skill

| Skill | Nível 1 | Nível 2 | Nível 3 |
|---|---|---|---|
| Disparo Preciso | Dano médio contra backline. | Dano médio aumentado pela curva Fibonacci e bônus de Accuracy. | Dano alto pela curva Fibonacci contra backline e chance de crítico aumentada. |
| Inspecionar Fraqueza | Reduz Defense do alvo por 2 turnos. | Reduz Defense e Evasion por 2 turnos. | Reduz Defense, Evasion e revela o alvo, aumentando chance de crítico contra ele. |
| Tiro de Cobertura | Dano leve e aumenta Evasion de um aliado por 1 turno. | Dano leve aumentado pela curva Fibonacci e Evasion por 2 turnos. | Dano médio pela curva Fibonacci, Evasion por 2 turnos e remove Mark do aliado. |
| Armadilha de Ossos | Aplica Slow no inimigo frontal. | Aplica Slow e chance de stun. | Aplica Slow, chance maior de stun e Vulnerable se o alvo estiver no rank 1. |
| Flecha Perfurante | Dano médio dividido entre dois inimigos alinhados. | Dano aumentado pela curva Fibonacci em dois inimigos alinhados. | Dano aumentado pela curva Fibonacci, ignora parte da Defense e aplica Bleed leve. |
| Relíquia Instável | Dano alto aleatório em um alvo e chance de Burn ou Stun. | Dano alto aumentado pela curva Fibonacci e chance maior de efeito aleatório. | Dano alto aumentado pela curva Fibonacci, aplica efeito aleatório garantido e pode atingir adjacente com dano leve. |

## Perfil de gameplay

O Caçador de Relíquias melhora o controle tático da run e adiciona utilidade. É útil como recompensa/desbloqueio após o primeiro boss.

---

# 2.6 Ilyra Vey — Exorcista Errante

```text
Classe: Exorcista Errante
ID sugerido: exorcist
Personagem base: Ilyra Vey
Função: anti-morto-vivo, debuff, dano sagrado
Ranks ideais: 2-3
Dificuldade: média
Disponibilidade sugerida: desbloqueável após o andar 10
```

## Equipamentos permitidos

```text
Armas:
- Florete
- Chicote
- Sino ritual
- Lâmina consagrada

Defesa:
- Armadura leve
- Armadura média

Acessórios:
- Relíquia sagrada
- Medalhão de prata
- Selo de exorcismo
- Livro de ritos
```

## Skills da classe

Skills iniciais:

```text
- Golpe Consagrado
- Repreensão
```

| Skill | ID sugerido | Tipo | Inicial | Aprendizado sugerido | Ranks de uso | Alvos válidos |
|---|---|---|---|---|---|---|
| Golpe Consagrado | `skill.exorcist.consecrated_strike` | Ofensiva | Sim | Inicial | 1-3 | Inimigos 1-2 |
| Repreensão | `skill.exorcist.rebuke` | Debuff | Sim | Inicial | 2-3 | Inimigos 1-4 |
| Selo de Banimento | `skill.exorcist.banishment_seal` | Debuff | Não | Resting site após nível 2 | 2-3 | Inimigos 1-4 |
| Chicote de Prata | `skill.exorcist.silver_whip` | Ofensiva | Não | Resting site após nível 4 | 2-3 | Inimigos 1-3 |
| Rito de Proteção | `skill.exorcist.rite_of_protection` | Defensiva/Buff | Não | Resting site após nível 6 | 2-3 | Aliado único |
| Juízo Final | `skill.exorcist.final_judgment` | Ofensiva/Debuff | Não | Resting site após nível 8 | 2-3 | Todos os inimigos |

## Efeitos por nível de skill

| Skill | Nível 1 | Nível 2 | Nível 3 |
|---|---|---|---|
| Golpe Consagrado | Dano médio sagrado; bônus contra undead. | Dano médio aumentado pela curva Fibonacci; bônus maior contra undead. | Dano alto pela curva Fibonacci contra undead e aplica Weak por 1 turno. |
| Repreensão | Reduz Attack do alvo por 2 turnos. | Reduz Attack e Accuracy por 2 turnos. | Reduz Attack, Accuracy e Critical por 2 turnos. |
| Selo de Banimento | Alvo recebe mais dano sagrado por 2 turnos. | Alvo recebe mais dano sagrado e perde Resistance por 2 turnos. | Alvo recebe mais dano sagrado, perde Resistance e não pode receber buff por 1 turno. |
| Chicote de Prata | Dano médio contra ranks 1-3; bônus contra undead. | Dano médio aumentado pela curva Fibonacci e aplica Bleed leve contra não-undead. | Dano alto pela curva Fibonacci, bônus contra undead e aplica Mark. |
| Rito de Proteção | Protege um aliado contra o próximo debuff. | Protege contra o próximo debuff e aumenta Resistance por 2 turnos. | Protege contra até 2 debuffs, aumenta Resistance e remove Weak. |
| Juízo Final | Dano leve em todos os inimigos; dano maior contra undead. | Dano em área aumentado pela curva Fibonacci e aplica Weak em undead. | Dano em área aumentado pela curva Fibonacci, aplica Weak em undead e reduz Resistance dos inimigos atingidos. |

## Perfil de gameplay

A Exorcista é particularmente forte no primeiro bioma, mas deve ser menos dominante em biomas futuros para evitar dependência excessiva de uma classe.

---
## 3. Resumo das classes jogáveis

| Personagem base | Classe | ID | Função | Ranks ideais | Disponibilidade |
|---|---|---|---|---|---|
| Aldren Voss | Guardião Sepulcral | guardian | Tanque/proteção | 1-2 | Inicial |
| Neria Vale | Lâmina Velada | rogue | Dano/mobilidade | 2-3 | Inicial |
| Mirella Thorne | Acólita da Vela | acolyte | Cura/suporte | 3-4 | Inicial |
| Cael Morvain | Arcanista do Túmulo | arcanist | Dano mágico/controle | 3-4 | Inicial |
| Bram Halvek | Caçador de Relíquias | relic_hunter | Dano/utilidade/loot | 2-4 | Após andar 5 |
| Ilyra Vey | Exorcista Errante | exorcist | Anti-morto-vivo/debuff | 2-3 | Após andar 10 |

---

## 4. Nomes alternativos para heróis contratáveis

Use essas pools para gerar variações de personagens por classe sem criar uma classe nova.

### Guardião Sepulcral

```text
Aldren Voss
Garrick Holt
Darian Kross
Baldric Mourn
Roderic Vale
```

### Lâmina Velada

```text
Neria Vale
Sylas Crowe
Vexa Norr
Kael Draven
Liora Shade
```

### Acólita da Vela

```text
Mirella Thorne
Elianora Voss
Serah Wren
Daliah Mourn
Aveline Cross
```

### Arcanista do Túmulo

```text
Cael Morvain
Orin Veyr
Malrec Sable
Eldric Noct
Vaelis Thorne
```

### Caçador de Relíquias

```text
Bram Halvek
Tobin Marr
Cedric Flint
Rowan Locke
Osric Vale
```

### Exorcista Errante

```text
Ilyra Vey
Cassian Morn
Helena Dusk
Varric Sol
Selene Ashford
```

---

## 5. Inimigos comuns — Cripta Esquecida

# 5.1 Ossário Rastejante

```text
Nome: Ossário Rastejante
ID sugerido: crawling_boneheap
Tipo: morto-vivo
Função: inimigo fraco, tutorial
Ranks ocupados: 1-2
Andares sugeridos: 1-4
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Arranhão Ósseo | Ataque | Dano leve em um herói no rank 1. |
| Recompor Ossos | Cura própria | Recupera pequena quantidade de HP se não for derrotado rapidamente. |

---

# 5.2 Esqueleto Recruta

```text
Nome: Esqueleto Recruta
ID sugerido: skeleton_grunt
Tipo: morto-vivo
Função: frontline básico
Ranks ocupados: 1-2
Andares sugeridos: 1-8
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Espadada Enferrujada | Ataque | Dano físico leve/médio. |
| Guarda Quebrada | Defesa | Aumenta defesa própria por 1 turno. |

---

# 5.3 Arqueiro da Cripta

```text
Nome: Arqueiro da Cripta
ID sugerido: crypt_archer
Tipo: morto-vivo
Função: backline ranged
Ranks ocupados: 3-4
Andares sugeridos: 2-10
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Flecha Partida | Ataque à distância | Dano contra ranks 2-4 da party. |
| Mirar nos Fracos | Ataque tático | Prioriza aliado com menor HP percentual. |

---

# 5.4 Rato Cadavérico

```text
Nome: Rato Cadavérico
ID sugerido: corpse_rat
Tipo: fera/corrompido
Função: rápido, poison
Ranks ocupados: 1-3
Andares sugeridos: 2-8
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Mordida Infectada | Ataque/status | Dano leve e chance de poison. |
| Fugir pelas Sombras | Defesa | Aumenta evasão por 1 turno. |

---

# 5.5 Acólito Sepulcral

```text
Nome: Acólito Sepulcral
ID sugerido: grave_acolyte
Tipo: cultista
Função: suporte inimigo
Ranks ocupados: 3-4
Andares sugeridos: 4-14
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Cântico Fúnebre | Buff | Aumenta ataque de um aliado inimigo. |
| Toque Profano | Ataque/debuff | Dano mágico leve e redução de defesa. |
| Sutura Negra | Cura | Cura pequena em um morto-vivo aliado. |

---

# 5.6 Guarda de Ossos

```text
Nome: Guarda de Ossos
ID sugerido: bone_guard
Tipo: morto-vivo
Função: tanque inimigo
Ranks ocupados: 1-2
Andares sugeridos: 5-15
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Escudo de Costelas | Defesa | Aumenta defesa própria. |
| Maçada Pesada | Ataque | Dano médio no rank 1 da party. |
| Interpor-se | Proteção | Protege um inimigo de backline por 1 turno. |

---

# 5.7 Cão Amaldiçoado

```text
Nome: Cão Amaldiçoado
ID sugerido: cursed_hound
Tipo: fera/corrompido
Função: agressor rápido
Ranks ocupados: 1-3
Andares sugeridos: 6-16
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Dilacerar | Ataque/status | Dano físico com chance de bleed. |
| Uivo Profano | Debuff | Reduz precisão ou ataque da party por 1 turno. |

---

# 5.8 Portador da Peste

```text
Nome: Portador da Peste
ID sugerido: plague_carrier
Tipo: morto-vivo/corrompido
Função: poison e dano contínuo
Ranks ocupados: 2-4
Andares sugeridos: 7-18
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Cuspe Pestilento | Status | Aplica poison em um alvo. |
| Nuvem Tóxica | Área/status | Chance de poison em dois heróis aleatórios. |
| Corpo Instável | Passiva | Ao morrer, pequena chance de aplicar poison no atacante. |

---

# 5.9 Cultista Esquecido

```text
Nome: Cultista Esquecido
ID sugerido: forgotten_cultist
Tipo: cultista
Função: dano mágico/debuff
Ranks ocupados: 2-4
Andares sugeridos: 8-20
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Maldição Menor | Debuff | Reduz defesa ou velocidade de um herói. |
| Lança Sombria | Ataque mágico | Dano mágico contra ranks 2-4. |
| Rito de Sangue | Buff/sacrifício | Sacrifica HP próprio para aumentar dano de aliados. |

---

# 5.10 Cavaleiro Sem Túmulo

```text
Nome: Cavaleiro Sem Túmulo
ID sugerido: tomb_knight
Tipo: morto-vivo/elite
Função: elite frontline
Ranks ocupados: 1-2
Andares sugeridos: 12-20
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Corte Funerário | Ataque | Dano médio/alto no rank 1. |
| Presença Aterradora | Debuff | Reduz ataque da party por 1 turno. |
| Armadura Profanada | Passiva | Ganha defesa extra quando abaixo de 50% de HP. |

---

# 5.11 Monge das Catacumbas

```text
Nome: Monge das Catacumbas
ID sugerido: catacomb_monk
Tipo: cultista
Função: controle
Ranks ocupados: 2-4
Andares sugeridos: 10-20
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Sussurro Hipnótico | Controle | Chance de stun em um herói. |
| Meditação Profana | Cura/buff | Cura leve e aumenta resistência própria. |
| Olhar Vazio | Debuff | Reduz velocidade de um alvo. |

---

# 5.12 Aparição de Cinzas

```text
Nome: Aparição de Cinzas
ID sugerido: ash_wraith
Tipo: espectro
Função: evasão, dano mágico
Ranks ocupados: 2-4
Andares sugeridos: 14-20
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Toque Etéreo | Ataque mágico | Dano mágico que ignora parte da defesa. |
| Desaparecer | Defesa | Aumenta evasão por 1 turno. |
| Lamento Cinzento | Área | Dano leve em toda a party. |

---

## 6. Bosses — Cripta Esquecida

# 6.1 Andar 5 — Capitão dos Ossos

```text
Nome: Capitão dos Ossos
ID sugerido: bone_captain
Tipo: morto-vivo/boss
Função: primeiro teste de frontline
Ranks ocupados: 1-2
Andar: 5
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Comando Rachado | Buff | Aumenta ataque dos esqueletos aliados. |
| Golpe do Capitão | Ataque | Dano médio/alto no rank 1. |
| Erguer Recruta | Summon | Invoca um Esqueleto Recruta se houver slot livre. |
| Defesa Militar | Defesa | Ganha defesa por 2 turnos. |

## Identidade do boss

Primeiro boss do jogo. Ensina que o jogador precisa lidar com invocações e controlar a frontline.

---

# 6.2 Andar 10 — Sacerdote da Cova

```text
Nome: Sacerdote da Cova
ID sugerido: grave_priest
Tipo: cultista/boss
Função: suporte, debuff, sustain
Ranks ocupados: 3-4
Andar: 10
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Liturgia dos Mortos | Cura | Cura todos os mortos-vivos aliados. |
| Maldição da Terra Fria | Debuff | Reduz velocidade e defesa da party. |
| Sangue pelo Túmulo | Sacrifício/cura | Sacrifica aliado para recuperar HP. |
| Chamado da Cova | Summon | Invoca Ossário Rastejante ou Esqueleto Recruta. |

## Identidade do boss

Primeiro boss antes de resting site. Testa foco em alvo prioritário e capacidade de atingir backline.

---

# 6.3 Andar 15 — Abominação da Cripta

```text
Nome: Abominação da Cripta
ID sugerido: crypt_abomination
Tipo: morto-vivo/corrompido/boss
Função: dano bruto, poison, pressão de HP
Ranks ocupados: 1-2
Andar: 15
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Massa Esmagadora | Ataque | Dano alto no rank 1. |
| Jorro Pestilento | Status | Aplica poison em dois heróis. |
| Carne Remendada | Cura própria | Recupera HP moderado uma vez por combate. |
| Fúria Cadavérica | Buff condicional | Ganha ataque quando abaixo de 40% de HP. |

## Identidade do boss

Boss de desgaste. Testa cura, purificação e capacidade de finalizar rapidamente quando entra em fase perigosa.

---

# 6.4 Andar 20 — O Carcereiro Esquecido

```text
Nome: O Carcereiro Esquecido
ID sugerido: forgotten_warden
Tipo: espectro/boss final do bioma
Função: controle, dano mágico, mudança de fase
Ranks ocupados: 2-4
Andar: 20
```

| Habilidade | Tipo | Descrição |
|---|---|---|
| Correntes da Cripta | Controle | Chance de stun em dois heróis. |
| Sentença Silenciosa | Ataque mágico | Dano mágico alto em alvo marcado. |
| Abrir Cela | Summon | Invoca uma Aparição de Cinzas. |
| Julgamento dos Mortos | Área | Dano em área quando abaixo de 50% de HP. |
| Portão do Próximo Abismo | Progressão | Ao morrer, libera transição para o próximo tema da dungeon. |

## Identidade do boss

Finaliza o primeiro bloco de 20 andares. Deve parecer um marco de progressão, com controle, dano em área e invocação.

---

## 7. Categorias técnicas de inimigos

```text
Undead:
- crawling_boneheap
- skeleton_grunt
- crypt_archer
- bone_guard
- tomb_knight

Cultist:
- grave_acolyte
- forgotten_cultist
- catacomb_monk

Beast/Corrupted:
- corpse_rat
- cursed_hound
- plague_carrier

Specter:
- ash_wraith

Boss:
- bone_captain
- grave_priest
- crypt_abomination
- forgotten_warden
```

---

## 8. Composições de encontros sugeridas

# Andares 1-4

```text
Encontro 1:
- Ossário Rastejante
- Esqueleto Recruta

Encontro 2:
- Esqueleto Recruta
- Arqueiro da Cripta

Encontro 3:
- Rato Cadavérico
- Ossário Rastejante
```

# Andar 5 — Boss

```text
- Capitão dos Ossos
- Esqueleto Recruta
```

# Andares 6-9

```text
Encontro 1:
- Guarda de Ossos
- Arqueiro da Cripta

Encontro 2:
- Rato Cadavérico
- Cão Amaldiçoado
- Acólito Sepulcral

Encontro 3:
- Esqueleto Recruta
- Guarda de Ossos
- Acólito Sepulcral
```

# Andar 10 — Boss + resting site

```text
- Sacerdote da Cova
- Guarda de Ossos
- Ossário Rastejante
```

# Andares 11-14

```text
Encontro 1:
- Portador da Peste
- Cão Amaldiçoado

Encontro 2:
- Cultista Esquecido
- Guarda de Ossos
- Arqueiro da Cripta

Encontro 3:
- Monge das Catacumbas
- Esqueleto Recruta
- Rato Cadavérico
```

# Andar 15 — Boss

```text
- Abominação da Cripta
- Portador da Peste
```

# Andares 16-19

```text
Encontro 1:
- Cavaleiro Sem Túmulo
- Cultista Esquecido

Encontro 2:
- Aparição de Cinzas
- Monge das Catacumbas
- Cão Amaldiçoado

Encontro 3:
- Cavaleiro Sem Túmulo
- Portador da Peste
- Arqueiro da Cripta
```

# Andar 20 — Boss + resting site + transição de tema

```text
- O Carcereiro Esquecido
- Aparição de Cinzas
```

---

## 9. Ordem sugerida de desbloqueio

```text
Início do jogo:
- Guardião Sepulcral
- Lâmina Velada
- Acólita da Vela
- Arcanista do Túmulo

Após vencer o andar 5:
- Caçador de Relíquias disponível para contratação

Após vencer o andar 10:
- Exorcista Errante disponível para contratação

Após vencer o andar 20:
- Próximo tema da dungeon desbloqueado
```

---

## 10. Conjunto mínimo para MVP

Se for necessário reduzir escopo, implementar apenas este conjunto primeiro.

### Classes jogáveis MVP

```text
1. Guardião Sepulcral
2. Lâmina Velada
3. Acólita da Vela
4. Arcanista do Túmulo
```

### Inimigos comuns MVP

```text
1. Ossário Rastejante
2. Esqueleto Recruta
3. Arqueiro da Cripta
4. Rato Cadavérico
5. Acólito Sepulcral
6. Guarda de Ossos
7. Portador da Peste
8. Cultista Esquecido
```

### Bosses MVP

```text
1. Capitão dos Ossos
2. Sacerdote da Cova
3. Abominação da Cripta
4. O Carcereiro Esquecido
```

---

## 11. Notas de implementação para Codex

### Classes e personagens

- `HeroClassDefinition` deve representar a classe.
- `HeroInstance` deve representar um personagem contratado pelo jogador.
- O nome do personagem deve ser parte da instância, não da classe.
- A classe define skills possíveis, equipamentos permitidos, stats base e ranks ideais.
- A instância define nível, XP, skills aprendidas, equipamentos atuais, HP atual e modificadores temporários.

### Habilidades

- Cada habilidade deve virar um `SkillDefinition`.
- Usar IDs estáveis em snake_case.
- Evitar acoplar texto de UI diretamente à lógica de efeito.
- Preferir efeitos composáveis: damage, heal, buff, debuff, status, summon, move, guard, taunt.

### Inimigos

- Inimigos comuns podem usar `EnemyDefinition`.
- Bosses podem usar `BossDefinition` herdando ou compondo `EnemyDefinition`, mas evitar herança profunda.
- Habilidades de bosses devem ser configuráveis por dados sempre que possível.

### Encontros

- `EncounterDefinition` deve apontar para IDs ou references de enemy definitions.
- Encounter tables devem usar pesos.
- Floor generator decide se o andar é comum, boss, resting site ou theme transition.
- Encounter generator escolhe o encontro adequado para tema + andar + tipo de floor.

---

## 12. Exemplo de estrutura data-driven

```json
{
  "heroClassId": "guardian",
  "displayName": "Guardião Sepulcral",
  "idealRanks": [1, 2],
  "allowedWeaponTypes": ["short_sword", "long_sword", "mace", "warhammer"],
  "allowedArmorTypes": ["medium_armor", "heavy_armor", "light_shield", "heavy_shield"],
  "skillsByLevel": [
    { "level": 1, "skillId": "guard_strike" },
    { "level": 1, "skillId": "iron_stance" },
    { "level": 2, "skillId": "shield_charge" },
    { "level": 4, "skillId": "protect_ally" },
    { "level": 6, "skillId": "crypt_oath" },
    { "level": 8, "skillId": "bone_breaker" },
    { "level": 10, "skillId": "last_wall" }
  ]
}
```

```json
{
  "enemyId": "skeleton_grunt",
  "displayName": "Esqueleto Recruta",
  "enemyType": "undead",
  "preferredRanks": [1, 2],
  "floorRange": { "min": 1, "max": 8 },
  "skills": [
    "rusted_slash",
    "broken_guard"
  ]
}
```

