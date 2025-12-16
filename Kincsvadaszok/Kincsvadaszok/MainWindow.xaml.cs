using Kincsvadaszok.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // a .sum() miatt kell!
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kincsvadaszok
{
    public partial class MainWindow : Window
    {
        // csak az eredmenyeket taroljuk a lobby-hoz
        private List<GameResult> gameHistory = new List<GameResult>();
        private const string HistoryFile = "history.json";
        private string currentSavePath = null; // itt jegyezzuk meg h hova mentettunk utoljara

        public MainWindow()
        {
            InitializeComponent();
        }

        // --- 1. a start gomb logikaja ---
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            // nevek beolvasasa a textboxokbol
            string p1Name = txtP1Name.Text;
            string p2Name = txtP2Name.Text;

            // ha ures akkor alapertelmezett nevet adunk
            if (string.IsNullOrWhiteSpace(p1Name)) p1Name = "Player 1";
            if (string.IsNullOrWhiteSpace(p2Name)) p2Name = "Player 2";

            // uj jatek ablak letrehozasa es megjelenitese
            GameWindow gameWindow = new GameWindow(p1Name, p2Name);
            gameWindow.ShowDialog(); // itt megall a kod amig be nem zarjak a jatekot

            // --- jatek vege ---

            // kinyerjuk a jatek ablakbol h mit gyujtottek
            var p1Loot = gameWindow.LootP1 ?? new List<Treasure>();
            var p2Loot = gameWindow.LootP2 ?? new List<Treasure>();

            // modositas: akkor is mentsunk, ha lepeslimit miatt lett vege (meg ha 0 kincs is van)
            if (p1Loot.Count > 0 || p2Loot.Count > 0 || gameWindow.IsDrawBySteps)
            {
                // pontszamitas osszeadassal
                int score1 = p1Loot.Sum(t => t.Value);
                int score2 = p2Loot.Sum(t => t.Value);

                string winner;

                // modositas: eloszor megnezzuk, hogy lepeslimit volt-e
                if (gameWindow.IsDrawBySteps)
                {
                    winner = "Döntetlen (Lépéslimit)";
                }
                else if (score1 > score2) winner = p1Name;
                else if (score2 > score1) winner = p2Name;
                else winner = "Döntetlen";

                // eredmeny objektum letrehozasa a listahoz
                GameResult result = new GameResult
                {
                    Date = DateTime.Now,
                    P1Name = p1Name,
                    P2Name = p2Name,
                    P1Score = score1,
                    P2Score = score2,
                    WinnerName = winner
                };

                // hozzaadjuk a listahoz es frissitjuk a kepernyot
                gameHistory.Add(result);
                RefreshHistory();

                // (opcionalis: ha mar van mentesi hely, automatikusan mentunk oda is)
                if (!string.IsNullOrEmpty(currentSavePath))
                {
                    try
                    {
                        var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                        string json = System.Text.Json.JsonSerializer.Serialize(gameHistory, options);
                        System.IO.File.WriteAllText(currentSavePath, json);
                    }
                    catch { } // ha hiba van itt nem szolunk csak nem mentunk
                }
            }
        }

        // --- 2. lista frissitese ---
        private void RefreshHistory()
        {
            // nullazni kell h frissuljon a ui
            lstHistory.ItemsSource = null;
            lstHistory.ItemsSource = gameHistory;
        }

        // --- 3. mentes / betoltes gombok ---
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. ha meg nincs kivalasztva mentesi hely, nyissuk meg a tallozot
            if (string.IsNullOrEmpty(currentSavePath))
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

                // beallitasok: csak .json fajlokat mutasson, es ez legyen az alap
                saveFileDialog.Filter = "JSON fájlok (*.json)|*.json|Minden fájl (*.*)|*.*";
                saveFileDialog.DefaultExt = ".json";
                saveFileDialog.FileName = "eredmenyek"; // alapertelmezett fajlnev

                // megnyitjuk az ablakot. ha a felhasznalo ranyom a "mentes"-re (true)
                if (saveFileDialog.ShowDialog() == true)
                {
                    // elmentjuk az utvonalat a memoriaba
                    currentSavePath = saveFileDialog.FileName;
                }
                else
                {
                    // ha a megse gombra nyomott, nem csinalunk semmit
                    return;
                }
            }

            // 2. mentes a (mar biztosan letezo) utvonalra
            try
            {
                // json formazasa h olvashato legyen
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(gameHistory, options);

                // fajl irasa
                File.WriteAllText(currentSavePath, json);
                MessageBox.Show($"Sikeres mentés ide:\n{currentSavePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt a mentéskor: " + ex.Message);
                // ha hiba volt (pl. irasvedett hely), toroljuk az utvonalat, hogy legkozelebb ujra kerdezze
                currentSavePath = null;
            }
        }

        // elozmenyek betoltese (ez a fix history.json fajlbol probal)
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(HistoryFile))
            {
                try
                {
                    // beolvassuk es visszaalakitjuk listava
                    string json = File.ReadAllText(HistoryFile);
                    gameHistory = JsonSerializer.Deserialize<List<GameResult>>(json);
                    RefreshHistory();
                    MessageBox.Show("Előzmények betöltve!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba a betöltéskor: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Nincs mentett előzmény fájl.");
            }
        }

        // felbehagyott jatek betoltese .sav fajlbol
        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Kincsvadászok mentés (*sav)|*sav"; // csak a menteseket mutatja

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    // beolvasas es deserialize a savedgame osztalyba
                    string json = File.ReadAllText(ofd.FileName);
                    SavedGame state = JsonSerializer.Deserialize<SavedGame>(json);

                    // atadjuk a betoltott allapotot az uj ablaknak
                    GameWindow gameWindow = new GameWindow(state);
                    gameWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba a játék betöltésekor: " + ex.Message);
                }
            }
        }
    }
}