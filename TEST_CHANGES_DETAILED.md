# Detailed Test Changes Audit

This document explains, in detail, all test-related changes made in this session, why each change was made, and what each line in each modified/added file is doing.

## Scope of this document

This covers the following files:

1. `CalculatorService.Tests/CalculatorService.Tests.csproj`
2. `CalculatorService.Tests/CalculatorOperationsTests.cs`
3. `CalculatorService.Tests/CalculatorControllerTests.cs`
4. `CalculatorService.Tests/JournalControllerTests.cs`
5. `CalculatorService.Tests/JournalServiceTests.cs`
6. `CalculatorService.Tests/ApiIntegrationTests.cs`
7. `CalculatorService.slnx`

No production code files (`Client`, `Core`, `Server`) were modified in these test-improvement steps.

---

## 1) `CalculatorService.Tests/CalculatorService.Tests.csproj`

### What changed
- Aligned test target framework to `net8.0`.
- Added `Microsoft.AspNetCore.TestHost` package to enable in-memory API integration tests.

### Line-by-line explanation
- **Line 1**: Declares SDK-style project using `Microsoft.NET.Sdk`.
- **Lines 3-9**: Project properties.
  - **Line 4**: `TargetFramework` = `net8.0` (alignment with solution/runtime).
  - **Line 5**: Language version latest.
  - **Line 6**: Implicit global using directives enabled.
  - **Line 7**: Nullable reference type analysis enabled.
  - **Line 8**: Project is not packable as NuGet.
- **Lines 11-19**: Package references for test tooling.
  - **Line 12**: Coverage collector.
  - **Line 13**: `Microsoft.AspNetCore.TestHost` for hosting the API in-memory in tests.
  - **Line 14**: .NET test SDK.
  - **Line 15**: Moq mocking framework.
  - **Lines 16-18**: NUnit + analyzers + adapter.
- **Lines 21-23**: Global using declaration for `NUnit.Framework`.
- **Lines 25-28**: References to `Core` and `Server` projects so tests can use domain and controller types.
- **Line 30**: Project end.

---

## 2) `CalculatorService.Tests/CalculatorOperationsTests.cs`

### What changed
Added edge-case tests to strengthen numeric behavior and validation coverage.

### Line-by-line explanation
- **Lines 1-3**: Imports NUnit, operation implementation, and custom exceptions.
- **Lines 5-6**: Namespace declaration.
- **Line 7**: Marks class as NUnit test fixture.
- **Lines 8-10**: Test class and private SUT (`CalculatorOperations`) field.
- **Lines 12-16**: `[SetUp]` method creates fresh SUT before each test.

#### Existing and added tests
- **Lines 18-23** `Add_WithValidNumbers_ReturnsCorrectSum`
  - Calls `Add([1,2,3])`, asserts sum is `6`.
- **Lines 25-29** `Add_WithLessThanTwoNumbers_ThrowsInvalidArgumentsException`
  - Verifies guard clause for insufficient operands.
- **Lines 31-36** `Add_WithFloatingPointNumbers_ReturnsExpectedSumWithinTolerance`
  - Verifies floating point result with tolerance (`Within(1e-12)`), avoiding brittle exact comparison.
- **Lines 38-42** `Multiply_WithLessThanTwoNumbers_ThrowsInvalidArgumentsException`
  - Validates multiplication input guard.
- **Lines 44-50** `Multiply_WithLargeValues_ReturnsExpectedProduct`
  - `1e100 * 1e100` check.
  - Asserts result is finite (**line 48**) and approximately `1e200` with tolerance (**line 49**).
- **Lines 52-58** `Divide_WithValidNumbers_ReturnsCorrectQuotientAndRemainder`
  - Verifies normal divide semantics used by service.
- **Lines 60-64** `Divide_ByZero_ThrowsDivisionByZeroException`
  - Verifies domain-specific divide-by-zero exception.
- **Lines 66-72** `Divide_WithNegativeDividend_ReturnsFloorQuotientAndRemainder`
  - Confirms current implementation behavior for negative dividend.
- **Lines 74-79** `Sqrt_WithPositiveNumber_ReturnsSquareRoot`
  - Positive root scenario.
- **Lines 81-85** `Sqrt_WithNegativeNumber_ThrowsInvalidArgumentsException`
  - Input validation for negative root.
- **Lines 87-91** `Sqrt_WithZero_ReturnsZero`
  - Explicit edge case for `0`.
- **Lines 93-94**: Class/namespace close.

---

## 3) `CalculatorService.Tests/CalculatorControllerTests.cs`

### What changed
Expanded from mostly `Add` coverage to include `Sub`, `Mult`, `Div`, `Sqrt`, null request guards, and stricter journal assertions.

### Line-by-line explanation
- **Lines 1-9**: Imports for NUnit, Moq, MVC types, domain contracts/models/exceptions, and ASP.NET HTTP context.
- **Lines 11-12**: Namespace.
- **Line 13**: NUnit fixture marker.
- **Lines 14-20**: Test class with mocks and controller under test fields.
- **Lines 21-37**: Setup
  - Creates mocks for calculator, journal, logger.
  - Instantiates `CalculatorController` with mocks.
  - Assigns `ControllerContext` + `DefaultHttpContext` so headers can be set.

#### Test methods
- **Lines 39-55** `Add_WithValidRequest_ReturnsSumAndDoesNotLogToJournalIfNoTrackingId`
  - Arranges addends and calculator return.
  - Calls controller action.
  - Asserts `200` payload (`AddResponse.Sum = 10`) and verifies no journal write without tracking id.
- **Lines 57-71** `Add_WithTrackingId_SavesToJournal`
  - Sets `X-Evi-Tracking-Id` header.
  - Verifies journal save called exactly once with precise `JournalEntry` content:
    - `Operation == "Sum"`
    - `Calculation == "5 + 5 = 10"`
- **Lines 73-81** `Add_WithNullAddends_ThrowsInvalidArgumentsException`
  - Ensures invalid add request is rejected.
- **Lines 83-94** `Add_WithWhitespaceTrackingId_DoesNotSaveToJournal`
  - Ensures whitespace tracking id is treated as absent; no journal save.
- **Lines 96-107** `Sub_WithTrackingId_SavesToJournal`
  - Verifies subtract action journals with expected operation and calculation text.
- **Lines 109-114** `Mult_WithLessThanTwoFactors_ThrowsInvalidArgumentsException`
  - Verifies minimum factor validation through controller.
- **Lines 116-120** `Mult_WithNullRequest_ThrowsInvalidArgumentsException`
  - Verifies null request guard in `Mult` action.
- **Lines 122-135** `Div_WithValidRequest_ReturnsQuotientAndRemainder`
  - Mocks divide result and verifies response body fields.
- **Lines 137-141** `Div_WithNullRequest_ThrowsInvalidArgumentsException`
  - Verifies null request guard in `Div` action.
- **Lines 143-155** `Sqrt_WithValidRequest_ReturnsSquareRoot`
  - Verifies sqrt response payload path.
- **Lines 157-161** `Sqrt_WithNullRequest_ThrowsInvalidArgumentsException`
  - Verifies null request guard.
- **Lines 163-167** `Sub_WithNullRequest_ThrowsInvalidArgumentsException`
  - Verifies null request guard.
- **Lines 168-169**: Close class/namespace.

---

## 4) `CalculatorService.Tests/JournalControllerTests.cs`

### What changed
- Added/kept controller tests for journal query.
- Replaced brittle reflection checks with typed deserialization helper for bad-request payload verification.

### Line-by-line explanation
- **Lines 1-8**: Imports for contracts/models/controller, MVC result types, logging mock, NUnit, and JSON.
- **Lines 10-11**: Namespace.
- **Line 12**: Test fixture attribute.
- **Lines 13-17**: Mock/logger/controller fields with nullable suppression initialization pattern.
- **Lines 19-25**: Setup creates mocks and controller instance.

#### Tests
- **Lines 27-38** `Query_WithMissingId_ReturnsBadRequestWithStandardShape`
  - Calls query with empty id.
  - Asserts result is `BadRequestObjectResult`.
  - Converts anonymous payload into typed `ErrorResponse` via helper.
  - Verifies:
    - `ErrorCode == "InvalidArguments"`
    - `ErrorStatus == 400`
    - `ErrorMessage` not empty.
- **Lines 40-57** `Query_WithValidId_ReturnsOperations`
  - Mocks journal service returning two entries.
  - Calls query with valid id.
  - Asserts `OkObjectResult`, typed `JournalQueryResponse`, and operation count = 2.

#### Helper + local DTO
- **Lines 59-68** `ToErrorResponse`
  - Defensive null check.
  - Serializes unknown object and deserializes to known DTO type for robust assertion.
- **Lines 70-75** `ErrorResponse`
  - Simple internal DTO used only for assertion shape.
- **Lines 76-77**: Close class/namespace.

---

## 5) `CalculatorService.Tests/JournalServiceTests.cs`

### What changed
Added meaningful journal service tests for whitespace ids, concurrency integrity, persistence/reload, and corrupt file behavior.

### Line-by-line explanation
- **Lines 1-3**: Imports for journal models/service and NUnit.
- **Lines 5-6**: Namespace.
- **Line 7**: Fixture attribute.
- **Lines 8-9**: Class declaration.

#### Tests
- **Lines 10-26** `Save_WithWhitespaceTrackingId_DoesNotPersistEntry`
  - Creates temp file path.
  - Instantiates service.
  - Saves with whitespace tracking id.
  - Asserts no entries returned for whitespace id.
  - Cleanup in `finally` ensures temp file is always deleted.
- **Lines 28-54** `Save_WhenCalledConcurrently_PreservesAllEntries`
  - Parallel writes (`Parallel.For`) to same tracking id.
  - Asserts count equals total writes.
  - Strengthens integrity by asserting unique calculations and expected boundary entries exist.
- **Lines 56-76** `JournalService_PersistsAndLoadsEntriesAcrossInstances`
  - Writes using one service instance.
  - Instantiates second service on same file.
  - Verifies entries are reloaded correctly (count + operation names).
- **Lines 78-97** `JournalService_WithCorruptFile_StartsEmptyWithoutThrowing`
  - Writes invalid JSON.
  - Asserts constructor/read path does not throw and resulting journal query is empty.
- **Lines 98-99**: Close class/namespace.

---

## 6) `CalculatorService.Tests/ApiIntegrationTests.cs`

### What changed
Added full in-memory integration tests for:
- routing/model binding/serialization,
- middleware standardized `400/500` contracts,
- journal bad-request contract,
- end-to-end tracking header -> add -> journal query flow.

### Line-by-line explanation
- **Lines 1-13**: Imports for HTTP APIs, collections, app contracts/models, server components, TestHost hosting, DI, NUnit.
- **Lines 15-16**: Namespace.
- **Line 17**: Fixture attribute.
- **Lines 18-19**: Class declaration.

#### Integration tests
- **Lines 20-32** `Add_Endpoint_UsesRoutingBindingAndSerialization`
  - Spins up test server/client.
  - Calls `/calculator/add` via JSON.
  - Asserts `200 OK` and correct deserialized sum.
- **Lines 34-48** `ExceptionMiddleware_ReturnsStandard400_ForBusinessExceptions`
  - Sends invalid add request (`1` addend).
  - Asserts status `400` and complete error payload contract.
- **Lines 50-64** `ExceptionMiddleware_ReturnsStandard500_ForUnhandledExceptions`
  - Injects special calculator implementation that throws generic exception.
  - Asserts middleware returns status `500` + standardized error payload.
- **Lines 66-80** `Journal_Query_WithMissingId_ReturnsBadRequest`
  - Calls `/journal/query` with empty id.
  - Asserts status `400` and complete error contract fields.
- **Lines 82-106** `Add_WithTrackingId_ThenQueryJournal_ReturnsTrackedOperation`
  - Sends add request with `X-Evi-Tracking-Id` header.
  - Queries journal by same id.
  - Asserts both requests succeed and journal contains expected operation + calculation string.

#### Test host composition
- **Lines 108-126** `CreateServer`
  - Creates `WebHostBuilder` with minimal pipeline required for integration tests.
  - Registers logging.
  - Registers either provided calculator or default `CalculatorOperations`.
  - Registers in-memory journal service implementation for deterministic tests.
  - Adds controllers from server assembly.
  - Configures middleware + routing + endpoint mapping.
  - Returns `TestServer`.

#### Internal test doubles
- **Lines 128-158** `InMemoryJournalService`
  - Thread-safe in-memory implementation of `IJournalService` for integration tests.
  - Uses `ConcurrentDictionary<string, List<JournalEntry>>`.
  - `Save` ignores blank ids and synchronizes list writes with lock.
  - `GetOperations` returns a copy (`ToList`) to avoid exposing mutable internal list.
- **Lines 160-167** `ThrowingCalculatorOperations`
  - Minimal `ICalculatorOperations` implementation that throws in `Add` to force middleware 500 path.
- **Lines 169-174** `ErrorResponse`
  - DTO used for deserializing and asserting standardized error payload from API.
- **Lines 175-176**: Close class/namespace.

---

## 7) `CalculatorService.slnx`

### What changed
Added test project to solution so it appears in Solution Explorer and participates in normal solution workflows.

### Line-by-line explanation
- **Line 1**: Solution root element.
- **Lines 2-4**: Client/Core/Server projects.
- **Line 5**: Added `CalculatorService.Tests` project reference.
- **Line 6**: Solution end.

---

## Functional intent of the full test update

The full update was intended to:

1. Align test project framework with solution baseline (`.NET 8`).
2. Expand unit tests from narrow happy-path checks to meaningful edge-case checks.
3. Verify controller orchestration and journaling side effects (including exact payload expectations).
4. Verify journal service concurrency and file persistence behavior.
5. Add in-memory integration tests to validate actual HTTP pipeline behavior and standardized error payloads.
6. Ensure tests are visible/executable at solution level by including test project in `.slnx`.

---

## Verification status

At the time of writing this document:
- `dotnet build` succeeded.
- `dotnet test CalculatorService.slnx` succeeded.
- Total tests passed: `34/34`.

---

## Transparency note

This document explains all lines in all files that were changed or newly created for the testing work performed in this session. If you want an additional **diff-style appendix** (before/after snippets for each file), I can generate a second markdown file with that format as well.
