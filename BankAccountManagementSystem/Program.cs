
namespace BankAccountManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            UserInterface MyBank = new UserInterface();
            MyBank.ShowLoginWindow(10, 60, 5, 3);
        }
    }
}
