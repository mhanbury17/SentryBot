using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinchAPI;

namespace SentryBotFinch
{
    class Program
    {
            //********************************************
            // Title: SentryBot Finch
            // Author: Miles Hanbury
            // Last Revised: 04-08-2019
            // Date Created: 04-08-2019
            //********************************************


        static void Main(string[] args)
        {
            DisplayWelcomeScreen();
            DisplayMenuScreen();
            DisplayClosingScreen();
        }

        static void DisplayMenuScreen()
        {
            bool exiting = false;
            string menuChoice;
            Finch finch = new Finch();
            double lowerTempThreshold = 0;

            finch.connect();

            while (!exiting)
            {
                DisplayHeader("Main Menu");
                Console.WriteLine("1) Setup");
                Console.WriteLine("2) Activate SentryBot");
                Console.WriteLine("E) Exit");
                Console.WriteLine();
                Console.Write("Menu Choice: ");
                menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "1":
                        lowerTempThreshold = DisplaySetup(finch);
                        break;
                    case "2":
                        DisplayActivateSentryBot(lowerTempThreshold, finch);
                        break;
                    case "e":
                    case "E":
                        exiting = true;
                        break;
                    default:
                        break;
                }
            }
        }

        static void DisplayActivateSentryBot(double lowerTempThreshold, Finch finch)
        {
            DisplayHeader("Activate SentryBot");

            while (!TemperatureBelowThresholdValue(lowerTempThreshold, finch))
            {
                TemperatureNominalIndicator(finch);
            }

            DisplayContinuePrompt();
        }

        static void TemperatureNominalIndicator(Finch finch)
        {
            finch.setLED(0, 255, 0);
            finch.wait(1000);
            finch.setLED(0, 0, 0);
        }

        static bool TemperatureBelowThresholdValue(double lowerTempThreshold, Finch finch)
        {
            if (finch.getTemperature()<=lowerTempThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static double DisplaySetup(Finch finch)
        {
            double tempDiff, lowerTempThreshold, ambientTemp;

            DisplayHeader("Setup SentryBot");

            Console.Write("Enter Desired Change in Temperature: ");
            double.TryParse(Console.ReadLine(), out tempDiff);

            ambientTemp = finch.getTemperature();

            lowerTempThreshold = ambientTemp - tempDiff;
            // what if temp is > ambient

            DisplayContinuePrompt();

            return lowerTempThreshold;
        }

        #region HELPER CODE
        static void DisplayContinuePrompt()
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }

            static void DisplayHeader(string headText)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine(headText);
                Console.WriteLine();
            }

            static void DisplayWelcomeScreen()
            {
                Console.Clear();
                Console.WriteLine("Welcome to SentryBot Finch");
                DisplayContinuePrompt();
            }

            static void DisplayClosingScreen()
            {
                Console.Clear();
                Console.WriteLine("Thank you for using SentryBot Finch.");
                DisplayContinuePrompt();
            }
            #endregion
    }
}
