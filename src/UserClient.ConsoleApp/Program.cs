using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using UserClient.Infrastructure;
using UserClient.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using UserClient.Infrastructure.Extensions;
using UserClient.Core.Interfaces;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();          
        logging.AddConsole();              
        logging.AddDebug();                
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<ApiSettings>(context.Configuration.GetSection("ApiSettings"));
        services.AddUserClient(context.Configuration);
    })
    .Build();

var service = host.Services.GetRequiredService<IExternalUserService>();




bool exitRequested = false;

while (!exitRequested)
{
    Console.Clear();
    Console.WriteLine("*************************USER API******************************");
    Console.WriteLine("Choose an option:");
    Console.WriteLine("1. Get all users");
    Console.WriteLine("2. Get user by ID");
    Console.WriteLine("3. Exit");
    Console.WriteLine("*************************USER API******************************");

    Console.Write("Please enter your choice : ");
    var choice = Console.ReadLine();

    Console.WriteLine();

    switch (choice)
    {
        case "1":
            try
            {
                var users = await service.GetAllUsersAsync();
                foreach (var user in users)
                {
                    Console.WriteLine($"{user.Id}: {user.First_Name} {user.Last_Name} - {user.Email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            break;

        case "2":
            Console.Write("Enter User ID: ");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                try
                {
                    var user = await service.GetUserByIdAsync(userId);
                    if (user != null)
                    {
                        Console.WriteLine($"{user.Id}: {user.First_Name} {user.Last_Name} - {user.Email}");
                    }
                    else
                    {
                        Console.WriteLine("User not found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID.");
            }
            break;

        case "3":
            exitRequested = true;
            Console.WriteLine("Exiting...");
            break;

        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }

    if (!exitRequested)
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}