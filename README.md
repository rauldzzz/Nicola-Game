# Nicola Game

## Description

**Nicola Game** is a humorous and informative 2D action-adventure inspired by "Zelda II: The Adventure of Link".

The game combines:

* A **top-down overworld**
* A variety of **minigames** (rhythm, dungeon crawler, puzzle, obstacle courses, etc.)

### Core Principle

The player takes on the role of an international exchange student who moves to **Hagenberg, Upper Austria**, to begin their studies.

During the first days of student life, the player experiences:

* Arriving in town
* Finding their room in the student dormitory
* Participating in the "orientation days" 
* Completing the EU citizen registration process

The game conveys **useful information about Hagenberg, the university environment, and Austrian culture**, all presented in a fun, playful, and humorous way.

### Target Audience

Students — especially **incoming international students** who plan to study or spend an exchange semester in Hagenberg.

### Tone & Atmosphere

Light-hearted, playful, humorous, and informative.

---

## Requirements

### To Play

* Operating System: Windows / macOS / Linux
* No additional software required

### For Development

* Unity Hub
* Unity Version: **6000.0.60f1**

---

## ▶ Running the Game (Build)

1. Extract the ZIP archive
2. Open the folder `Nicola Game Prototype`
3. Start the correct executable:

   * **Windows:** `Nicola Game.exe`
   * **macOS:** `Nicola Game Prototype.app`
   * **Linux:** `Nicola Game Linux.x86_64`
4. The game should start automatically

---

## 🧑‍💻 Continuing Development (Unity)

1. Clone the repository
2. Open Unity Hub
3. Click **"Add project from disk"**
4. Select the repository folder
5. Make sure the correct Unity version is installed
6. Open the project

---

## 📁 Project Structure

```
Assets/
 ├─ Audio/          → Music and sound effects
 ├─ C# Scripts/     → All scripts used in the game
 ├─ Cutscenes/      → Files for in-game cutscenes
 ├─ Images/         → Background images used in some minigames
 ├─ NPCDialog/      → Dialogue created via Scriptable Dialog files
 ├─ Physics/        → Unused (originally planned for the 2D platformer minigame)
 ├─ Prefabs/        → Prefabs
 ├─ Scenes/         → Unity scenes
 ├─ Sprites/        → Animations, UI elements, and other sprites
Packages/
```

---

## 🚧 TODO

* [ ] Add a slideshow showing student life in Hagenberg in the rhythm game
* [ ] Ensure cutscenes and certain triggers are not retriggered in the overworld
* [ ] Expand the overworld (more of Hagenberg, Linz for the Klimaticket, more of Freistadt, more of the campus, etc.)
* [ ] Use the same overworld tiles in the minigames (where applicable)
* [ ] Add more sprites for NPCs (currently most NPCs share the same sprite)
* [ ] Add interactive points of interest showing real-life images and facts about specific locations
* [ ] Add a skip function for minigames so players cannot get stuck
* [ ] Add sound effects to bring more life into the game
* [ ] Find a meaningful use for the 2D platformer minigame (puzzles, coin usage, etc.)
* [ ] Add more NPCs that provide useful information or make the world feel more alive
* [ ] Expand the EU citizen registration segment with additional tasks or minigames
* [ ] Polish the game overall (bug fixes, improved both overworld and minigame visuals, etc.)

---

## 📜 License

This project is currently intended for educational and development purposes only.
The license may be updated or changed in the future.

---

## ℹ Notes

* The folders `Library/`, `Temp/`, and `Logs/` are **not** included in the repository
* Using a Unity `.gitignore` file is strongly recommended

