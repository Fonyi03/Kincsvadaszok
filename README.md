# Kincsvadászok (Treasure Hunters)

WPF alapú, helyi többjátékos (local multiplayer) stratégiai játék, ahol két játékos verseng a kincsek összegyűjtéséért egy négyzethálós pályán. A projekt demonstrálja a C# és WPF technológiák használatát, a JSON alapú adatkezelést, valamint az automatizált tesztelést (Unit Testing).

## 🚀 Funkciók

### Játékmenet
- **Lobby Rendszer:** A játékosok megadhatják a nevüket a játék kezdete előtt.
- **Kétjátékos Mód:** Körökre osztott játékmenet (Turn-based) közös billentyűzeten.
- **Dinamikus Pálya:** Véletlenszerűen generált kincsek a térképen.
- **Pontozás:** A győzelem az összegyűjtött kincsek **értéke** alapján dől el (nem csak a darabszám számít!).

### Adatkezelés & Architektúra
- **Match History:** A lejátszott meccsek eredményeinek (Dátum, Nyertes, Pontszámok) automatikus mentése `history.json` fájlba.
- **JSON Serializáció:** `System.Text.Json` használata az adatok perzisztens tárolásához.
- **Unit Tesztek:** Külön projekt (`Kincsvadaszok.Tests`) a logika ellenőrzésére (MSTest keretrendszer).

## 🎮 Így játssz
1. Írd be a **Játékos 1** (Zöld) és **Játékos 2** (Kék) nevét a főképernyőn.
2. Kattints a **JÁTÉK INDÍTÁSA** gombra.
3. Használd a **Nyilakat** (Arrow Keys) a mozgáshoz.
   - A játék kiírja, kinek a köre következik.
   - Lépj rá a sárga mezőkre a kincsek felvételéhez.
4. Ha az összes kincs elfogyott, a játék véget ér, és kihirdeti a győztest.
5. Az eredmény bekerül a főképernyőn látható listába.

## 🛠 Technológiák
- **Nyelv:** C# (.NET 6/8)
- **UI:** WPF (Windows Presentation Foundation) - XAML
- **Tesztelés:** MSTest Framework
- **Környezet:** Visual Studio 2022

## 🧪 Tesztelés
A projekt tartalmaz egy külön teszt projektet, amely ellenőrzi:
- A kincsek létrehozását.
- A győzelmi logika és pontszámítás helyességét.
- Az eredmények szöveges formázását.

Futtatás Visual Studio-ban: `Test` -> `Run All Tests`.

## 👤 Szerző
FTP Server Room