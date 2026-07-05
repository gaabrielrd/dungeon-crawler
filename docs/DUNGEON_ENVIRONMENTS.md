# Dungeon Environments

Documento de referência para os ambientes, biomas, temas visuais, progressão mecânica, inimigos, bosses, eventos e recompensas da dungeon principal do jogo.

Este arquivo deve servir como base para criação de `DungeonThemeDefinition`, `EncounterTableDefinition`, `EncounterDefinition`, `BossDefinition`, loot tables, eventos específicos de bioma, backgrounds, tilesets, músicas e variações visuais no Unity.

> Observação: o nome do arquivo segue o solicitado: `DUNGEON_ENVINROMENTS.md`.

---

## 1. Visão geral

A dungeon principal é organizada em **5 temas de 20 andares**, cobrindo a progressão inicial até o andar 100.

Cada tema deve introduzir uma identidade visual clara, um conjunto próprio de inimigos, uma pressão mecânica dominante e uma curva de dificuldade compatível com a progressão do jogador.

```text
Andares 1-20   -> Cripta Esquecida
Andares 21-40  -> Profundezas Fúngicas
Andares 41-60  -> Minas de Brasa
Andares 61-80  -> Templo Submerso
Andares 81-100 -> Abismo Astral
```

---

## 2. Regras globais de progressão

### 2.1 Marcos de andar

Cada bloco de 20 andares segue a mesma estrutura:

```text
Andar X+5  -> boss intermediário
Andar X+10 -> boss + safe resting site
Andar X+15 -> boss intermediário
Andar X+20 -> boss final do tema + safe resting site + transição de tema
```

Exemplo no primeiro tema:

```text
Andar 5  -> boss intermediário
Andar 10 -> boss + safe resting site
Andar 15 -> boss intermediário
Andar 20 -> boss final + safe resting site + transição para o próximo tema
```

### 2.2 Estrutura recomendada por tema

Cada tema deve conter:

```text
1. DungeonThemeDefinition
2. Paleta visual própria
3. Background principal
4. Tileset ou kit visual do ambiente
5. Música de exploração/combat theme
6. 8 a 12 inimigos comuns
7. 2 a 4 inimigos elite
8. 4 bosses
9. Encounter tables por faixa de andar
10. Loot table temática
11. Eventos específicos
12. Shop modifiers do resting site
13. Recompensas ou materiais únicos
```

### 2.3 Progressão mecânica entre temas

| Tema | Mecânica dominante | Pressão principal no jogador |
|---|---|---|
| Cripta Esquecida | Fundamentos, mortos-vivos, frontline/backline | Aprender prioridade de alvo |
| Profundezas Fúngicas | Poison, regeneração, explosões ao morrer | Controlar desgaste |
| Minas de Brasa | Burn, defesa alta, dano carregado | Finalizar ameaças antes do burst |
| Templo Submerso | Slow, push/pull, accuracy down | Manter formação e resposta tática |
| Abismo Astral | Mark, stun, distorção de ranks | Dominar composição e timing |

---

# 3. Tema 1 — Cripta Esquecida

```text
Nome: Cripta Esquecida
ID sugerido: forgotten_crypt
Andares: 1-20
Função na progressão: tutorial sombrio e introdução aos fundamentos
```

## 3.1 Conceito

Catacumbas antigas sob ruínas esquecidas, ossários, corredores estreitos, cultistas, mortos-vivos, relíquias profanadas, altares funerários e runas antigas.

Este é o primeiro contato do jogador com a dungeon. Deve ensinar a leitura de ranks, prioridade de alvo, proteção da backline, uso de cura e resposta a status simples.

## 3.2 Direção visual

```text
Tom visual: pixel art sombrio, baixa saturação, contraste alto, luz de velas, pedra fria, ossos e runas antigas.
```

### Paleta sugerida

```text
- Cinza frio
- Azul escuro
- Verde apagado
- Roxo escuro
- Dourado envelhecido
- Vermelho seco para sangue antigo
```

### Elementos visuais

```text
- Ossários
- Sarcófagos quebrados
- Velas derretidas
- Estandartes rasgados
- Runas azuis ou verdes fracas
- Pilhas de ossos
- Portões de ferro
- Estátuas funerárias
- Livros profanados
```

## 3.3 Mecânicas principais

```text
- Ataques físicos simples
- Poison leve
- Stun ocasional
- Frontline protegendo backline
- Primeiros summons de esqueletos
- Debuffs simples de defesa e velocidade
```

## 3.4 Inimigos comuns sugeridos

```text
1. Ossário Rastejante       -> crawling_boneheap
2. Esqueleto Recruta        -> skeleton_grunt
3. Arqueiro da Cripta       -> crypt_archer
4. Rato Cadavérico          -> corpse_rat
5. Acólito Sepulcral        -> grave_acolyte
6. Guarda de Ossos          -> bone_guard
7. Portador da Peste        -> plague_carrier
8. Cultista Esquecido       -> forgotten_cultist
9. Cavaleiro Sem Túmulo     -> tomb_knight
10. Monge das Catacumbas    -> catacomb_monk
11. Aparição de Cinzas      -> ash_wraith
```

## 3.5 Bosses

| Andar | Boss | ID sugerido | Identidade mecânica |
|---:|---|---|---|
| 5 | Capitão dos Ossos | `bone_captain` | Summon e controle de frontline |
| 10 | Sacerdote da Cova | `grave_priest` | Cura, debuff e backline prioritária |
| 15 | Abominação da Cripta | `crypt_abomination` | Dano bruto, poison e pressão de HP |
| 20 | O Carcereiro Esquecido | `forgotten_warden` | Controle, dano mágico, summon e transição de tema |

## 3.6 Eventos específicos

```text
Altar Profanado:
- Purificar para remover debuff da party.
- Saquear para ganhar gold, mas aplicar mark no próximo combate.

Sarcófago Selado:
- Abrir para chance de equipamento.
- Ignorar para evitar emboscada.

Fonte de Cinzas:
- Beber para recuperar HP.
- Pode aplicar weak ou poison se falhar.

Livro Funerário:
- Ler para ganhar buff arcano temporário.
- Pode invocar cultistas.
```

## 3.7 Loot temático

```text
- Ossos rúnicos
- Medalhões quebrados
- Lâminas enferrujadas
- Grimórios funerários
- Relíquias profanadas
- Escudos de cripta
- Pó de ossário
- Velas consagradas
```

## 3.8 Música e áudio

```text
Música: lenta, cavernosa, com drones graves, sinos distantes e percussão leve.
SFX: ossos rangendo, correntes, velas, passos em pedra, sussurros, portas de ferro.
```

---

# 4. Tema 2 — Profundezas Fúngicas

```text
Nome: Profundezas Fúngicas
ID sugerido: fungal_depths
Andares: 21-40
Função na progressão: testar resistência, sustain e gerenciamento de poison
```

## 4.1 Conceito

Cavernas úmidas tomadas por fungos gigantes, raízes pulsantes, cadáveres colonizados por micélio, colônias de esporos e druidas corrompidos por uma inteligência subterrânea.

Este tema deve ser o primeiro aumento real de complexidade após a cripta. O jogador já conhece ataque, cura, defesa e targeting; agora precisa gerenciar desgaste contínuo, poison, regeneração inimiga e mortes perigosas.

## 4.2 Direção visual

```text
Tom visual: caverna orgânica, úmida, tóxica, claustrofóbica e bioluminescente.
```

### Paleta sugerida

```text
- Verdes tóxicos
- Roxos escuros
- Marrons úmidos
- Amarelo pálido de esporos
- Azul petróleo para profundidade
- Branco leitoso para fungos luminosos
```

### Elementos visuais

```text
- Cogumelos gigantes
- Esporos flutuantes
- Micélio nas paredes
- Cadáveres cobertos por fungos
- Lagoas tóxicas
- Raízes pulsantes
- Cascas de insetos
- Brilho bioluminescente
```

## 4.3 Mecânicas principais

```text
- Poison frequente
- Regeneração inimiga
- Explosões de esporos ao morrer
- Cura e shield em inimigos
- Debuffs de accuracy e speed
- Inimigos que se multiplicam ou crescem se ignorados
```

## 4.4 Inimigos comuns sugeridos

| Inimigo | ID sugerido | Função |
|---|---|---|
| Cogumelo Esporífero | `spore_cap` | Frágil, aplica poison quando morre |
| Carniçal Micótico | `fungal_ghoul` | Morto-vivo colonizado, regenera HP |
| Besouro de Casca Verde | `green_shell_beetle` | Tanque pequeno com alta defesa |
| Tecelão de Micélio | `mycelium_weaver` | Suporte que cura ou aplica shield |
| Druida Corrompido | `corrupted_druid` | Backline mágico, poison e slow |
| Colônia Viva | `living_colony` | Invoca fungos menores |
| Larva Luminescente | `glow_larva` | Rápida, aplica mark ou vulnerable |
| Besta Esporada | `spore_beast` | Frontline bruto com ataque em área |

## 4.5 Bosses

| Andar | Boss | ID sugerido | Identidade mecânica |
|---:|---|---|---|
| 25 | Mãe dos Esporos | `spore_mother` | Invoca fungos e pune mortes mal calculadas |
| 30 | Druida da Raiz Negra | `blackroot_druid` | Cura, poison e transformação de cadáveres |
| 35 | Colosso de Micélio | `mycelium_colossus` | Regeneração alta e defesa crescente |
| 40 | O Coração Fúngico | `fungal_heart` | Boss estacionário em fases, esporos e explosões |

## 4.6 Eventos específicos

```text
Poço de Esporos:
- Coletar material raro.
- Risco de aplicar poison em toda a party.

Cadáver Florescido:
- Saquear para gold ou item.
- Pode ativar emboscada de fungos.

Raiz Pulsante:
- Sacrificar HP para remover debuff persistente.
- Cortar a raiz para ganhar material.

Druida Aprisionado:
- Libertar para ganhar buff temporário.
- Roubar relíquia para recompensa imediata e risco moral/mecânico.
```

## 4.7 Loot temático

```text
- Esporos secos
- Seiva tóxica
- Couro de besouro
- Cogumelo medicinal
- Raiz negra
- Pó luminescente
- Núcleo de micélio
- Frasco de veneno bruto
```

## 4.8 Música e áudio

```text
Música: ambiência úmida, drones orgânicos, sons borbulhantes e notas agudas espaçadas.
SFX: estalos de fungos, insetos, líquido pingando, sopro de esporos, carne vegetal rasgando.
```

---

# 5. Tema 3 — Minas de Brasa

```text
Nome: Minas de Brasa
ID sugerido: ember_mines
Andares: 41-60
Função na progressão: testar burst, defesa e resposta a dano alto
```

## 5.1 Conceito

Minas abandonadas e forjas subterrâneas onde o carvão ainda respira, trilhos quebrados cortam túneis instáveis, goblins saqueiam equipamentos antigos, construtos patrulham corredores e mineradores amaldiçoados queimam sem morrer.

Este tema muda o ritmo do jogo. Sai o desgaste progressivo por poison e entra a ameaça de dano explosivo, burn e inimigos de alta defesa.

## 5.2 Direção visual

```text
Tom visual: industrial medieval, quente, sufocante, metálico e vulcânico.
```

### Paleta sugerida

```text
- Preto carvão
- Laranja de brasa
- Vermelho escuro
- Amarelo quente
- Cinza metálico
- Azul escuro para sombras profundas
```

### Elementos visuais

```text
- Trilhos de mineração
- Vagões quebrados
- Forjas acesas
- Lava ou metal derretido
- Correntes industriais
- Bigornas rachadas
- Engrenagens antigas
- Pontes metálicas
- Cristais incandescentes
```

## 5.3 Mecânicas principais

```text
- Burn por turno
- Inimigos com alta defesa
- Ataques carregados/telegrafados
- Explosões ao morrer
- Resistência física elevada
- Necessidade de dano mágico, debuffs ou foco rápido
```

## 5.4 Inimigos comuns sugeridos

| Inimigo | ID sugerido | Função |
|---|---|---|
| Goblin Mineiro | `goblin_miner` | Comum, rápido, dano físico leve |
| Goblin Demolidor | `goblin_demolisher` | Carrega bomba e explode se ignorado |
| Minerador Amaldiçoado | `cursed_miner` | Morto-vivo com picareta e burn |
| Sentinela de Ferro | `iron_sentinel` | Construto tanque com defesa alta |
| Morcego de Cinzas | `ash_bat` | Rápido, evasivo, ameaça backline |
| Elemental de Brasa | `ember_elemental` | Aplica burn e resiste a fogo |
| Forjador Profanado | `profane_smith` | Buffa armadura dos aliados |
| Vagão Assombrado | `haunted_minecart` | Avança ranks e causa colisão |

## 5.5 Bosses

| Andar | Boss | ID sugerido | Identidade mecânica |
|---:|---|---|---|
| 45 | Capataz Queimado | `burned_foreman` | Marca heróis e chama mineradores |
| 50 | O Forjador Sem Rosto | `faceless_forgemaster` | Cria armas vivas e melhora inimigos |
| 55 | Titã da Escória | `slag_titan` | Baixa velocidade, defesa alta e dano extremo |
| 60 | Coração da Fornalha | `furnace_heart` | Fases de calor, explosão e resfriamento |

## 5.6 Eventos específicos

```text
Forja Abandonada:
- Melhorar equipamento por gold.
- Risco de aplicar burn ou quebrar item temporariamente.

Veio de Cristal Ardente:
- Coletar material raro.
- Pode acordar elemental de brasa.

Trilho Instável:
- Avançar rapidamente com dano na party.
- Escolher rota segura com combate.

Mineiro Moribundo:
- Receber mapa de loot.
- Aceitar item amaldiçoado com bônus e penalidade.
```

## 5.7 Loot temático

```text
- Ferro negro
- Carvão vivo
- Fragmento de escória
- Núcleo de brasa
- Engrenagens antigas
- Martelo rachado
- Cristal ardente
- Cinzas encantadas
```

## 5.8 Música e áudio

```text
Música: percussão pesada, batidas metálicas, graves industriais e tensão crescente.
SFX: forja, martelos, metal rangendo, fogo, explosões abafadas, trilhos e vapor.
```

---

# 6. Tema 4 — Templo Submerso

```text
Nome: Templo Submerso
ID sugerido: sunken_temple
Andares: 61-80
Função na progressão: testar posicionamento, controle e resposta a debuffs
```

## 6.1 Conceito

Um templo antigo soterrado sob águas escuras, com corredores inundados, altares cobertos por algas, estátuas afogadas, sinos submersos, monstros anfíbios e sacerdotes de uma religião esquecida.

Este tema deve ser mais tático e menos direto. O jogador precisa lidar com slow, redução de precisão, movimentação forçada e inimigos que atacam ranks específicos.

## 6.2 Direção visual

```text
Tom visual: sagrado, aquático, frio, decadente e ritualístico.
```

### Paleta sugerida

```text
- Azul profundo
- Verde algas
- Turquesa apagado
- Pedra molhada cinza
- Dourado oxidado
- Branco pálido de conchas e ossos
```

### Elementos visuais

```text
- Corredores parcialmente inundados
- Algas penduradas
- Estátuas chorosas
- Sinos enferrujados
- Portões cobertos de coral
- Altares submersos
- Velas protegidas por redomas
- Peixes cegos
- Mosaicos de maré
```

## 6.3 Mecânicas principais

```text
- Slow
- Redução de accuracy
- Bleed por criaturas aquáticas
- Push/pull de ranks
- Magias de maré em área
- Inimigos que submergem e aumentam evasão
```

## 6.4 Inimigos comuns sugeridos

| Inimigo | ID sugerido | Função |
|---|---|---|
| Afogado do Templo | `temple_drowned` | Morto-vivo aquático resistente |
| Acólito das Marés | `tide_acolyte` | Suporte mágico, slow e cura |
| Sapo Abissal | `abyss_toad` | HP alto, poison ou bleed leve |
| Lâmina Coralina | `coral_blade` | Duelista anfíbio com bleed |
| Guardião de Concha | `shell_guardian` | Tanque com proteção |
| Serpente de Cisterna | `cistern_serpent` | Ataca backline e aplica mark |
| Estátua Chorosa | `weeping_statue` | Construto mágico, reduz accuracy |
| Oráculo Afogado | `drowned_oracle` | Fortalece o próximo ataque inimigo |

## 6.5 Bosses

| Andar | Boss | ID sugerido | Identidade mecânica |
|---:|---|---|---|
| 65 | Guardião da Cisterna | `cistern_guardian` | Proteção e empurrões de rank |
| 70 | Oráculo das Marés Mortas | `dead_tide_oracle` | Debuff, cura e prioridade de backline |
| 75 | A Serpente do Altar | `altar_serpent` | Boss móvel, bleed e ataques em alvos marcados |
| 80 | O Santo Afogado | `drowned_saint` | Cura, dano mágico, invocação e fase ritual |

## 6.6 Eventos específicos

```text
Fonte Submersa:
- Curar party.
- Risco de aplicar slow no próximo combate.

Altar Coberto por Algas:
- Remover debuff.
- Pode invocar guardião.

Sino Afogado:
- Tocar para ganhar relíquia.
- Pode chamar inimigos aquáticos.

Mosaico de Maré:
- Escolher buff de defesa, speed ou resistência.
- Pode alterar a formação da party.
```

## 6.7 Loot temático

```text
- Pérola escura
- Coral cortante
- Água consagrada turva
- Concha rúnica
- Escama abissal
- Bronze oxidado
- Sino quebrado
- Lodo sagrado
```

## 6.8 Música e áudio

```text
Música: coral distante, sons submersos, sinos abafados e drones aquáticos.
SFX: água pingando, bolhas, passos molhados, sinos, rugidos anfíbios, correntes submersas.
```

---

# 7. Tema 5 — Abismo Astral

```text
Nome: Abismo Astral
ID sugerido: astral_abyss
Andares: 81-100
Função na progressão: teste final de domínio sistêmico
```

## 7.1 Conceito

Uma ruptura entre a dungeon física e o vazio cósmico. Corredores impossíveis, estrelas mortas, geometrias quebradas, sombras inteligentes, ecos dos heróis, entidades arcanas e portais que distorcem espaço e tempo.

Este é o tema final do primeiro ciclo de conteúdo. Deve misturar mecânicas anteriores e adicionar distorções mais avançadas sem perder legibilidade mobile.

## 7.2 Direção visual

```text
Tom visual: cósmico, sombrio, irreal, arcano e ameaçador.
```

### Paleta sugerida

```text
- Preto azulado
- Violeta profundo
- Magenta escuro
- Ciano espectral
- Prata fria
- Dourado pálido para runas astrais
```

### Elementos visuais

```text
- Portais quebrados
- Estrelas mortas
- Pontes flutuantes
- Runas suspensas
- Fragmentos de realidade
- Olhos no vazio
- Espelhos impossíveis
- Silhuetas duplicadas
- Céu cósmico dentro da dungeon
```

## 7.3 Mecânicas principais

```text
- Stun frequente
- Mark e vulnerable
- Dano mágico alto
- Inimigos que trocam ranks
- Distorção de targeting
- Summons temporários
- Fases de boss
- Punições por deixar inimigos vivos tempo demais
```

## 7.4 Inimigos comuns sugeridos

| Inimigo | ID sugerido | Função |
|---|---|---|
| Sombra Estelar | `star_shadow` | Evasivo, aplica mark |
| Olho do Vazio | `void_eye` | Backline mágico, aplica vulnerable |
| Aberração Fraturada | `fractured_aberration` | Troca ranks e ataca múltiplos alvos |
| Cultista Astral | `astral_cultist` | Buffa entidades e sacrifica HP |
| Sentinela de Obsidiana | `obsidian_sentinel` | Tanque mágico resistente |
| Eco do Herói | `hero_echo` | Cópia sombria de classe jogável |
| Devora-Luz | `light_devourer` | Reduz cura recebida pela party |
| Arconte do Vazio | `void_archon` | Elite com dano mágico e controle |

## 7.5 Bosses

| Andar | Boss | ID sugerido | Identidade mecânica |
|---:|---|---|---|
| 85 | O Olho Entre Mundos | `eye_between_worlds` | Mark, foco em alvos frágeis e sombras |
| 90 | O Arconte Partido | `broken_archon` | Alterna forma defensiva e ofensiva |
| 95 | Reflexo da Party | `party_reflection` | Cria ecos das classes do jogador |
| 100 | A Fenda Primordial | `primordial_rift` | Boss final em fases, distorção de ranks e dano em área |

## 7.6 Eventos específicos

```text
Espelho Impossível:
- Duplicar recompensa.
- Pode criar eco inimigo.

Constelação Morta:
- Escolher buff permanente da run.
- Aplicar debuff colateral.

Porta Sem Lado:
- Trocar posição da party.
- Pular combate com risco.

Relógio Quebrado:
- Reduzir cooldowns.
- Aplicar slow na próxima luta.
```

## 7.7 Loot temático

```text
- Fragmento astral
- Pó de estrela morta
- Lente do vazio
- Runa impossível
- Cristal de obsidiana
- Núcleo de fenda
- Vidro cósmico
- Essência do nada
```

## 7.8 Música e áudio

```text
Música: textura etérea, coros distorcidos, pulsos graves, notas reversas e ambiência cósmica.
SFX: portais, ecos, distorções, cristais, sussurros invertidos, impacto mágico grave.
```

---

# 8. Tabela geral de bosses por andar

| Andar | Tema | Boss | ID sugerido |
|---:|---|---|---|
| 5 | Cripta Esquecida | Capitão dos Ossos | `bone_captain` |
| 10 | Cripta Esquecida | Sacerdote da Cova | `grave_priest` |
| 15 | Cripta Esquecida | Abominação da Cripta | `crypt_abomination` |
| 20 | Cripta Esquecida | O Carcereiro Esquecido | `forgotten_warden` |
| 25 | Profundezas Fúngicas | Mãe dos Esporos | `spore_mother` |
| 30 | Profundezas Fúngicas | Druida da Raiz Negra | `blackroot_druid` |
| 35 | Profundezas Fúngicas | Colosso de Micélio | `mycelium_colossus` |
| 40 | Profundezas Fúngicas | O Coração Fúngico | `fungal_heart` |
| 45 | Minas de Brasa | Capataz Queimado | `burned_foreman` |
| 50 | Minas de Brasa | O Forjador Sem Rosto | `faceless_forgemaster` |
| 55 | Minas de Brasa | Titã da Escória | `slag_titan` |
| 60 | Minas de Brasa | Coração da Fornalha | `furnace_heart` |
| 65 | Templo Submerso | Guardião da Cisterna | `cistern_guardian` |
| 70 | Templo Submerso | Oráculo das Marés Mortas | `dead_tide_oracle` |
| 75 | Templo Submerso | A Serpente do Altar | `altar_serpent` |
| 80 | Templo Submerso | O Santo Afogado | `drowned_saint` |
| 85 | Abismo Astral | O Olho Entre Mundos | `eye_between_worlds` |
| 90 | Abismo Astral | O Arconte Partido | `broken_archon` |
| 95 | Abismo Astral | Reflexo da Party | `party_reflection` |
| 100 | Abismo Astral | A Fenda Primordial | `primordial_rift` |

---

# 9. IDs sugeridos de DungeonThemeDefinition

```json
[
  {
    "id": "forgotten_crypt",
    "displayName": "Cripta Esquecida",
    "firstFloor": 1,
    "lastFloor": 20
  },
  {
    "id": "fungal_depths",
    "displayName": "Profundezas Fúngicas",
    "firstFloor": 21,
    "lastFloor": 40
  },
  {
    "id": "ember_mines",
    "displayName": "Minas de Brasa",
    "firstFloor": 41,
    "lastFloor": 60
  },
  {
    "id": "sunken_temple",
    "displayName": "Templo Submerso",
    "firstFloor": 61,
    "lastFloor": 80
  },
  {
    "id": "astral_abyss",
    "displayName": "Abismo Astral",
    "firstFloor": 81,
    "lastFloor": 100
  }
]
```

---

# 10. Exemplo de estrutura para ScriptableObject

```json
{
  "id": "theme.forgotten_crypt",
  "displayName": "Cripta Esquecida",
  "firstFloor": 1,
  "lastFloor": 20,
  "visualPalette": [
    "cold_gray",
    "dark_blue",
    "muted_green",
    "dark_purple",
    "aged_gold"
  ],
  "backgroundId": "background.forgotten_crypt.main",
  "musicId": "music.combat_crypt_theme",
  "commonEncounterTableId": "encounter_table.forgotten_crypt.common",
  "eliteEncounterTableId": "encounter_table.forgotten_crypt.elite",
  "bossEncounterTableId": "encounter_table.forgotten_crypt.boss",
  "shopTableId": "shop_table.forgotten_crypt.resting_site",
  "lootTableId": "loot_table.forgotten_crypt"
}
```

---

# 11. Recomendações de implementação

## 11.1 MVP

Implementar primeiro apenas a **Cripta Esquecida completa**:

```text
- 20 andares jogáveis
- 4 classes iniciais
- 8 inimigos comuns mínimos
- 4 bosses
- 1 resting site funcional
- Loot table básica
- Encounter tables comuns e boss
```

Os demais temas devem entrar inicialmente como documentação e dados planejados, sem bloquear o protótipo jogável.

## 11.2 Data-driven

Todos os temas devem ser configuráveis por dados.

Evitar codificar regras específicas de tema diretamente em serviços de dungeon ou combate. Preferir IDs, tabelas e definições estáticas.

## 11.3 IDs estáveis

IDs de temas, inimigos, bosses, eventos, encounters e loot não devem ser renomeados após entrarem em produção.

Caso seja necessário substituir conteúdo, criar um novo ID e tratar migração se o conteúdo já puder existir no save do jogador.

## 11.4 Mobile-first

Todos os ambientes devem priorizar:

```text
- Silhuetas legíveis
- Pouco ruído visual atrás dos combatentes
- Contraste suficiente entre party, inimigos e background
- Pontos de luz controlados
- Interface limpa sobre o cenário
- Performance estável em aparelhos intermediários
```

---

# 12. Backlog pós-nível 100

Temas candidatos para expansões futuras:

```text
- Biblioteca Proibida
- Jardim de Carne
- Prisão dos Gigantes
- Catedral Invertida
- Necrópole Real
- Torre do Eclipse
- Covil do Dragão Ossificado
- Reino Congelado Sob a Terra
- Máquina Ancestral
- Sonho do Deus Morto
```

Cada tema futuro deve seguir o mesmo padrão:

```text
20 andares
4 bosses
8 a 12 inimigos comuns
paleta própria
loot table própria
eventos próprios
mecânica dominante clara
```
