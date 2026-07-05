# RPG Equipment Items
Documento de referência para equipamentos do MVP, separado em **pool comum de equipamentos** e **pool específica de armas por classe**.
Este arquivo deve servir como base para criação de `EquipmentDefinition`, loot tables, shop tables, regras de equipamento e testes de integridade de conteúdo.
---
## 1. Modelo de equipamentos
### 1.1 Slots de equipamento
```text
Weapon   -> arma específica por classe
Shield   -> comum entre classes, mas bloqueado por arma de duas mãos
Helmet   -> comum entre classes
Armor    -> comum entre classes
Boots    -> comum entre classes
```
### 1.2 Regra de arma e escudo
```text
Arma 1H equipada  -> personagem pode equipar escudo.
Arma 2H equipada  -> personagem não pode equipar escudo.
Sem arma equipada -> personagem pode equipar escudo, se o sistema permitir slot vazio.
```
No `EquipmentDefinition`, a arma deve expor um campo como `hands: one_handed | two_handed` ou `blocksShield: true/false`. O `EquipmentService` deve validar essa regra, não a UI.
### 1.3 Regra de escala
Os itens usam a mesma curva de poder definida em `RPG_PROGRESSION_SCALING.md`.
| Nível do item | Raridade permitida | Potência Fibonacci base |
|---:|---|---:|
| 2 | Comum | 5 |
| 5 | Comum / Raro | 21 |
| 8 | Raro / Épico | 89 |
| 10 | Épico / Épico | 233 |

Interpretação:

```text
Armas: potência = dano médio base ou contribuição ofensiva equivalente.
Escudos: potência = shield, mitigação ou proteção equivalente.
Capacetes: potência = HP, defesa, resistência ou prevenção de controle equivalente.
Armaduras: potência = HP, defesa, resistência ou barreira equivalente.
Botas: potência = speed, evasão, estabilidade de posição ou defesa leve equivalente.
```
A raridade altera principalmente o efeito secundário. No MVP, evitar multiplicadores agressivos sobre a potência principal para preservar a curva Fibonacci.
---
## 2. Convenções de IDs
```text
equipment.weapon.<class_id>.<item_id>
equipment.common.shield.<item_id>
equipment.common.helmet.<item_id>
equipment.common.armor.<item_id>
equipment.common.boots.<item_id>
```
Campos mínimos sugeridos para `EquipmentDefinition`:

```text
id
displayName
slot
equipmentType
requiredLevel
rarity
fibonacciPower
allowedClassIds, vazio/null para itens comuns
hands, apenas para armas
blocksShield, derivado de hands == two_handed
primaryEffect
secondaryEffect
```
---
## 3. Pool comum de equipamentos
A pool comum pode ser equipada por qualquer classe. A única exceção prática é o escudo, que não pode ser usado com arma de duas mãos.
## 3.1 Escudos
**Slot:** Escudo  
**Regra:** Equipável por qualquer classe, desde que a arma equipada não seja de duas mãos.

| Nível | Raridade | ID sugerido | Nome | Tipo | Potência | Efeito principal | Efeito secundário |
|---:|---|---|---|---|---:|---|---|
| 2 | Comum | `equipment.common.shield.splintered_buckler` | Broquel Lascado | Escudo leve | 5 | Mitigação 5 | Mitiga dano frontal baixo; opção inicial universal. |
| 2 | Comum | `equipment.common.shield.bone_laced_roundshield` | Escudo Redondo Trançado de Osso | Escudo leve | 5 | Shield 5 | Ao entrar em combate, concede pequeno escudo temporário. |
| 5 | Comum | `equipment.common.shield.crypt_iron_kite_shield` | Escudo de Ferro da Cripta | Escudo médio | 21 | Mitigação 21 | Boa opção comum para reduzir dano físico. |
| 5 | Raro | `equipment.common.shield.oathkeeper_heavy_shield` | Escudo Pesado do Juramento | Escudo pesado | 21 | Proteção 21 | Ao proteger aliado, reduz dano recebido pelo protegido. |
| 8 | Raro | `equipment.common.shield.candlelit_aegis` | Égide à Luz de Velas | Escudo médio | 89 | Resistência 89 | Reduz chance de debuffs leves no portador. |
| 8 | Épico | `equipment.common.shield.gate_of_silent_bones` | Portão dos Ossos Silenciosos | Escudo pesado | 89 | Barreira 89 | Uma vez por combate, gera barreira ao cair abaixo de 35% HP. |
| 10 | Épico | `equipment.common.shield.aegis_of_the_forgotten_gate` | Égide do Portão Esquecido | Escudo pesado | 233 | Escudo de grupo 233 | Ao usar skill defensiva, concede escudo menor à party. |
| 10 | Épico | `equipment.common.shield.saints_grave_bulwark` | Baluarte do Túmulo do Santo | Escudo pesado | 233 | Mitigação/Resistência 233 | Reduz dano físico e mágico recebido por 1 turno após sofrer crítico. |

## 3.2 Capacetes
**Slot:** Capacete  
**Regra:** Equipável por qualquer classe.

| Nível | Raridade | ID sugerido | Nome | Tipo | Potência | Efeito principal | Efeito secundário |
|---:|---|---|---|---|---:|---|---|
| 2 | Comum | `equipment.common.helmet.rusted_crypt_helm` | Elmo Enferrujado da Cripta | Capacete leve | 5 | HP/Defesa 5 | Aumenta sobrevivência inicial. |
| 2 | Comum | `equipment.common.helmet.candlewax_hood` | Capuz de Cera de Vela | Capuz | 5 | Resistência 5 | Reduz levemente dano mágico recebido. |
| 5 | Comum | `equipment.common.helmet.gravewatch_sallet` | Celada da Vigília Fúnebre | Capacete médio | 21 | Defesa 21 | Opção comum de defesa física. |
| 5 | Raro | `equipment.common.helmet.mourning_crown` | Coroa do Luto Antigo | Elmo ritual | 21 | Resistência/Controle 21 | Reduz chance de stun ou slow. |
| 8 | Raro | `equipment.common.helmet.bone_visor_helm` | Elmo de Viseira Óssea | Capacete médio | 89 | HP/Defesa 89 | Aumenta HP e reduz dano de frontline. |
| 8 | Épico | `equipment.common.helmet.wraithglass_circlet` | Diadema de Vidro Espectral | Diadema | 89 | Resistência/Evasão 89 | Melhora resistência contra efeitos mágicos. |
| 10 | Épico | `equipment.common.helmet.crown_of_the_last_warden` | Coroa do Último Carcereiro | Coroa pesada | 233 | HP/Defesa 233 | Ao vencer boss, remove um debuff persistente do usuário. |
| 10 | Épico | `equipment.common.helmet.halo_of_dead_stars` | Halo das Estrelas Mortas | Halo arcano | 233 | Resistência/Crítico 233 | Reduz dano mágico e aumenta chance crítica de skills especiais. |

## 3.3 Armaduras
**Slot:** Armadura  
**Regra:** Equipável por qualquer classe. A diferença entre leve/média/pesada é tratada como peso/estilo, não como restrição de classe no MVP.

| Nível | Raridade | ID sugerido | Nome | Tipo | Potência | Efeito principal | Efeito secundário |
|---:|---|---|---|---|---:|---|---|
| 2 | Comum | `equipment.common.armor.patched_travelers_garb` | Veste Remendada de Viajante | Armadura leve | 5 | HP 5 | Aumenta HP máximo de forma simples. |
| 2 | Comum | `equipment.common.armor.cracked_chain_vest` | Cota Rachada de Corrente | Armadura média | 5 | Defesa 5 | Reduz dano físico recebido. |
| 5 | Comum | `equipment.common.armor.crypt_leather_coat` | Casaco de Couro da Cripta | Armadura leve | 21 | Evasão/Defesa 21 | Opção comum para personagens de backline. |
| 5 | Raro | `equipment.common.armor.warden_scale_armor` | Armadura de Escamas do Carcereiro | Armadura média | 21 | Defesa/Resistência 21 | Reduz dano físico e parte de dano mágico. |
| 8 | Raro | `equipment.common.armor.consecrated_vestments` | Vestes Consagradas da Vigília | Vestes | 89 | Resistência/Suporte 89 | Melhora efeitos de cura recebidos. |
| 8 | Épico | `equipment.common.armor.last_wall_plate` | Armadura da Última Muralha | Armadura pesada | 89 | Barreira 89 | Gera barreira uma vez por combate ao ficar com HP baixo. |
| 10 | Épico | `equipment.common.armor.nightless_catacomb_plate` | Placas da Catacumba Sem Noite | Armadura pesada | 233 | Defesa/HP 233 | Grande proteção contra dano físico pesado. |
| 10 | Épico | `equipment.common.armor.shroud_of_the_undying_candle` | Mortalha da Vela Imorredoura | Veste ritual | 233 | Resistência/Cura 233 | Aumenta cura recebida e reduz dano mágico. |

## 3.4 Botas
**Slot:** Botas  
**Regra:** Equipável por qualquer classe.

| Nível | Raridade | ID sugerido | Nome | Tipo | Potência | Efeito principal | Efeito secundário |
|---:|---|---|---|---|---:|---|---|
| 2 | Comum | `equipment.common.boots.dusty_wayfarer_boots` | Botas Poeirentas do Errante | Botas leves | 5 | Speed 5 | Aumenta levemente iniciativa. |
| 2 | Comum | `equipment.common.boots.grave_mud_greaves` | Grevas de Lama Sepulcral | Grevas | 5 | Defesa 5 | Reduz dano recebido por ataques frontais leves. |
| 5 | Comum | `equipment.common.boots.catacomb_stride_boots` | Botas do Passo de Catacumba | Botas leves | 21 | Speed/Evasão 21 | Boa opção comum para reposicionamento. |
| 5 | Raro | `equipment.common.boots.pilgrims_silent_greaves` | Grevas Silenciosas do Peregrino | Grevas | 21 | Speed/Resistência 21 | Reduz chance de slow e melhora ordem de turno. |
| 8 | Raro | `equipment.common.boots.ash_walker_boots` | Botas do Andarilho das Cinzas | Botas leves | 89 | Evasão/Speed 89 | Ao mudar de rank, ganha evasão temporária. |
| 8 | Épico | `equipment.common.boots.ironroot_catacomb_greaves` | Grevas de Raiz de Ferro da Cripta | Grevas pesadas | 89 | Defesa/Controle 89 | Reduz chance de push/pull ou deslocamento forçado. |
| 10 | Épico | `equipment.common.boots.boots_of_the_last_threshold` | Botas do Último Limiar | Botas épicas | 233 | Speed/Evasão 233 | Primeira ação do combate ganha prioridade aumentada. |
| 10 | Épico | `equipment.common.boots.starburied_greaves` | Grevas das Estrelas Enterradas | Grevas épicas | 233 | Defesa/Resistência 233 | Ao resistir a debuff, concede pequeno bônus de speed. |

---
## 4. Pool específica de armas por classe
Armas são restritas por classe porque cada classe possui fantasia, animação, ícones e regras de habilidade próprias.
## 4.1 Guardião Sepulcral — Aldren Voss
```text
Class ID: guardian
Função: tanque, proteção, controle de frontline
```

| Nível | Raridade | ID sugerido | Nome | Tipo | Mãos | Potência | Efeito principal | Efeito secundário |
|---:|---|---|---|---|---|---:|---|---|
| 2 | Comum | `equipment.weapon.guardian.crypt_watch_short_sword` | Espada Curta do Vigia da Cripta | Espada curta | 1H | 5 | Dano físico 5 | Ataque frontal estável; mantém escudo disponível. |
| 2 | Comum | `equipment.weapon.guardian.boneguard_mace` | Maça do Guarda-Ossos | Maça | 1H | 5 | Dano físico 5 | Pequeno bônus contra inimigos undead; mantém escudo disponível. |
| 5 | Comum | `equipment.weapon.guardian.warden_long_sword` | Espada Longa do Carcereiro | Espada longa | 1H | 21 | Dano físico 21 | Boa opção comum para dano com escudo. |
| 5 | Raro | `equipment.weapon.guardian.mournstone_warhammer` | Martelo de Guerra da Pedra-Luto | Martelo de guerra | 2H | 21 | Dano físico 21 | Bloqueia escudo; ganha bônus contra inimigos armored/undead. |
| 8 | Raro | `equipment.weapon.guardian.oathbound_mace` | Maça do Juramento de Ossário | Maça | 1H | 89 | Dano físico 89 | Investida de Escudo ganha pequena chance adicional de stun. |
| 8 | Épico | `equipment.weapon.guardian.gatebreaker_warhammer` | Martelo Quebra-Portões | Martelo de guerra | 2H | 89 | Dano físico 89 | Bloqueia escudo; Quebra-Ossos reduz defesa com maior intensidade. |
| 10 | Épico | `equipment.weapon.guardian.sepulchral_oath_blade` | Lâmina do Juramento Sepulcral | Espada longa | 1H | 233 | Dano físico 233 | Mantém escudo disponível; Proteger Aliado reduz dano adicional. |
| 10 | Épico | `equipment.weapon.guardian.last_wall_colossus_hammer` | Martelo Colosso da Última Muralha | Martelo de guerra | 2H | 233 | Dano físico 233 | Bloqueia escudo; Última Muralha concede contra-ataque ao Guardião. |

## 4.2 Lâmina Velada — Neria Vale
```text
Class ID: rogue
Função: dano físico, mobilidade, execução
```

| Nível | Raridade | ID sugerido | Nome | Tipo | Mãos | Potência | Efeito principal | Efeito secundário |
|---:|---|---|---|---|---|---:|---|---|
| 2 | Comum | `equipment.weapon.rogue.veil_dagger` | Adaga do Véu | Adaga | 1H | 5 | Dano físico 5 | Corte Rápido fica mais consistente; mantém escudo disponível. |
| 2 | Comum | `equipment.weapon.rogue.grave_pairing_knives` | Facas Pareadas da Cova | Par de adagas | 2H | 5 | Dano físico 5 | Bloqueia escudo; ataques contra alvo com debuff recebem bônus leve. |
| 5 | Comum | `equipment.weapon.rogue.rusted_short_sword` | Espada Curta Enferrujada | Espada curta | 1H | 21 | Dano físico 21 | Opção comum para builds defensivas com escudo. |
| 5 | Raro | `equipment.weapon.rogue.blackwatch_crossbow` | Besta da Vigília Negra | Besta leve | 2H | 21 | Dano físico à distância 21 | Bloqueia escudo; permite pressão mais segura nos ranks 2–4. |
| 8 | Raro | `equipment.weapon.rogue.viper_stiletto` | Estilete da Víbora | Adaga | 1H | 89 | Dano físico/poison 89 | Lâmina Envenenada aplica poison com maior consistência. |
| 8 | Épico | `equipment.weapon.rogue.thirteen_cuts_twin_daggers` | Adagas dos Treze Cortes | Par de adagas | 2H | 89 | Dano físico 89 | Bloqueia escudo; Dança das Facas distribui melhor os acertos. |
| 10 | Épico | `equipment.weapon.rogue.deathmark_stiletto` | Estilete da Marca Fatal | Adaga | 1H | 233 | Dano físico 233 | Marca da Morte aumenta dano recebido pelo alvo marcado. |
| 10 | Épico | `equipment.weapon.rogue.executioners_black_crossbow` | Besta Negra do Executor | Besta leve | 2H | 233 | Dano físico à distância 233 | Bloqueia escudo; Execução Silenciosa ganha bônus contra alvos abaixo de 35% HP. |

## 4.3 Acólita da Vela — Mirella Thorne
```text
Class ID: acolyte
Função: cura, suporte, purificação
```

| Nível | Raridade | ID sugerido | Nome | Tipo | Mãos | Potência | Efeito principal | Efeito secundário |
|---:|---|---|---|---|---|---:|---|---|
| 2 | Comum | `equipment.weapon.acolyte.candlewood_staff` | Cajado de Madeira de Vela | Cajado | 2H | 5 | Dano sagrado/cura 5 | Bloqueia escudo; Luz da Vela causa dano estável contra undead. |
| 2 | Comum | `equipment.weapon.acolyte.ceremonial_dagger_of_wax` | Punhal Cerimonial de Cera | Punhal cerimonial | 1H | 5 | Dano sagrado 5 | Mantém escudo disponível; Prece Menor recebe cura levemente maior. |
| 5 | Comum | `equipment.weapon.acolyte.silver_ritual_bell` | Sino Ritual de Prata | Sino ritual | 1H | 21 | Dano sagrado/suporte 21 | Opção comum para suporte com escudo. |
| 5 | Raro | `equipment.weapon.acolyte.ashwood_pilgrim_staff` | Cajado Peregrino de Cinzas | Cajado | 2H | 21 | Cura 21 | Bloqueia escudo; Purificar Feridas melhora a cura base. |
| 8 | Raro | `equipment.weapon.acolyte.vigil_bell_of_warm_ashes` | Sino da Vigília das Cinzas Mornas | Sino ritual | 1H | 89 | Cura/suporte 89 | Círculo de Cinzas reduz dano com mais eficiência. |
| 8 | Épico | `equipment.weapon.acolyte.undying_candle_staff` | Cajado da Vela Imorredoura | Cajado | 2H | 89 | Dano sagrado/cura 89 | Bloqueia escudo; Luz da Vela converte parte do dano em cura. |
| 10 | Épico | `equipment.weapon.acolyte.martyrs_ceremonial_blade` | Lâmina Cerimonial da Mártir | Punhal cerimonial | 1H | 233 | Cura/suporte 233 | Voto de Sacrifício reduz o dano autoinfligido. |
| 10 | Épico | `equipment.weapon.acolyte.last_flame_staff` | Cajado da Última Chama | Cajado | 2H | 233 | Cura de grupo 233 | Bloqueia escudo; Chama Consagrada ganha potência adicional. |

## 4.4 Arcanista do Túmulo — Cael Morvain
```text
Class ID: arcanist
Função: dano mágico, controle, dano em área
```

| Nível | Raridade | ID sugerido | Nome | Tipo | Mãos | Potência | Efeito principal | Efeito secundário |
|---:|---|---|---|---|---|---:|---|---|
| 2 | Comum | `equipment.weapon.arcanist.cracked_grimoire` | Grimório Rachado | Grimório | 2H | 5 | Dano mágico 5 | Bloqueia escudo; Faísca Profana ganha dano estável. |
| 2 | Comum | `equipment.weapon.arcanist.dull_arcane_focus` | Foco Arcano Opaco | Foco arcano | 1H | 5 | Controle mágico 5 | Mantém escudo disponível; Selo de Lentidão melhora a redução de speed. |
| 5 | Comum | `equipment.weapon.arcanist.gravewand` | Varinha da Sepultura | Varinha | 1H | 21 | Dano mágico 21 | Opção comum para dano mágico com escudo. |
| 5 | Raro | `equipment.weapon.arcanist.hushed_rune_grimoire` | Grimório das Runas Silenciadas | Grimório | 2H | 21 | Dano mágico 21 | Bloqueia escudo; Explosão Rúnica recebe bônus leve por alvo. |
| 8 | Raro | `equipment.weapon.arcanist.ether_chain_focus` | Foco das Correntes Etéreas | Foco arcano | 1H | 89 | Controle mágico 89 | Correntes Etéreas aumenta chance de stun. |
| 8 | Épico | `equipment.weapon.arcanist.starfall_ritual_staff` | Cajado Ritual da Queda Estelar | Cajado ritual | 2H | 89 | Dano mágico 89 | Bloqueia escudo; Cometa Sepulcral causa dano maior aos adjacentes. |
| 10 | Épico | `equipment.weapon.arcanist.void_choir_wand` | Varinha do Coro Vazio | Varinha | 1H | 233 | Dano mágico 233 | Faísca Profana e Estilhaço Arcano escalam melhor contra backline. |
| 10 | Épico | `equipment.weapon.arcanist.tombstar_grimoire` | Grimório da Estrela Tumular | Grimório | 2H | 233 | Dano mágico em área 233 | Bloqueia escudo; Explosão Rúnica e Cometa Sepulcral ganham sinergia. |

## 4.5 Caçador de Relíquias — Bram Halvek
```text
Class ID: relic_hunter
Função: dano à distância, utilidade, loot
```

| Nível | Raridade | ID sugerido | Nome | Tipo | Mãos | Potência | Efeito principal | Efeito secundário |
|---:|---|---|---|---|---|---:|---|---|
| 2 | Comum | `equipment.weapon.relic_hunter.relic_seekers_crossbow` | Besta do Buscador de Relíquias | Besta leve | 2H | 5 | Dano à distância 5 | Bloqueia escudo; Disparo Preciso fica mais estável. |
| 2 | Comum | `equipment.weapon.relic_hunter.utility_dagger` | Adaga de Ferramenta | Adaga | 1H | 5 | Dano físico/utilidade 5 | Mantém escudo disponível; Inspecionar Fraqueza melhora defesa reduzida. |
| 5 | Comum | `equipment.weapon.relic_hunter.old_duelist_pistol` | Pistola Antiga do Duelista | Pistola antiga | 1H | 21 | Dano à distância 21 | Opção comum para utilidade com escudo. |
| 5 | Raro | `equipment.weapon.relic_hunter.heavy_crypt_crossbow` | Besta Pesada da Cripta | Besta pesada | 2H | 21 | Dano à distância 21 | Bloqueia escudo; pressão superior contra ranks 2–4. |
| 8 | Raro | `equipment.weapon.relic_hunter.tomb_raider_pistol` | Pistola do Saqueador de Túmulos | Pistola antiga | 1H | 89 | Dano à distância 89 | Flecha Perfurante/tiros ignoram parte da defesa. |
| 8 | Épico | `equipment.weapon.relic_hunter.vaultbreaker_crossbow` | Besta Quebra-Cofres | Besta pesada | 2H | 89 | Dano à distância 89 | Bloqueia escudo; bônus contra elites e bosses. |
| 10 | Épico | `equipment.weapon.relic_hunter.buried_star_pistol` | Pistola da Estrela Enterrada | Pistola antiga | 1H | 233 | Dano à distância/utilidade 233 | Relíquia Instável ganha melhor resultado mínimo. |
| 10 | Épico | `equipment.weapon.relic_hunter.reliquary_hunter_crossbow` | Besta do Caçador de Relicários | Besta pesada | 2H | 233 | Dano à distância 233 | Bloqueia escudo; Flecha Perfurante atinge com potência aumentada. |

## 4.6 Exorcista Errante — Ilyra Vey
```text
Class ID: exorcist
Função: anti-morto-vivo, debuff, dano sagrado
```

| Nível | Raridade | ID sugerido | Nome | Tipo | Mãos | Potência | Efeito principal | Efeito secundário |
|---:|---|---|---|---|---|---:|---|---|
| 2 | Comum | `equipment.weapon.exorcist.silvered_foil` | Florete Prateado | Florete | 1H | 5 | Dano físico/sagrado 5 | Golpe Consagrado fica mais estável contra undead. |
| 2 | Comum | `equipment.weapon.exorcist.minor_ritual_bell` | Sino Ritual Menor | Sino ritual | 1H | 5 | Dano sagrado/debuff 5 | Repreensão reduz ataque com maior consistência. |
| 5 | Comum | `equipment.weapon.exorcist.ritual_whip` | Chicote Ritual | Chicote | 1H | 21 | Dano físico/sagrado 21 | Chicote de Prata mantém bom dano de alcance. |
| 5 | Raro | `equipment.weapon.exorcist.banishment_blade` | Lâmina do Banimento | Lâmina consagrada | 1H | 21 | Dano sagrado 21 | Selo de Banimento aumenta vulnerabilidade sagrada. |
| 8 | Raro | `equipment.weapon.exorcist.pale_sun_foil` | Florete do Sol Pálido | Florete | 1H | 89 | Dano sagrado 89 | Expulsar Corrupção causa bônus contra inimigos buffados. |
| 8 | Épico | `equipment.weapon.exorcist.silent_ward_whip` | Chicote da Vigília Silenciosa | Chicote | 1H | 89 | Dano sagrado/controle 89 | Rito de Proteção pode bloquear parte do dano recebido. |
| 10 | Épico | `equipment.weapon.exorcist.final_judgement_foil` | Florete do Juízo Final | Florete | 1H | 233 | Dano sagrado 233 | Juízo Final causa dano aumentado contra undead e specter. |
| 10 | Épico | `equipment.weapon.exorcist.saintless_consecrated_blade` | Lâmina Consagrada do Santo Ausente | Lâmina consagrada | 1H | 233 | Dano sagrado em área 233 | Juízo Final remove um buff inimigo antes de causar dano. |

---
## 5. Resumo quantitativo
| Pool | Quantidade | Regra |
|---|---:|---|
| Itens comuns — escudos | 8 | 2 itens nos níveis 2, 5, 8 e 10 |
| Itens comuns — capacetes | 8 | 2 itens nos níveis 2, 5, 8 e 10 |
| Itens comuns — armaduras | 8 | 2 itens nos níveis 2, 5, 8 e 10 |
| Itens comuns — botas | 8 | 2 itens nos níveis 2, 5, 8 e 10 |
| Armas específicas — 6 classes | 48 | 2 armas por classe nos níveis 2, 5, 8 e 10 |
| **Total** | **80** | 32 comuns + 48 armas específicas |

---
## 6. Regras de implementação
### 6.1 Validação de equipamento
```text
1. Validar requiredLevel <= hero.Level.
2. Validar classRestriction apenas para armas.
3. Validar slot disponível.
4. Se equipar arma 2H, remover ou bloquear escudo.
5. Se escudo equipado e jogador tentar equipar arma 2H, exigir confirmação e desequipar escudo.
6. Itens comuns não devem conter allowedClassIds, salvo exceção futura.
```
### 6.2 Loot e loja
```text
Itens nível 2: aparecem nos primeiros andares e loja inicial.
Itens nível 5: começam a aparecer antes/depois do boss do andar 5.
Itens nível 8: começam a aparecer entre os andares 10 e 15.
Itens nível 10: devem ser raros no loot e apropriados para boss/resting site avançado.
```
### 6.3 Testes obrigatórios
```text
[ ] Todos os IDs são únicos.
[ ] Todos os itens têm requiredLevel, rarity, slot e fibonacciPower.
[ ] Armas específicas possuem allowedClassIds.
[ ] Itens comuns não possuem restrição de classe.
[ ] Armas 2H bloqueiam escudo.
[ ] Escudo não pode ser equipado se arma 2H estiver equipada.
[ ] Raridade respeita nível: 2 comum; 5 comum/raro; 8 raro/épico; 10 épico.
[ ] Loot tables não oferecem item acima do nível permitido da faixa, salvo recompensa especial de boss.
```
