namespace EventDetection.Tests;

using System.Reflection;
using System.Reflection.Metadata;
using EventDetection.Services;
using EventDetection.Models;

[TestClass]
public class EventDetectorTest
{
    private EventDetector _detector = null!;
    private List<Measurement> _measurementsSingleEvent = null!;
    private double _thresholdDefault = 0.0;
    private int _minDurationDefault = 5;
    private int _maxGapDefault = 10;

    [TestInitialize]
    public void Setup()
    {
        _detector = new EventDetector();
        _measurementsSingleEvent = new List<Measurement>
        {
            new Measurement(0, 0),
            new Measurement(120000, 5),
            new Measurement(240000, 10),
            new Measurement(360000, 5),
            new Measurement(480000, 0)
        };
    }

    [TestMethod]
    public void DetectEvents_SingleEvent_ReturnsEvent()
    {
        // Arrange
        var detector = _detector;
        var measurements = _measurementsSingleEvent;
        var threshold = _thresholdDefault;
        var minDuration = _minDurationDefault;
        var maxGap = _maxGapDefault;

        // Act
        var events = detector.DetectEvents(measurements, threshold, minDuration, maxGap);

        // Assert
        Assert.IsTrue(events.Count == 1);
    }

    [TestMethod]
    public void DetectEvents_NoEvents_ReturnsEmptyList()
    {
        // Arrange
        var detector = _detector;
        var measurements = new List<Measurement>
        {
            new Measurement(0, 0),
            new Measurement(120000, 0),
            new Measurement(240000, 0),
        };
        var threshold = _thresholdDefault;
        var minDuration = _minDurationDefault;
        var maxGap = _maxGapDefault;

        // Act
        var events = detector.DetectEvents(measurements, threshold, minDuration, maxGap);

        // Assert
        Assert.IsTrue(events.Count == 0);
    }

    [TestMethod]
    public void DetectEvents_EventBelowThreshold_Ignored()
    {
        // Arrange:
        var detector = _detector;
        var measurements = _measurementsSingleEvent;
        var threshold = 11; // Set threshold higher than any measurement
        var minDuration = _minDurationDefault;
        var maxGap = _maxGapDefault;

        // Act
        var events = detector.DetectEvents(measurements, threshold, minDuration, maxGap);

        // Assert
        Assert.IsTrue(events.Count == 0);
    }

    [TestMethod]
    public void DetectEvents_ShortEventWithinMaxGap_MergesIntoSingleEvent()
    {
        // Arrange
        var detector = _detector;
        var measurements = new List<Measurement>
        {
            new Measurement(0, 0),
            new Measurement(120000, 5),    // above threshold (2 min)
            new Measurement(240000, 0),    // short gap (2 min) - within maxGap
            new Measurement(360000, 5),    // above threshold again (6 min)
            new Measurement(480000, 0)
        };
        var threshold = _thresholdDefault;
        var minDuration = _minDurationDefault;
        var maxGap = _maxGapDefault; // 10 minutes

        // Act
        var events = detector.DetectEvents(measurements, threshold, minDuration, maxGap);

        // Assert - the two above-threshold segments should be merged into one event
        Assert.IsTrue(events.Count == 1);

        var ev = events[0];
        var expectedStart = DateTimeOffset.FromUnixTimeMilliseconds(120000).UtcDateTime;
        var expectedEnd = DateTimeOffset.FromUnixTimeMilliseconds(360000).UtcDateTime;
        Assert.AreEqual(expectedStart, ev.Start);
        Assert.AreEqual(expectedEnd, ev.End);
    }

    [TestMethod]
    public void DetectEvents_EventShorterThanMinDuration_Ignored()
    {
        // Arrange
        var detector = _detector;
        var measurements = new List<Measurement>
        {
            new Measurement(0, 0),
            new Measurement(120000, 5),    // event starts at 2 min
            new Measurement(240000, 5),    // still above at 4 min
            new Measurement(360000, 5),    // still above at 6 min
            new Measurement(480000, 0)     // event ends before 5 min minimum
        };
        var threshold = _thresholdDefault;
        var minDuration = _minDurationDefault; // 5 minutes
        var maxGap = _maxGapDefault;

        // Act
        var events = detector.DetectEvents(measurements, threshold, minDuration, maxGap);

        // Assert - the detected event is shorter than minDuration and should be ignored
        Assert.IsTrue(events.Count == 0);
    }
}