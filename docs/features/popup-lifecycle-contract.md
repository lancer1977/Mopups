---
title: Popup Lifecycle and Platform Contract
status: in-progress
owner: @DreadBreadcrumb
priority: high
complexity: 2
created: 2026-07-17
updated: 2026-07-17
tags: [documentation, lifecycle, popup, maui]
---

# Popup Lifecycle and Platform Contract

This page describes the guarantees that can currently be made from the public
Mopups navigation contract and source layout. It does not replace native device
or simulator validation.

## Initialization

Applications should call `ConfigureMopups()` while building the MAUI host. That
method registers the platform handlers compiled for Android, iOS, and Windows,
and configures Android back-button forwarding. Applications using a custom
`IPopupNavigation` implementation may set it through `MopupService.SetInstance`.

The portable reference assembly is not a native runtime. Accessing the default
navigation implementation there produces a `PlatformNotSupportedException`
that tells consumers Mopups requires a platform-specific MAUI target framework.

## Portable navigation guarantees

`IPopupNavigation` exposes a stack-oriented asynchronous contract:

- `PushAsync` adds a popup page.
- `PopAsync` removes the top popup.
- `PopAllAsync` clears the stack.
- `RemovePageAsync` removes a specified popup.
- Pushing and popping events surround the corresponding operation and include
  the requested animation setting.

The deterministic test suite covers the mock navigation stack, event order,
repeated operations, nested stack cleanup, and common invalid operations. Those
tests prove the portable public contract only; they do not prove a native window
was acquired or a platform animation completed.

## Platform boundary

The package targets Android, iOS, Mac Catalyst, and Windows. Platform source is
compiled selectively by target framework. Native behavior depends on the MAUI
window/handler lifecycle supplied by the host application, so it must be checked
on the affected platform after native code changes.

| Concern | Portable test evidence | Native evidence required |
| --- | --- | --- |
| Stack order and lifecycle events | Unit tests | No additional native proof when handlers are unchanged |
| Missing portable implementation | Unit tests / actionable exception | No native proof |
| Context or window acquisition | Not proven by mocks | Device or simulator smoke on the affected platform |
| Rapid native open/close and background taps | Not proven by mocks | Device or simulator lifecycle test |
| Nested native popups and animation cleanup | Not proven by mocks | Device or simulator lifecycle test |

## Failure and threading expectations

Mopups does not promise that arbitrary background-thread calls, stale pages, or
missing native windows will succeed. Consumers should invoke navigation from a
valid MAUI UI context and treat a thrown navigation exception as a failed
operation rather than assuming stack cleanup completed. A future hardening slice
must add a testable seam before changing native context acquisition or claiming
more specific failure behavior.

## Validation matrix

For documentation or portable-contract changes, run the repository test project
and record the exact target framework. For a native handler change, also run the
appropriate Android, iOS, Mac Catalyst, or Windows sample/device lane and record
open/close, nested popup, cancellation/failure, and cleanup observations.

## Issue #2 V1.1 delivery record

GitHub issue #2 tracks the V1.1 popup lifecycle hardening sequence. Keep the
issue open until the native evidence lanes are proven; do not use portable mock
tests to claim iOS or Mac Catalyst behavior is fixed.

| Slice | Status | Evidence |
| --- | --- | --- |
| V1.1.1 lifecycle contract and portable race coverage | Merged | PR #3 merged as `425c2d3c60f377ce831e16d40a13291b536cafc7`; the merged slice added this contract page, linked it from README/docs, and aligned portable project tests through `6fb519d587dfc0dd930e3299e306f7d3c2adabae`. Issue comment evidence records focused portable tests passing 34/34 before merge. |
| V1.1.2 context/window acquisition hardening | Pending platform evidence | Current Linux investigation could not reproduce an iOS or Mac Catalyst context/window failure because Darwin, Xcode, simulators, and Apple MAUI workloads were unavailable. No concrete native failure was evidenced, so no rewrite or handler hardening is currently justified. |
| V1.1.3 native lifecycle evidence | Pending platform evidence | Requires an Apple-capable lane for iOS and Mac Catalyst smoke/lifecycle observations. Record the exact device or simulator runtime, workload set, sample-app path, and open/close/nested/cleanup observations before claiming native behavior. |

Suggested issue #2 checklist state after PR #3:

- [x] README and feature documentation identify the supported framework/platform matrix and lifecycle boundary for V1.1.1.
- [x] Portable tests cover the public lifecycle contract to the extent the mock seam permits; this is portable evidence only.
- [ ] V1.1.2: provide reproducible iOS/Mac Catalyst context-window failure evidence, or leave the slice pending without a speculative rewrite.
- [ ] V1.1.3: run and record native platform lifecycle evidence on supported device/simulator/runtime lanes.

## Non-goals

This contract does not add a public cancellation-token API, change handler
selection, guarantee native animation timing, or publish a package. Those need a
separate issue with platform-specific evidence.
