using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Learning;
using SFML.System;
using SFML.Window;

namespace _9_Cards
{
    public class Program
    {
        static void Main()
        {
            var game = new MyGame();
            game.Start();
        }
    }

    public class MyGame : Game
    {
        private string iconSuccess;
        private string iconCover;
        private readonly List<string> icons = new List<string>();
        private readonly float deltaCardDistance = 120;
        private readonly float offsetBorder = 20;
        private readonly Random rnd = new Random();
        private readonly int delay = 50;

        private int selectedCards = 0;

        public void Start()
        {
            LoadAssets();
            var list = CreateCardList();
            ShuffleList(list);
            AssignCoordinates(list);

            var width = Convert.ToUInt32(offsetBorder) + Convert.ToUInt32(deltaCardDistance * icons.Count);
            var height = Convert.ToUInt32(offsetBorder) + Convert.ToUInt32(deltaCardDistance * 2);

            InitWindow(width, height, "Cards Casino");
            SetFont("assets/comic.ttf");

            var clock = new Clock();
            var isInitialStage = true;

            while (true)
            {
                ClearWindow(43, 73, 32);
                DispatchEvents();

                if (list.Any(x => !x.IsCompleted))
                {
                    var isMouse = GetMouseButtonDown(Mouse.Button.Left);

                    if (isInitialStage)
                    {
                        if (clock.ElapsedTime.AsSeconds() > 5)
                        {
                            HideCards(list);
                            isInitialStage = false;
                        }
                    }
                    else if (isMouse && HandleClick(list, MouseX, MouseY))
                    {
                        if (selectedCards == 2)
                            clock.Restart();
                    }

                    CheckCompletedCards(list);

                    if (selectedCards == 2 && clock.ElapsedTime.AsSeconds() > 2)
                        HideCards(list);

                    DrawCards(list);
                }
                else
                {
                    DrawText(110, 100, "CONGRATS YOU WON!!!", 40);
                }

                DisplayWindow();
                Delay(delay);
            }
        }

        private void CheckCompletedCards(List<Card> list)
        {
            var selected = list.Where(x => x.IsVisible && !x.IsCompleted).ToList();

            if (selectedCards == 2 && selected.Count >= 2 && selected[0].Icon == selected[1].Icon)
            {
                selected[0].IsCompleted = true;
                selected[1].IsCompleted = true;

                selectedCards = 0;
            }
        }

        private bool HandleClick(List<Card> list, int mouseX, int mouseY)
        {
            if (selectedCards >= 2)
                return false;

            var isClicked = false;

            foreach (var l in list)
            {
                if (mouseX >= l.PosCornerX && mouseX <= l.PosCornerX + l.Width && mouseY >= l.PosCornerY && mouseY <= l.PosCornerY + l.Height)
                {
                    l.IsVisible = true;
                    isClicked = true;
                    selectedCards++;
                }
            }

            return isClicked;
        }

        private void HideCards(List<Card> list)
        {
            foreach (var l in list)
            {
                if (!l.IsCompleted)
                    l.IsVisible = false;
            }

            selectedCards = 0;
        }

        private void DrawCards(List<Card> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].IsCompleted)
                {
                    var icon = list[i].IsVisible ? list[i].Icon : iconCover;

                    DrawSprite(icon, list[i].PosCornerX, list[i].PosCornerY);
                    DrawSprite(iconSuccess, list[i].PosCornerX, list[i].PosCornerY);

                    continue;
                }

                if (list[i].IsVisible)
                    DrawSprite(list[i].Icon, list[i].PosCornerX, list[i].PosCornerY);
                else
                    DrawSprite(iconCover, list[i].PosCornerX, list[i].PosCornerY);
            }
        }

        private void LoadAssets()
        {
            iconSuccess = LoadTexture("assets/check-solid.png");
            iconCover = LoadTexture("assets/card_cover.png");
            icons.Add(LoadTexture("assets/Icon_1.png"));
            icons.Add(LoadTexture("assets/Icon_2.png"));
            icons.Add(LoadTexture("assets/Icon_3.png"));
            icons.Add(LoadTexture("assets/Icon_4.png"));
            icons.Add(LoadTexture("assets/Icon_5.png"));
            icons.Add(LoadTexture("assets/Icon_6.png"));
        }

        private List<Card> CreateCardList()
        {
            var result = new List<Card>();

            for (var i = 0; i < icons.Count; i++)
            {
                var c = new Card
                {
                    Height = 100,
                    Width = 100,
                    Icon = icons[i],
                    IsVisible = true,
                };

                result.Add(c);
                result.Add(c.CreateCopy());
            }

            return result;
        }

        private void ShuffleList(List<Card> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                int j;

                do
                {
                    j = rnd.Next(0, list.Count);

                    if (j == i)
                        continue;
                } while (j == i);

                var current = list[i];
                var other = list[j];

                list[j] = current;
                list[i] = other;
            }
        }

        private void AssignCoordinates(List<Card> list)
        {
            var x = offsetBorder;
            var y = offsetBorder;

            for (var i = 0; i < list.Count; i++)
            {
                if (i > 0 && i % 6 == 0)
                {
                    x = offsetBorder;
                    y += deltaCardDistance;
                }

                list[i].PosCornerX = x;
                list[i].PosCornerY = y;

                x += deltaCardDistance;
            }
        }
    }

    public class Card
    {
        public string Icon { get; set; }
        public bool IsVisible { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFailed { get; set; }
        public float PosCornerX { get; set; }
        public float PosCornerY { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Card CreateCopy()
        {
            var c = (Card)MemberwiseClone();
            return c;
        }
    }
}
