# Branching Strategy

## Overview

Git Flow with `development` as the integration branch. Feature branches merge to `development`, which is CI/CD deployed to lower environments. Production releases merge from `development` to `main`.

## Branch Flow

```
feature/PROJ-123 ──→ development ──→ main
                          │            │
                          ▼            ▼
                    dev/itg/qua    production
```

## Environment Mapping

| Branch | Environments | Purpose |
|--------|--------------|---------|
| `development` | dev, itg, qua | Integration testing, QA validation |
| `main` | production | Live releases |

## Branch Naming

| Type | Format | Example |
|------|--------|---------|
| Feature | `feature/{JIRA-TICKET}` | `feature/PROJ-123` |
| Bug fix | `bugfix/{JIRA-TICKET}` | `bugfix/PROJ-456` |
| Hotfix | `hotfix/{JIRA-TICKET}` | `hotfix/PROJ-789` |
| Release | `release/v{version}` | `release/v1.2.0` |

Optional description after ticket: `feature/PROJ-123-order-validation`

## Commit Messages

```
feat: add customer reassignment endpoint
fix: resolve null reference in order validation
refactor: consolidate DbConnectionFactory pattern
docs: update branching strategy
chore: update package dependencies
test: add integration tests for product creation
```

## Quick Reference

```bash
# Start feature work
git checkout development && git pull
git checkout -b feature/PROJ-123

# Push and create PR to development
git push -u origin feature/PROJ-123

# After merge to development, cleanup
git checkout development && git pull
git branch -d feature/PROJ-123

# Production release (from development to main)
git checkout main && git pull
git merge development
git push
```

## Branch Protection

### development
- Require PR with approval
- Require passing CI (build, tests)
- No direct commits

### main
- Require PR with approval
- Require passing CI (build, tests)
- No direct commits
- Merges from `development` only

## Anti-Patterns

| Don't | Do Instead |
|-------|------------|
| Branch without JIRA ticket | `feature/PROJ-123` |
| Long-lived branches (weeks) | Break into smaller PRs |
| Direct commits to development/main | Always use PRs |
| Giant PRs (1000+ lines) | Smaller, focused PRs |
| Merge to main without QA sign-off | Validate in qua environment first |
