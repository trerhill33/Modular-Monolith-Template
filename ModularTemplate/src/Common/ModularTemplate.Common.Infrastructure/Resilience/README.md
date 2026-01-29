# Resilience Patterns

This folder contains resilience infrastructure for chaos engineering readiness, implementing circuit breaker, retry, and timeout patterns using [Polly](https://github.com/App-vNext/Polly).

## Overview

The resilience layer protects the application from cascading failures when external dependencies (AWS EventBridge, SQS, Redis, etc.) become slow or unavailable.

## Components

| Component | Purpose |
|-----------|---------|
| `ResilienceOptions` | Configuration for retry, circuit breaker, and timeout policies |
| `ResilientEventBridgeEventBus` | Decorator that wraps `IEventBus` with resilience patterns |

## Resilience Pipeline

The resilience pipeline is applied in the following order (outermost to innermost):

```
┌─────────────────────────────────────────────────────────────┐
│  1. Total Timeout                                           │
│     Caps entire operation including all retries             │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  2. Retry                                             │  │
│  │     Retries failed attempts with exponential backoff  │  │
│  │  ┌─────────────────────────────────────────────────┐  │  │
│  │  │  3. Circuit Breaker                             │  │  │
│  │  │     Fails fast when error threshold exceeded    │  │  │
│  │  │  ┌───────────────────────────────────────────┐  │  │  │
│  │  │  │  4. Attempt Timeout                       │  │  │  │
│  │  │  │     Caps each individual attempt          │  │  │  │
│  │  │  │                                           │  │  │  │
│  │  │  │     → External Call (EventBridge, etc.)   │  │  │  │
│  │  │  │                                           │  │  │  │
│  │  │  └───────────────────────────────────────────┘  │  │  │
│  │  └─────────────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## Circuit Breaker States

```
     ┌────────────────────────────────────────────┐
     │                  CLOSED                     │
     │    (Normal operation - calls go through)    │
     │                                             │
     │  Tracks: failure count / total requests     │
     └──────────────────┬──────────────────────────┘
                        │
            Failure threshold exceeded
            (e.g., 20% failures over 10s)
                        │
                        ▼
     ┌────────────────────────────────────────────┐
     │                   OPEN                      │
     │   (Fail-fast - immediately reject calls)   │
     │                                             │
     │          Wait for BreakDuration             │
     └──────────────────┬──────────────────────────┘
                        │
              Break duration elapsed
                        │
                        ▼
     ┌────────────────────────────────────────────┐
     │               HALF-OPEN                     │
     │    (Test the waters - allow one call)      │
     │                                             │
     │   Success → CLOSED   |   Failure → OPEN    │
     └────────────────────────────────────────────┘
```

## Configuration

Configure resilience behavior in `appsettings.json`:

```json
{
  "Resilience": {
    "Retry": {
      "MaxRetryAttempts": 3,
      "BaseDelayMilliseconds": 1000,
      "UseJitter": true
    },
    "CircuitBreaker": {
      "SamplingDurationSeconds": 10,
      "FailureRatio": 0.2,
      "MinimumThroughput": 3,
      "BreakDurationSeconds": 30
    },
    "Timeout": {
      "TotalTimeoutSeconds": 30,
      "AttemptTimeoutSeconds": 10
    }
  }
}
```

### Configuration Options

#### Retry Options

| Option | Default | Description |
|--------|---------|-------------|
| `MaxRetryAttempts` | 3 | Maximum number of retry attempts before failing |
| `BaseDelayMilliseconds` | 1000 | Base delay between retries (increases exponentially) |
| `UseJitter` | true | Add randomness to prevent thundering herd problems |

#### Circuit Breaker Options

| Option | Default | Description |
|--------|---------|-------------|
| `SamplingDurationSeconds` | 10 | Time window for measuring failure rate |
| `FailureRatio` | 0.2 | Failure percentage (0.0-1.0) to trigger circuit open |
| `MinimumThroughput` | 3 | Minimum requests before circuit can evaluate |
| `BreakDurationSeconds` | 30 | How long circuit stays open before testing |

#### Timeout Options

| Option | Default | Description |
|--------|---------|-------------|
| `TotalTimeoutSeconds` | 30 | Maximum time for entire operation including retries |
| `AttemptTimeoutSeconds` | 10 | Maximum time for each individual attempt |

### Environment-Specific Configuration

**Development** (`appsettings.Development.json`):
```json
{
  "Resilience": {
    "Retry": {
      "MaxRetryAttempts": 2,
      "BaseDelayMilliseconds": 500
    },
    "CircuitBreaker": {
      "SamplingDurationSeconds": 5,
      "BreakDurationSeconds": 10
    },
    "Timeout": {
      "TotalTimeoutSeconds": 10,
      "AttemptTimeoutSeconds": 3
    }
  }
}
```

Shorter timeouts and fewer retries in development for faster feedback.

## Logging

The resilience layer logs important events:

| Event | Level | Description |
|-------|-------|-------------|
| Retry attempt | Warning | Each retry with attempt number and delay |
| Circuit opened | Error | Circuit breaker tripped due to failures |
| Circuit half-open | Information | Circuit testing if service recovered |
| Circuit closed | Information | Service recovered, normal operation resumed |
| Attempt timeout | Warning | Individual attempt exceeded timeout |
| Total timeout | Error | Entire operation exceeded total timeout |

Example log output:
```
[WRN] Retry attempt 1/3 for EventBus publish after 1000ms. Exception: Request timeout
[WRN] Retry attempt 2/3 for EventBus publish after 2000ms. Exception: Request timeout
[ERR] Circuit breaker OPENED for EventBus. Break duration: 30s. Failure ratio exceeded threshold.
[INF] Circuit breaker HALF-OPEN for EventBus. Testing with next request
[INF] Circuit breaker CLOSED for EventBus. Resuming normal operations
```

## Usage

The `ResilientEventBridgeEventBus` is automatically registered in production environments. No code changes are required - it decorates the existing `EventBridgeEventBus`.

```csharp
// This automatically uses the resilient wrapper in production
await eventBus.PublishAsync(new OrderCreatedIntegrationEvent(...), ct);
```

## Behavior During Failures

| Scenario | Behavior |
|----------|----------|
| **Transient failure** | Retries with exponential backoff + jitter |
| **Slow response** | Attempt times out, triggers retry |
| **Repeated failures** | Circuit opens, fails fast for `BreakDurationSeconds` |
| **Circuit open** | Throws `BrokenCircuitException` immediately |
| **Total timeout exceeded** | Throws `TimeoutRejectedException` |

## Integration with Outbox Pattern

The resilience layer works alongside the outbox pattern:

1. **Outbox** ensures events are persisted before publishing
2. **Resilience** handles transient failures during publish
3. If resilience exhausts retries, the outbox will retry later
4. Circuit breaker prevents overwhelming a failing EventBridge

```
Outbox Job                    Resilient EventBus
──────────                    ──────────────────
Read pending messages
        │
        ▼
PublishAsync() ──────────────→ Timeout → Retry → Circuit → AWS
        │                              ↑    │
        │                              └────┘ (on failure)
        │
        ▼
Mark as processed (or retry later)
```

## Related Documentation

- [Event Bus](../EventBus/README.md) - Event publishing and handling
- [Outbox Pattern](../Outbox/README.md) - Reliable event persistence
- [Polly Documentation](https://www.pollydocs.org/) - Resilience library docs
