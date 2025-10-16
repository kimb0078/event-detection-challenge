namespace EventDetection.Models
{
    public class Event
    {
        // Start time of the event
        public DateTime Start { get; set; }

        // End time of the event
        public DateTime End { get; set; }

        // Duration in minutes (computed)
        public double DurationMinutes => (End - Start).TotalMinutes;

        // Peak value associated with the event
        public double PeakValue { get; set; }

        public Event() { }

        public Event(DateTime start, DateTime end, double peakValue)
        {
            Start = start;
            End = end;
            PeakValue = peakValue;
        }
    }
}