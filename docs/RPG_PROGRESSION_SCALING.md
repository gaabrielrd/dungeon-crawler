# RPG Progression Scaling

Documento de referência para a curva de poder dos personagens, dano médio por nível, experiência necessária para evolução e regras de aplicação da sequência Fibonacci no sistema RPG.

Este arquivo deve servir como base para criação de `HeroProgressionDefinition`, balanceamento inicial de `SkillDefinition`, `EquipmentDefinition`, recompensas de XP e testes de progressão.

---

## 1. Regra base

O power-scaling dos personagens segue a sequência Fibonacci, começando em **3** para o dano médio do nível 1.

```text
Dano médio por nível:
Nível 1 = 3
Nível 2 = 5
Nível 3 = 8
Nível 4 = 13
...
```

Fórmula:

```text
damage(level 1) = 3
damage(level 2) = 5
damage(level n) = damage(level n - 1) + damage(level n - 2), para n >= 3
```

A experiência necessária para subir de nível usa a mesma sequência deslocada em um nível:

```text
XP para ir do nível 1 para o nível 2 = 5
XP para ir do nível 2 para o nível 3 = 8
XP para ir do nível 3 para o nível 4 = 13
...
```

Fórmula:

```text
xpToNext(level) = damage(level + 1)
```

Para o MVP, o nível máximo documentado é **10**. O valor de XP do nível 10 para 11 fica documentado apenas para expansão futura.

---

## 2. Tabela de dano e experiência até o nível 10

| Nível | Dano médio base | Faixa sugerida de dano | XP para próximo nível | XP acumulado para alcançar o nível |
|---:|---:|---:|---:|---:|
| 1 | 3 | 2–4 | 5 | 0 |
| 2 | 5 | 4–6 | 8 | 5 |
| 3 | 8 | 6–10 | 13 | 13 |
| 4 | 13 | 10–16 | 21 | 26 |
| 5 | 21 | 17–25 | 34 | 47 |
| 6 | 34 | 27–41 | 55 | 81 |
| 7 | 55 | 44–66 | 89 | 136 |
| 8 | 89 | 71–107 | 144 | 225 |
| 9 | 144 | 115–173 | 233 | 369 |
| 10 | 233 | 186–280 | 377 *(pós-MVP / futuro nível 11)* | 602 |

---

## 3. Interpretação de dano

O valor de **dano médio base** é o orçamento principal para ataques básicos e habilidades ofensivas de alvo único no nível correspondente.

Exemplo:

```text
Personagem nível 1:
- ataque básico médio: 3

Personagem nível 5:
- ataque básico médio: 21

Personagem nível 10:
- ataque básico médio: 233
```

A faixa sugerida de dano permite pequenas variações visuais e mecânicas sem quebrar a curva principal.

---

## 4. Multiplicadores sugeridos por tipo de habilidade

Para manter a curva legível, habilidades devem derivar do dano médio base do nível do usuário.

| Tipo de habilidade | Multiplicador recomendado | Observação |
|---|---:|---|
| Ataque básico | 1.00x | Referência principal da curva. |
| Ataque rápido | 0.75x–0.90x | Pode compensar com speed, crit ou baixo cooldown. |
| Ataque pesado | 1.25x–1.60x | Deve ter cooldown, restrição de rank ou baixa precisão. |
| Ataque em área | 0.40x–0.70x por alvo | O dano total não deve trivializar grupos. |
| Ataque com status | 0.60x–0.90x | Parte do orçamento vai para poison, bleed, stun, mark etc. |
| Cura single-target | 0.80x–1.20x | Baseada no mesmo orçamento para simplificar balanceamento. |
| Cura em área | 0.35x–0.60x por aliado | Deve preservar tensão de combate. |
| Shield/Barrier | 0.75x–1.25x | Considerar duração e stack. |
| Ultimate | 1.80x–2.50x | Deve ter cooldown alto, requisito ou custo claro. |

---

## 5. Regra para equipamentos

Equipamentos com dano, cura, shield ou efeito numérico principal devem seguir a mesma escala do nível exigido.

| Nível do item | Potência Fibonacci base |
|---:|---:|
| 2 | 5 |
| 5 | 21 |
| 8 | 89 |
| 10 | 233 |

Regras:

```text
Item nível 2: sempre comum.
Item nível 5: comum ou raro.
Item nível 8: raro ou épico.
Item nível 10: sempre épico.
```

Para evitar power creep prematuro, a raridade não deve multiplicar fortemente o dano base no MVP. A raridade deve diferenciar principalmente:

```text
efeito secundário
condição de ativação
tipo de status aplicado
sinergia com classe
cooldown reduzido
bônus situacional
```

---

## 6. Sugestão de modelo data-driven

```json
{
  "level": 5,
  "baseAverageDamage": 21,
  "damageRangeMin": 17,
  "damageRangeMax": 25,
  "xpToNextLevel": 34,
  "totalXpRequired": 47
}
```

```json
{
  "itemLevel": 8,
  "rarity": "epic",
  "fibonacciPower": 89,
  "primaryEffectType": "damage",
  "primaryEffectValue": 89
}
```

---

## 7. Regras de implementação

- O valor Fibonacci deve ser tratado como **orçamento de poder**, não como único número possível.
- Habilidades ofensivas de alvo único devem girar em torno de `baseAverageDamage`.
- Habilidades em área devem reduzir o dano por alvo.
- Habilidades com controle forte devem reduzir dano ou exigir cooldown.
- Equipamentos devem usar `requiredLevel` e `fibonacciPower`.
- Não salvar valores derivados no save se eles puderem ser recalculados a partir do nível e das definitions.
- IDs de progressão devem ser estáveis.
- Qualquer mudança futura na curva depois de produção deve considerar migração ou versionamento de balanceamento.

---

## 8. IDs sugeridos

```text
progression.hero.fibonacci_v1
progression.damage.fibonacci_v1
progression.xp.fibonacci_v1
progression.equipment_power.fibonacci_v1
```