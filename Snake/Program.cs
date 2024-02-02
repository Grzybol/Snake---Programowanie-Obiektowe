using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static int screenWidth = 80; // Szerokość konsoli
    static int screenHeight = 40; // Wysokość konsoli
    static int snakeX, snakeY, fruitX, fruitY, score;
    static bool isGameOver, isGamePaused;
    static Queue<int> snakeBodyX = new Queue<int>();
    static Queue<int> snakeBodyY = new Queue<int>();
    static char direction = 'R'; // Kierunek początkowy: 'R' - w prawo
    static int scoreDisplayY = screenHeight; // Pozycja wyniku

    static void Main()
    {
        // Zwiększenie szerokości konsoli i bufora o 2, aby zmieścić znaki na krawędziach
        Console.WindowHeight = screenHeight + 1;
        Console.WindowWidth = screenWidth + 2;
        Console.BufferHeight = screenHeight + 1;
        Console.BufferWidth = screenWidth + 2;

        InitializeGame();

        Thread inputThread = new Thread(InputListener);
        inputThread.Start();

        while (!isGameOver)
        {
            if (!isGamePaused)
            {
                Update();
                Draw();
            }
            Thread.Sleep(75);
        }

        Console.Clear();
        Console.WriteLine("Gra zakończona! Twój wynik: " + score);
    }

    static void InitializeGame()
    {
        snakeX = screenWidth / 2;
        snakeY = screenHeight / 2;
        RandomizeFruitPosition();
        score = 0;
        isGameOver = false;
        isGamePaused = false;
    }

    static void RandomizeFruitPosition()
    {
        Random random = new Random();
        fruitX = random.Next(1, screenWidth - 1);
        fruitY = random.Next(1, screenHeight - 1);
    }

    static void InputListener()
    {
        while (!isGameOver)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.Spacebar)
                {
                    isGamePaused = !isGamePaused;
                }
                else if (key == ConsoleKey.A)
                {
                    // Zmiana kierunku w lewo o 90 stopni
                    switch (direction)
                    {
                        case 'U':
                            direction = 'L'; // Zmiana z góry na lewo
                            break;
                        case 'D':
                            direction = 'R'; // Zmiana z dołu na prawo
                            break;
                        case 'L':
                            direction = 'D'; // Zmiana z lewej na dół
                            break;
                        case 'R':
                            direction = 'U'; // Zmiana z prawej na górę
                            break;
                    }
                }
                else if (key == ConsoleKey.D)
                {
                    // Zmiana kierunku w prawo o 90 stopni
                    switch (direction)
                    {
                        case 'U':
                            direction = 'R'; // Zmiana z góry na prawo
                            break;
                        case 'D':
                            direction = 'L'; // Zmiana z dołu na lewo
                            break;
                        case 'L':
                            direction = 'U'; // Zmiana z lewej na górę
                            break;
                        case 'R':
                            direction = 'D'; // Zmiana z prawej na dół
                            break;
                    }
                }
            }
        }
    }

    static void Update()
    {
        int newSnakeX = snakeX;
        int newSnakeY = snakeY;

        switch (direction)
        {
            case 'L':
                newSnakeX--;
                break;
            case 'R':
                newSnakeX++;
                break;
            case 'U':
                newSnakeY++;
                break;
            case 'D':
                newSnakeY--;
                break;
        }

        if (snakeX == fruitX && snakeY == fruitY)
        {
            score++;
            RandomizeFruitPosition();
        }
        else
        {
            snakeBodyX.Enqueue(snakeX);
            snakeBodyY.Enqueue(snakeY);

            if (snakeBodyX.Count > score)
            {
                snakeBodyX.Dequeue();
                snakeBodyY.Dequeue();
            }
        }

        if (newSnakeX <= 0 || newSnakeX >= screenWidth || newSnakeY <= 0 || newSnakeY >= screenHeight)
        {
            isGameOver = true;
        }

        for (int i = 0; i < snakeBodyX.Count; i++)
        {
            if (newSnakeX == snakeBodyX.ToArray()[i] && newSnakeY == snakeBodyY.ToArray()[i])
            {
                isGameOver = true;
            }
        }

        snakeX = newSnakeX;
        snakeY = newSnakeY;
    }

    static void Draw()
    {
        Console.Clear();

        for (int i = 0; i < screenWidth; i++)
        {
            Console.SetCursorPosition(i, 0);
            Console.Write("#");
            Console.SetCursorPosition(i, screenHeight - 1);
            Console.Write("#");
        }

        for (int i = 0; i < screenHeight; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write("#");
            Console.SetCursorPosition(screenWidth - 1, i);
            Console.Write("#");
        }

        Console.SetCursorPosition(fruitX, fruitY);
        Console.Write("*");

        Console.SetCursorPosition(snakeX, snakeY);
        Console.Write("O");

        for (int i = 0; i < snakeBodyX.Count; i++)
        {
            Console.SetCursorPosition(snakeBodyX.ToArray()[i], snakeBodyY.ToArray()[i]);
            Console.Write("o");
        }

        if (isGamePaused)
        {
            Console.SetCursorPosition(screenWidth / 2 - 5, screenHeight / 2);
            Console.Write("Gra zatrzymana");
        }

        Console.SetCursorPosition(0, scoreDisplayY);
        Console.Write("Wynik: " + score);
    }
}
