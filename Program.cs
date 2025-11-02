class Program
{
    const int playingFieldWithBordersLength = 20 + 2;
    const int playingFieldWithBordersWidth = 40 + 2;

    const int minObstacleCount = 4;
    const int maxObstacleCount = 20;

    const int snakeGameFPS = 10;
    const int snakeGameEatenFoodWinCount = 20;

    const char nothing = '·';
    const char wall = '█';
    const char snakeHead = '◯';
    const char snakeBody = '⬤';
    const char food = '★';

    static Random random = new Random();

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        bool isRunning = true;

        while (isRunning)
        {
            PrepareSnakeGame();

            isRunning = IsReplayConfirmed();
        }
    }

    static void PrepareSnakeGame()
    {
        Console.Clear();
        Console.CursorVisible = false;

        char[,] playingField = CreatePlayingField();
        int snakeLength = 2 + 1;
        (int y, int x)[] snakePositions = new (int y, int x)[snakeLength + snakeGameEatenFoodWinCount];
        int eatenFoodCount = 0;

        PlaceSnake(playingField, snakePositions, snakeLength);
        PlaceRandomObstacles(playingField);
        PlaceFood(playingField);
        ShowSnakeGameState(playingField, snakeLength, eatenFoodCount);
        StartSnakeGame(playingField, snakePositions, snakeLength, eatenFoodCount);

        Console.CursorVisible = true;
    }

    static char[,] CreatePlayingField()
    {
        char[,] playingField = new char[playingFieldWithBordersLength, playingFieldWithBordersWidth];

        for (int y = 0; y < playingFieldWithBordersLength; y++)
        {
            for (int x = 0; x < playingFieldWithBordersWidth; x++)
            {
                if (y == 0 || y == playingFieldWithBordersLength - 1)
                {
                    playingField[y, x] = wall;
                }
                else if (x == 0 || x == playingFieldWithBordersWidth - 1)
                {
                    playingField[y, x] = wall;
                }
                else
                {
                    playingField[y, x] = nothing;
                }
            }
        }
        return playingField;
    }

    static void PlaceSnake(char[,] playingField, (int y, int x)[] positions, int length)
    {
        int y = random.Next(1, playingFieldWithBordersLength - 1);
        int x = random.Next(1, playingFieldWithBordersWidth - length - 1);

        for (int i = length - 1; i > 0; i--)
        {
            positions[i] = (y, x + length - 1 - i);
            playingField[positions[i].y, positions[i].x] = snakeBody;
        }
        positions[0] = (y, x + length - 1);
        playingField[positions[0].y, positions[0].x] = snakeHead;
    }

    static void PlaceRandomObstacles(char[,] playingField)
    {
        for (int i = 0; i < random.Next(minObstacleCount, maxObstacleCount + 1); i++)
        {
            int obstacleType = random.Next(1, 7 + 1);

            switch (obstacleType)
            {
                case 1:
                    PlaceRectangle(playingField, random.Next(2, 4 + 1), random.Next(2, 4 + 1));
                    break;
                case 2:
                    PlaceVerticalLine(playingField, random.Next(4, 8 + 1));
                    break;
                case 3:
                    PlaceHorizontalLine(playingField, random.Next(4, 8 + 1));
                    break;
                case 4:
                    PlaceUpLeftCorner(playingField, random.Next(4, 8 + 1), random.Next(4, 8 + 1));
                    break;
                case 5:
                    PlaceUpRightCorner(playingField, random.Next(4, 8 + 1), random.Next(4, 8 + 1));
                    break;
                case 6:
                    PlaceDownLeftCorner(playingField, random.Next(4, 8 + 1), random.Next(4, 8 + 1));
                    break;
                case 7:
                    PlaceDownRightCorner(playingField, random.Next(4, 8 + 1), random.Next(4, 8 + 1));
                    break;
            }
        }
    }

    static void PlaceRectangle(char[,] playingField, int sizeY, int sizeX)
    {
        if (IsSuitableCellFound(playingField, sizeY, sizeX, 1, out (int y, int x) randomSuitableCell))
        {
            for (int offsetY = 0; offsetY < sizeY; offsetY++)
            {
                for (int offsetX = 0; offsetX < sizeX; offsetX++)
                {
                    playingField[randomSuitableCell.y + offsetY, randomSuitableCell.x + offsetX] = wall;
                }
            }
        }
    }

    static void PlaceVerticalLine(char[,] playingField, int sizeY, int sizeX = 1)
    {
        PlaceRectangle(playingField, sizeY, sizeX);
    }

    static void PlaceHorizontalLine(char[,] playingField, int sizeX, int sizeY = 1)
    {
        PlaceRectangle(playingField, sizeY, sizeX);
    }

    static void PlaceUpLeftCorner(char[,] playingField, int sizeY, int sizeX)
    {
        if (IsSuitableCellFound(playingField, sizeY, sizeX, 1, out (int y, int x) randomSuitableCell))
        {
            for (int offsetY = 0; offsetY < sizeY; offsetY++)
            {
                playingField[randomSuitableCell.y + offsetY, randomSuitableCell.x] = wall;
            }

            for (int offsetX = 0; offsetX < sizeX; offsetX++)
            {
                playingField[randomSuitableCell.y, randomSuitableCell.x + offsetX] = wall;
            }
        }
    }

    static void PlaceUpRightCorner(char[,] playingField, int sizeY, int sizeX)
    {
        if (IsSuitableCellFound(playingField, sizeY, sizeX, 1, out (int y, int x) randomSuitableCell))
        {
            for (int offsetY = 0; offsetY < sizeY; offsetY++)
            {
                playingField[randomSuitableCell.y + offsetY, randomSuitableCell.x + sizeX - 1] = wall;
            }

            for (int offsetX = 0; offsetX < sizeX; offsetX++)
            {
                playingField[randomSuitableCell.y, randomSuitableCell.x + offsetX] = wall;
            }
        }
    }

    static void PlaceDownLeftCorner(char[,] playingField, int sizeY, int sizeX)
    {
        if (IsSuitableCellFound(playingField, sizeY, sizeX, 1, out (int y, int x) randomSuitableCell))
        {
            for (int offsetY = 0; offsetY < sizeY; offsetY++)
            {
                playingField[randomSuitableCell.y + offsetY, randomSuitableCell.x] = wall;
            }

            for (int offsetX = 0; offsetX < sizeX; offsetX++)
            {
                playingField[randomSuitableCell.y + sizeY - 1, randomSuitableCell.x + offsetX] = wall;
            }
        }
    }

    static void PlaceDownRightCorner(char[,] playingField, int sizeY, int sizeX)
    {
        if (IsSuitableCellFound(playingField, sizeY, sizeX, 1, out (int y, int x) randomSuitableCell))
        {
            for (int offsetY = 0; offsetY < sizeY; offsetY++)
            {
                playingField[randomSuitableCell.y + offsetY, randomSuitableCell.x + sizeX - 1] = wall;
            }

            for (int offsetX = 0; offsetX < sizeX; offsetX++)
            {
                playingField[randomSuitableCell.y + sizeY - 1, randomSuitableCell.x + offsetX] = wall;
            }
        }
    }

    static bool IsSuitableCellFound(char[,] playingField, int sizeY, int sizeX, int sizeAround,
                                    out (int y, int x) randomSuitableCell)
    {
        List<(int y, int x)> suitableCells = new List<(int y, int x)>();

        for (int y = 1 + sizeAround; y < playingFieldWithBordersLength - 1 - sizeY - sizeAround; y++)
        {
            for (int x = 1 + sizeAround; x < playingFieldWithBordersWidth - 1 - sizeX - sizeAround; x++)
            {
                bool isCellSuitable = true;

                for (int offsetY = -sizeAround; offsetY < sizeY + sizeAround; offsetY++)
                {
                    for (int offsetX = -sizeAround; offsetX < sizeX + sizeAround; offsetX++)
                    {
                        if (playingField[y + offsetY, x + offsetX] != nothing)
                        {
                            isCellSuitable = false;
                        }
                    }
                }

                if (isCellSuitable)
                {
                    suitableCells.Add((y, x));
                }
            }
        }

        if (suitableCells.Count > 0)
        {
            randomSuitableCell = suitableCells[random.Next(suitableCells.Count)];
            return true;
        }
        randomSuitableCell = (-1, -1);
        return false;
    }

    static void PlaceFood(char[,] playingField)
    {
        if (IsSuitableCellFound(playingField, 1, 1, 0, out (int y, int x) randomSuitableCell))
        {
            playingField[randomSuitableCell.y, randomSuitableCell.x] = food;
        }
    }

    static void ShowSnakeGameState(char[,] playingField, int snakeLength, int eatenFoodCount)
    {
        Console.SetCursorPosition(0, 0);

        for (int y = 0; y < playingFieldWithBordersLength; y++)
        {
            for (int x = 0; x < playingFieldWithBordersWidth; x++)
            {
                Console.Write(playingField[y, x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine($"\nДлина змейки: {snakeLength}" +
                          $"\nКол-во съеденной еды: {eatenFoodCount}/{snakeGameEatenFoodWinCount}");
    }

    static void StartSnakeGame(char[,] playingField, (int y, int x)[] positions, int length, int eatenFoodCount)
    {
        (int y, int x) direction = (0, 0);

        while (true)
        {
            direction = UpdateSnakeDirection(direction);

            if (direction != (0, 0))
            {
                (bool isCollided, bool isFoodEaten) = MoveSnake(playingField, positions, direction, ref length);

                if (isCollided)
                {
                    Console.WriteLine("\nВы проиграли!");
                    return;
                }
                else if (isFoodEaten)
                {
                    eatenFoodCount++;

                    if (eatenFoodCount == snakeGameEatenFoodWinCount)
                    {
                        ShowSnakeGameState(playingField, length, eatenFoodCount);
                        Console.WriteLine("\nВы выиграли!");
                        return;
                    }
                    PlaceFood(playingField);
                }
                ShowSnakeGameState(playingField, length, eatenFoodCount);
                Thread.Sleep(1000 / snakeGameFPS);
            }
        }
    }

    static (int y, int x) UpdateSnakeDirection((int y, int x) direction)
    {
        if (Console.KeyAvailable)
        {
            if (direction == (0, 0))
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.W:
                        direction = (-1, 0);
                        break;
                    case ConsoleKey.S:
                        direction = (1, 0);
                        break;
                    case ConsoleKey.D:
                        direction = (0, 1);
                        break;
                }
            }
            else
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.W:
                        if (direction.y == 0)
                        {
                            direction = (-1, 0);
                        }
                        break;
                    case ConsoleKey.S:
                        if (direction.y == 0)
                        {
                            direction = (1, 0);
                        }
                        break;
                    case ConsoleKey.A:
                        if (direction.x == 0)
                        {
                            direction = (0, -1);
                        }
                        break;
                    case ConsoleKey.D:
                        if (direction.x == 0)
                        {
                            direction = (0, 1);
                        }
                        break;
                }
            }
        }
        return direction;
    }

    static (bool isCollided, bool isFoodEaten) MoveSnake(char[,] playingField, (int y, int x)[] positions,
                                                         (int y, int x) direction, ref int length)
    {
        char targetCell = playingField[positions[0].y + direction.y, positions[0].x + direction.x];

        bool isCollided = targetCell == wall || targetCell == snakeBody;
        bool isFoodEaten = targetCell == food;

        if (isCollided)
        {
            return (isCollided, false);
        }

        if (isFoodEaten)
        {
            length++;
            positions[length - 1] = positions[length - 2];
        }
        playingField[positions[length - 1].y, positions[length - 1].x] = nothing;

        for (int i = length - 1; i > 0; i--)
        {
            positions[i] = positions[i - 1];
            playingField[positions[i].y, positions[i].x] = snakeBody;
        }
        positions[0] = (positions[0].y + direction.y, positions[0].x + direction.x);
        playingField[positions[0].y, positions[0].x] = snakeHead;

        return (false, isFoodEaten);
    }

    static bool IsReplayConfirmed()
    {
        Console.Write("\nНажмите Enter/Escape, чтобы начать заново/вернуться в меню");

        while (true)
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.Enter:
                    return true;
                case ConsoleKey.Escape:
                    return false;
            }
        }
    }
}