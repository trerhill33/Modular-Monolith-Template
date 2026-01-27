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
- **Naming**: `feature/{JIRA-TICKET}-{short-description}`
- **Lifetime**: 1-3 days max
- **Base**: Branch from `main`
- **Merge**: PR back to `main`

```bash
# Example
git checkout main
git pull origin main
git checkout -b feature/PROJ-123-add-customer-reassign
# ... work ...
git push origin feature/PROJ-123-add-customer-reassign
# Create PR to main
```

### `bugfix/*`
- **Purpose**: Non-urgent bug fixes
- **Naming**: `bugfix/{JIRA-TICKET}-{short-description}`
- **Lifetime**: Hours to 1 day
- **Base**: Branch from `main`
- **Merge**: PR back to `main`

```bash
git checkout -b bugfix/PROJ-456-fix-order-validation
```

### `hotfix/*`
- **Purpose**: Urgent production fixes
- **Naming**: `hotfix/{JIRA-TICKET}-{short-description}`
- **Lifetime**: Hours
- **Base**: Branch from `main`
- **Merge**: PR to `main`, then cherry-pick to any active release branches

```bash
git checkout main
git checkout -b hotfix/PROJ-789-fix-critical-auth-bug
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

Follow conventional commits with JIRA ticket reference:

```
feat(PROJ-123): add customer reassignment endpoint
fix(PROJ-456): resolve null reference in order validation
refactor(PROJ-789): consolidate DbConnectionFactory pattern
docs(PROJ-101): update branching strategy
chore(PROJ-102): update package dependencies
test(PROJ-103): add integration tests for product creation
```

The JIRA ticket in the commit message enables automatic linking in JIRA and makes it easy to trace changes back to requirements.

### Pull Request Guidelines

**Title**: Include JIRA ticket
```
feat(PROJ-123): add customer reassignment endpoint
```

**Description**:
```markdown
## JIRA
[PROJ-123](https://yourcompany.atlassian.net/browse/PROJ-123)

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

When working on a specific module, include both JIRA ticket and module context:

```bash
# Good: JIRA ticket + module context + description
git checkout -b feature/PROJ-123-orders-add-split-shipment
git checkout -b bugfix/PROJ-456-customer-fix-email-validation

# Avoid: Missing JIRA ticket or vague names
git checkout -b feature/update
git checkout -b fix
git checkout -b feature/add-stuff
```

## Quick Reference

```bash
# Start new feature (always include JIRA ticket)
git checkout main && git pull
git checkout -b feature/PROJ-123-my-feature

# Keep branch updated (if long-lived)
git fetch origin
git rebase origin/main

# Push and create PR
git push -u origin feature/PROJ-123-my-feature

# After PR merged, cleanup
git checkout main && git pull
git branch -d feature/PROJ-123-my-feature
```

## Anti-Patterns to Avoid

| Don't | Do Instead |
|-------|------------|
| Branch without JIRA ticket | Always include ticket: `feature/PROJ-123-...` |
| Long-lived feature branches (weeks) | Break into smaller PRs |
| Merge commits from main into feature | Rebase onto main |
| Direct commits to main | Always use PRs |
| Giant PRs (1000+ lines) | Smaller, focused PRs |
| Vague branch names | Descriptive names with JIRA + context |
| Commits without ticket reference | Include ticket: `feat(PROJ-123): ...` |
