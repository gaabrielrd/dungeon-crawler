# CONTENT_PIPELINE.md

## 1. Objetivo

Definir como criar, organizar, importar, validar e expandir conteúdo do jogo:

```text
pixel art
sprites
tilesets
ícones
UI
música
SFX
dados de heróis
dados de inimigos
dados de habilidades
equipamentos
itens
eventos
temas de dungeon
```

---

## 2. Fontes de assets

### 2.1 Fontes gratuitas recomendadas

```text
Kenney
OpenGameArt
itch.io
Unity Asset Store
```

### 2.2 Regras de licença

Todo asset externo deve ter registro de licença.

Criar arquivo:

```text
Assets/Game/Art/ASSET_LICENSES.md
```

Cada entrada deve conter:

```text
nome do asset
autor
origem/link
licença
data de download
modificações realizadas
uso dentro do jogo
```

### 2.3 Licenças aceitáveis

Preferir:

```text
CC0
MIT
licença própria permitindo uso comercial
assets pagos com licença comercial clara
```

Usar com cautela:

```text
CC-BY, exige atribuição
CC-BY-SA, pode exigir compartilhamento sob mesma licença
```

Evitar:

```text
assets sem licença clara
assets extraídos de jogos existentes
fan art de IP protegida
assets gerados com base explícita em franquias existentes
```

---

## 3. Uso de IA para assets

### 3.1 Usos recomendados

```text
concept art
placeholders
ícones simples
variações de paleta
backgrounds auxiliares
referências visuais
```

### 3.2 Usos que exigem revisão manual

```text
sprites animados
personagens jogáveis
bosses principais
UI final
assets centrais de identidade visual
```

### 3.3 Regra de consistência

Todo asset gerado por IA deve ser revisado manualmente para:

```text
paleta
resolução
silhueta
outline
contraste
legibilidade mobile
coesão com outros assets
ausência de artefatos
```

### 3.4 Registro

Assets gerados por IA devem ser registrados em:

```text
Assets/Game/Art/AI_ASSET_LOG.md
```

Campos:

```text
asset id
prompt resumido
ferramenta utilizada
data
autor humano responsável
edições feitas
uso no jogo
```

---

## 4. Direção visual

### 4.1 Estilo

```text
2D pixel art
fantasia sombria estilizada
alto contraste
silhuetas fortes
poucas cores por sprite
animações curtas
UI limpa
```

### 4.2 Paleta

Definir paletas por tema.

Exemplo:

```text
Forgotten Crypt:
- cinzas frios
- verdes apagados
- roxo escuro
- dourado envelhecido para highlights
```

```text
Fungal Depths:
- verdes tóxicos
- roxos
- marrons úmidos
- amarelos pálidos para esporos
```

### 4.3 Tamanhos padrão

```text
Tiles: 16x16 ou 32x32
Ícones pequenos: 32x32
Ícones grandes: 64x64
Personagens: 64x64 ou 96x96
Inimigos pequenos: 64x64
Inimigos médios: 96x96
Bosses: 128x128 ou 192x192
Backgrounds: múltiplos de resolução-alvo
```

Manter um padrão escolhido. Evitar misturar escalas sem regra.

---

## 5. Importação no Unity

### 5.1 Configurações de sprite pixel art

Configuração base:

```text
Texture Type: Sprite (2D and UI)
Sprite Mode: Single ou Multiple
Pixels Per Unit: padronizar por projeto
Filter Mode: Point (no filter)
Compression: None ou baixa, conforme teste
Generate Mip Maps: Off para pixel art 2D comum
```

### 5.2 Sprite Atlas

Usar Sprite Atlas para:

```text
personagens
inimigos
UI
ícones
tilesets
```

### 5.3 Naming

```text
hero_guardian_idle_01.png
hero_guardian_attack_01.png
enemy_skeleton_idle_01.png
boss_crypt_lord_idle_01.png
icon_skill_shield_bash.png
tile_crypt_floor_01.png
ui_button_primary.png
```

### 5.4 Organização

```text
Assets/Game/Art/Characters/Heroes/Guardian/
Assets/Game/Art/Characters/Enemies/Crypt/Skeleton/
Assets/Game/Art/Characters/Bosses/Crypt/CryptLord/
Assets/Game/Art/Tilesets/Crypt/
Assets/Game/Art/UI/Buttons/
Assets/Game/Art/Icons/Skills/
Assets/Game/Art/Icons/Items/
```

---

## 6. Animações

### 6.1 Animações mínimas por herói

```text
idle
attack
skill
hit
death
victory, opcional
```

### 6.2 Animações mínimas por inimigo

```text
idle
attack
hit
death
```

### 6.3 Regras

```text
Animações devem ser curtas.
Ataques devem ter bom timing visual.
Feedback de hit é mais importante que frames complexos.
Bosses podem ter animações mais longas.
```

---

## 7. Áudio

### 7.1 Ferramentas

```text
Audacity
ChipTone
Bfxr
Bosca Ceoil, opcional
```

### 7.2 Tipos de SFX

```text
button_click
skill_select
attack_hit
attack_miss
critical_hit
heal
buff
debuff
poison_tick
burn_tick
enemy_death
hero_death
victory
boss_intro
reward_claim
purchase_success
ad_reward_granted
```

### 7.3 Música

Músicas iniciais:

```text
main_menu_theme
combat_crypt_theme
boss_crypt_theme
resting_site_theme
```

---

## 8. Conteúdo data-driven

### 8.1 ScriptableObjects

Criar conteúdo através de ScriptableObjects em:

```text
Assets/Game/Data/Heroes/
Assets/Game/Data/Skills/
Assets/Game/Data/Enemies/
Assets/Game/Data/Bosses/
Assets/Game/Data/Items/
Assets/Game/Data/Equipment/
Assets/Game/Data/DungeonThemes/
Assets/Game/Data/Encounters/
Assets/Game/Data/Shops/
```

### 8.2 IDs

Todos os conteúdos precisam de ID estável.

Exemplos:

```text
hero_class.guardian
skill.guardian.shield_bash
enemy.crypt.skeleton_grunt
boss.crypt.crypt_lord
item.consumable.small_health_potion
equipment.weapon.rusty_sword
theme.forgotten_crypt
encounter.crypt.common_001
```

Regra:

```text
Nunca renomear ID depois que o conteúdo estiver em save de produção.
Se precisar substituir, criar novo ID e migrar.
```

---

## 9. Pipeline de criação de classe

Para criar nova classe:

```text
1. Definir função tática.
2. Definir posições ideais.
3. Criar HeroClassDefinition.
4. Criar 4 a 8 SkillDefinitions.
5. Criar sprite idle/attack/hit/death.
6. Criar ícones de skills.
7. Criar balanceamento inicial.
8. Adicionar à tabela de contratação.
9. Testar em combate.
10. Registrar no changelog de conteúdo.
```

Template:

```text
Class ID:
Display Name:
Role:
Preferred Ranks:
Base HP:
Base Attack:
Base Magic:
Base Defense:
Base Speed:
Starting Skills:
Unlockable Skills:
Strengths:
Weaknesses:
```

---

## 10. Pipeline de criação de inimigo

```text
1. Definir tema.
2. Definir função no combate.
3. Criar EnemyDefinition.
4. Criar habilidades.
5. Criar sprite/animações.
6. Adicionar a encounter tables.
7. Testar sozinho.
8. Testar em composição.
9. Ajustar recompensas.
```

Template:

```text
Enemy ID:
Theme:
Role:
Ranks Occupied:
Preferred Skills:
HP:
Damage:
Speed:
Resistances:
Weaknesses:
Reward Weight:
```

---

## 11. Pipeline de boss

Bosses devem ter identidade mecânica clara.

Cada boss deve ter:

```text
mecânica principal
fase ou mudança de padrão, opcional
habilidade perigosa telegrafada
recompensa especial
entrada visual/sonora
```

Template:

```text
Boss ID:
Theme:
Floor Range:
Main Mechanic:
Adds/Summons:
Danger Skill:
Counterplay:
Rewards:
```

---

## 12. Pipeline de dungeon theme

Para criar novo tema:

```text
1. Definir paleta.
2. Definir tileset/background.
3. Definir música.
4. Criar inimigos comuns.
5. Criar elite encounters.
6. Criar bosses.
7. Criar loot table.
8. Criar shop modifiers.
9. Criar eventos específicos.
10. Definir floor range.
```

Template:

```text
Theme ID:
Display Name:
Floor Start:
Floor End:
Visual Palette:
Music:
Common Enemies:
Elite Enemies:
Bosses:
Unique Items:
Special Events:
```

---

## 13. Balanceamento

### 13.1 Dados que devem ser fáceis de ajustar

```text
HP de heróis
HP de inimigos
dano de skills
cooldowns
chance de status
recompensas de gold
chance de loot
custo de upgrades
custo de contratação
recompensa de rewarded ads
```

### 13.2 Remote Config

Valores que podem mudar sem nova build:

```text
goldRewardMultiplier
xpRewardMultiplier
rewardedAdGoldMultiplier
shopRerollCost
heroHireCostMultiplier
bossRewardMultiplier
rareDropChance
```

---

## 14. Localização

Mesmo que o MVP esteja em português, preparar o projeto para localização.

Idiomas iniciais sugeridos:

```text
pt-BR
en-US
```

Textos localizáveis:

```text
nomes de habilidades
descrições
nomes de itens
tutoriais
botões
mensagens de erro
loja
termos de compra
```

---

## 15. Checklist de asset antes de entrar no projeto

```text
[ ] Licença registrada.
[ ] Nome segue padrão.
[ ] Resolução correta.
[ ] Paleta coerente.
[ ] Import settings corretos.
[ ] Funciona em tela pequena.
[ ] Não infringe IP de terceiros.
[ ] Não está duplicado.
[ ] Está na pasta correta.
[ ] Está incluído em Sprite Atlas, se aplicável.
```

---

## 16. Checklist de conteúdo antes de release

```text
[ ] Todos os IDs são únicos.
[ ] Nenhum ID foi renomeado sem migração.
[ ] Todas as skills têm ícone.
[ ] Todos os inimigos têm pelo menos idle/attack/hit/death.
[ ] Bosses foram testados.
[ ] Encounter tables não geram composição impossível.
[ ] Loot tables não geram item inválido.
[ ] Loja não oferece item sem preço.
[ ] Conteúdo aparece corretamente em Android e iOS.
```

