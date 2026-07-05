# Build Android e iOS/iPadOS

Este documento descreve a build minima do projeto para Android e iOS/iPadOS.

## Configuracoes atuais

- Unity: 6000.5.2f1.
- Android package name: `com.gaabrielrd.dungeoncrawler`.
- iOS bundle identifier: `com.gaabrielrd.dungeoncrawler`.
- Versao do app: `0.1.0`.
- Android version code: `1`.
- iOS build number: `0`.
- Orientacao inicial: portrait-only.
- Alvos iOS: iPhone e iPad.
- Cena inicial: `Assets/Game/Scenes/Bootstrap.unity`.

## Cenas no Build Settings

As cenas devem permanecer nesta ordem:

1. `Assets/Game/Scenes/Bootstrap.unity`
2. `Assets/Game/Scenes/MainMenu.unity`
3. `Assets/Game/Scenes/RunPreparation.unity`
4. `Assets/Game/Scenes/Settings.unity`
5. `Assets/Game/Scenes/CombatPrototype.unity`
6. `Assets/Game/Scenes/RestSite.unity`

O fluxo esperado e `Bootstrap -> MainMenu`, sem exigir login, cloud save, monetizacao, certificados finais, TestFlight ou Google Play Console.

## Android

### Requisitos

- Unity 6000.5.2f1 com Android Build Support instalado.
- Android SDK & NDK Tools instalados pelo Unity Hub.
- OpenJDK instalado pelo Unity Hub.

No Unity Hub, abra:

```text
Installs -> Unity 6000.5.2f1 -> Add modules
```

Marque:

```text
Android Build Support
Android SDK & NDK Tools
OpenJDK
```

### Gerar APK local

1. Abra o projeto no Unity.
2. Abra `File -> Build Profiles`.
3. Selecione Android.
4. Confirme que as cenas listadas acima estao habilitadas.
5. Use `Build` e escolha:

```text
Builds/Android/DungeonCrawler.apk
```

O APK gerado e apenas uma build tecnica minima. Ele nao deve ser versionado.

## iOS/iPadOS

### Requisitos

- Mac com Unity 6000.5.2f1.
- iOS Build Support instalado no Unity Hub.
- Xcode instalado.
- Command Line Tools do Xcode configurado.

No Unity Hub do Mac, abra:

```text
Installs -> Unity 6000.5.2f1 -> Add modules
```

Marque:

```text
iOS Build Support
```

### Exportar projeto Xcode

1. Abra o projeto no Unity em um Mac.
2. Abra `File -> Build Profiles`.
3. Selecione iOS.
4. Confirme que o target inclui iPhone e iPad.
5. Confirme que as cenas listadas acima estao habilitadas.
6. Use `Build` e escolha:

```text
Builds/iOS/DungeonCrawlerXcode
```

7. Abra o projeto exportado no Xcode.
8. Configure signing manual ou automatico apenas quando for necessario instalar em dispositivo real.

O export iOS desta etapa serve para validar que o projeto abre no Xcode. Publicacao, certificados finais, TestFlight e App Store Connect ficam fora do escopo.

## Checklist de validacao

- O console do Unity nao tem erros antes da build.
- `Bootstrap` esta no build index `0`.
- `MainMenu` esta no build index `1`.
- `RestSite` esta no build index `5`.
- A cena inicial carrega `MainMenu` em Play Mode.
- A build Android gera `Builds/Android/DungeonCrawler.apk`.
- O export iOS gera `Builds/iOS/DungeonCrawlerXcode` em um Mac.
- Nenhum artefato em `Builds/` e commitado.
