# TrickyArrow

A mobile puzzle game built with Unity.

## Overview

**TrickyArrow** is a line-based puzzle game where players strategically launch directional arrows/lines to clear the level board without colliding into intersecting paths.

* **Core Gameplay**: Tap arrows to launch them forward along their paths.
* **Objective**: Clear all lines on the board to win the level.
* **Failure Condition**: Collisions between line heads and other line bodies cost a life. Depleting all lives results in level failure.
* **Camera Framing**: Dynamic orthographic framing automatically adapts to any level layout and aspect ratio.

## Tech Stack & Specifications

* **Engine**: Unity 6 (`6000.0.58f2`)
* **Render Pipeline**: Universal Render Pipeline (URP)
* **Target Platforms**: Android (ARM64, IL2CPP) / iOS
* **UI**: Unity UI (uGUI) + TextMeshPro
* **Tweening**: DOTween

## Project Setup & Quick Start

1. Open the project in **Unity Hub** using Unity version `6000.0.58f2` (or compatible Unity 6 version).
2. Open the main gameplay scene:
   ```
   Assets/_Game/Scenes/GameScene.unity
   ```
3. Enter Play mode in the Unity Editor to test gameplay.

## License

This project is derived from the open-source Arrows project by Serap Kerem and is distributed under the MIT License. See individual third-party libraries for their respective licenses.
