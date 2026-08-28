# YU-GI-OH! VR

A Unity implementation of the *Yu-Gi-Oh! Trading Card Game*, featuring real-time multiplayer duels and a virtual reality game mode.

Originally built as part of a dissertation project, it recreates the core duel loop — decks, hands, the field, tributes, and the graveyard — and lets two players face off online or in VR.

## Features

- **Full duel loop** — deck, hand, field, graveyard, tribute summons, and turn management, modeled after the real TCG rules.
- **Card database** — monster and non-monster (spell/trap) cards backed by a local SQLite database (`Yugioh.db`).
- **Deck construction** — build and edit your own deck before entering a duel.
- **Online multiplayer** — real-time duels between two players over the network, powered by [Photon PUN 2](https://www.photonengine.com/pun).
- **VR mode** — play a duel in virtual reality using [VRTK](https://vrtoolkit.readme.io/), interacting with cards and the field directly with motion controllers.

## Tech stack

- **Engine:** Unity 2018.4.5f1
- **Networking:** Photon Unity Networking 2 (PUN 2), Photon Chat, Photon Realtime
- **VR:** VRTK (Virtual Reality Toolkit)
- **Data:** SQLite (card database)
- **Language:** C#

## Playing a multiplayer duel

The game uses Photon PUN 2 for networking. To test multiplayer locally, make sure a valid Photon App ID is configured in the PUN 2 setup wizard, then launch two instances of the game (e.g. via two Editor/build instances) and join the same room from the lobby.

## Playing in VR

The VR mode is built on VRTK and expects a compatible headset/controller setup (e.g. SteamVR-supported hardware). Launch the VR scene with your headset connected to duel using motion controllers.

## Project structure

- `Assets/Scripts/Card Scripts` — card data and behavior
- `Assets/Scripts/Deck Construction` — deck building UI and logic
- `Assets/Scripts/Managers` — game/duel flow managers
- `Assets/Scripts/PUN 2` — multiplayer networking scripts
- `Assets/VRTK` — VR interaction toolkit
- `Assets/Photon` — Photon networking SDKs
- `Yugioh.db` — SQLite database of cards

## License

This is an educational/dissertation project. No license has been specified — all rights reserved unless stated otherwise.

## Design

<img width="480" height="270" alt="MainMenu" src="https://github.com/user-attachments/assets/bab915e8-6835-4599-a2bf-c3c306860784" />
<img width="480" height="270" alt="Networking" src="https://github.com/user-attachments/assets/86fc9ad1-db7a-437d-8e8c-6380fa667d94" />
<img width="480" height="320" alt="deck" src="https://github.com/user-attachments/assets/f6835ad3-89a9-4ed8-bd08-53e97c38a910" />
<img width="480" height="270" alt="Scene" src="https://github.com/user-attachments/assets/4bef0c86-deee-4d76-b25e-8759a8b28583" />
<img width="480" height="220" alt="player" src="https://github.com/user-attachments/assets/5611b92e-373b-4134-a5bf-6269c6fdc5c8" />
<img width="480" height="220" alt="enemy" src="https://github.com/user-attachments/assets/c31b1c77-231e-4c22-b543-36487d81cab7" />
<img width="480" height="270" alt="Graveyard" src="https://github.com/user-attachments/assets/5a3e99a4-96a4-4181-90fa-bf243d6cca7b" />

## Demo

[<img src="https://img.youtube.com/vi/4rrq2kjN1H8/0.jpg" width="480">](https://youtu.be/4rrq2kjN1H8)

