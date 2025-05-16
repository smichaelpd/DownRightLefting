using System.Runtime.CompilerServices;

namespace DownRightLefting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool startOver = true;

            while (startOver)
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;

                Console.WriteLine("**************( Welcome to Down Right Lefting )**************");
                Console.WriteLine(string.Empty);
                Console.WriteLine("This program will use text to draw out the steps you enter.");
                Console.WriteLine("Format each step like (DistanceDirection).");
                Console.WriteLine("Examples: 5R, 4D, 7L = 5 right, 4 down, and 7 left.");
                Console.WriteLine(string.Empty);
                Console.WriteLine("Begin entering steps below and tell me to stop by entering the word STOP.");
                Console.WriteLine(string.Empty);

                List<Step> stepList = new List<Step>();
                string inputVal = string.Empty;

                while (inputVal.Trim().ToUpper() != "STOP")
                {
                    if (inputVal.Trim().ToUpper() == "STOP")
                    {
                        continue;
                    }

                    Console.Write("Enter Step: ");
                    inputVal = Console.ReadLine().ToUpper();

                    Step? inputStep = GetStep(inputVal);

                    if (inputStep == null)
                    {
                        if (inputVal.Trim().ToUpper() != "STOP")
                            Console.WriteLine("Invalid step, please try again!");
                        continue;
                    }
                    else if (inputStep.Distance > 100)
                    {
                        if (inputVal.Trim().ToUpper() != "STOP")
                            Console.WriteLine("Let's keep it under 100, please!");
                        continue;
                    }
                    else
                    {
                        stepList.Add(inputStep);
                    }
                }

                if (stepList?.Count > 0)
                {
                    // Lets display the list of steps up front
                    string stepListStr = string.Empty;
                    foreach (var step in stepList)
                    {
                        stepListStr += step.Distance + step.Direction + " " + GetArrowChar(step.Direction) + " ";
                    }
                    Console.WriteLine(stepListStr);

                    var allSteps = GetPrintSteps(stepList.ToArray());
                    foreach (var step in allSteps)
                    {
                        Console.WriteLine(step);
                    }
                }

                Console.Write("Would you like to try again? [Y/N]: ");
                var option = Console.ReadKey().Key;

                if (option == ConsoleKey.Y)
                {
                    startOver = true;
                    Console.Clear();
                }
                else
                {
                    startOver = false;
                }
            }
        }

        private static List<string> GetPrintSteps(Step[] steps)
        {
            List<string> ListOfStepsToPrint = new List<string>();

            int leftOffset = GetLeftOffset(steps);

            bool isFirstStep = true;
            int cursorLoc = 0;
            foreach (var step in steps)
            {
                string prtStep = string.Empty;
                char arw = GetArrowChar(step.Direction).ToCharArray()[0];

                if (step.Direction == "L")
                {
                    for (int i = 0; i < step.Distance; i++)
                    {
                        prtStep += arw.ToString();
                    }

                    if (isFirstStep)
                    {
                        prtStep = prtStep.PadLeft(leftOffset - 1, ' ');
                    }
                    else
                    {
                        prtStep = prtStep.PadLeft(cursorLoc + 1, ' ');
                    }

                    string directionChar = prtStep.FirstOrDefault(x => x != ' ').ToString();

                    cursorLoc = prtStep.IndexOf(directionChar);

                    ListOfStepsToPrint.Add(prtStep);
                }
                else if (step.Direction == "R")
                {
                    for (int i = 0; i < step.Distance; i++)
                    {
                        prtStep += arw.ToString();
                    }

                    if (isFirstStep)
                    {
                        if (leftOffset > 0)
                        {
                            prtStep = prtStep.PadLeft(leftOffset + step.Distance + 1, ' ');
                        }
                    }
                    else
                    {
                        prtStep = prtStep.PadLeft(cursorLoc + step.Distance, ' ');
                    }

                    string directionChar = prtStep.FirstOrDefault(x => x != ' ').ToString();
                    cursorLoc = prtStep.IndexOf(directionChar) + step.Distance - 1;

                    ListOfStepsToPrint.Add(prtStep);
                }
                else if (step.Direction == "D")
                {
                    for (int i = 0; i < step.Distance; i++)
                    {
                        if (isFirstStep)
                        {
                            prtStep = arw.ToString().PadLeft(leftOffset, ' ');
                        }
                        else
                        {
                            prtStep = arw.ToString().PadLeft(cursorLoc + 1, ' ');
                        }

                        string directionChar = prtStep.FirstOrDefault(x => x != ' ').ToString();
                        cursorLoc = prtStep.IndexOf(directionChar);

                        ListOfStepsToPrint.Add(prtStep);
                    }
                }

                isFirstStep = false;
            }

            // This is a hack to get rid of extra offset when lots of lefts
            // I'm not sure where these are coming from.
            int minCursor = ListOfStepsToPrint.Min(x => x.IndexOf(x.FirstOrDefault(x => x != ' ').ToString()));
            if (minCursor > 0)
            {
                List<string> trimmedList = new List<string>();
                foreach (string step in ListOfStepsToPrint)
                {
                    trimmedList.Add(step.Remove(0, minCursor));
                }

                ListOfStepsToPrint = trimmedList;
            }

            return ListOfStepsToPrint;
        }

        private static int GetLeftOffset(Step[] steps)
        {
            int rtnOffset = 0;

            int calcOffset = 0;
            bool isFirstStep = true;
            foreach (var step in steps)
            {
                if (step.Direction == "L")
                {
                    calcOffset += step.Distance;
                }
                else if (step.Direction == "R")
                {
                    calcOffset -= isFirstStep ? step.Distance : step.Distance - 1;
                }

                if (rtnOffset < calcOffset)
                {
                    rtnOffset = calcOffset;
                }

                isFirstStep = false;
            }

            rtnOffset = Math.Abs(rtnOffset);

            return rtnOffset;
        }

        private static string GetArrowChar(string direction)
        {
            if (!string.IsNullOrEmpty(direction))
            {
                switch (direction)
                {
                    case "D":
                        return "˅";
                    case "R":
                        return ">";
                    case "L":
                        return "<";
                }
            }

            return "?";
        }

        private static Step? GetStep(string inputVal)
        {
            string directionChar = inputVal.FirstOrDefault(x => !char.IsNumber(x)).ToString();
            if (directionChar != "D" && directionChar != "R" && directionChar != "L")
            {
                return null;
            }

            int indexOfDir = inputVal.IndexOf(directionChar);
            if (indexOfDir < 1)
            {
                return null;
            }

            string distanceStr = inputVal.Substring(0, inputVal.IndexOf(directionChar));
            int.TryParse(distanceStr, out int distanceInt);

            if (distanceInt > 0)
            {
                return new Step(distanceInt, directionChar);
            }

            return null;
        }

        private class Step
        {
            public Step(int distance, string direction)
            {
                this.Distance = distance;
                this.Direction = direction;
            }

            public int Distance { get; set; }
            public string Direction { get; set; } = string.Empty;
        }
    }
}