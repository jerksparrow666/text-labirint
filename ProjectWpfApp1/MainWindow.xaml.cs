using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ProjectWpfApp1
{
    public partial class MainWindow : Window
    {
        public class Item
        {
            public string Title { get; set; }
            public string Type { get; set; }
            public string Stats { get; set; }
        }

        public ObservableCollection<Item> Tasks { get; set; }
        public int MaxInventory = 5; // Ограничение объёма инвентаря

         // Карта комнат 5x5 ("" = стены / комнаты нет)
        string[,] map = new string[5, 5]
        {
            { "", "/Images/finish_room_1.jpg", "", "/Images/finish_room_2.jpg", "" },
            { "/Images/room_1.jpg", "/Images/crossroad_1.jpg", "/Images/room_2.jpg", "/Images/crossroad_2.jpg", "/Images/room_3.jpg" },
            { "", "/Images/crossroad_3.jpg", "/Images/room_4.jpg", "/Images/crossroad_4.jpg", "" },
            { "/Images/finish_room_3.jpg", "/Images/crossroad_5.jpg", "/Images/finish_room_4.jpg", "/Images/room_1.jpg", "" },
            { "", "", "", "", "" }
        };

        // Позиция игрока (старт: строка 2, столбец 2)
        int playerRow = 2;
        int playerCol = 2;

        // Финиш (строка 0, столбец 1 или 3)
        int finishRow = 0;
        int finishCol = 1;

        bool isInventoryOpen = false;
        bool isGameOver = false;

        public MainWindow()
        {
            InitializeComponent();

            Tasks = new ObservableCollection<Item>();

            // Добавляем предметы с проверкой лимита
            AddItem(new Item { Title = "Зелье здоровья", Type = "Расходник", Stats = "+50 HP" });
            AddItem(new Item { Title = "Меч", Type = "Оружие", Stats = "Урон: 15" });
            AddItem(new Item { Title = "Щит", Type = "Броня", Stats = "Защита: 10" });

            DataContext = this;

            ShowRoom();
            ShowActions();
        }

        public void AddItem(Item item)
        {
            if (Tasks.Count < MaxInventory)
            {
                Tasks.Add(item);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isGameOver)
            {
                return;
            }

            Button button = sender as Button;
            string action = button.Content.ToString();

            if (isInventoryOpen)
            {
                if (action == "Закрыть")
                {
                    ToggleView();
                    Print("");
                }
            }
            else
            {
                if (action == "Пойти вверх" ||
                    action == "Пойти вниз" ||
                    action == "Пойти вправо" ||
                    action == "Пойти влево")
                {
                    pathCheck(action);
                }
                else if (action == "Просмотр инвентаря")
                {
                    ToggleView();
                    Print("");
                }
            }
        }

        public void pathCheck(string path)
        {
            int nextRow = playerRow;
            int nextCol = playerCol;

            if (path == "Пойти вверх") nextRow--;
            else if (path == "Пойти вниз") nextRow++;
            else if (path == "Пойти влево") nextCol--;
            else if (path == "Пойти вправо") nextCol++;

            // Если выход за границу или пустая клетка
            if (nextRow < 0 || nextRow >= 5 || nextCol < 0 || nextCol >= 5 || map[nextRow, nextCol] == "")
            {
                Print("Туда пройти нельзя");
                return;
            }

            playerRow = nextRow;
            playerCol = nextCol;
            ShowRoom();

            // Проверка на финиш
            if (playerRow == finishRow && playerCol == finishCol)
            {
                isGameOver = true;
                Print("Конец! Вы дошли до финиша.");
                ShowActions();
            }
            else
            {
                Print("");
            }
        }

        public void ShowRoom()
        {
            try
            {
                string imagePath = map[playerRow, playerCol];
                if (string.IsNullOrEmpty(imagePath))
                {
                    MyImage.Source = null;
                    return;
                }

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri($"pack://application:,,,{imagePath}", UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                MyImage.Source = bitmap;
                Print($"Позиция: ({playerRow}, {playerCol})");
            }
            catch (Exception ex)
            {
                Print($"Ошибка загрузки: {ex.Message}");
            }
        }

        public void Print(string text)
        {
            StatusTextBlock.Text = text;
        }

        private void ToggleView()
        {
            isInventoryOpen = !isInventoryOpen;

            if (isInventoryOpen)
            {
                MyImage.Visibility = Visibility.Collapsed;
                MyListBox.Visibility = Visibility.Visible;
                MyListBox.Height = 320; // Высота списка больше картинки

                if (Tasks.Count == 0)
                {
                    Print("- Нет предметов -");
                }
            }
            else
            {
                MyListBox.Visibility = Visibility.Collapsed;
                MyImage.Visibility = Visibility.Visible;
            }

            ShowActions();
        }

        private void ShowActions()
        {
            Button[] buttons = { Button1, Button2, Button3, Button4, Button5 };

            foreach (Button btn in buttons)
            {
                btn.Visibility = Visibility.Collapsed;
            }

            if (isInventoryOpen)
            {
                Button1.Content = "Закрыть";
                Button1.Visibility = Visibility.Visible;
            }
            else
            {
                Button1.Content = "Просмотр инвентаря";
                Button1.Visibility = Visibility.Visible;
                Button2.Content = "Пойти вверх";
                Button2.Visibility = Visibility.Visible;
                Button3.Content = "Пойти вниз";
                Button3.Visibility = Visibility.Visible;
                Button4.Content = "Пойти влево";
                Button4.Visibility = Visibility.Visible;
                Button5.Content = "Пойти вправо";
                Button5.Visibility = Visibility.Visible;
            }
        }
    }
}
