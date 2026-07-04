namespace DungeonCrawler.Combat
{
    public readonly struct TargetingValidationResult
    {
        private TargetingValidationResult(bool isValid, string errorCode, string errorMessage)
        {
            IsValid = isValid;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public bool IsValid { get; }

        public string ErrorCode { get; }

        public string ErrorMessage { get; }

        public static TargetingValidationResult Valid()
        {
            return new TargetingValidationResult(true, string.Empty, string.Empty);
        }

        public static TargetingValidationResult Invalid(string errorCode, string errorMessage)
        {
            return new TargetingValidationResult(false, errorCode, errorMessage);
        }
    }
}
