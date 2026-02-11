# eClaims - Test Cases (MVC + API)

## Purpose
This document lists manual and automated test cases covering primary use cases for the MVC application (`eClaims.Web`) and the REST API (`eClaims.API`). Each test contains Preconditions, Steps, Expected Result and example requests/responses where applicable.

---

## Test Environment
- API URL (local): `https://localhost:5001` (or run `dotnet run --project eClaims.API`)
- Web URL (local): run `eClaims.Web` in Visual Studio or `dotnet run --project eClaims.Web`
- DB: In-memory repository is used by default in `eClaims.API/Program.cs` (swap to MongoDB by registering `Repository<>` and configuring `MongoDbSettings`).

Run commands (example):
```powershell
dotnet run --project eClaims.API
dotnet run --project eClaims.Web
```

---

## Common Test Data (sample Claim JSON)

Request body for API `POST /api/claims` and sample JSON used in examples:

```json
{
  "userId": "607f1f77bcf86cd799439011",
  "policyNumber": "POL-123456",
  "status": "DRAFT",
  "incident": {
    "date": "2025-11-01T00:00:00Z",
    "description": "Rear-end collision",
    "location": "123 Main St"
  },
  "workOrders": [],
  "documents": []
}
```

Fields to validate: `UserId`, `PolicyNumber`, `Status`, `Incident`, `Documents`.

---

## MVC (eClaims.Web) Test Cases

1) Claim List (Index)
- Preconditions: User is authenticated.
- Steps:
  1. Navigate to `/Claims`.
  2. Observe list of claims.
- Expected:
  - HTTP 200, claims are rendered in a table.
  - If no claims, page shows an empty state message.

2) Claim Details
- Preconditions: Claim exists in repository with Id `:id`.
- Steps:
  1. Navigate to `/Claims/Details/:id`.
- Expected:
  - HTTP 200, claim detail view displays `PolicyNumber`, incident info, documents, audit log.
  - If not found, 404 page shown.

3) Create Claim (valid)
- Preconditions: User is authenticated.
- Steps:
  1. Navigate to `/Claims/Create`.
  2. Fill form fields: `PolicyNumber`, incident date/description/location, attach files using the file control.
  3. Submit form.
- Expected:
  - Files saved to `wwwroot/uploads` and `Claim.Documents` populated with file metadata.
  - Claim saved (redirect to Index).
  - Notification service called (check logs for `[NOTIFICATION] Sending Email`).

4) Create Claim (invalid model)
- Preconditions: User is authenticated.
- Steps:
  1. Submit the Create form with missing required fields (e.g., empty `PolicyNumber`).
- Expected:
  - Model validation fails; the Create view is returned with validation messages.

5) Create Claim (file upload errors)
- Steps:
  1. Try uploading a large or locked file (simulate by setting file length 0 or permissions).
- Expected:
  - If file saving fails, appropriate error is shown or server logs the exception. Files should not be referenced in the claim unless saved successfully.

6) Authorization test
- Preconditions: Anonymous user.
- Steps:
  1. Access `/Claims`.
- Expected:
  - Redirect to login (because `ClaimsController` is decorated with `[Authorize]`).

Notes:
- Verify server logs for email notification calls from `EmailNotificationService` located in `eClaims.Infrastructure/Services/EmailNotificationService.cs`.

---

## API (eClaims.API) Test Cases

Base path: `/api/claims`

1) GET /api/claims - Get all claims
- Request:
  - Method: `GET`
  - URL: `https://localhost:5001/api/claims`
- Expected:
  - 200 OK
  - JSON array of claim objects

2) GET /api/claims/{id} - Get by Id
- Request:
  - Method: `GET`
  - URL: `https://localhost:5001/api/claims/{id}`
- Steps/Validation:
  - Provide valid existing id -> expect 200 and the claim JSON.
  - Provide non-existent id -> expect 404 Not Found.

3) POST /api/claims - Create claim
- Request:
  - Method: `POST`
  - URL: `https://localhost:5001/api/claims`
  - Headers: `Content-Type: application/json`
  - Body: sample JSON (see Common Test Data)
- Expected:
  - 201 Created with Location header referencing `GET /api/claims/{id}`
  - Response body contains created claim including generated `Id`.

Example curl:
```bash
curl -k -X POST https://localhost:5001/api/claims \
  -H "Content-Type: application/json" \
  -d '{"policyNumber":"POL-123456","incident": {"date":"2025-11-01T00:00:00Z","description":"Rear-end collision","location":"123 Main St"}}'
```

4) PUT /api/claims/{id} - Update claim
- Request:
  - Method: `PUT`
  - URL: `https://localhost:5001/api/claims/{id}`
  - Body: full claim JSON with matching `Id`.
- Expected:
  - If `id` equals claim.Id -> 204 No Content and repository `UpdateAsync` called.
  - If `id` does not match -> 400 Bad Request.

5) DELETE /api/claims/{id}
- Request:
  - Method: `DELETE`
  - URL: `https://localhost:5001/api/claims/{id}`
- Expected:
  - 204 No Content; subsequent `GET` returns 404.

6) Validation and negative tests
- Create with invalid JSON -> 400 Bad Request.
- Create with missing required fields -> server side may accept (no strict model attributes in code) — if business rules require, add model validation and assert 400.

7) Concurrency / Idempotency
- Test adding the same claim twice -> InMemoryRepository will assign new `Id` and store both entries. For Mongo-backed repository, consider unique constraints.

---

## Suggested Automated Tests
- Unit tests for `Repository<T>` and `InMemoryRepository<T>` covering CRUD operations.
- Integration tests for `eClaims.API` endpoints using `WebApplicationFactory<T>` (Microsoft.AspNetCore.Mvc.Testing) to spin up in-memory server and test endpoints.
- UI tests for `eClaims.Web` using Playwright or Selenium to cover Create flow including file upload.

Example of a minimal integration test command (dotnet CLI):
```powershell
dotnet test ./tests/IntegrationTests/ --filter Category=Integration
```

---

## Notes & Assumptions
- `ClaimsController` in `eClaims.Web` sets `UserId` from `User.Identity.Name` when authenticated.
- By default `eClaims.API` registers `InMemoryRepository<>` in `Program.cs`. To test against MongoDB, update DI to `Repository<>` and set `MongoDbSettings`.
- Notification service currently logs messages; tests should assert log entries or refactor service to allow injection of a test double.

---

File location: `architecture/tests/eClaims_test_cases.md`
