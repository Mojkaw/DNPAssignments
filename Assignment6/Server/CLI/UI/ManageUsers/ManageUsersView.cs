using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public sealed class ManageUsersView
{
   private readonly IUserRepository userRepository;
   
   public ManageUsersView(IUserRepository userRepository)
      {
      this.userRepository = userRepository;
      }

   public async Task ShowAsync()
   {
      while (true)
      {
         PrintOptions();
         var choice = Console.ReadLine();

         switch (choice)
         {
            case "1":
               await new CreateUserView(userRepository).ShowAsync();
               break;
            case "2":
               await new ListUsersView(userRepository).ShowAsync();
               break;
            case "3":
               await new DeleteUserView(userRepository).ShowAsync();
               break;
            case "0":
               return;
            default:
               Console.WriteLine("Please enter a valid option.");
               break;
         }
      }
   }

   private static void PrintOptions()
   {
      Console.WriteLine();
      Console.WriteLine("=== Manage Users ===");
      Console.WriteLine("1) Create User");
      Console.WriteLine("2) List Users");
      Console.WriteLine("3) Delete User");   
      Console.WriteLine("0) Back");
      Console.Write("> ");
   }
}