# Brief — Overflow Event Detector

You’re given a **2-minute sampled time-series** for an [overflow structure](https://en.wikipedia.org/wiki/Sanitary_sewer_overflow) for three months.  
The column `value` is the parameter **“Overflow Height [m]”**, where **`> 0` means overflow**. The timestamp is UTC.

---

##  Task

Build an **event detector** that:

### 1. Detects overflow events using:
- **`threshold`**: `0.0 m` (strictly `> threshold` marks overflow samples)  
- **`minDuration`**: minimum contiguous overflow duration to accept an event *(default: 5 minutes)*  
- **`maxGap`**: allow stitching of short dry gaps between overflow samples into the same event *(default: 2 minutes)*  

### 2. Outputs an ordered list of events with:
- `start`, `end`, `durationMinutes`  
- `peakValue` *(max)*  
- `sumExceedance` *(∑ (value - threshold) · Δt)* — treat **Δt = 60 s per sample**

### 3. Exposes the result two ways:
- **CLI:**  
  ```bash
  detect --input data/month.csv --minDuration 5 --maxGap 2 --threshold 0.0 --out events.json
  ```
- **HTTP (optional but recommended):**  
  ```
  GET /events?minDuration=...&maxGap=...&threshold=...
  ```
  → returns JSON

---

##  Deliverables (Approx. 2 hours)

-  Working detector with CLI  
-  Unit tests *(happy path + edge cases)*  
-  README explaining how to run and your assumptions  
- 💡 **Bonus:** HTTP endpoint and summary stats  

---

## What We Evaluate

| Area | Focus |
|------|-------|
| **Correctness** | Event boundaries & metrics |
| **Code clarity & structure** | Readability, separation of concerns |
| **Testing** | Coverage of both happy path & edge cases |
| **Performance** | Single-pass or efficient logic, low memory |
| **API/CLI ergonomics** | Clean, usable interface |

---

## 💡 Hints (to keep you on-rails)

- Treat input as **monotonically increasing timestamps** at 1-minute steps; if gaps exist, the algorithm should still behave.  
- Prefer a **single linear pass** with a simple state machine:  
  `idle → inEvent → maybeGap → inEvent`.  
- Be explicit about **inclusive/exclusive end time** — note it in your README and test against it.  
- Use **UTC time handling**; don’t localize.  
