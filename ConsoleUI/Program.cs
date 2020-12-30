using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleshipLiteLibrary;
using BattleshipLiteLibrary.Models;

namespace BattleShipLite
{
    class Program
    {
        static void Main(string[] args)
        {           
            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel opponent = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                // Display grid from activePlayer on where they fired
                DisplayShotGrid(activePlayer);

                // Ask activePlayer for a shot
                // Determine if shot is valid
                // Determine shot results
                RecordPlayerShot(activePlayer, opponent);

                // Determine if game is over
                bool doesGameContinue = GameLogic.PlayerStillActive(opponent);

                if (doesGameContinue) // Swap player position, if no winner found. (activePlayer swaps with opponent)
                {
                    // Use Tuple to swap
                    (activePlayer, opponent) = (opponent, activePlayer);
                }
                else // If over, set activePlayer as winner
                {
                    winner = activePlayer;
                }

                if (winner != null)
                {
                    IdentifyWinner(winner);
                }
                else
                {
                    Console.Clear();
                }
            } while (winner == null); // Continue loop while no winner is found    
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Color("Cyan");
            Console.WriteLine(" _ _ _         _                                   _                      _ ");
            Console.WriteLine("| | | | ___   | |_  ___  _ _  ___    ___    _ _ _ <_>._ _ ._ _  ___  _ _ | |");
            Console.WriteLine("| | | |/ ._>  | . |<_> || | |/ ._>  <_> |  | | | || || ' || ' |/ ._>| '_>|_/");
            Console.WriteLine(@"|__/_/ \___.  |_|_|<___||__/ \___.  <___|  |__/_/ |_||_|_||_|_|\___.|_|  <_>");
            Console.WriteLine("");
            Color("White");
            Console.WriteLine($"Congratulations to {winner.Name}!");
            Console.WriteLine($"{winner.Name} took {GameLogic.GetShotCount(winner)} shots to win.");
            Console.WriteLine("Press enter to end game.");
        }

        private static void Color(string v)
        {
            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), v);
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel opponent)
        {
            bool isValidShot = false;
            string row = "";
            int column = 0;
            do
            {
                string shot = AskForShot(activePlayer); // Asks for shot (Syntax example : B2)

                bool isSyntaxOk = GameLogic.ValidateSyntax(shot);

                if (isSyntaxOk == false)
                {
                    Color("DarkRed");
                    Console.WriteLine("Invalid syntax. Use e.g. 'B2'. Try again.");
                    Color("White");
                    continue;
                }

                (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot); // Determine row and column (split) 

                isValidShot = GameLogic.ValidateShot(activePlayer, row, column); // Determine if valid shot or not
                if (isValidShot == false)
                {
                    Color("DarkRed");
                    Console.WriteLine("You already placed a shot here. Try again.");
                    Color("White");
                }
            } while (isValidShot == false); // Go to beginning, if not valid shot

            bool isAHit = GameLogic.IdentifyShotResult(opponent, row, column); // Determine result - hit or miss

            if (isAHit == true)
            {
                DisplayHitMessage();
            }
            else
            {
                DisplayMissMessage();
            }

            GameLogic.AdjustShotGridStatus(activePlayer, row, column, isAHit);
            //GameLogic.MarkShotResult(activePlayer, row, column, IsAHit); // Record results    
        }

        private static void DisplayMissMessage()
        {
            Color("DarkYellow");
            Console.WriteLine("");
            Console.WriteLine(" ███╗   ███╗██╗███████╗███████╗          ");
            Console.WriteLine(" ████╗ ████║██║██╔════╝██╔════╝          ");
            Console.WriteLine(" ██╔████╔██║██║███████╗███████╗          ");
            Console.WriteLine(" ██║╚██╔╝██║██║╚════██║╚════██║          ");
            Console.WriteLine(" ██║ ╚═╝ ██║██║███████║███████║██╗██╗██╗ ");
            Console.WriteLine(" ╚═╝     ╚═╝╚═╝╚══════╝╚══════╝╚═╝╚═╝╚═╝ ");
            Color("White");
            Console.WriteLine(" Press enter for next player.");


            Console.ReadLine();
        }

        private static void DisplayHitMessage()
        {
            Color("Red");
            Console.WriteLine("");
            Console.WriteLine(" ██╗  ██╗██╗████████╗    ██╗ ");
            Console.WriteLine(" ██║  ██║██║╚══██╔══╝    ██║ ");
            Console.WriteLine(" ███████║██║   ██║       ██║ ");
            Console.WriteLine(" ██╔══██║██║   ██║       ╚═╝ ");
            Console.WriteLine(" ██║  ██║██║   ██║       ██╗ ");
            Console.WriteLine(" ╚═╝  ╚═╝╚═╝   ╚═╝       ╚═╝ ");
            Color("White");
            Console.WriteLine(" Press enter for next player.");
            Console.ReadLine();
        }

        private static string AskForShot(PlayerInfoModel activePlayer)
        {
            Console.WriteLine($"{activePlayer.Name}, Please enter your shot: ");
            string shot = Console.ReadLine();
            return shot.ToUpper();
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;
            Console.WriteLine($"{activePlayer.Name}'s shot grid:\n");
            Color("Cyan");
            Console.WriteLine("     1   2   3   4   5");
            Console.WriteLine("    -------------------");
            Console.Write(" A |");
            Color("White");
            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                    Color("Cyan");
                    Console.Write($" {currentRow} |");
                    Color("White");
                }

                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Color("Blue");
                    Console.Write($" .  ");
                    Color("White");
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                {
                    Color("Red");
                    Console.Write(" *  ");
                    Color("White");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    Color("DarkYellow");
                    Console.Write(" -  ");
                    Color("White");
                }
                else
                {
                    Console.Write(" ?  ");
                }
            }
            Console.WriteLine("\n");
        }

        private static void WelcomeMessage()
        {
            Color("Green");
            Console.WriteLine(@"    _       __     __                             __      ");
            Console.WriteLine(@"   | |     / /__  / /________  ____ ___  ___     / /_____ ");
            Console.WriteLine(@"   | | /| / / _ \/ / ___/ __ \/ __ `__ \/ _ \   / __/ __ \");
            Console.WriteLine(@"   | |/ |/ /  __/ / /__/ /_/ / / / / / /  __/  / /_/ /_/ /");
            Console.WriteLine(@"   |__/|__/\___/_/\___/\____/_/ /_/ /_/\___/   \__/\____/ ");
            Console.WriteLine();
            Console.WriteLine(@"  ____        _   _   _           _     _         _     _ _       ");
            Console.WriteLine(@" | __ )  __ _| |_| |_| | ___  ___| |__ (_)_ __   | |   (_) |_ ___ ");
            Console.WriteLine(@" |  _ \ / _` | __| __| |/ _ \/ __| '_ \| | '_ \  | |   | | __/ _ \");
            Console.WriteLine(@" | |_) | (_| | |_| |_| |  __/\__ \ | | | | |_) | | |___| | ||  __/");
            Console.WriteLine(@" |____/ \__,_|\__|\__|_|\___||___/_| |_|_| .__/  |_____|_|\__\___|");
            Console.WriteLine(@"                                         |_|                      ");
            Color("White");
        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel newPlayer = new PlayerInfoModel();

            WelcomeMessage();

            Console.WriteLine($"\nPlayer information for {playerTitle}");

            // Ask for name
            newPlayer.Name = AskForPlayerName();

            // Load up the shot grid
            GameLogic.InitializeGrid(newPlayer);

            DisplayShotGrid(newPlayer);

            // Ask user for their 5 ship placements
            PlaceShips(newPlayer);

            // Clear console
            Console.Clear();

            return newPlayer;
        }

        private static string AskForPlayerName()
        {
            Console.Write("\nWhat is you name? ");
            string output = Console.ReadLine();
            Console.WriteLine();
            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            Console.WriteLine("You must now place 5 ships.");
            do
            {
                Console.Write($"Where do you want to place ship #{model.ShipLocations.Count + 1}? ");
                string location = Console.ReadLine().ToUpper();
                Console.WriteLine();
                bool isValidLocation = GameLogic.PlaceShip(model, location);
                if (isValidLocation == false)
                {
                    Color("DarkRed");
                    Console.WriteLine("That was not a valid location. Please try again.");
                    Color("White");
                }
                else
                {
                    Console.WriteLine($"Ship placed at {location}.");
                }



            } while (model.ShipLocations.Count < 5);
        }
    }
}