namespace Databasprojekt.Helpers;
public class AdminHelper
{
    private static string? _adminPassword = string.Empty;

    public static async Task AdminCheckAsync()
    {
        if (_adminPassword == string.Empty)
        {
            await CreatePasswordAsync();
        }
        else
        {
            await EnterPasswordAsync();
        }
    }
    
    public static async Task CreatePasswordAsync()
    {
        Console.WriteLine("Please create a admin password:");
        _adminPassword = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(_adminPassword) || _adminPassword.Length > 50)
        {
            Console.WriteLine("Invalid password, can not be empty or more than 50 characters");
        }
        _adminPassword = EncryptionHelper.Encrypt(_adminPassword);
    }

    public static async Task EnterPasswordAsync()
    {
        while (true)
        {
            Console.WriteLine("Please enter admin password:");
            var password = Console.ReadLine()?.Trim();
            if (password == EncryptionHelper.Decrypt(_adminPassword))
            {
                Console.WriteLine("Password entered successfully");
                return;
            }
            else
            {
                Console.WriteLine("Invalid password");
            }
        }
    }
}