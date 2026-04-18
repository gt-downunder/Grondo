# Security Policy

## Supported Versions

Only the latest released minor version receives security updates. Older versions
will not receive patches. Upgrade to the latest release before filing a security
report.

| Version | Supported          |
| ------- | ------------------ |
| Latest  | :white_check_mark: |
| Older   | :x:                |

## Reporting a Vulnerability

If you believe you have found a security vulnerability in Grondo, please report
it privately using GitHub's
[private vulnerability reporting](https://github.com/gt-downunder/Grondo/security/advisories/new)
feature.

Please do **not** open a public issue, Pull Request, or Discussion thread for a
suspected vulnerability.

When reporting, include as much of the following as is relevant:

- A clear description of the issue and the affected API(s)
- A minimal reproduction (code sample, input, or unit test)
- The library version and target framework (e.g. `net10.0`)
- The potential impact and attack scenario
- Any suggested mitigation or fix, if known

## Response Expectations

- **Acknowledgement** — we aim to acknowledge private reports within a few
  business days.
- **Investigation** — we will work with you to validate and scope the issue.
- **Fix and disclosure** — once a fix is available, we will coordinate a
  release and, with your consent, credit you in the release notes.

## Scope

In-scope:

- Any code under `src/` shipped in the `Grondo` NuGet package.

Out-of-scope:

- Test, benchmark, and documentation code under `tests/`, `benchmarks/`,
  and `docs/`.
- Vulnerabilities in transitive NuGet packages or in the .NET runtime itself —
  please report those upstream.
- Build- or CI-system issues that are not exploitable by consumers of the
  published package.
