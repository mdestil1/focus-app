// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace SpotifyTestApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } // Primary Key



        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } // First Name

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } // Last Name
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Store hashed passwords


        //// New Fields for Subscription
       // public bool IsSubscribed { get; set; } = false; // Indicates if the user has an active subscription
        //public string SubscriptionId { get; set; } // Stripe subscription ID
        //public DateTime? SubscriptionEndDate { get; set; } // When the subscription ends
    
    
        // Additional fields can be added as needed
    }
}
