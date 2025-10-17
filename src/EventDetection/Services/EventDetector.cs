using EventDetection.Models;

namespace EventDetection.Services;

public class EventDetector
{
    public List<Event> DetectEvents(
        List<Measurement> measurements,
        double threshold,
        int minDuration,
        int maxGap)
    {
        var events = new List<Event>();
        Event? currentEvent = null;
        DateTime? lastAboveThresholdTime = null;

        foreach (var measurement in measurements)
        {
            var time = DateTimeOffset.FromUnixTimeMilliseconds(measurement.TimestampMsUtc).UtcDateTime;
            if (measurement.ValueM > threshold)
            {
                if (currentEvent == null)
                {
                    currentEvent = new Event
                    {
                        Start = time,
                        End = time,
                        PeakValue = measurement.ValueM
                    };
                }
                else
                {
                    currentEvent.End = time;
                    if (measurement.ValueM > currentEvent.PeakValue)
                        currentEvent.PeakValue = measurement.ValueM;
                }
                lastAboveThresholdTime = time;
            }
            else
            {
                if (currentEvent != null && lastAboveThresholdTime.HasValue)
                {
                    var gap = (time - lastAboveThresholdTime.Value).TotalMinutes;
                    if (gap > maxGap)
                    {
                        // Close current event if it meets the minimum duration
                        if (currentEvent.DurationMinutes >= minDuration)
                        {
                            events.Add(currentEvent);
                        }
                        currentEvent = null;
                    }
                }
            }
        }

        // Add the last event if it exists and meets the minimum duration
        if (currentEvent != null && currentEvent.DurationMinutes >= minDuration)
        {
            events.Add(currentEvent);
        }

        return events;
    }
}