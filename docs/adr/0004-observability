# 0004 — Observability strategy (logging, metrics, debugging)

**Date:** 2026-06-01 | **Status:** Accepted | **Owner:** Dev team

---

## Context

We needed a clear approach to observability for the application — specifically what we log, what we measure, and how we debug when things go wrong. Without this, production issues are hard to reproduce and AI response quality is invisible.

---

## Decision

Use **ILogger** (via .NET's built-in logging abstraction) as the primary logging mechanism, with structured logs and metrics focused on AI response behaviour.

---

## What we track

| Signal | What | Why |
|---|---|---|
| **AI response time** | Time from request to completed response | Spot latency spikes; set performance baselines |
| **Response length** | Token/character count per response | Detect runaway outputs or unexpectedly short replies |
| **Response relevance** | Relevance score or manual tag | Catch degraded output quality over time |
| **Structured logs** | Request context, errors, warnings | Reproduce bugs; trace failures end-to-end |

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
- Straightforward to pipe into Azure Monitor / Application Insights if needed later.

**Watch out for:**
- Relevance scoring requires defining what "relevant" means — needs a rubric or human review loop.
- Logging response content may have privacy implications depending on what users send.

---
