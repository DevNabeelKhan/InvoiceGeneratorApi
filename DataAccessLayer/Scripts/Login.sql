-- Run this against the InvoiceGenerator database.
CREATE OR ALTER PROCEDURE [dbo].[Login]
    @UserId NVARCHAR(255) = NULL,
    @Password NVARCHAR(300) = NULL
AS
BEGIN
    SELECT
        u.*, r.Title AS RoleTitle
    FROM [User] u (NOLOCK)
    LEFT JOIN [Role] r (NOLOCK) ON r.Id = u.RoleId AND r.IsActive = 1
    WHERE LOWER(u.UserName) = LOWER(@UserId)
      AND u.[Password] = @Password
      AND u.IsActive = 1
END
