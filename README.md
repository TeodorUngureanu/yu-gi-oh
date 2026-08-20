# YuGiHoHoHo

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

## Getting started

1. Install [Unity Hub](https://unity.com/download) and Unity Editor version **2018.4.5f1**.
2. Clone the repository:
   ```
   git clone https://github.com/TeodorUngureanu/yu-gi-oh.git
   ```
3. Open the project folder in Unity Hub / the Unity Editor.
4. Open the main menu scene from `Assets/Scripts/Scenes` (or the scene configured in Build Settings) and press Play.

### Playing a multiplayer duel

The game uses Photon PUN 2 for networking. To test multiplayer locally, make sure a valid Photon App ID is configured in the PUN 2 setup wizard, then launch two instances of the game (e.g. via two Editor/build instances) and join the same room from the lobby.

### Playing in VR

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
