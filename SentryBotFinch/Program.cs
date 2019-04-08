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
            double lowerTempThreshold = 0, upperLightThreshold = 0;
            
            while (!exiting)
            {
                DisplayHeader("Main Menu");
                Console.WriteLine("\t1) Connect To SentryBot");
                Console.WriteLine("\t2) SentryBot Setup");
                Console.WriteLine("\t3) Activate SentryBot");
                Console.WriteLine("\tE) Exit");
                Console.WriteLine();
                Console.Write("Menu Choice: ");
                menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "1":
                        DisplayConnectToFinch(finch);
                        break;
                    case "2":
                        lowerTempThreshold = DisplaySetup(finch);
                        upperLightThreshold = DisplayLightSetup(finch);
                        break;
                    case "3":
                        DisplayActivateSentryBot(lowerTempThreshold, finch, upperLightThreshold);
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

        static double DisplayLightSetup(Finch finch)
        {
            double lightDiff, upperLightThreshold, ambientLight;
            bool valid;

            do
            {
                Console.Clear();
                DisplayHeader("Setup SentryBot");
                valid = true;
                Console.Write("Enter Desired Change in Light: ");
                if (!double.TryParse(Console.ReadLine(), out lightDiff))
                {
                    valid = false;
                    Console.WriteLine("Invalid Input. Press Any Key To Try Again.");
                    Console.ReadKey();
                }
            } while (!valid);

            ambientLight = (finch.getLeftLightSensor() + finch.getRightLightSensor()) / 2;

            upperLightThreshold = ambientLight + lightDiff;

            DisplayContinuePrompt();

            return upperLightThreshold;

        }

        static void DisplayConnectToFinch(Finch finch)
        {
            int note=300;

            finch.connect();
            for (int i = 0; i < 10; i++)
            {
                finch.noteOn(note);
                finch.wait(5);
                finch.noteOff();
                note = note + 100;
            }
        }

        static void DisplayActivateSentryBot(double lowerTempThreshold, Finch finch, double upperLightThreshold)
        {
            DisplayHeader("Activate SentryBot");

            Console.WriteLine();
            Console.WriteLine($"Current Ambient Temperature: {finch.getTemperature()}");
            Console.WriteLine($"Minimum Temperature Threshold: {lowerTempThreshold}");
            Console.WriteLine();
            Console.WriteLine($"Current Light Level: {(finch.getLeftLightSensor() + finch.getRightLightSensor()) / 2}");
            Console.WriteLine($"Maximum Light Threshold: {upperLightThreshold}");

            Console.WriteLine("Press any key to begin.");
            Console.ReadKey();

            while (!SentryVersusThresholdValue(lowerTempThreshold, finch, upperLightThreshold))
            {
                TemperatureNominalIndicator(finch);
            }

            DisplayContinuePrompt();
        }

        static void TemperatureNominalIndicator(Finch finch)
        {
            finch.setLED(0, 255, 0);
            finch.wait(500);
            Console.WriteLine($"Current Temperature: {finch.getTemperature()}");
            Console.WriteLine($"Current Light Level: {(finch.getLeftLightSensor() + finch.getRightLightSensor()) / 2}");
            finch.setLED(0, 0, 0);
            finch.wait(500);
        }

        static bool SentryVersusThresholdValue(double lowerTempThreshold, Finch finch, double upperLightThreshold)
        {
            if (finch.getTemperature()<=lowerTempThreshold)
            {
                for (int i = 0; i < 5; i++)
                {
                    finch.noteOn(600);
                    finch.wait(50);
                    finch.noteOff();
                }
                Console.WriteLine("Temperature Reached Threshold. Press Any Key To Continue.");
                Console.ReadKey();
                return true;
            }
            else if ((finch.getLeftLightSensor() + finch.getRightLightSensor()) / 2>=upperLightThreshold)
            {
                for (int i = 0; i < 5; i++)
                {
                    finch.noteOn(600);
                    finch.wait(50);
                    finch.noteOff();
                }
                Console.WriteLine("Light Reached Threshold. Press Any Key To Continue.");
                Console.ReadKey();
                return true;
            }
            else
            {
                return false;
            }
        }

        static double DisplaySetup(Finch finch)
        {
            double tempDiff, lowerTempThreshold, ambientTemp;
            bool valid;

            do
            {
                Console.Clear();
                DisplayHeader("Setup SentryBot");
                valid = true;
                Console.Write("Enter Desired Change in Temperature: ");
                if (!double.TryParse(Console.ReadLine(), out tempDiff))
                {
                    valid = false;
                    Console.WriteLine("Invalid Input. Press Any Key To Try Again.");
                    Console.ReadKey();
                }
            } while (!valid);

            ambientTemp = finch.getTemperature();

            lowerTempThreshold = ambientTemp - tempDiff;

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
