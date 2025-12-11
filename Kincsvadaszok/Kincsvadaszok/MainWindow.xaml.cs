using Kincsvadaszok.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // A .Sum() miatt kell!
using System.Text.Json;
using System.Windows;

namespace Kincsvadaszok
{
    public partial class MainWindow : Window
    {
        // Csak az eredményeket tároljuk a Lobby-hoz
        private List<GameResult> gameHistory = new List<GameResult>();
        private const string HistoryFile = "history.json";

        public MainWindow()
        {
            InitializeComponent();
        }

        // --- 1. A START GOMB LOGIKÁJA ---
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Nevek kiolvasása a Lobby-ból
            string p1Name = txtP1Name.Text;
            string p2Name = txtP2Name.Text;

            // Ha üresen hagyták, adjunk alapértelmezett nevet
            if (string.IsNullOrWhiteSpace(p1Name)) p1Name = "Player 1";
            if (string.IsNullOrWhiteSpace(p2Name)) p2Name = "Player 2";

            // Játék indítása a nevekkel
            GameWindow gameWindow = new GameWindow(p1Name, p2Name);
            gameWindow.ShowDialog();

            // --- JÁTÉK VÉGE ---

            // Adatok kinyerése
            var p1Loot = gameWindow.LootP1 ?? new List<Treasure>();
            var p2Loot = gameWindow.LootP2 ?? new List<Treasure>();

            // Csak akkor mentünk, ha szereztek valamit
            if (p1Loot.Count > 0 || p2Loot.Count > 0)
            {
                // Pontszámítás
                int score1 = p1Loot.Sum(t => t.Value);
                int score2 = p2Loot.Sum(t => t.Value);

                // Nyertes megállapítása
                string winner;
                if (score1 > score2) winner = p1Name;
                else if (score2 > score1) winner = p2Name;
                else winner = "Döntetlen";

                // Eredmény objektum
                GameResult result = new GameResult
                {
                    Date = DateTime.Now,
                    P1Name = p1Name,
                    P2Name = p2Name,
                    P1Score = score1,
                    P2Score = score2,
                    WinnerName = winner
                };

                // Mentés a listába
                gameHistory.Add(result);

                // Képernyő frissítése
                RefreshHistory();
            }
        }

        // --- 2. LISTA FRISSÍTÉSE ---
        private void RefreshHistory()
        {
            lstHistory.ItemsSource = null;
            lstHistory.ItemsSource = gameHistory;
        }

        // --- 3. MENTÉS / BETÖLTÉS GOMBOK ---
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(gameHistory, options);
                File.WriteAllText(HistoryFile, json);
                MessageBox.Show("Előzmények sikeresen mentve!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a mentéskor: " + ex.Message);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(HistoryFile))
            {
                try
                {
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

        // Fontos: Mivel a felületről töröltük a manuális hozzáadást (AddButton_Click)
        // és a kincses listát (lstTreasures), azokat a kódokat innen is töröltük,
        // hogy ne okozzanak "CS0103" hibát.
    }
}