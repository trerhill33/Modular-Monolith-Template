# Branching Strategy

## Overview

This repository follows a **trunk-based development** approach with short-lived feature branches. This strategy balances rapid iteration with stability.

```
main (production)
  │
  ├── feature/add-customer-endpoint     (short-lived, 1-3 days)
  ├── feature/update-order-validation   (short-lived, 1-3 days)
  ├── bugfix/fix-null-reference         (short-lived, hours to 1 day)
  └── release/v1.2.0                    (cut when preparing release)
```

## Branch Types

### `main`
- **Purpose**: Production-ready code
- **Protection**: Requires PR approval, passing CI
- **Deploys to**: Production
- **Rule**: Never commit directly; always merge via PR

### `feature/*`
- **Purpose**: New functionality
- **Naming**: `feature/{JIRA-TICKET}` or `feature/{JIRA-TICKET}-{description}`
- **Lifetime**: 1-3 days max
- **Base**: Branch from `main`
- **Merge**: PR back to `main`

```bash
# Example
git checkout main
git pull origin main
git checkout -b feature/PROJ-123
# ... work ...
git push origin feature/PROJ-123
# Create PR to main
```

### `bugfix/*`
- **Purpose**: Non-urgent bug fixes
- **Naming**: `bugfix/{JIRA-TICKET}`
- **Lifetime**: Hours to 1 day
- **Base**: Branch from `main`
- **Merge**: PR back to `main`

```bash
git checkout -b bugfix/PROJ-456
```

### `hotfix/*`
- **Purpose**: Urgent production fixes
- **Naming**: `hotfix/{JIRA-TICKET}`
- **Lifetime**: Hours
- **Base**: Branch from `main`
- **Merge**: PR to `main`, then cherry-pick to any active release branches

```bash
git checkout main
git checkout -b hotfix/PROJ-789
# ... fix ...
# PR to main (expedited review)
```

### `release/*`
- **Purpose**: Stabilization before release
- **Naming**: `release/v{major}.{minor}.{patch}`
- **Lifetime**: Days to 1 week
- **Base**: Branch from `main`
- **Merge**: Tag and merge back to `main` when released

```bash
git checkout main
git checkout -b release/v1.2.0
# Only bugfixes go here, no new features
# When ready:
git tag v1.2.0
git checkout main
git merge release/v1.2.0
```

## Workflow

### Daily Development

```
1. Pull latest main
2. Create feature branch
3. Make changes (small, focused commits)
4. Push branch
5. Create PR
6. Address review feedback
7. Squash merge to main
8. Delete feature branch
```

### Commit Messages

Follow conventional commits:

```
feat: add customer reassignment endpoint
fix: resolve null reference in order validation
refactor: consolidate DbConnectionFactory pattern
docs: update branching strategy
chore: update package dependencies
test: add integration tests for product creation
```

The JIRA ticket is tracked via the branch name. Commit messages focus on describing the change.

### Pull Request Guidelines

**Title**: Conventional commit format
```
feat: add customer reassignment endpoint
```

**Description**:
```markdown
## Summary
Brief description of changes

## Changes
- Added ReassignCustomerCommand
- Added ReassignCustomerEndpoint
- Updated customer entity with Reassign method

## Testing
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Manual testing completed
```

Note: JIRA linking happens automatically via branch name (e.g., `feature/PROJ-123`).

## CI/CD Integration

| Branch | Build | Tests | Deploy |
|--------|-------|-------|--------|
| `main` | ✅ | ✅ | Production |
| `feature/*` | ✅ | ✅ | Preview (optional) |
| `bugfix/*` | ✅ | ✅ | - |
| `hotfix/*` | ✅ | ✅ | - |
| `release/*` | ✅ | ✅ | Staging |

## Branch Protection Rules

### `main` Branch
- Require pull request before merging
- Require at least 1 approval
- Require status checks to pass (build, tests)
- Require branches to be up to date
- Do not allow bypassing the above settings

## Module-Specific Considerations

Branch names just need the JIRA ticket. Add description if helpful:

```bash
# Good: JIRA ticket (minimal)
git checkout -b feature/PROJ-123
git checkout -b bugfix/PROJ-456

# Good: JIRA ticket + optional context
git checkout -b feature/PROJ-123-orders-split-shipment

# Avoid: Missing JIRA ticket
git checkout -b feature/update
git checkout -b fix
```

## Quick Reference

```bash
# Start new feature
git checkout main && git pull
git checkout -b feature/PROJ-123

# Keep branch updated (if long-lived)
git fetch origin
git rebase origin/main

# Push and create PR
git push -u origin feature/PROJ-123

# After PR merged, cleanup
git checkout main && git pull
git branch -d feature/PROJ-123
```

## Anti-Patterns to Avoid

| Don't | Do Instead |
|-------|------------|
| Branch without JIRA ticket | Always include ticket: `feature/PROJ-123` |
| Long-lived feature branches (weeks) | Break into smaller PRs |
| Merge commits from main into feature | Rebase onto main |
| Direct commits to main | Always use PRs |
| Giant PRs (1000+ lines) | Smaller, focused PRs |
| Vague branch names | Use JIRA ticket in branch name |
