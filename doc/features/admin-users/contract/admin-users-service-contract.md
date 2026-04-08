# Admin Users Service Contract

## Application Flows

- `GetAdminUsersQuery`
  - input: `search`
  - output: summary counters plus user list

- `ChangeUserPasswordCommand`
  - input: `userId`, `newPassword`
  - output: operation result with success message

## Dependencies

- `IAppUserQueryRepository`
- `IAppUserCommandRepository`
- `IPasswordHashingService`
