# Branching Strategy

## Overview

Trunk-based development with short-lived feature branches. All branches merge to `main` via PR.

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
# Start work
git checkout main && git pull
git checkout -b feature/PROJ-123

# Push and create PR
git push -u origin feature/PROJ-123

# After merge, cleanup
git checkout main && git pull
git branch -d feature/PROJ-123
```

## Branch Protection (main)

- Require PR with approval
- Require passing CI (build, tests)
- No direct commits

## Anti-Patterns

| Don't | Do Instead |
|-------|------------|
| Branch without JIRA ticket | `feature/PROJ-123` |
| Long-lived branches (weeks) | Break into smaller PRs |
| Direct commits to main | Always use PRs |
| Giant PRs (1000+ lines) | Smaller, focused PRs |
