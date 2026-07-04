# MONETIZATION_DESIGN.md

## 1. Modelo de negócio

O jogo será **free-to-play** com:

```text
ads recompensados
compras internas
moeda premium
pacotes opcionais
remoção de anúncios
```

O objetivo é monetizar sem bloquear a progressão essencial. O jogo deve permitir avanço orgânico sem compra, mas oferecer conveniência, aceleração moderada e opções cosméticas ou de valor controlado.

---

## 2. Princípios de monetização

### 2.1 Não quebrar a confiança do jogador

Evitar práticas que façam o jogador sentir que perdeu por não pagar.

Não recomendado:

```text
boss artificialmente impossível sem compra
cura básica bloqueada por anúncio
compra obrigatória para continuar run
interstitial depois de toda ação
premium currency como única forma de progressão
```

Recomendado:

```text
rewarded ads opcionais
compra de conveniência
pacotes transparentes
remoção de anúncios
cosméticos
rerolls limitados
aceleração sem invalidar o gameplay
```

### 2.2 Ads devem ser majoritariamente opt-in

O formato principal deve ser **rewarded ad**. Interstitials, se usados, devem ser raros, previsíveis e nunca aparecer no meio do combate.

### 2.3 Compras devem ser simples no MVP

Começar com poucos produtos. Evitar battle pass, eventos sazonais e loja complexa antes de validar retenção.

---

## 3. Moedas

### 3.1 Gold

Moeda comum obtida jogando.

Usos:

```text
comprar consumíveis
contratar personagens comuns
melhorar habilidades básicas
comprar equipamentos comuns
curar party parcialmente, se aplicável
```

### 3.2 Gems

Moeda premium obtida por compra, recompensas limitadas ou eventos especiais.

Usos:

```text
comprar pacotes especiais
contratar personagens raros
reroll premium da loja
comprar cosméticos
acelerar upgrades não críticos
```

### 3.3 Materiais

Moedas secundárias não premium para upgrades.

Exemplos:

```text
Iron Shards
Arcane Dust
Boss Sigils
Class Tokens
```

---

## 4. Produtos IAP iniciais

### 4.1 Produtos mínimos para MVP

| Produto | Tipo | Descrição |
|---|---|---|
| Remove Ads | Non-consumable | Remove interstitials, se existirem, e pode preservar rewarded ads opcionais. |
| Small Gem Pack | Consumable | Pequeno pacote de moeda premium. |
| Medium Gem Pack | Consumable | Pacote intermediário. |
| Large Gem Pack | Consumable | Pacote maior. |
| Starter Pack | Non-consumable ou consumable limitado | Pacote único com gems, gold, item e personagem inicial opcional. |

### 4.2 Produtos pós-MVP

```text
Monthly Pass
Season Pass
Cosmetic Pack
Hero Skin Pack
Dungeon Theme Cosmetic Pack
Limited-time Offer
```

Não implementar no MVP.

---

## 5. Ads

### 5.1 Formatos

#### Rewarded ads

Principal formato recomendado.

Usos adequados:

```text
gold extra após combate
reroll grátis da loja
recompensa diária duplicada
revive limitado
redução pequena de custo de upgrade
abrir baú bônus
```

#### Interstitial ads

Usar apenas se necessário.

Locais aceitáveis:

```text
após encerrar uma run
após retornar ao menu principal
após completar um bloco de andares
```

Locais proibidos:

```text
no meio do combate
antes de confirmar compra
durante tutorial crítico
imediatamente após derrota frustrante
na primeira sessão antes de o jogador entender o jogo
```

### 5.2 Frequência inicial recomendada

```text
Rewarded ads: ilimitados por disponibilidade, mas com limites por recurso.
Interstitials: desabilitados no MVP ou no máximo 1 a cada várias sessões.
```

### 5.3 Limites anti-abuso

```text
Reroll de loja por ad: máximo 1 por resting site.
Gold extra por ad: máximo 1 por combate.
Revive por ad: máximo 1 por run ou 1 por bloco de 10 andares.
Baú bônus por ad: máximo 1 por boss.
```

---

## 6. Loja

### 6.1 Loja principal

Disponível no menu principal.

Conteúdo:

```text
Gem packs
Starter pack
Remove ads
Ofertas limitadas, pós-MVP
```

### 6.2 Loja do resting site

Disponível a cada 10 andares.

Conteúdo:

```text
consumíveis
equipamentos
personagens disponíveis para contratação
upgrades temporários
upgrades permanentes limitados
reroll de seleção
```

### 6.3 Reroll da loja

Opções:

```text
1 reroll grátis por resting site, opcional
1 reroll por rewarded ad
rerolls adicionais por gold/gems, com custo crescente
```

---

## 7. Compras e economia de progressão

### 7.1 O que pode ser comprado

```text
moeda premium
pacotes de início
remoção de anúncios
cosméticos
conveniência
personagens raros, com cuidado
```

### 7.2 O que evitar vender diretamente

```text
vitória instantânea
pular boss principal
item obrigatório para progredir
habilidade exclusiva extremamente mais forte
poder irreversível sem alternativa gratuita
```

---

## 8. Integrações técnicas

### 8.1 Serviços recomendados

```text
Unity IAP
Unity Economy
Unity Remote Config
Unity Cloud Code
Unity Analytics
Google Mobile Ads Unity Plugin
```

### 8.2 Responsabilidade dos serviços

| Serviço | Uso |
|---|---|
| Unity IAP | Compras reais em Android/iOS. |
| Unity Economy | Definição de moedas, itens econômicos, compras virtuais. |
| Unity Remote Config | Ajustar preços, recompensas e frequências sem nova build. |
| Unity Cloud Code | Validar concessões sensíveis e processar lógica server-side. |
| Unity Analytics | Medir comportamento e monetização. |
| Google Mobile Ads | Exibir rewarded ads e, se necessário, interstitials. |

---

## 9. Produtos e IDs sugeridos

### 9.1 IAP product IDs

```text
com.company.dungeoncrawler.remove_ads
com.company.dungeoncrawler.gems_small
com.company.dungeoncrawler.gems_medium
com.company.dungeoncrawler.gems_large
com.company.dungeoncrawler.starter_pack
```

Substituir `company` pelo identificador real do estúdio ou conta.

### 9.2 Economy resources

```text
currency.gold
currency.gems
material.iron_shards
material.arcane_dust
material.boss_sigils
item.health_potion
item.revive_token
pack.starter_pack
```

---

## 10. Fluxo de compra

```text
Jogador abre loja
→ App carrega produtos disponíveis
→ Jogador seleciona produto
→ Unity IAP inicia compra
→ Loja confirma transação
→ Compra é validada
→ Recompensa é concedida
→ Save local atualizado
→ Cloud save sincronizado
→ Analytics registra evento
→ UI exibe confirmação
```

### 10.1 Falhas

O sistema deve tratar:

```text
compra cancelada
compra pendente
falha de rede
produto indisponível
recibo inválido
recompensa não concedida
app fechado durante compra
```

### 10.2 Restore purchases

Obrigatório principalmente para iOS.

Deve restaurar:

```text
Remove Ads
Starter Pack, se non-consumable
Cosméticos non-consumable
```

Consumíveis normalmente não são restauráveis pela loja após consumo. O estado final deve estar no save/cloud.

---

## 11. Rewarded ad flow

```text
Jogador toca em ação com anúncio
→ App verifica disponibilidade
→ UI explica recompensa
→ Jogador confirma
→ Ad é exibido
→ SDK retorna conclusão
→ Recompensa é concedida
→ Save local atualizado
→ Cloud save sincronizado
→ Analytics registra evento
```

### 11.1 Recompensa só após conclusão

Não conceder recompensa se:

```text
ad falhar
jogador fechar antes do fim
SDK não confirmar reward
estado do jogo mudar de forma inválida
```

---

## 12. Balanceamento inicial

### 12.1 Recompensas por rewarded ad

Valores iniciais sugeridos:

```text
Gold extra pós-combate: +50% da recompensa base
Reroll loja: 1 gratuito via ad por resting site
Baú extra pós-boss: chance de item raro ou material
Revive: reviver 1 personagem com 30% HP, máximo 1 por run
```

### 12.2 Starter pack inicial

Exemplo:

```text
300 gems
3000 gold
1 personagem raro inicial
3 health potions
remove interstitial ads por 7 dias, se aplicável
```

Avaliar cuidadosamente para não prejudicar retenção orgânica.

---

## 13. Métricas de monetização

Eventos mínimos:

```text
shop_opened
iap_product_viewed
iap_started
iap_completed
iap_failed
iap_cancelled
restore_purchases_started
restore_purchases_completed
rewarded_ad_available
rewarded_ad_started
rewarded_ad_completed
rewarded_ad_failed
rewarded_ad_reward_granted
interstitial_shown
currency_earned
currency_spent
item_purchased
hero_hired
shop_rerolled
```

KPIs:

```text
ARPDAU
ARPU
payer conversion
rewarded ad impressions per DAU
rewarded ad completion rate
IAP conversion by product
D1/D7 retention by ad exposure
shop open rate
starter pack conversion
remove ads conversion
```

---

## 14. Configuração remota

Controlar via Remote Config:

```text
quantidade de gold por andar
multiplicador de recompensa por ad
custo de reroll
chance de raridade na loja
frequência de ofertas
preço virtual de itens
limite de revives por run
ativar/desativar interstitials
ativar/desativar rewarded ads específicos
```

---

## 15. Compliance e UX

### 15.1 Transparência

Todo botão de compra deve deixar claro:

```text
o que será comprado
qual moeda será gasta
se é dinheiro real ou moeda virtual
se a compra é consumível ou permanente
```

### 15.2 Crianças e famílias

Se o jogo for publicado para público amplo, verificar exigências de:

```text
classificação indicativa
políticas de anúncios
políticas de dados
compras internas
controle parental
consentimento por região
```

### 15.3 Login e compras

A conta do jogador e o save cloud devem preservar:

```text
compras permanentes
moeda premium comprada
estado de remove ads
histórico essencial de concessões
```

---

## 16. Decisões abertas

- Haverá interstitial ads ou apenas rewarded ads?
- Remove Ads remove apenas interstitials ou também oferece daily reward sem assistir ad?
- Personagens raros poderão ser comprados diretamente?
- Gems serão concedidas jogando ou apenas por compra/eventos?
- Haverá gacha? Recomendação inicial: não implementar no MVP.
- Haverá battle pass? Recomendação inicial: pós-MVP.

