using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace hb29.API.Helpers
{
    public class BusinessValidation
    {
        private readonly ILogger _logger;

        public class BusinessValidationError
        {
            public string Message { get; set; }
        }


        public BusinessValidation() { }
        public BusinessValidation(ILogger logger)
        {
            _logger = logger;
        }

        public List<BusinessValidationError> Errors { get; set; } = new List<BusinessValidationError>();

        public void AddError(BusinessValidationError error, object[] args)
        {
            if (_logger != null)
                _logger.LogWarning(error.Message, args);

            this.Errors.Add(error);
        }

        public void AddError(string message)
        {
            AddError(new BusinessValidationError() { Message = message }, null );
        }
        public void AddError(string message, params object[] args)
        {
            AddError(new BusinessValidationError() { Message = message }, args);
        }

        public bool IsValid
        {
            get { return this.Errors.Count == 0; }
        }

        /// <summary>
        /// Returns a JSON string representation of current object state.
        /// </summary>
        public override string ToString()
        {
            //return string.Join("\n", Errors.Select(e => e.Message));
            return JsonSerializer.Serialize(this);
        }
    }
}
