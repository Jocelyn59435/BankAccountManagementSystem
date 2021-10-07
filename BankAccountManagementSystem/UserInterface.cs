using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace BankAccountManagementSystem
{
    class UserInterface
    {
        //Account Info
        static Dictionary<string, string> accountDict = new Dictionary<string, string>();

        //Transaction Info
        static List<List<string>> transactionList = new List<List<string>>();

        //Store user key info when typing
        static ConsoleKeyInfo userKeyInfo;

        // Login Window attributes
        static string[] loginWindowFields = { "Username: ", "Password: " };
        static string[] loginBanners = { "Welcome to Simple Bank", "Login To Start" };
        static int[,] loginFieldPosition = new int[2, 2];
        static string[] loginUserInputs = new string[2];

        //Main Menu attributes
        static string[] mainMenuOptions = { "1. Create a new account",
                                            "2. Search for a new account",
                                            "3. Deposit",
                                            "4. Withdraw",
                                            "5. A/C statement",
                                            "6. Delete an account",
                                            "7. Exit" };
        static string mainMenuBanner = "Welcome to Simple Bank";
        static string mainMenuOptionField = "Enter your choice (1 - 7): ";
        static int mainMenuUserInput = 0;
        static int[] mainMenuOptionFieldPosition = new int[2];

        // Create Account Window attributes
        static string[] createAccountFields = { "First Name: ",
                                             "Last Name: ",
                                             "Address: ",
                                             "Phone: ",
                                             "Email: ",
                                           };
        static string[] createAccountBanners = { "Create a New Acount", "Enter the Details" };
        static string confirmField = "Is the information correct (y / n)?";
        static int[,] accountFieldPosition = new int[5, 2];
        static string[] accountInfo = new String[5];

        //Search Account Window attributes
        static string[] searchAccountBanners = { "Search an Account", "Enter the Details" };
        static string accountNoField = "Account Number: ";
        static int[] accountNoFieldPosition = new int[2];
        static string userInputAccountNo = "";
        static bool carryOn = false;

        //Account Statement Window attribute
        static string[] accountStatementBanners = { "Statement", "Enter the Details" };

        //Delete Account Window attribute
        static string[] deleteAccountBanners = { "Delete an Account", "Enter the Details" };

        //Deposit Window attributes
        static string[] depositBanners = { "Deposit", "Enter the Details" };
        static string[] amountWindowFields = { "Account Number: ", "Amount: $" };

        //Withdraw Window attributes
        static string[] withdrawBanners = { "Withdraw", "Enter the Details" };
        static int[] amountFieldPosition = new int[2];
        static string userInputAmount = "";
        static double userInputAmountDouble = 0;
        Account userAccount = new Account();

        public static void WriteAt(string s, int x, int y)
        {
            try
            {
                Console.SetCursorPosition(x, y);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }

        }

        public static bool ValidUserInput(string userInput, int minLength, int maxLength)
        {
            if (userInput.Length < minLength | userInput.Length > maxLength)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public static Dictionary<string, string> GetTextFileInfo(string path)
        {
            string line;
            string filePath = path;
            string[] fileInfoList;
            Dictionary<string, string> fileInfo = new Dictionary<string, string>();
            if (File.Exists(filePath))
            {
                StreamReader file = new StreamReader(filePath);
                while ((line = file.ReadLine()) != null)
                {
                    fileInfoList = line.Split('|');
                    fileInfo.Add(fileInfoList[0], fileInfoList[1]);
                }
                file.Close();
            }
            return fileInfo;
        }

        public static void MapTextFileInfo(string path)
        {
            string line;
            string filePath = path;
            string[] splitedList;
            try
            {
                accountDict = new Dictionary<string, string>();
                transactionList = new List<List<string>>();
                StreamReader file = new StreamReader(filePath);
                while ((line = file.ReadLine()) != null)
                {
                    splitedList = line.Split('|');
                    if (splitedList.Length == 2)
                    {
                        accountDict.Add(splitedList[0], splitedList[1]);
                    }
                    if (splitedList.Length == 4)
                    {
                        transactionList.Add(new List<string>(splitedList));
                    }
                }
                file.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        //get the last file name to increment when creating a new account
        public static string[] GetFileNames(string path)
        {
            string[] fileNames = Directory.GetFiles(path);
            if (fileNames.Length > 0)
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    fileNames[i] = fileNames[i].Replace(@"..\..\accounts\", "").Replace(@".txt", "");
                }

            }
            return fileNames;
        }

        // for printing account statement
        public static void ShowFileContent(string[] contentLines, string header, int formWidth, int startX, int startY)
        {
            DrawWindow(contentLines.Length + 4, formWidth, startX, startY);
            WriteAt(header, startX + (formWidth - header.Length) / 2, startY + 1);
            for (int i = 0; i < contentLines.Length; i++)
            {
                if (contentLines[i].Split('|').Length == 2)
                { 
                    contentLines[i] = contentLines[i].Replace("|", ": ");
                    WriteAt(contentLines[i], startX + 5, startY + 3 + i);
                }
                if (contentLines[i].Split('|').Length == 4)
                {
                    contentLines[i] = contentLines[i].Replace("|", "  ");
                    WriteAt(contentLines[i], startX + 5, startY + 3 + i);
                }
            }
        }

        public static void DrawWindow(int noLines, int formWidth, int startX, int startY)
        {
            // draw the outline of the window
            for (int line = 0; line < noLines; line++)
            {
                if (line == 0 | line == 2 | line == (noLines - 1))
                {
                    for (int col = 0; col < formWidth; col++)
                    {
                        if (line == 0 && col == 0)
                        {
                            WriteAt("╒", startX + col, startY + line);
                        }
                        else if (line == 0 && col == formWidth - 1)
                        {
                            WriteAt("╕", startX + col, startY + line);
                        }
                        else if (line == 2 && col == 0)
                        {
                            WriteAt("╞", startX + col, startY + line);
                        }
                        else if (line == 2 && col == formWidth - 1)
                        {
                            WriteAt("╡", startX + col, startY + line);
                        }
                        else if (line == noLines - 1 && col == 0)
                        {
                            WriteAt("╘", startX + col, startY + line);
                        }
                        else if (line == noLines - 1 && col == formWidth - 1)
                        {
                            WriteAt("╛", startX + col, startY + line);
                        }
                        else
                        {
                            WriteAt("═", startX + col, startY + line);
                        }
                    }
                }
                else
                {
                    WriteAt("|", startX, startY + line);
                    WriteAt("|", startX + formWidth - 1, startY + line);
                }

            }
        }

        public void ShowLoginWindow(int noLines, int formWidth, int startX, int startY)
        {
            // get user inputs
            do
            {
                Console.Clear();
                // clear stored values in involved variables
                userKeyInfo = new ConsoleKeyInfo();
                loginUserInputs = new string[2];

                DrawWindow(noLines, formWidth, startX, startY);
                // add banners
                WriteAt(loginBanners[0], startX + (formWidth - loginBanners[0].Length) / 2, startY + 1);
                WriteAt(loginBanners[1], startX + (formWidth - loginBanners[1].Length) / 2, startY + 3);

                

                // add fields
                for (int i = 0; i < loginWindowFields.Length; i++)
                {
                    WriteAt(loginWindowFields[i] + "", startX + 8, startY + 6 + i);
                    loginFieldPosition[i, 0] = Console.CursorLeft;
                    loginFieldPosition[i, 1] = Console.CursorTop;
                }
                
                for (int i = 0; i < loginWindowFields.Length; i++)
                {
                    Console.SetCursorPosition(loginFieldPosition[i, 0], loginFieldPosition[i, 1]);
                    // for the password field, mask it
                    if (i == 1)
                    {
                        while ((userKeyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter)
                        {
                            // hide user input and type *
                            if (!char.IsControl(userKeyInfo.KeyChar))
                            {
                                loginUserInputs[i] += userKeyInfo.KeyChar;
                                Console.Write("*");
                            }
                            // check input length while typing
                            if (!ValidUserInput(loginUserInputs[i], 1, 15))
                            {
                                WriteAt("Invalid input, the length is within 15. Press [enter] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                                loginUserInputs[i] = "";
                            }
                        }

                    }
                    else
                    {
                        while ((userKeyInfo = Console.ReadKey(false)).Key != ConsoleKey.Enter)
                        {
                            loginUserInputs[i] += userKeyInfo.KeyChar;
                            if (!ValidUserInput(loginUserInputs[i], 1, 15))
                            {
                                WriteAt("Invalid input, the length is within 15. Press [enter] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                            }
                        }
                    }


                }
                // read file and get login credentials, then check user input
                Dictionary<string, string> loginInfo = GetTextFileInfo(@"..\..\login.txt");
                try
                {
                    if (loginUserInputs[1] != null && loginUserInputs[1].CompareTo(loginInfo[loginUserInputs[0]]) == 0)
                    {
                        WriteAt("Valid Credentials. Press [any key] to the main menu.", startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        break;
                    }
                    else
                    {
                        WriteAt("Invalid Credentials. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                    }
                }
                catch(Exception e)
                {
                    WriteAt($"Username does not exist: {e.Message} Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                    _ = Console.ReadKey();
                }
               
            }
            while (true);
            ShowMainMenu(15, 60, 5, 3);
        }

        public void ShowMainMenu(int noLines, int formWidth, int startX, int startY)
        {
            Console.Clear();
            DrawWindow(noLines, formWidth, startX, startY);
            // add banner
            WriteAt(mainMenuBanner, startX + (formWidth - mainMenuBanner.Length) / 2, startY + 1);

            // add options
            for (int i = 0; i < mainMenuOptions.Length; i++)
            {
                WriteAt(mainMenuOptions[i] + "", startX + 8, startY + 4 + i);
            }

            // add option field and get its position
            do
            {
                WriteAt(mainMenuOptionField, startX + 8, startY + noLines - 3);
                mainMenuOptionFieldPosition[0] = Console.CursorLeft;
                mainMenuOptionFieldPosition[1] = Console.CursorTop;

                userKeyInfo = Console.ReadKey(false);
                // cast char to int
                mainMenuUserInput = userKeyInfo.KeyChar - '0';
                switch (mainMenuUserInput)
                {
                    case (1): ShowCreateAccountWindow(15, 60, 5, 3); break;
                    case (2): ShowSearchAccountWindow(10, 60, 5, 3); ; break;
                    case (3): ShowDepositWindow(10, 60, 5, 3); break;
                    case (4): ShowWithdrawWindow(10, 60, 5, 3); break;
                    case (5): ShowAccountStatementWindow(10, 60, 5, 3); ; break;
                    case (6): DeleteAccountWindow(10, 60, 5, 3); break;
                    case (7): ShowLoginWindow(10, 60, 5, 3); break;
                    default:
                        WriteAt("Invalid input, should be 1-7, please try again." + new string(' ', 100), startX + 8, startY + noLines + 1);
                        Console.SetCursorPosition(mainMenuOptionFieldPosition[0], mainMenuOptionFieldPosition[1]);
                        Console.Write(new string(' ', 10));
                        break;
                }
                // back to main menu
                DrawWindow(noLines, formWidth, startX, startY);
                // add banner
                WriteAt(mainMenuBanner, startX + (formWidth - mainMenuBanner.Length) / 2, startY + 1);

                // add options
                for (int i = 0; i < mainMenuOptions.Length; i++)
                {
                    WriteAt(mainMenuOptions[i] + "", startX + 8, startY + 4 + i);
                }
            }
            while (true);

        }

        public void ShowCreateAccountWindow(int noLines, int formWidth, int startX, int startY)
        {
            
            // get user input
            do
            {
                
                Console.Clear();
                // clear stored values in involved variables
                accountInfo = new String[5];
                userKeyInfo = new ConsoleKeyInfo();
                // draw window
                DrawWindow(noLines, formWidth, startX, startY);
                WriteAt(createAccountBanners[0], startX + (formWidth - createAccountBanners[0].Length) / 2, startY + 1);
                WriteAt(createAccountBanners[1], startX + (formWidth - createAccountBanners[1].Length) / 2, startY + 3);

                // add fields
                for (int i = 0; i < createAccountFields.Length; i++)
                {
                    WriteAt(createAccountFields[i] + "", startX + 8, startY + 6 + i);
                    accountFieldPosition[i, 0] = Console.CursorLeft;
                    accountFieldPosition[i, 1] = Console.CursorTop;
                }

                for (int i = 0; i < createAccountFields.Length; i++)
                {
                    Console.SetCursorPosition(accountFieldPosition[i, 0], accountFieldPosition[i, 1]);
                

                    while ((userKeyInfo = Console.ReadKey(false)).Key != ConsoleKey.Enter)
                    {
                        accountInfo[i] += userKeyInfo.KeyChar;
                        // validate input while typing
                        if (!ValidUserInput(accountInfo[i], 0, 35))
                        {
                            WriteAt("Invalid input, the length is within 35." + new string(' ', 100), startX, startY + noLines + 2);
                            break;
                        }
                    }
                }
                // validate email address
                bool validEmail = accountInfo[4] != null &&
                                  Regex.IsMatch(accountInfo[4],
                                  @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                                  RegexOptions.IgnoreCase);
                if (!validEmail)
                {
                    WriteAt("Invalid email address." + new string(' ', 100), startX, startY + noLines + 2);
                    _ = Console.ReadKey();
                }
                // validate phone number: integer and length <= 10
                bool validPhoneNumber = accountInfo[3] != null &&
                                         Regex.IsMatch(accountInfo[3],
                                         @"^\d{10}$");
                if (!validPhoneNumber)
                {
                    WriteAt("Invalid phone number, should be ten digits." + new string(' ', 100), startX, startY + noLines + 2);
                    _ = Console.ReadKey();
                }
                // validate all inputs again
                bool allInputAreValid = false;
                foreach (string field in accountInfo)
                {
                    allInputAreValid = field != null && ValidUserInput(field, 0, 35);
                }
                if (allInputAreValid && validEmail && validPhoneNumber)
                {
                    // ask user to confirm, if yes, create account file
                    WriteAt(confirmField + new string(' ', 100), startX, startY + noLines + 2);
                    Console.SetCursorPosition(startX, startY + noLines + 3);
                    userKeyInfo = Console.ReadKey();
                    if (userKeyInfo.KeyChar == 'y')
                    {
                        break;
                    }
                }
            }
            while (true);
            WriteAt("Processing..." + new string(' ', 100), startX, startY + noLines + 2);
            WriteAt(new string(' ', 100), startX, startY + noLines + 3);

            // Get all account filenames
            string[] accountFileNames = GetFileNames(@"..\..\accounts");
            int accountNo = 10000;
            // get the last added file' accountNo, increment by 1, and assign to new accountNo 
            if (accountFileNames.Length > 0)
            {
                accountNo = Int32.Parse(accountFileNames[accountFileNames.Length - 1]) + 1;
            }

            // write to new account file
            string filePath = $@"..\..\accounts\{accountNo}.txt";
            userAccount.CreateAccount(accountNo, accountInfo, filePath);

            // add the account number to list（accounts）
            WriteAt("Account created, an account statement is sent to your email." + new string(' ', 100), startX, startY + noLines + 5);
            WriteAt($"Account number: {accountNo}" + new string(' ', 100), startX, startY + noLines + 6);
            _ = Console.ReadKey();
            ShowMainMenu(15, 60, 5, 3);
        }

        public void ShowSearchAccountWindow(int noLines, int formWidth, int startX, int startY)
        {
            //get user input
            do
            {
                // clear stored values in involved variables
                userInputAccountNo = "";
                userKeyInfo = new ConsoleKeyInfo();
                //draw the window
                Console.Clear();
                DrawWindow(noLines, formWidth, startX, startY);
                WriteAt(searchAccountBanners[0], startX + (formWidth - searchAccountBanners[0].Length) / 2, startY + 1);
                WriteAt(searchAccountBanners[1], startX + (formWidth - searchAccountBanners[1].Length) / 2, startY + 3);
                WriteAt(accountNoField, startX + 4, startY + 6);
                accountNoFieldPosition[0] = Console.CursorLeft;
                accountNoFieldPosition[1] = Console.CursorTop;

                Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                while ((userKeyInfo = Console.ReadKey(false)).Key != ConsoleKey.Enter)
                {
                    if (!Char.IsDigit(userKeyInfo.KeyChar))
                    {
                        WriteAt("Invalid input, should be numbers. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        userInputAccountNo = "";
                        Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                        Console.Write(new string(' ', 100));
                        carryOn = false;
                        break;
                    }
                    userInputAccountNo += userKeyInfo.KeyChar;
                    // validate input while typing
                    if (!ValidUserInput(userInputAccountNo, 0, 10))
                    {
                        WriteAt("Invalid input, the length is within 10. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        userInputAccountNo = "";
                        Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                        Console.Write(new string(' ', 100));
                        carryOn = false;
                        break;
                    }
                    carryOn = true;
                }
                if (carryOn) 
                {
                    WriteAt("Searching...", startX, startY + noLines + 2);
                    string filePath = $@"..\..\accounts\{userInputAccountNo}.txt";
                    // check whether the account file exist or not
                    if (!userAccount.CheckAccount(filePath))
                    {
                        WriteAt("Account not found." + new string(' ', 100), startX, startY + noLines + 2);
                        WriteAt("Check another account (y / n) ?", startX, startY + noLines + 3);
                        Console.SetCursorPosition(startX, startY + noLines + 4);
                        ConsoleKeyInfo pressedKey;
                        if ((pressedKey = Console.ReadKey(false)).Key == ConsoleKey.Y)
                        {
                            userInputAccountNo = "";
                            Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                            Console.Write(new string(' ', 20));
                            Console.SetCursorPosition(startX, startY + noLines + 4);
                            Console.Write(new string(' ', 20));
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        WriteAt("Account found.", startX, startY + noLines + 2);
                        string[] accountInfo = File.ReadAllLines(filePath);
                        ShowFileContent(accountInfo, "Account Details", 60, 5, startY + noLines + 4);
                        WriteAt("Check another account (y / n) ?", startX, startY + noLines + accountInfo.Length + 8);
                        userInputAccountNo = "";
                        Console.SetCursorPosition(startX, startY + noLines + accountInfo.Length + 9);
                        ConsoleKeyInfo pressedKey;
                        if ((pressedKey = Console.ReadKey(false)).Key == ConsoleKey.Y)
                        {
                            userInputAccountNo = "";
                            Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                            Console.Write(new string(' ', 20));
                            Console.SetCursorPosition(startX, startY + noLines + accountInfo.Length + 9);
                            Console.Write(new string(' ', 20));
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            while (true);
            ShowMainMenu(15, 60, 5, 3);
        }


        public void ShowAccountStatementWindow(int noLines, int formWidth, int startX, int startY)
        {
            //get user input
            do
            {
                //draw the window
                Console.Clear();
                DrawWindow(noLines, formWidth, startX, startY);
                WriteAt(accountStatementBanners[0], startX + (formWidth - accountStatementBanners[0].Length) / 2, startY + 1);
                WriteAt(accountStatementBanners[1], startX + (formWidth - accountStatementBanners[1].Length) / 2, startY + 3);
                WriteAt(accountNoField, startX + 4, startY + 6);
                accountNoFieldPosition[0] = Console.CursorLeft;
                accountNoFieldPosition[1] = Console.CursorTop;
                // clear stored values in involved variables
                userInputAccountNo = "";
                userKeyInfo = new ConsoleKeyInfo();

                Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);

                while ((userKeyInfo = Console.ReadKey(false)).Key != ConsoleKey.Enter)
                {
                    if (!Char.IsDigit(userKeyInfo.KeyChar))
                    {
                        WriteAt("Invalid input, should be numbers. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        userInputAccountNo = "";
                        Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                        Console.Write(new string(' ', 100));
                        carryOn = false;
                        break;
                    }
                    userInputAccountNo += userKeyInfo.KeyChar;
                    // validate input while typing
                    if (!ValidUserInput(userInputAccountNo, 0, 10))
                    {
                        WriteAt("Invalid input, the length is within 10. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        userInputAccountNo = "";
                        Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                        Console.Write(new string(' ', 100));
                        carryOn = false;
                        break;
                    }
                    carryOn = true;
                }
                if (carryOn)
                {
                    WriteAt("Searching...", startX, startY + noLines + 2);
                    string filePath = $@"..\..\accounts\{userInputAccountNo}.txt";
                    // check whether the account file exist or not
                    if (!userAccount.CheckAccount(filePath))
                    {
                        WriteAt("Account not found." + new string(' ', 100), startX, startY + noLines + 2);
                        WriteAt("Check another account (y / n) ?", startX, startY + noLines + 3);
                        Console.SetCursorPosition(startX, startY + noLines + 4);
                        ConsoleKeyInfo pressedKey;
                        if ((pressedKey = Console.ReadKey(false)).Key == ConsoleKey.Y)
                        {
                            userInputAccountNo = "";
                            Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                            Console.Write(new string(' ', 20));
                            Console.SetCursorPosition(startX, startY + noLines + 4);
                            Console.Write(new string(' ', 20));
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        WriteAt("Account found.", startX, startY + noLines + 2);
                        string[] accountInfo = File.ReadAllLines(filePath);
                        ShowFileContent(accountInfo, "Account Details", 60, 5, startY + noLines + 4);
                        WriteAt("Email statement (y / n) ?", startX, startY + noLines + accountInfo.Length + 8);
                        Console.SetCursorPosition(startX, startY + noLines + accountInfo.Length + 9);
                        ConsoleKeyInfo pressedKey;
                        if ((pressedKey = Console.ReadKey(false)).Key == ConsoleKey.Y)
                        {
                            WriteAt("Processing...", startX, startY + noLines + accountInfo.Length + 10);
                            MapTextFileInfo(filePath);
                            string emailBody = $"Hi {accountDict["First Name"]}, here is your account statement: {Environment.NewLine}";
                            string to = accountDict["Email"];
                            string from = "xinxinhuang2021@outlook.com";
                            foreach (KeyValuePair<string, string> kvp in accountDict)
                            {
                                emailBody += $"{Environment.NewLine} {kvp.Key}: {kvp.Value}";
                            }
                            foreach (List<string> list in transactionList)
                            {
                                string combinebString = string.Join(",", list.ToArray());
                                emailBody += $"{Environment.NewLine} {combinebString}";
                            }
                            try
                            {
                                emailBody += Environment.NewLine + Environment.NewLine + "Best Regards," + Environment.NewLine + "Simple Bank";
                                MailMessage message = new MailMessage(from, to)
                                { 
                                     Subject = "Your Account Statement",
                                     Body = emailBody
                                };
                                SmtpClient client = new SmtpClient("smtp-mail.outlook.com", 587);
                                client.EnableSsl = true;
                                client.UseDefaultCredentials = false;
                                client.Credentials = new System.Net.NetworkCredential("xinxinhuang2021@outlook.com", "internet123");
                                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                                client.Send(message);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Failed to send email: {e.Message}");
                                _ = Console.ReadKey();
                            }
                            WriteAt("Email sent successfully. Press any key back to main menu.", startX, startY + noLines + accountInfo.Length + 10);
                            _ = Console.ReadKey();
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            while (true);
            ShowMainMenu(15, 60, 5, 3);
        }

        public void ShowDepositWindow(int noLines, int formWidth, int startX, int startY)
        {
            //get user input
            do
            {
                // clear stored values in involved variables
                userInputAccountNo = "";
                userInputAmount = "";
                userKeyInfo = new ConsoleKeyInfo();
                userInputAmountDouble = 0;

                //draw the window
                Console.Clear();
                DrawWindow(noLines, formWidth, startX, startY);
                WriteAt(depositBanners[0], startX + (formWidth - depositBanners[0].Length) / 2, startY + 1);
                WriteAt(depositBanners[1], startX + (formWidth - depositBanners[1].Length) / 2, startY + 3);
                WriteAt(amountWindowFields[0], startX + 4, startY + 6);
                accountNoFieldPosition[0] = Console.CursorLeft;
                accountNoFieldPosition[1] = Console.CursorTop;
                WriteAt(amountWindowFields[1], startX + 4, startY + 7);
                amountFieldPosition[0] = Console.CursorLeft;
                amountFieldPosition[1] = Console.CursorTop;
                
                Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);

                while ((userKeyInfo = Console.ReadKey(false)).Key != ConsoleKey.Enter)
                {
                    if (!Char.IsDigit(userKeyInfo.KeyChar))
                    {
                        WriteAt("Invalid input, should be numbers. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        carryOn = false;
                        break;
                    }
                    userInputAccountNo += userKeyInfo.KeyChar;
                    // validate input while typing
                    if (!ValidUserInput(userInputAccountNo, 0, 10))
                    {
                        WriteAt("Invalid input, the length is within 10. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        carryOn = false;
                        break;
                    }
                    carryOn = true;
                }
                if (carryOn)
                {
                    WriteAt("Searching...", startX, startY + noLines + 2);
                    string filePath = $@"..\..\accounts\{userInputAccountNo}.txt";
                    // check whether the account file exist or not
                    if (!userAccount.CheckAccount(filePath))
                    {
                        WriteAt("Account not found." + new string(' ', 100), startX, startY + noLines + 2);
                        WriteAt("Check another account (y / n) ?", startX, startY + noLines + 3);
                        Console.SetCursorPosition(startX, startY + noLines + 4);
                        ConsoleKeyInfo pressedKey;
                        if ((pressedKey = Console.ReadKey(false)).Key == ConsoleKey.Y)
                        {
                            userInputAccountNo = "";
                            Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                            Console.Write(new string(' ', 20));
                            Console.SetCursorPosition(startX, startY + noLines + 4);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        WriteAt("Account found, enter the amount...", startX, startY + noLines + 2);
                        Console.SetCursorPosition(amountFieldPosition[0], amountFieldPosition[1]);
                        while ((userKeyInfo = Console.ReadKey(false)).Key != ConsoleKey.Enter)
                        {
                            if (!Char.IsDigit(userKeyInfo.KeyChar))
                            {
                                WriteAt("Invalid input, should be numbers. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                                userInputAmount = "";
                                Console.SetCursorPosition(amountFieldPosition[0], amountFieldPosition[1]);
                                Console.Write(new string(' ', 100));
                                carryOn = false;
                                break;
                            }
                            userInputAmount += userKeyInfo.KeyChar;
                            // validate input while typing
                            if (!ValidUserInput(userInputAmount, 0, 11))
                            {
                                WriteAt("Invalid input, the length is within 10. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                                userInputAmount = "";
                                Console.SetCursorPosition(amountFieldPosition[0], amountFieldPosition[1]);
                                Console.Write(new string(' ', 100));
                                carryOn = false;
                                break;
                            }
                            carryOn = true;
                        }
                        if (carryOn)
                        {
                            MapTextFileInfo(filePath);
                            try
                            {
                                userInputAmountDouble = Double.Parse(userInputAmount);
                            }
                            catch (Exception e)
                            {
                                WriteAt($"Invalid deposit amount: {e.Message}  Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                            }
                           
                            
                            double previousBalance = Double.Parse(accountDict["Balance"]);
                            double currentBalance = previousBalance + userInputAmountDouble;
                            bool validAmount = userInputAmount != null &&
                                               Regex.IsMatch(userInputAmount,@"([0-9]*[.])?[0-9]+") && 
                                               currentBalance > previousBalance;
                            Account userAcccount = new Account();
                            if (!validAmount)
                            {
                                WriteAt("Invalid deposit amount, should be smaller than your current balance. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                            }
                            else 
                            {
                                accountDict["Balance"] = currentBalance.ToString();
                                List<string> currentTrasaction = new List<string> 
                                                             { DateTime.Now.ToString("MM.dd.yyyy"), "Deposit", userInputAmount, currentBalance.ToString()};
                                if (transactionList.Count == 5)
                                {
                                    transactionList.RemoveAt(0);
                                    transactionList.Add(currentTrasaction);
                                }
                                else
                                {
                                    transactionList.Add(currentTrasaction);
                                }
                            }
                            if (userAccount.CheckAccount(filePath) && validAmount)
                            {
                                string textBody = "";
                                foreach (KeyValuePair<string, string> kvp in accountDict)
                                {
                                    textBody += $"{kvp.Key}|{kvp.Value}{Environment.NewLine}";
                                }
                                foreach (List<string> list in transactionList)
                                {
                                    string combinebString = string.Join("|", list.ToArray());
                                    textBody += $"{combinebString}{Environment.NewLine}";
                                }
                                File.WriteAllText(filePath, textBody);
                                WriteAt($"Deposit successfully. Your current balance is ${currentBalance}" + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                                break;
                            }
                        }
                    }
                }
            }
            while (true);
            ShowMainMenu(15, 60, 5, 3);
        }

        public void ShowWithdrawWindow(int noLines, int formWidth, int startX, int startY)
        {   
            do
            {
                // clear stored values in involved variables
                userInputAccountNo = "";
                userInputAmount = "";
                userKeyInfo = new ConsoleKeyInfo();
                userInputAmountDouble = 0;
                
                //draw the window
                Console.Clear();
                DrawWindow(noLines, formWidth, startX, startY);
                WriteAt(withdrawBanners[0], startX + (formWidth - depositBanners[0].Length) / 2, startY + 1);
                WriteAt(withdrawBanners[1], startX + (formWidth - depositBanners[1].Length) / 2, startY + 3);
                WriteAt(amountWindowFields[0], startX + 4, startY + 6);
                accountNoFieldPosition[0] = Console.CursorLeft;
                accountNoFieldPosition[1] = Console.CursorTop;
                WriteAt(amountWindowFields[1], startX + 4, startY + 7);
                amountFieldPosition[0] = Console.CursorLeft;
                amountFieldPosition[1] = Console.CursorTop;
                
                //get user input
                Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);

                while ((userKeyInfo = Console.ReadKey(false)).Key != ConsoleKey.Enter)
                {
                    if (!Char.IsDigit(userKeyInfo.KeyChar))
                    {
                        WriteAt("Invalid input, should be numbers. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        carryOn = false;
                        break;
                    }
                    userInputAccountNo += userKeyInfo.KeyChar;
                    // validate input while typing
                    if (!ValidUserInput(userInputAccountNo, 0, 10))
                    {
                        WriteAt("Invalid input, the length is within 10. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        carryOn = false;
                        break;
                    }
                    carryOn = true;
                }
                if (carryOn)
                { 
                    WriteAt("Searching...", startX, startY + noLines + 2);
                    string filePath = $@"..\..\accounts\{userInputAccountNo}.txt";
                    // check whether the account file exist or not
                    if (!userAccount.CheckAccount(filePath))
                    {
                        WriteAt("Account not found." + new string(' ', 100), startX, startY + noLines + 2);
                        WriteAt("Check another account (y / n) ?", startX, startY + noLines + 3);
                        Console.SetCursorPosition(startX, startY + noLines + 4);
                        ConsoleKeyInfo pressedKey;
                        if ((pressedKey = Console.ReadKey(false)).Key == ConsoleKey.Y)
                        {
                            userInputAccountNo = "";
                            Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                            Console.Write(new string(' ', 20));
                            Console.SetCursorPosition(startX, startY + noLines + 4);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        WriteAt("Account found, enter the amount...", startX, startY + noLines + 2);
                        Console.SetCursorPosition(amountFieldPosition[0], amountFieldPosition[1]);
                        while ((userKeyInfo = Console.ReadKey(false)).Key != ConsoleKey.Enter)
                        {
                            if (!Char.IsDigit(userKeyInfo.KeyChar))
                            {
                                WriteAt("Invalid input, should be numbers. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                                userInputAmount = "";
                                Console.SetCursorPosition(amountFieldPosition[0], amountFieldPosition[1]);
                                Console.Write(new string(' ', 100));
                                carryOn = false;
                                break;
                            }
                            userInputAmount += userKeyInfo.KeyChar;
                            // validate input while typing
                            if (!ValidUserInput(userInputAmount, 0, 11))
                            {
                                WriteAt("Invalid input, the length is within 10." + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                                userInputAmount = "";
                                Console.SetCursorPosition(amountFieldPosition[0], amountFieldPosition[1]);
                                Console.Write(new string(' ', 100));
                                carryOn = false;
                                break;
                            }
                            carryOn = true;
                        }
                        if (carryOn)
                        {
                            MapTextFileInfo(filePath);
                            try
                            {
                                userInputAmountDouble = Double.Parse(userInputAmount);
                            }
                            catch (Exception e)
                            {
                                WriteAt($"Invalid deposit amount: {e.Message}  Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                            }
                            double previousBalance = Double.Parse(accountDict["Balance"]);
                            double currentBalance = previousBalance - userInputAmountDouble;
                            bool validAmount = userInputAmount != null &&
                                               Regex.IsMatch(userInputAmount, @"([0-9]*[.])?[0-9]+") && 
                                               currentBalance < previousBalance && 
                                               currentBalance >= 0;
                            if (!validAmount)
                            {
                                WriteAt("Invalid withdraw amount, should be smaller than your current balance. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                            }
                            else
                            {
                                accountDict["Balance"] = currentBalance.ToString();
                                List<string> currentTrasaction = new List<string>
                                                             { DateTime.Now.ToString("MM.dd.yyyy"), "Withdraw", userInputAmount, currentBalance.ToString()};
                                if (transactionList.Count == 5)
                                {
                                    transactionList.RemoveAt(0);
                                    transactionList.Add(currentTrasaction);
                                }
                                else
                                {
                                    transactionList.Add(currentTrasaction);
                                }
                            }

                            if (userAccount.CheckAccount(filePath) && validAmount)
                            { 
                                string textBody = "";
                                foreach (KeyValuePair<string, string> kvp in accountDict)
                                {
                                    textBody += $"{kvp.Key}|{kvp.Value}{Environment.NewLine}";
                                }
                                foreach (List<string> list in transactionList)
                                {
                                    string combinebString = string.Join("|", list.ToArray());
                                    textBody += $"{combinebString}{Environment.NewLine}";
                                }
                                File.WriteAllText(filePath, textBody);
                                
                                WriteAt($"Withdraw successfully. Your current balance is ${currentBalance}" + new string(' ', 100), startX, startY + noLines + 2);
                                _ = Console.ReadKey();
                                break;
                            }
                        }
                    }
                }
            }
            while (true);
            ShowMainMenu(15, 60, 5, 3);
        }

        public void DeleteAccountWindow(int noLines, int formWidth, int startX, int startY)
        {
            //get user input
            do
            {
                // clear stored values in involved variables
                userInputAccountNo = "";
                userKeyInfo = new ConsoleKeyInfo();

                //draw the window
                Console.Clear();
                DrawWindow(noLines, formWidth, startX, startY);
                WriteAt(deleteAccountBanners[0], startX + (formWidth - deleteAccountBanners[0].Length) / 2, startY + 1);
                WriteAt(deleteAccountBanners[1], startX + (formWidth - deleteAccountBanners[1].Length) / 2, startY + 3);
                WriteAt(accountNoField, startX + 4, startY + 6);
                accountNoFieldPosition[0] = Console.CursorLeft;
                accountNoFieldPosition[1] = Console.CursorTop;
                
                Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);

                while ((userKeyInfo = Console.ReadKey(false)).Key != ConsoleKey.Enter)
                {
                    // validate input while typing
                    if (!Char.IsDigit(userKeyInfo.KeyChar))
                    {
                        WriteAt("Invalid input, should be numbers. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        userInputAccountNo = "";
                        Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                        Console.Write(new string(' ', 100));
                        carryOn = false;
                        break;
                    }
                    userInputAccountNo += userKeyInfo.KeyChar;

                    if (!ValidUserInput(userInputAccountNo, 0, 10))
                    {
                        WriteAt("Invalid input, the length is within 10. Press [any key] to try again." + new string(' ', 100), startX, startY + noLines + 2);
                        _ = Console.ReadKey();
                        userInputAccountNo = "";
                        Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                        Console.Write(new string(' ', 100));
                        carryOn = false;
                        break;
                    }
                    carryOn = true;
                }
                if (carryOn)
                {
                    WriteAt("Searching...", startX, startY + noLines + 2);
                    string filePath = $@"..\..\accounts\{userInputAccountNo}.txt";
                    // check whether the account file exist or not
                    if (!userAccount.CheckAccount(filePath))
                    {
                        WriteAt("Account not found." + new string(' ', 100), startX, startY + noLines + 2);
                        WriteAt("Check another account (y / n) ?", startX, startY + noLines + 3);
                        Console.SetCursorPosition(startX, startY + noLines + 4);
                        ConsoleKeyInfo pressedKey;
                        if ((pressedKey = Console.ReadKey(false)).Key == ConsoleKey.Y)
                        {
                            userInputAccountNo = "";
                            Console.SetCursorPosition(accountNoFieldPosition[0], accountNoFieldPosition[1]);
                            Console.Write(new string(' ', 20));
                            Console.SetCursorPosition(startX, startY + noLines + 4);
                            Console.Write(new string(' ', 20));
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        WriteAt("Account found.", startX, startY + noLines + 2);
                        string[] accountInfo = File.ReadAllLines(filePath);
                        ShowFileContent(accountInfo, "Account Details", 60, 5, startY + noLines + 4);
                        WriteAt("Delete (y / n) ?", startX, startY + noLines + accountInfo.Length + 8);
                        Console.SetCursorPosition(startX, startY + noLines + accountInfo.Length + 9);
                        ConsoleKeyInfo pressedKey;
                        if ((pressedKey = Console.ReadKey(false)).Key == ConsoleKey.Y)
                        {
                            userInputAccountNo = "";
                            userAccount.DeleteAccount(filePath);
                            WriteAt("Account Deleted.", startX, startY + noLines + accountInfo.Length + 10);
                            _ = Console.ReadKey();
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            while (true);
            ShowMainMenu(15, 60, 5, 3);
        }
    }
}