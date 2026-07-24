using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace Warehouse.Infrastructure.Authentication;

public static class FirebaseRoleSeeder
{
    public static async Task SeedRolesAsync(string projectId, string adminUid, string userUid, CancellationToken cancellationToken = default)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.GetApplicationDefault(),
            ProjectId = projectId
        });

        var firebaseAuth = FirebaseAuth.DefaultInstance;

        await firebaseAuth.SetCustomUserClaimsAsync(adminUid,
            new Dictionary<string, object>
            {
                ["role"] = "admin"
            }, 
            cancellationToken);

        await firebaseAuth.SetCustomUserClaimsAsync(userUid,
            new Dictionary<string, object>
            {
                ["role"] = "user"
            },
            cancellationToken);
    }
}