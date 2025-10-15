# Brief — Overflow Event Detector

You’re given a **2-minute sampled time-series** for an [overflow structure](https://en.wikipedia.org/wiki/Sanitary_sewer_overflow) for three months.  
The column `value` is the parameter **“Overflow Height [m]”**, where **`> 0` means overflow**. The timestamp is UTC.

---

## Data Format

- File: `overflow-timeseries.json`
- Structure: 2D array of `[timestamp_ms_utc, value_m]`
- `timestamp_ms_utc`: Unix epoch time in milliseconds (UTC)
- `value_m`: overflow height in meters
- Sampling: nominal 2-minute interval (gaps may exist, IoT data is typically incomplete)
- Example: `[1752537658000.0, 0.0]`

##  Task

Build an **event detector** that:

### 1. Detects overflow events using:
- **`threshold`**: `0.0 m` (value `> threshold` marks overflow samples)  
- **`minDuration`**: minimum contiguous overflow duration to accept an event *(default: 5 minutes)*  
- **`maxGap`**: allow stitching of short dry gaps between overflow samples into the same event *(default: 60 minutes)*  

### 2. Outputs an ordered list of events with:
- `start`, `end`, `durationMinutes`  
- `peakValue` *(max)*  

### 3. Exposes the results:
- **HTTP:**  
  ```
  GET /events?minDuration=...&maxGap=...&threshold=...
  ```
  → returns JSON

---

##  Deliverable

-  Working detector with HTTP endpoint  
-  Unit tests *(happy path + edge cases)*  
-  README explaining how to run and your assumptions  
- 💡 **Bonus - but optional:** Add Dockerfile for your application and expose the HTTP endpoint as mentioned in Task section #3

---

## What We Evaluate

| Area | Focus |
|------|-------|
| **Correctness** | Event boundaries & metrics |
| **Code clarity & structure** | Readability, separation of concerns |
| **Testing** | Basic coverage of both happy path & edge cases |
| **Performance** | Single-pass or efficient logic, low memory |
| **API ergonomics** | Clean, usable interface |

---

## 💡 Hints/Implementation Notes (to keep you on-rails)

- Treat input as **monotonically increasing timestamps** at 2-minute steps; if gaps exist, the algorithm should still behave.  
- Prefer a **single linear pass**; maintain rolling state for current event (start, last_seen, peak, running length); “close” when dry gap exceeds maxGap.
- Be explicit about **inclusive/exclusive end time** — note it in your README and test against it.  
- Use **UTC time handling**; don’t localize.  
- Reject samples <= threshold.
