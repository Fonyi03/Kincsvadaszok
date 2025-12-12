# Kincsvadászok (Treasure Hunters) v1.0

WPF alapú, helyi többjátékos (local multiplayer) stratégiai játék, egyedi grafikával és perzisztens adatmentéssel. A projekt demonstrálja a C# és WPF technológiák használatát, a JSON szerializációt, valamint az automatizált tesztelést.

## 🚀 Újdonságok a v1.0 verzióban
- **Teljes grafikai csomag:** Egyedi karakterek, kincsesláda ikonok és kőfal textúrák a színes négyzetek helyett.
- **Okos mentés:** A felhasználó tallózhatja ki, hova és milyen néven szeretné menteni az eredményeket (`SaveFileDialog`).
- **Single File Exe:** A program egyetlen hordozható .exe fájlként is futtatható.
- **Akadályrendszer:** A pályán véletlenszerűen generált falak nehezítik a mozgást.

## 🎮 Funkciók

### Játékmenet
- **Lobby Rendszer:** Játékosok elnevezése indítás előtt.
- **Kétjátékos Mód (Hotseat):** Körökre osztott játékmenet közös billentyűzeten.
- **Dinamikus Pálya:** 10x10-es rács, véletlenszerű kincsekkel és akadályokkal.
- **Pontozás:** A győzelem az összegyűjtött kincsek **értéke** alapján dől el.

### Technikai Háttér
- **Match History:** Eredmények (Dátum, Nyertes, Pontszámok) naplózása JSON formátumban.
- **Unit Tesztek:** MSTest alapú tesztprojekt a kritikus üzleti logika (modellek, pontszámítás) ellenőrzésére.
- **Resource Kezelés:** A képek és ikonok beágyazott erőforrásként utaznak a programmal.

## 🕹 Így játssz
1. **Lobby:** Írd be a **Játékos 1** (Zöld lovag) és **Játékos 2** (Kék varázsló) nevét.
2. **Start:** Kattints a **JÁTÉK INDÍTÁSA** gombra.
3. **Mozgás:** Használd a **Nyilakat** (Arrow Keys).
   - A fejléc jelzi, kinek a köre van.
   - A falakon (szürke kő) nem lehet átmenni.
   - Lépj a kincsesládákra a begyűjtéshez.
4. **Vége:** Ha elfogyott a kincs, a játék kihirdeti a győztest, és visszavisz a Lobby-ba.
5. **Mentés:** Az "Előzmények Mentése" gombbal exportálhatod az eredményeket.

## 🛠 Technológiák
- **Nyelv:** C# (.NET 9.0)
- **UI:** WPF (Windows Presentation Foundation)
- **Tesztelés:** MSTest Framework
- **IDE:** Visual Studio 2022 Community

## 📦 Telepítés és Futtatás
A program nem igényel telepítést.
1. Töltsd le a `Kincsvadaszok.exe` fájlt a Releases oldalról (vagy a `bin/Release` mappából).
2. Indítsd el.
3. Jó játékot!

## 👤 Szerző
FTP Server Room
