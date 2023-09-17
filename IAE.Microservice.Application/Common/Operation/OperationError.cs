namespace IAE.Microservice.Application.Common.Operation
{
    /// <summary>
    /// Encapsulates an error.
    /// </summary>
    public class OperationError
    {
        /// <summary>
        /// Gets or sets the code for this error.
        /// </summary>
        /// <value>
        /// The code for this error.
        /// </value>
        public string Code { get; private set; }

        /// <summary>
        /// Gets or sets the description for this error.
        /// </summary>
        /// <value>
        /// The description for this error.
        /// </value>
        public string Description { get; private set; }

        public static OperationError Init(string code, string description)
        {
            return new OperationError
            {
                Code = code,
                Description = description
            };
        }
    }
}