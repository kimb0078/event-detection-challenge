using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Load input data
var json = File.ReadAllText("overflow-timeseries.json");
var measurements = JsonSerializer.Deserialize<List<Measurement>>(json);
if (measurements == null)
    throw new InvalidOperationException("Failed to parse JSON data.");

// GET /events?threshold=0.0&minDuration=5&maxGap=10
app.MapGet("/events", (double? threshold, int? minDuration, int? maxGap) =>
{
    var detector = new EventDetector();

    var events = detector.DetectEvents(
        measurements,
        threshold ?? 0.0,
        minDuration ?? 5,
        maxGap ?? 10
    );

    return Results.Json(events);
});

app.Run();



