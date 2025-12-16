# 🏰 Kincsvadászok (Treasure Hunters) v1.0

Egy körökre osztott, helyi többjátékos (hotseat) stratégiai játék, WPF technológiával megvalósítva. A játék célja több kincset gyűjteni az ellenfélnél egy akadályokkal teli, véletlenszerűen generált pályán.

![Verzió](https://img.shields.io/badge/version-1.1-green)
![Nyelv](https://img.shields.io/badge/language-C%23-blue)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)

## ✨ Újdonságok a v1.1 verzióban
* **Fejlett Játékállás Mentés:** A játék bezárásakor a program rákérdez a mentésre. A félbehagyott meccsek (`.sav` fájlok) később bármikor folytathatók pontosan onnan, ahol abbahagytátok.
* **Grafikai Tuning:** Egyedi textúrák a padlóhoz és a falakhoz, karakter ikonok a színes négyzetek helyett.
* **Szeparált Irányítás:** Külön gombkiosztás a két játékosnak (WASD vs Nyilak) a kényelmesebb játékélményért.
* **Lépésszámláló & Döntetlen:** Ha a játékosok összesen 100 lépést tesznek meg, a játék automatikusan döntetlennel zárul (elkerülve a végtelen kergetőzést).

## 🎮 Játékmenet és Szabályok

A játékot ketten játsszák egy billentyűzeten. A pálya egy 10x10-es rács, tele falakkal és kincsekkel.

### Irányítás
| Játékos | Karakter | Mozgás |
| :--- | :--- | :--- |
| **Player 1** | 🟢 Zöld Felfedező | **W, A, S, D** |
| **Player 2** | 🔵 Kék Felfedező | **Nyilak (⬆️⬇️⬅️➡️)** |
| **Egyéb** | | **ESC** (Kilépés / Mentés) |

### Szabályok
1.  **Gyűjtés:** Lépj rá a kincsesládára a begyűjtéshez. Minden kincs véletlenszerű pontszámot ér.
2.  **Akadályok:** A kőfalakon nem lehet átmenni.
3.  **Ütközés:** Nem léphetsz arra a mezőre, ahol a másik játékos áll.
4.  **Győzelem:**
    * Ha elfogynak a kincsek, az nyer, akinek több pontja (értéke) van.
    * Ha eléritek a **100. lépést**, a játék döntetlennel ér véget.

## 💾 Funkciók

* **Lobby Rendszer:** Játékosok elnevezése és meccselőzmények megtekintése.
* **Match History:** A befejezett játékok eredményeit (Nyertes, Pontszámok, Dátum) a program JSON formátumban naplózza (`history.json`).
* **Smart Save:** Kilépéskor (`ESC` vagy ablak bezárása) a rendszer felajánlja a játékállás mentését, amit a főmenü "JÁTÉK FOLYTATÁSA" gombjával tölthetsz vissza.
* **Single File Exe:** A program egyetlen hordozható fájlként futtatható, nem igényel telepítést.

## 🛠 Technológiák
A projekt demonstrálja a modern C# fejlesztési elveket:
* **Nyelv:** C# (.NET 9.0)
* **UI:** WPF (Windows Presentation Foundation) XAML alapokon.
* **Adatkezelés:** `System.Text.Json` a mentésekhez és előzményekhez.
* **Tesztelés:** MSTest keretrendszerrel írt Unit tesztek a logika (Modellek) ellenőrzésére.

## 🚀 Telepítés és Futtatás

Nincs szükség telepítésre!
1.  Töltsd le a legfrissebb `Kincsvadaszok.exe` fájlt a **Releases** menüpontból.
2.  Indítsd el a fájlt.
3.  Jó szórakozást!

## 👤 Szerző
Készítette: **FTP Server Room**
Egyetemi Beadandó Projekt - 2025


