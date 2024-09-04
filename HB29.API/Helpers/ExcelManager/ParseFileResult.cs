using System.Collections.Generic;

namespace hb29.API.Helpers
{
    public class ParseFileResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public T Item { get; set; }
        public BusinessValidation Validations { get; private set; } = new BusinessValidation();
    }
}
