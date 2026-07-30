namespace StitchFluentOcrPro.Models
{
    /// <summary>
    /// Represents the processing state of a PDF job in the queue.
    /// </summary>
    public enum JobStatus
    {
        Queued,
        Processing,
        Paused,
        Completed,
        Failed,
        Skipped
    }
}
