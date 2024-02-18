namespace SeminarHub.Data.Common
{
    public class DataConstants
    {
        public const int TopicNameMinimumLength = 3;
        public const int TopicNameMaximumLength = 100;

        public const int LecturerMinimumLength = 5;
        public const int LecturerMaximumLength = 60;

        public const int DetailsMinimumLength = 10;
        public const int DetailsMaximumLength = 500;

        public const string DateFormat = "dd/MM/yyyy HH:mm";

        public const int DurationMinimum = 30;
        public const int DurationMaximum = 180;

        public const int CategoryNameMinimumLength = 3;
        public const int CategoryNameMaximumLength = 50;

        public const string RequireErrorMessage = "The field {0} is required";
        public const string StringLengthErrorMessage = "The field {0} must be between {1} and {2} characters long";
    }
}
