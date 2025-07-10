namespace Bookify.Web.Core.Consts
{
    public static class Errors
    {
        // General Validation
        public const string RequiredField = "Required field";
        public const string MaxLength = "Length cannot be more than {1} characters";
        public const string MaxMinLength = "The {0} must be at least {2} and at max {1} characters long.";
        public const string InvalidRange = "{0} should be between {1} and {2}!";
        public const string NotAllowFutureDates = "Date cannot be in the future!";
        public const string DenySpecialCharacters = "Special characters are not allowed.";

        // Format & Input
        public const string InvalidMobileNumber = "Invalid mobile number.";
        public const string InvalidNationalId = "Invalid national ID.";
        public const string InvalidSerialNumber = "Invalid serial number.";
        public const string InvalidUsername = "Username can only contain letters or digits.";
        public const string OnlyEnglishLetters = "Only English letters are allowed.";
        public const string OnlyArabicLetters = "Only Arabic letters are allowed.";
        public const string OnlyNumbersAndLetters = "Only Arabic/English letters or digits are allowed.";

        // File Upload
        public const string NotAllowedExtension = "Only .png, .jpg, .jpeg files are allowed!";
        public const string MaxSize = "File cannot be more than 2 MB!";
        public const string EmptyImage = "Please select an image.";

        // Authentication & Password
        public const string ConfirmPasswordNotMatch = "The password and confirmation password do not match.";
        public const string WeakPassword = "Passwords contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least 8 characters long.";

        // Duplicates
        public const string Duplicated = "Another record with the same {0} already exists!";
        public const string DuplicatedBook = "Book with the same title already exists with the same author!";
        public const string DuplicateCopyInRental = "This copy is already included in the current rental.";

        // Rental System
        public const string NotAvilableRental = "This book/copy is not available for rental.";
        public const string CopyIsInRental = "This copy is already rentaled.";
        public const string MaxCopiesReached = "This subscriber has reached the max number for rentals.";
        public const string CannotCancelRental = "You can only cancel rentals created today.";
        public const string ExtendNotAllowed = "Extension is not allowed because the rental period has expired.";
        public const string PanltyShoudPaid = "Penalty must be paid if there is a delay.";


        // Subscriber Status
        public const string BlackListedSubscriber = "This subscriber is blacklisted.";
        public const string InactiveSubscriber = "This subscriber is inactive. Please renew their subscription.";
    }
}
