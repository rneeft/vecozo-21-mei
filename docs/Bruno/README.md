# Online Toestemming Bruno API Collection

This Bruno collection contains HTTP requests for testing the Online Toestemming Workshop APIs.

## Getting Started

### Prerequisites
- [Bruno](https://usebruno.com/) API client
- Online Toestemming services running locally

### Environment Setup
The collection includes a `Local` environment with the following variables:
- `company_token`: JWT token for healthcare company authentication
- `internal_token`: JWT token for internal service authentication  
- `patient_token`: JWT token for patient authentication
- `company_id`: ID of the registered company
- `company_name`: Name of the registered company

## API Endpoints

### Identity API (Port 7061)
- **Get Internal Token**: Generates an internal service token
- **Get Company Token**: Generates a token for a healthcare company
- **Patient Login**: Authenticates a patient

### Pseudoniem API (Port varies)
- **Get Pseudoniem by BSN**: Retrieves pseudoniem for a given BSN (requires internal token)

### Dossier API (Port 7203)
- **Register Company**: Register a new healthcare company (no auth required)
- **Create Dossier**: Create a new patient dossier (requires company token)
- **Check Patient Permission**: Check if patient gave permission (requires company token)
- **Delete Dossier**: Delete a patient dossier (requires company token)

## Typical Workflow

1. **Register Company** (if not already registered)
   - Use the `RegisterCompany` request
   - This will set `company_id` and `company_name` environment variables

2. **Get Company Token**
   - Use the `GetToken` request
   - This will set the `company_token` environment variable

3. **Create/Manage Dossiers**
   - Use `CreateDossier` to create new dossiers
   - Use `CheckPatientPermission` to verify permissions
   - Use `DeleteDossier` to remove dossiers

## Key Features

### Company Registration
- No authentication required
- Returns unique company ID
- Prevents duplicate company names

### Permission Checking
- Always returns HTTP 200 OK (unless authentication fails)
- `GavePermission = false` if company has no dossier
- `GavePermission = false` if patient didn't give permission
- `GavePermission = true` only when company has dossier AND patient gave permission

### Dossier Creation
- If company already has dossier for patient: returns OK (no conflict)
- Creates new dossier if none exists
- Requires valid BSN and patient existence

### Error Handling
- 401 Unauthorized: Invalid or missing token
- 403 Forbidden: Insufficient permissions
- 404 Not Found: Patient or dossier not found
- 409 Conflict: Duplicate company registration

## Security Notes

- Company registration endpoint is intentionally not authenticated
- All other dossier operations require healthcare company authentication
- Internal tokens are for service-to-service communication only
- Patient tokens are for patient-facing operations only