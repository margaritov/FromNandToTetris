using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyAssembler
{
    class MyAssembler
    {
        static string path = "";
        static string asmFileName = "";

        static void Main(string[] args)
        {
            Dictionary<string, int> symbolTable = new Dictionary<string, int>
                { {"R0",0},{"R1",1},{"R2",2},{"R3",3},{"R4",4},{"R5",5},{"R6",6},{"R7",7},
                {"R8",8},{"R9",9},{"R10",10},{"R11",11},{"R12",12},{"R13",13},{"R14",14},{"R15",15},
                {"SCREEN",16384}, {"KBD",24576}, {"SP",0}, {"LCL",1}, {"ARG",2}, {"THIS",3},{"THAT",4} };

            List<string> source = new List<string>();
            if (args.Length == 0)
            {
                Console.WriteLine("assembly file name required.");
                return;
            }
            else
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
                asmFileName = args[0];
                string[] lines = System.IO.File.ReadAllLines(path+asmFileName);

                foreach (var line in lines)
                {
                    string input = Regex.Replace(line, @"\s+", "");
                    if (input.Contains("//"))
                    {
                        input = input.Substring(0, input.IndexOf("//"));
                    }
                    if (!input.StartsWith("//") && input.Length > 0)
                    {
                        source.Add(input);
                    }
                }
            }

            // first pass - scan for label declarations (NAME)
            int instructionCounter = 0;
            foreach (var line in source)
            {
                if (line.StartsWith('(') && line.EndsWith(')'))
                {
                    string labelName = line.Substring(1, line.Length - 2);
                    if (symbolTable.ContainsKey(labelName))
                    {
                        Console.WriteLine($"{instructionCounter}: invalid label declaration!");
                    }
                    else
                    {
                        symbolTable[labelName] = instructionCounter;
                    }
                }
                else
                {
                    instructionCounter++;
                }
            }

            //second pass - scan for variable names
            int nextFreeMemoryAddress = 16;
            for (int i = 0; i < source.Count(); i++)
            {
                //is it A-instruction
                if (source[i].StartsWith('@'))
                {
                    string body = source[i].Substring(1);
                    string valueBinary = "";

                    //@instruction contains number - do nothing
                    if (IsAnumber(body))
                    {
                        int value = int.Parse(source[i].Substring(1));
                        valueBinary = Convert.ToString(value, 2);
                        valueBinary = valueBinary.PadLeft(15, '0');
                    }
                    else
                    { //@instruction contains symbol - shoud be resolved
                        if (symbolTable.ContainsKey(body) == false)
                        {
                            symbolTable[body] = nextFreeMemoryAddress;
                            nextFreeMemoryAddress++;
                        }
                        source[i] = $"@{symbolTable[body]}";
                    }
                }
            }

            foreach (var line in source)
            {
                //is it A-instruction
                if (line.StartsWith('('))
                {
                    //label declaration - ignore line
                }
                else if (line.StartsWith('@'))
                {
                    string body = line.Substring(1);
                    string valueBinary = "";

                    if (IsAnumber(body))
                    {
                        int value = int.Parse(line.Substring(1));
                        valueBinary = Convert.ToString(value, 2);
                        valueBinary = valueBinary.PadLeft(15, '0');
                    }
                    else
                    { //@instruction contains symbol - shoud be resolved
                    }
                    Console.WriteLine("0" + valueBinary);
                    instructionCounter++;
                }
                else //it is C-instruction
                {
                    string symbolicJump = "";
                    string symbolicDestination = "";
                    string symbolicComp = "";


                    string binaryJump = "";
                    string binaryComp = "";
                    string binaryDest = "";
                    //process C-instruction
                    if (line.Contains(';'))
                    {
                        symbolicComp = line.Substring(0, line.IndexOf(';'));
                        symbolicJump = line.Substring(line.IndexOf(';') + 1);

                    }
                    else
                    if (line.Contains('='))
                    {
                        symbolicDestination = line.Substring(0, line.IndexOf('='));
                        symbolicComp = line.Substring(line.IndexOf('=') + 1);
                    }
                    binaryJump = DecodeJump(symbolicJump);
                    binaryComp = DecodeComp(symbolicComp);
                    binaryDest = DecodeDest(symbolicDestination);
                    Console.WriteLine($"111{binaryComp}{binaryDest}{binaryJump}");
                    instructionCounter++;
                }
            }

        }

        private static bool IsAnumber(string body)
        {
            foreach (var c in body.ToCharArray())
            {
                if ((c < '0' || c > '9'))
                {
                    return false;
                }
            }
            return true;
        }


        private static string DecodeDest(string destinationField)
        {
            string result = "";
            switch (destinationField)
            {
                case "":
                    result = "000";
                    break;
                case "M":
                    result = "001";
                    break;
                case "D":
                    result = "010";
                    break;
                case "MD":
                    result = "011";
                    break;
                case "A":
                    result = "100";
                    break;
                case "AM":
                    result = "101";
                    break;
                case "AD":
                    result = "110";
                    break;
                case "AMD":
                    result = "111";
                    break;
                default:
                    result = "error";
                    break;

            }
            return result;
        }

        private static string DecodeComp(string compField)
        {
            string result = "";
            switch (compField)
            {
                case "0":
                    result = "0101010";
                    break;
                case "1":
                    result = "0111111";
                    break;
                case "-1":
                    result = "0111010";
                    break;
                case "D":
                    result = "0001100";
                    break;
                case "A":
                    result = "0110000";
                    break;
                case "!D":
                    result = "0001101";
                    break;
                case "!A":
                    result = "0110001";
                    break;
                case "-D":
                    result = "0001111";
                    break;
                case "-A":
                    result = "0110011";
                    break;
                case "D+1":
                    result = "0011111";
                    break;
                case "A+1":
                    result = "0110111";
                    break;
                case "D-1":
                    result = "0001110";
                    break;
                case "A-1":
                    result = "0110010";
                    break;
                case "D+A":
                    result = "0000010";
                    break;
                case "D-A":
                    result = "0010011";
                    break;
                case "A-D":
                    result = "0000111";
                    break;
                case "D&A":
                    result = "0000000";
                    break;
                case "D|A":
                    result = "0010101";
                    break;
                case "M":
                    result = "1110000";
                    break;
                case "!M":
                    result = "1110001";
                    break;
                case "-M":
                    result = "1110011";
                    break;
                case "M+1":
                    result = "1110111";
                    break;
                case "M-1":
                    result = "1110010";
                    break;
                case "D+M":
                    result = "1000010";
                    break;
                case "D-M":
                    result = "1010011";
                    break;
                case "M-D":
                    result = "1000111";
                    break;
                case "D&M":
                    result = "1000000";
                    break;
                case "D|M":
                    result = "1010101";
                    break;
                default:
                    result = "error";
                    break;
            }
            return result;
        }

        private static string DecodeJump(string jumpField)
        {
            string result = "";
            switch (jumpField)
            {
                case "":
                    result = "000";
                    break;
                case "JGT":
                    result = "001";
                    break;
                case "JEQ":
                    result = "010";
                    break;
                case "JGE":
                    result = "011";
                    break;
                case "JLT":
                    result = "100";
                    break;
                case "JNE":
                    result = "101";
                    break;
                case "JLE":
                    result = "110";
                    break;
                case "JMP":
                    result = "111";
                    break;
                default:
                    result = "error";
                    break;

            }
            return result;
        }

        private static void PrintSource(List<string> source)
        {
            foreach (var line in source)
            {
                Console.WriteLine(line);
            }
        }
    }
}
