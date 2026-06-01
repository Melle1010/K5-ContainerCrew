# 0004 — Observability strategy (logging, metrics, debugging)

**Date:** 2026-06-01 | **Status:** Accepted | **Owner:** Dev team

---

## Context

We needed a clear approach to observability for the application — specifically what we log, what we measure, and how we debug when things go wrong. Without this, production issues are hard to reproduce and AI response quality is invisible.

---

## Decision

Use **ILogger** (via .NET's built-in logging abstraction) as the primary logging mechanism, with structured logs and metrics focused on AI response behaviour. We target a **faithfulness score of ≥ 95%** — meaning at least 95% of responses should be relevant and accurate answers to the user's query.

---

## What we track

| Signal | What | Why |
|---|---|---|
| **AI response time** | Time from request to completed response | Spot latency spikes; set performance baselines |
| **Response length** | Token/character count per response | Detect runaway outputs or unexpectedly short replies |
| **Response relevance** | Relevance/faithfulness score per response | Track progress toward the 95% faithfulness target |
| **Structured logs** | Request context, errors, warnings | Reproduce bugs; trace failures end-to-end |

---

## Quality target

**Faithfulness ≥ 95%** — at least 95 out of 100 responses should be relevant, accurate, and on-topic. Responses that score below threshold are flagged for review. If the rolling average drops below 95%, it's treated as a production issue.

---

## Why ILogger

- Ships with .NET — no extra dependency.
- Works with any backend (console, Azure Monitor, Application Insights) by swapping the provider, not the code.
- Structured logging support built in; easy to query in Log Analytics.

---

## Consequences

**Good:**
- Consistent log format across the app from day one.
- AI response metrics give visibility into quality, not just uptime.
- The 95% faithfulness target gives the team a concrete bar to work toward and catch regressions early.
- Straightforward to pipe into Azure Monitor / Application Insights if needed later.

**Watch out for:**
- Faithfulness scoring requires a clear rubric for what counts as a good answer — needs to be defined and agreed on before measuring.
- Automated scoring alone may not be reliable enough; plan for a human review sample.
- Logging response content may have privacy implications depending on what users send.

---

## Follow-up

Review faithfulness scores after the first month in production. Check whether scores correlate with actual user complaints, and adjust the rubric or logging if noise outweighs signal.
