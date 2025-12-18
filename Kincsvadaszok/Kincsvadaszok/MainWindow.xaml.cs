using Kincsvadaszok.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace Kincsvadaszok
{
    public partial class MainWindow : Window
    {
        // ebben taroljuk az eredmenyeket
        private List<GameResult> gameHistory = new List<GameResult>();

        // ez a fajl neve, ide mentunk automatikusan
        private const string HistoryFile = "history.json";

        public MainWindow()
        {
            InitializeComponent();

            // INDULASKOR: automatikusan betoltjuk az elozmenyeket
            LoadHistory();
        }

        // --- 1. JATEK INDITASA ---
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            string p1Name = txtP1Name.Text;
            string p2Name = txtP2Name.Text;

            if (string.IsNullOrWhiteSpace(p1Name)) p1Name = "Player 1";
            if (string.IsNullOrWhiteSpace(p2Name)) p2Name = "Player 2";

            GameWindow gameWindow = new GameWindow(p1Name, p2Name);
            gameWindow.ShowDialog();

            // --- JATEK VEGE ---

            var p1Loot = gameWindow.LootP1 ?? new List<Treasure>();
            var p2Loot = gameWindow.LootP2 ?? new List<Treasure>();

            // ha tortent valami (van kincs vagy lepeslimit), akkor naplozzuk
            if (p1Loot.Count > 0 || p2Loot.Count > 0 || gameWindow.IsDrawBySteps)
            {
                int score1 = p1Loot.Sum(t => t.Value);
                int score2 = p2Loot.Sum(t => t.Value);
                string winner;

                if (gameWindow.IsDrawBySteps) winner = "Döntetlen (Lépéslimit)";
                else if (score1 > score2) winner = p1Name;
                else if (score2 > score1) winner = p2Name;
                else winner = "Döntetlen";

                GameResult result = new GameResult
                {
                    Date = DateTime.Now,
                    P1Name = p1Name,
                    P2Name = p2Name,
                    P1Score = score1,
                    P2Score = score2,
                    WinnerName = winner
                };

                // hozzaadjuk a listahoz
                gameHistory.Add(result);

                // frissitjuk a listat a kepernyon
                RefreshHistory();

                // AUTOMATIKUS MENTES: azonnal irjuk fajlba!
                SaveHistory();
            }
        }

        // --- 2. LISTA KEZELESE (BETOLTES / FRISSITES / MENTES) ---

        private void LoadHistory()
        {
            // megnezzuk letezik-e a fajl
            if (File.Exists(HistoryFile))
            {
                try
                {
                    string json = File.ReadAllText(HistoryFile);
                    gameHistory = JsonSerializer.Deserialize<List<GameResult>>(json) ?? new List<GameResult>();
                    RefreshHistory();
                }
                catch
                {
                    // ha serult a fajl, nem csinalunk semmit, marad ures a lista
                }
            }
        }

        private void SaveHistory()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(gameHistory, options);
                File.WriteAllText(HistoryFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nem sikerült az automata mentés: " + ex.Message);
            }
        }

        private void RefreshHistory()
        {
            lstHistory.ItemsSource = null;
            lstHistory.ItemsSource = gameHistory;
        }

        // --- 3. TORLES GOMB ---
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Biztosan törölni szeretnéd az összes előzményt?", "Törlés", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // toroljuk a memoriabol
                gameHistory.Clear();
                RefreshHistory();

                // toroljuk a fajlbol is (felulirjuk egy ures listaval)
                SaveHistory();
            }
        }

        // --- 4. FELBEHAGYOTT JATEK BETOLTESE (.sav) ---
        // ez marad a regi, mert ezt a felhasznalo inditja
        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Kincsvadász Mentés (*.sav)|*.sav";

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    string json = File.ReadAllText(ofd.FileName);
                    SavedGame state = JsonSerializer.Deserialize<SavedGame>(json);
                    GameWindow gameWindow = new GameWindow(state);
                    gameWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba a betöltéskor: " + ex.Message);
                }
            }
        }
    }
}