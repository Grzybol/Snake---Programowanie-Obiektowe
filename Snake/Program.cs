﻿using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static int screenWidth = 40; // Szerokość konsoli
    static int screenHeight = 20; // Wysokość konsoli
    static int snakeX, snakeY, fruitX, fruitY, score;
    static bool isGameOver, isGamePaused;
    static Queue<int> snakeBodyX = new Queue<int>();
    static Queue<int> snakeBodyY = new Queue<int>();
    static char direction = 'R'; // Kierunek początkowy: 'R' - w prawo
    static int scoreDisplayY = screenHeight; // Pozycja wyniku
    static int snakeSpeedHorizontal = 50; // Prędkość w poziomie jest 2x wieksza niż w pionie, ponieważ "rysuje" znaki w liniach konsoli zamiast rysować poszczególne piksele konsoli i trzeba przez to dostosować prędkość poruszania się/całej gry
    static int snakeSpeedVertical = 100; 


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
            //Thread.Sleep(15);
        }

        Console.Clear();
        Console.WriteLine("Gra zakończona! Twój wynik: " + score);
    }

    static void InitializeGame()
    {
        //ustawianie weża na środku mapy/ekranu
        snakeX = screenWidth / 2; 
        snakeY = screenHeight / 2;
        RandomizeFruitPosition();
        score = 0;
        isGameOver = false;
        isGamePaused = false;

        // 5-sekundowe odliczanie
        for (int i = 5; i > 0; i--)
        {
            Console.Clear();
            Console.SetCursorPosition(screenWidth / 2 - 5, screenHeight / 2);
            Console.Write("Gra rozpocznie się za " + i + "...");
            Thread.Sleep(1000); // Czekaj 1 sekundę
        }

        Console.Clear();
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
                //Pauzowanie gry na spacji - pierwszy z 3 przycisków w grze
                if (key == ConsoleKey.Spacebar)
                {
                    isGamePaused = !isGamePaused;
                }
                // Zmiana kierunku w lewo o 90 stopni pod "A" - drugi przycisk w grze
                else if (key == ConsoleKey.A)
                {
                    
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
                // Zmiana kierunku w prawo o 90 stopni - trzeci przycisk
                else if (key == ConsoleKey.D)
                {
                    
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
                Thread.Sleep(snakeSpeedHorizontal); // Prędkość w poziomie
                break;
            case 'R':
                newSnakeX++;
                Thread.Sleep(snakeSpeedHorizontal); // Prędkość w poziomie
                break;
            case 'U':
                newSnakeY--;
                Thread.Sleep(snakeSpeedVertical); // Prędkość w pionie
                break;
            case 'D':
                newSnakeY++;
                Thread.Sleep(snakeSpeedVertical); // Prędkość w pionie
                break;
        }
        //dodawanie punktów
        if (snakeX == fruitX && snakeY == fruitY)
        {
            score++;
            RandomizeFruitPosition();
        }
        //zapisywanie ruchów węża do kolejki
        else
        {
            //dodanie pozycji głowy węża na początek kolejki
            snakeBodyX.Enqueue(snakeX);
            snakeBodyY.Enqueue(snakeY);
            //usuwanie ostatniego elementu węża z kolejki
            if (snakeBodyX.Count > score)
            {
                snakeBodyX.Dequeue();
                snakeBodyY.Dequeue();
            }
        }
        //uderzenie w sciane
        if (newSnakeX <= 0 || newSnakeX >= screenWidth || newSnakeY <= 0 || newSnakeY >= screenHeight)
        {
            isGameOver = true;
        }
        //sprawdzenie, czy wąż nie zjada samego siebie
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
        //poziome granice
        for (int i = 0; i < screenWidth; i++)
        {
            Console.SetCursorPosition(i, 0);
            Console.Write("#");
            Console.SetCursorPosition(i, screenHeight - 1);
            Console.Write("#");
        }
        //pionowe granice
        for (int i = 0; i < screenHeight; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write("#");
            Console.SetCursorPosition(screenWidth - 1, i);
            Console.Write("#");
        }
        //owoc
        Console.SetCursorPosition(fruitX, fruitY);
        Console.Write("*");
        //głowa węża
        Console.SetCursorPosition(snakeX, snakeY);
        Console.Write("O");
        //ciało węża
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