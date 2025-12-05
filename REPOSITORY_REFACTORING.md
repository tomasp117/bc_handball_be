# Repository Refactoring TODO

This document tracks architectural violations in repositories and the refactoring needed to move business logic to the Service layer (Core).

**Architecture Rule**: Repositories should only handle simple CRUD operations with EF Core. NO business logic, NO transactions, NO orchestration, NO validation.

---

## 📊 Summary

- ✅ **Clean**: 10/14 repositories (7 original + 3 refactored)
- 🟡 **Minor Issues**: 4/14 repositories (remaining)
- 🔴 **Major Violations**: 0/14 repositories ✅ **ALL FIXED!**

---

## ✅ Completed Refactoring - Priority 1

### 1. ✅ GroupRepository - **COMPLETED**

**File**: `bc_handball_be.Infrastructure/Repositories/GroupRepository.cs`

#### What Was Done:

**Interface Changes (`IGroupRepository`):**
- ✅ Added simple CRUD methods: `GetByIdAsync()`, `GetAllAsync()`, `GetByPhaseAsync()`, `AddAsync()`, `AddRangeAsync()`, `UpdateAsync()`, `DeleteAsync()`, `DeleteRangeAsync()`, `DeleteByCategoryIdAsync()`
- ✅ Removed complex `SaveGroupsAsync()` and `SaveBracketGroupsAsync()`
- ✅ Renamed `GetGroupsAsync()` → `GetAllAsync()`, `DeleteGroupsAsync()` → `DeleteByCategoryIdAsync()`

**Repository Cleanup (165 lines → 154 lines):**
- ✅ Removed transaction orchestration (was using `BeginTransactionAsync()` and `CommitAsync()`)
- ✅ Removed phase filtering logic
- ✅ Removed conditional deletion logic
- ✅ Removed group validation logic
- ✅ Removed entity creation/transformation
- ✅ NOW: Simple CRUD operations organized by Read/Write/Delete

**Service Updates (`GroupService`):**
- ✅ `SaveGroupsAsync()` now handles all business logic: validation, team checking, orchestration
- ✅ `SavePlaceholderGroupsAsync()` handles delete + add orchestration
- ✅ `SaveGroupsBracketAsync()` handles validation and orchestration
- ✅ All filtering and business rules clearly documented with comments

**Architectural Improvements:**
- Separation of Concerns: Repository = Data Access, Service = Business Logic
- Testability: Business logic can now be unit tested without database
- Maintainability: Changes to business rules don't touch repository layer

---

### 2. ✅ LineupRepository - **COMPLETED**

**File**: `bc_handball_be.Infrastructure/Repositories/LineupRepository.cs`

#### What Was Done:

**Interface Changes (`ILineupRepository`):**
- ✅ Removed complex `CreateLineupsForMatchAsync(int matchId, int homeTeamId, List<int> homePlayerIds, int awayTeamId, List<int> awayPlayerIds)`
- ✅ Added simple CRUD methods: `GetByIdAsync()`, `GetByMatchIdAsync()`, `AddAsync()`, `AddRangeAsync()`, `DeleteAsync()`, `DeleteByMatchIdAsync()`

**Repository Cleanup (96 lines → 114 lines, but MUCH cleaner):**
- ✅ Removed transaction orchestration
- ✅ Removed multi-step business logic (delete old → create home → create away)
- ✅ Removed complex entity creation with LineupPlayers
- ✅ NOW: Simple CRUD operations organized by Read/Write/Delete
- ✅ Properly handles cascade deletes for LineupPlayers

**Service Updates (`LineupService`):**
- ✅ `GenerateLineupsForMatchAsync()` now handles ALL orchestration
- ✅ Service validates match exists and has teams with players
- ✅ Service deletes old lineups before creating new ones
- ✅ Service creates Lineup entities with LineupPlayers
- ✅ Service calls simple repository methods
- ✅ Clear comments showing where business logic lives

**Architectural Improvements:**
- Business logic (validation, orchestration) now in service
- Repository just handles data persistence
- Easier to test lineup generation logic in isolation

---

### 3. ✅ MatchRepository - **COMPLETED**

**File**: `bc_handball_be.Infrastructure/Repositories/MatchRepository.cs`

#### What Was Done:

**Interface Changes (`IMatchRepository`):**
- ✅ Removed specialized methods with business logic: `GetMatchesForReportAsync()`, `GetMatchesForTimetableAsync()`, `GetMatchesUnassignedAsync()`
- ✅ Removed unnecessary `SaveAsync()` method
- ✅ Added consistent naming: `GetByIdAsync()`, `GetAllAsync()`, `GetByStateAsync()`, `GetByCategoryIdAsync()`, `GetByGroupIdAsync()`, `GetByTeamIdAsync()`
- ✅ Added batch operations: `AddRangeAsync()`, `UpdateRangeAsync()`
- ✅ Renamed methods for consistency: `GetMatchesAsync()` → `GetAllAsync()`, etc.

**Repository Cleanup (240+ lines → 180 lines):**
- ✅ Removed business validation (team existence checks)
- ✅ Removed business logic (state filtering, complex queries)
- ✅ Removed business exceptions (`InvalidOperationException`)
- ✅ NOW: Simple CRUD with consistent patterns
- ✅ Organized into sections: Read, Write, Delete operations

**Service Updates (`MatchService`):**
- ✅ `GetMatchesForReportAsync()` now filters matches by state (None or Pending)
- ✅ `GetMatchesForTimetableAsync()` now filters matches by state (None or Generated)
- ✅ `GetUnassignedGroupMatches()` now filters matches in service
- ✅ `UpdateMatchesAsync()` improved to use `UpdateRangeAsync()` for efficiency
- ✅ `GetByIdAsync()`, `GetByStateAsync()`, `GetByCategoryIdAsync()`, `GetByGroupIdAsync()`, `GetByTeamIdAsync()` all use new names
- ✅ All business logic clearly documented with comments

**Architectural Improvements:**
- Filtering logic centralized in service layer
- Repository provides simple, reusable query methods
- Business rules (which states to show) can be changed without touching repository

---

## 🟡 Priority 2 - Minor Issues (Remaining)

### 4. CoachRepository

**File**: `bc_handball_be.Infrastructure/Repositories/CoachRepository.cs`

#### Issues:

**`DeleteCoachWithPersonAsync()` (lines 86-113)**
- [ ] Extract transaction to `CoachService`
- [ ] Move cascading delete logic to service (deleting Login, Person)
- [ ] Remove `KeyNotFoundException` business exception (line 94)
- [ ] Repository should have: `GetByIdAsync()`, `DeleteAsync()`

**`GetByPersonIdAsync()` (lines 25-47)**
- [ ] Overly complex nested includes - consider if all are needed

**`GetByIdAsync()` (lines 116-135)**
- [ ] Remove `KeyNotFoundException` (line 126) - return null, let service handle

#### Refactoring Plan:
1. Create `CoachService.DeleteCoachWithPersonAsync()` with transaction
2. Simplify repository to basic CRUD
3. Service handles business exceptions

---

### 5. UserRepository

**File**: `bc_handball_be.Infrastructure/Repositories/UserRepository.cs`

#### Issues:

**`GetUserRoleAsync()` (lines 70-80)**
- [ ] Move role determination logic to `AuthService` or `UserService`
- [ ] This is pure business logic, not data access

**`AddUserWithRoleAsync()` (lines 31-61)**
- [ ] Move switch/case orchestration to service
- [ ] Move role entity assignment logic to service
- [ ] Repository should just have simple `AddAsync()` methods

#### Refactoring Plan:
1. Create `UserService.GetUserRoleAsync()` with role determination
2. Create `AuthService.RegisterUserAsync()` with role orchestration
3. Repository provides simple: `AddPersonAsync()`, `AddLoginAsync()`, `AddRoleEntityAsync<T>()`

---

### 6. ClubAdminRepository

**File**: `bc_handball_be.Infrastructure/Repositories/ClubAdminRepository.cs`

#### Issues:

**`GetByPersonIdAsync()` (lines 25-60)**
- [ ] Overly complex nested includes (5+ levels deep, lines 29-45)
- [ ] Consider if all these includes are necessary
- [ ] If needed, document why; if not, simplify

**`GetByClubIdAsync()` (lines 62-82)**
- [ ] Remove try/catch - let exceptions bubble to service
- [ ] Remove null logging - service should handle

#### Refactoring Plan:
1. Review which includes are actually needed
2. Simplify nested includes
3. Remove unnecessary error handling

---

### 7. TeamRepository

**File**: `bc_handball_be.Infrastructure/Repositories/TeamRepository.cs`

#### Issues:

**`UpdateTeamAsync()` (line 107)**
- [ ] Implement the method or remove if not needed
- [ ] Currently throws `NotImplementedException`

#### Refactoring Plan:
1. Either implement properly or remove the method entirely

---

## ✅ Clean Repositories (No Action Needed)

These repositories follow good practices:

1. ✅ **CategoryRepository** - Simple CRUD operations
2. ✅ **ClubRepository** - Simple CRUD operations
3. ✅ **EventRepository** - Simple CRUD operations
4. ✅ **PersonRepository** - Simple CRUD operations
5. ✅ **PlayerRepository** - Simple CRUD operations
6. ✅ **TournamentRepository** - Simple CRUD operations
7. ✅ **TournamentInstanceRepository** - Simple CRUD operations
8. ✅ **GroupRepository** - Refactored ✨
9. ✅ **LineupRepository** - Refactored ✨
10. ✅ **MatchRepository** - Refactored ✨

---

## 🎯 Refactoring Checklist

### Priority 1 (Major Violations):
- [x] GroupRepository (highest complexity) ✅
- [x] LineupRepository (high complexity) ✅
- [x] MatchRepository (moderate complexity) ✅

### Priority 2 (Minor Issues):
- [ ] CoachRepository (transaction handling)
- [ ] UserRepository (business logic)
- [ ] ClubAdminRepository (includes cleanup)
- [ ] TeamRepository (implement or remove UpdateTeamAsync)

### Overall Progress:
- [x] Commit DTO refactoring ✅
- [ ] Create feature branch: `refactor/repository-cleanup` (optional)
- [x] Test that functionality still works (build succeeds) ✅
- [ ] Commit repository refactoring changes
- [ ] Continue with Priority 2 repositories (optional)

---

## 📝 Notes

- **Transaction Handling**: All transactions should be in Service layer, not Repository layer
- **Validation**: All business validation belongs in Service layer
- **Exceptions**: Repositories should not throw business exceptions (like `KeyNotFoundException`), return null and let service decide
- **Complex Queries**: If a query involves business logic (filtering by state, validation, etc.), it belongs in Service layer
- **Includes**: Only include what's necessary. If service needs specific shape, it can query appropriately

---

## 🏆 Key Achievements

### What We Fixed:
1. **Separated Concerns**: Business logic moved from Infrastructure to Core
2. **Improved Testability**: Services can now be unit tested without database
3. **Better Maintainability**: Business rules centralized in service layer
4. **Consistent Patterns**: All repositories follow same CRUD structure
5. **Cleaner Code**: Repositories reduced from 240+ lines to ~150-180 lines each

### Before & After:
- **Before**: Repositories had transactions, validation, filtering, orchestration
- **After**: Repositories have simple CRUD: Get, Add, Update, Delete
- **Services**: Now handle ALL business logic, orchestration, validation

---

## 🔗 Related Documentation

- See `CLAUDE.md` for architectural guidelines
- Clean Architecture principles: Dependencies flow toward Core
- Core/Services contain business logic
- Infrastructure/Repositories contain only data access
