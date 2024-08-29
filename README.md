# Capitalism & Elder Gods

## Overview

Currently, this README only contains development relevant information.
Feel free to add more by creating `doc/update-readme` branch and update this.

## Table of Contents

- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
  - [Feature Development](#feature-development)
  - [Issues and Project Management](#issues-and-project-management)
- [Git Practices](#git-practices)
  - [Commit Messages](#commit-messages)
  - [Branch Naming](#branch-naming)
- [Code Conventions](#code-conventions)
  - [C# Coding Standards](#c-coding-standards)
  - [Unity Specific Conventions](#unity-specific-conventions)
  - [Best Practices](#best-practices)

## Development Workflow

### Feature Development

1. **Create a Branch**:
   `git checkout -b feature/feature-name`  
   Refer to [branch naming](#branch-naming) for details.
2. **Develop the Feature**: Implement and test your feature in Unity.
3. **Commit Regularly**: Commit your changes with clear, concise [messages](#commit-messages).
4. **Open a Pull Request**: Push your branch and open a PR to the `main` branch.
5. **Link to Issues**: Reference any relevant issues in your PR description to automatically close them when the PR is merged.
6. **Request Review**: Have at least one team member review your PR.
7. **Merge after Approval**: Once approved, merg the PR into `main`.

### Issues and Project Management

1. **Creating Issues**:

   - Create issues in _GitHub Issues_ to track bugs, feature requests, and tasks.
   - Assign labels such as `bug`, `feature`, `reasearch`, `design`, etc.

2. **Using GitHub Projects**:

   - Use _GitHub Projects_ to organize issues and PRs into sprints or kanban-style boards.
   - Move issues across columns (e.g., `To Do`, `In Progress`, `Review`, `Done`) to track progress.

3. **Resolving Issues**:

   - Reference issues in your commits or PRs using `Fixes #issue_number` or `Closes #issue_number` to automatically close them when the code is merged.
   - Ensure the issue is fully resolved before closing.

4. **Milestones**:

   - Create milestones for major project phases or releases.
   - Assign issues and PRs to milestones to track progress towards specific goals.

## Git Practices

### Commit Messages

- Follow the structure:
  - `feat: Add new feature`
  - `fix:  Resolve Bug`
  - `refactor: Code improvements`
  - `doc: Update documentation`
  - `style: Code style changes (formatting, etc.)`
  - `chore: Routine tasks (e.g., package updates or any other non-fitting items with the above)`

### Branch Naming

- Use the following conventions
  - `feature/feature-name` for new features
  - `bugfix/bug-description` for bug fixes
  - `hotfix/hotfix-description` for urgent fixes
  - `doc/update-readme` for documentation updates
  - `chore/task-name` for routine tasks such as updating packages or refactoring

## Code Conventions

### C# Coding Standards

- Private Members: `_camelCase`
  Example: `_playerHealth`, `_enemyList`
- Public Members: `PascalCase`
  Example: `PlayerHealth`, `EnemyList`
- Local Variables: `camelCase`
  Example: `currentHealth`, `enemyCount`
- Constants: `SCREAMING_SNAKE_CASE`
  Example: `MAX_HEALTH`, `DEFAULT_SPEED`
- Methods: `PascalCase`
  Example: `CalculateDamage()`, `SpawnEnemies()`

### Unity Specific Conventions

- MonoBehaviour Methods:
  - Standard Unity methods like `Start()`, `Update()`, `Awake()` should always use PascalCase.
- Script Organization:
  - Group related methods together, and consider using regions (`#region`, `#endregion`) to organize large scripts.
- Serialization:
  - Use `[SerializeField]` for private variables that need to be exposed in the Unity Inspector.
  - Use properties instead of public fields when appropriate to encapsulate access to data.
    ```csharp
        // Property with a private field
    private int _num;
    public int Num
    {
        get { return _num; }
        set
        {
            if (value >= 0) // Example of validation
            {
                _num = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Num must be non-negative.");
            }
        }
    }
    ```

### Best Practices

- Null Checks:
  - Always perform null checks before accessing objects.
  - Example: `if (enemy != null) { enemy.TakeDamage(); }`
- Error Handling:
  - Handle exceptions gracefully, logging errors where necessary.
- Commenting:
  - Comment your code where the logic is complex or non-obvious.
  - Use XML documentation comments (`///`) for public methods and properties.
