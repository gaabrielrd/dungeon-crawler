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

## 2. Classes jogáveis

Para o MVP, implementar primeiro as quatro classes principais:

1. Guardião Sepulcral
2. Lâmina Velada
3. Acólita da Vela
4. Arcanista do Túmulo

As classes Caçador de Relíquias e Exorcista Errante podem entrar como desbloqueáveis após os primeiros bosses.

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

## Habilidades até o nível 10

| Nível | Habilidade | Tipo | Ranks de uso | Alvos válidos | Descrição |
|---:|---|---|---|---|---|
| 1 | Golpe de Guarda | Ataque | 1-2 | Inimigos 1-2 | Ataque básico físico. |
| 1 | Postura de Ferro | Defesa | 1-2 | Self | Aumenta defesa própria por 2 turnos. |
| 2 | Investida de Escudo | Ataque/controle | 1-2 | Inimigos 1-2 | Dano leve e chance de stun. |
| 4 | Proteger Aliado | Suporte | 1-2 | Aliados 2-4 | Redireciona parte do dano recebido por um aliado para si por 2 turnos. |
| 6 | Juramento da Cripta | Defesa/taunt | 1-2 | Self | Aumenta defesa e força inimigos a priorizá-lo por 1 turno. |
| 8 | Quebra-Ossos | Ataque/debuff | 1-2 | Inimigos 1-2 | Dano médio e redução de defesa do alvo. |
| 10 | Última Muralha | Ultimate defensiva | 1-2 | Todos os aliados | Concede escudo temporário para toda a party e grande defesa para si. |

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

## Habilidades até o nível 10

| Nível | Habilidade | Tipo | Ranks de uso | Alvos válidos | Descrição |
|---:|---|---|---|---|---|
| 1 | Corte Rápido | Ataque | 1-3 | Inimigos 1-2 | Ataque físico rápido. |
| 1 | Passo Sombrio | Mobilidade | 1-3 | Self | Move-se 1 rank para trás e ganha evasão por 1 turno. |
| 2 | Punhalada Exposta | Ataque | 1-3 | Inimigos 1-3 | Causa dano bônus contra alvos com debuff. |
| 4 | Lâmina Envenenada | Ataque/status | 1-3 | Inimigos 1-2 | Dano leve e aplica poison por 3 turnos. |
| 6 | Execução Silenciosa | Ataque | 1-3 | Inimigos 1-3 | Dano alto contra inimigos com menos de 35% de HP. |
| 8 | Dança das Facas | Área | 2-3 | Inimigos 1-3 | Ataca dois inimigos aleatórios. |
| 10 | Marca da Morte | Debuff/execução | 2-3 | Inimigos 1-4 | Marca um alvo; ataques contra ele causam dano aumentado por 2 turnos. |

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

## Habilidades até o nível 10

| Nível | Habilidade | Tipo | Ranks de uso | Alvos válidos | Descrição |
|---:|---|---|---|---|---|
| 1 | Prece Menor | Cura | 3-4 | Aliado único | Cura leve em um aliado. |
| 1 | Luz da Vela | Ataque sagrado | 3-4 | Inimigos 1-3 | Dano leve; bônus contra mortos-vivos. |
| 2 | Bênção Frágil | Suporte | 3-4 | Aliado único | Aumenta defesa de um aliado por 2 turnos. |
| 4 | Purificar Feridas | Cura/status | 3-4 | Aliado único | Cura leve e remove poison ou bleed. |
| 6 | Círculo de Cinzas | Defesa de grupo | 3-4 | Todos os aliados | Reduz dano recebido por toda a party por 1 turno. |
| 8 | Voto de Sacrifício | Cura avançada | 3-4 | Aliado único | Cura alta em um aliado, mas causa pequeno dano à própria Acólita. |
| 10 | Chama Consagrada | Ultimate suporte | 3-4 | Todos os aliados | Cura moderada toda a party e remove 1 debuff de cada aliado. |

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

## Habilidades até o nível 10

| Nível | Habilidade | Tipo | Ranks de uso | Alvos válidos | Descrição |
|---:|---|---|---|---|---|
| 1 | Faísca Profana | Ataque mágico | 3-4 | Inimigos 1-4 | Dano mágico contra um alvo. |
| 1 | Selo de Lentidão | Debuff | 3-4 | Inimigos 1-4 | Reduz velocidade do alvo por 2 turnos. |
| 2 | Estilhaço Arcano | Ataque mágico | 3-4 | Inimigos 2-4 | Dano médio contra backline. |
| 4 | Névoa do Túmulo | Controle | 3-4 | Todos os inimigos | Reduz precisão dos inimigos por 1 turno. |
| 6 | Explosão Rúnica | Área | 3-4 | Todos os inimigos | Dano leve em todos os inimigos. |
| 8 | Correntes Etéreas | Controle | 3-4 | Inimigos 1-3 | Chance de stun em um inimigo. |
| 10 | Cometa Sepulcral | Ultimate ofensiva | 3-4 | Inimigo único + adjacentes | Dano alto em um alvo e dano leve aos adjacentes. |

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

## Habilidades até o nível 10

| Nível | Habilidade | Tipo | Ranks de uso | Alvos válidos | Descrição |
|---:|---|---|---|---|---|
| 1 | Disparo Preciso | Ataque à distância | 2-4 | Inimigos 2-4 | Dano contra backline. |
| 1 | Inspecionar Fraqueza | Debuff | 2-4 | Inimigos 1-4 | Reduz defesa do alvo. |
| 2 | Tiro de Cobertura | Ataque/suporte | 2-4 | Inimigo 1-3 + aliado | Dano leve e aumenta evasão de um aliado. |
| 4 | Armadilha de Ossos | Controle | 2-4 | Inimigo 1 | Chance de stun ou slow no alvo frontal. |
| 6 | Saqueador Experiente | Passiva | Qualquer | Pós-combate | Aumenta levemente gold recebido após combate. |
| 8 | Flecha Perfurante | Ataque | 2-4 | Inimigos alinhados | Atinge dois inimigos alinhados. |
| 10 | Relíquia Instável | Ataque especial | 2-4 | Inimigos 1-4 | Dano alto aleatório com chance de burn ou stun. |

## Perfil de gameplay

O Caçador de Relíquias melhora o retorno econômico da run e adiciona utilidade. É útil como recompensa/desbloqueio após o primeiro boss.

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

## Habilidades até o nível 10

| Nível | Habilidade | Tipo | Ranks de uso | Alvos válidos | Descrição |
|---:|---|---|---|---|---|
| 1 | Golpe Consagrado | Ataque | 1-3 | Inimigos 1-2 | Dano físico/sagrado. |
| 1 | Repreensão | Debuff | 2-3 | Inimigos 1-4 | Reduz ataque de um inimigo por 2 turnos. |
| 2 | Selo de Banimento | Debuff | 2-3 | Inimigos 1-4 | Inimigo recebe mais dano sagrado por 2 turnos. |
| 4 | Chicote de Prata | Ataque | 2-3 | Inimigos 1-3 | Dano médio; bônus contra mortos-vivos. |
| 6 | Rito de Proteção | Suporte | 2-3 | Aliado único | Protege um aliado contra o próximo debuff. |
| 8 | Expulsar Corrupção | Ataque/status | 2-3 | Inimigos 1-4 | Remove buff de inimigo e causa dano sagrado. |
| 10 | Juízo Final | Ultimate | 2-3 | Todos os inimigos | Dano alto contra mortos-vivos; dano moderado contra outros tipos. |

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

