using System;

namespace hb29.Shared.DTO
{
    public class NodeTemplateQueue
    {
        public long CustomerId { get; set; }
        public long TechnologyId { get; set; }
        public string FileName { get; set; }
        public string QueueMessageId { get; set; }
        public long QueueSequenceNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}