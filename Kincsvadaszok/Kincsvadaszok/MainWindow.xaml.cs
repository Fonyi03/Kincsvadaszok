using System.Collections.Generic;
using System.IO; 
using System.Text.Json;
using System.Windows;

namespace Kincsvadaszok
{
    public partial class MainWindow : Window
    {
        private List<Treasure> treasures = new List<Treasure>(); //adatot memóriában tároljuk
        private const string FileName = "treasures.json";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtName.Text) && int.TryParse(txtValue.Text, out int value))
            {
                Treasure newTreasure = new Treasure 
                { 
                    Name = txtName.Text, 
                    Value = value 
                };

                treasures.Add(newTreasure);
                
                RefreshList();
                ClearInputs();
            }
            else
            {
                MessageBox.Show("Please enter a valid name and value!");
            }
        }

        private void RefreshList() // UI frissítésére helper metódus
        {
            lstTreasures.ItemsSource = null;
            lstTreasures.ItemsSource = treasures;
        }

        private void ClearInputs() // textboxok törlése
        {
            txtName.Clear();
            txtValue.Clear();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) //mentés gomb 
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(treasures, options);

                File.WriteAllText(FileName, jsonString);
                MessageBox.Show("Data saved successfully!");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e) // betöltés gomb 
        {
            if (File.Exists(FileName))
            {
                try
                {
                    string jsonString = File.ReadAllText(FileName);
                    treasures = JsonSerializer.Deserialize<List<Treasure>>(jsonString);

                    RefreshList();
                    MessageBox.Show("Data loaded successfully!");
                }
                catch (System.Exception ex)
                {
                     MessageBox.Show($"Error loading data: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No saved file found!");
            }
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.ShowDialog();
            RefreshList();

        }
    }
}