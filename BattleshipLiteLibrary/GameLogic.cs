using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLiteLibrary
{
    public static class GameLogic
    {
        public static void InitializeGrid(PlayerInfoModel model)
        {
            List<string> letters = new List<string>
            {
                "A",
                "B",
                "C",
                "D",
                "E"
            };

            List<int> numbers = new List<int>
            {
                1,2,3,4,5
            };

            foreach (string letter in letters)
            {
                foreach (int number in numbers)
                {
                    AddGridSpot(model, letter, number);
                }

            }
        }

        private static void AddGridSpot(PlayerInfoModel model, string letter, int number)
        {
            GridSpotModel spot = new GridSpotModel
            {
                SpotLetter = letter,
                SpotNumber = number,
                Status = GridSpotStatus.Empty
            };

            model.ShotGrid.Add(spot);
        }

        public static bool PlayerStillActive(PlayerInfoModel opponent)
        {
            bool isActive = false;

            foreach (GridSpotModel spot in opponent.ShipLocations)
            {
                if (spot.Status != GridSpotStatus.Sunk)
                {
                    isActive = true;
                }
            }
            return isActive;
        }

        public static bool PlaceShip(PlayerInfoModel model, string location)
        {
            bool isSyntaxOk = ValidateSyntax(location);
            if (isSyntaxOk == false)
            {
                return false;
            }

            (string letter, int number) = SplitShotIntoRowAndColumn(location);

            foreach (GridSpotModel spot in model.ShipLocations) // Loop trough player's locations
            {
                // If letter matches AND number matches, player already placed a ship here.
                if (letter == spot.SpotLetter && number == spot.SpotNumber)
                {
                    return false;
                }
            }

            GridSpotModel newShip = new GridSpotModel // Create new spot with ship
            {
                SpotLetter = letter,
                SpotNumber = number,
                Status = GridSpotStatus.Ship
            };
            model.ShipLocations.Add(newShip);

            return true;
        }

        public static bool ValidateSyntax(string spot)
        {
            bool resultOfValidation = true;

            if (spot.Length != 2)
            {
                return false; // Return false if length not 2
            }            

            char letter = spot[0];  // Grab first character as char
            if (letter < 65 || letter > 69) // Check for ASCII 'A' to 'E'
            {
                resultOfValidation = false; // Return false if letter not A to E
            }

            char number = spot[1]; // Grab second character
            if (number < 49 || number > 53) // Check for ASCII '1' to '5'
            {
                resultOfValidation = false; // Return false if number not 1 to 5
            }

            return resultOfValidation;
        }

        public static void AdjustShotGridStatus(PlayerInfoModel activePlayer, string row, int column, bool isAHit)
        {
            foreach (GridSpotModel spot in activePlayer.ShotGrid)
            {
                if(spot.SpotLetter == row && spot.SpotNumber == column && isAHit == true)
                {
                    spot.Status = GridSpotStatus.Hit;
                } else if (spot.SpotLetter == row && spot.SpotNumber == column && isAHit == false)
                {
                    spot.Status = GridSpotStatus.Miss;
                }
            }
        }

        public static int GetShotCount(PlayerInfoModel player)
        {
            int count = 0;
            foreach (GridSpotModel spot in player.ShotGrid)
            {
                if (spot.Status != GridSpotStatus.Empty)
                {
                    count++;
                }
            }
            return count;
        }

        public static (string row, int column) SplitShotIntoRowAndColumn(string spot)
        {
            return (spot.Substring(0,1), Int32.Parse(spot.Substring(1,1))); // Return row and column
        }

        public static bool ValidateShot(PlayerInfoModel activePlayer, string row, int column)
        {

            bool shotValid = true;

            foreach (GridSpotModel spot in activePlayer.ShotGrid)
            {
                if ((spot.SpotLetter == row && spot.SpotNumber == column && spot.Status == GridSpotStatus.Hit) 
                    || (spot.SpotLetter == row && spot.SpotNumber == column && spot.Status == GridSpotStatus.Miss))
                {
                    shotValid = false;
                }
                
            }
            return shotValid;
        }

        public static bool IdentifyShotResult(PlayerInfoModel opponent, string row, int column)
        {
            bool isAHit = false;
            foreach (GridSpotModel spot in opponent.ShipLocations)
            {
                if (spot.SpotLetter == row && spot.SpotNumber == column && spot.Status == GridSpotStatus.Ship)
                {
                    spot.Status = GridSpotStatus.Sunk;
                    isAHit = true;
                }                
            }            
            return isAHit;
        }

        public static void MarkShotResult(PlayerInfoModel activePlayer, string row, int column, bool isAHit)
        {
            foreach (GridSpotModel spot in activePlayer.ShotGrid)
            {
                if (spot.SpotLetter == row && spot.SpotNumber == column && isAHit == true)
                {
                    spot.Status = GridSpotStatus.Hit;
                }
                else
                {
                    if (spot.SpotLetter == row && spot.SpotNumber == column && isAHit == false)
                    {
                        spot.Status = GridSpotStatus.Miss;
                    }
                }
            }
        }
    }
}
