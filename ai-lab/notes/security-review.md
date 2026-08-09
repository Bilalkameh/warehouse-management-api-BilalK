# Exercise 8 — Security Review of the Invoice Upload Endpoint

## Code under review

```csharp
[HttpPost("upload-invoice")]
public async Task<IActionResult> UploadInvoice(IFormFile file)
{
    var targetFolder = Path.Combine(
        Directory.GetCurrentDirectory(),
        "wwwroot",
        "invoices");

    var fullPath = Path.Combine(targetFolder, file.FileName);

    using (var stream = new FileStream(fullPath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    return Ok(new { path = fullPath });
}
```

## Overall assessment

The endpoint is critically unsafe. It trusts a client-supplied filename, accepts arbitrary content without validation, writes the upload directly beneath `wwwroot`, overwrites existing files, and returns the server's physical path. These weaknesses can enable path traversal, unauthorized file replacement, public hosting of malicious content, stored cross-site scripting, denial of service, and disclosure of server information.

The supplied code does not show authentication, authorization, exception handling, request limits, malware scanning, or security logging. This review therefore does not assume that any of these controls exist elsewhere.

## Findings

| Severity | Finding | Risk | Recommended mitigation |
|---|---|---|---|
| Critical | `file.FileName` is used directly in the storage path | A malicious filename may escape the invoice directory and write to an unintended location | Never use the client filename as the storage name. Generate a unique server-side name and keep the original name only as sanitized metadata if needed |
| Critical | Files are stored under `wwwroot` | Uploaded files may become publicly accessible. HTML or SVG content may execute scripts in a user's browser | Store uploads outside the web root and expose them only through an authenticated, authorized download endpoint |
| Critical | `FileMode.Create` overwrites existing files | An attacker may replace or truncate an existing file when names collide | Generate unique names and use `FileMode.CreateNew` or an equivalent no-overwrite operation |
| High | No file type or content validation | Executables, scripts, malware, polyglot files, or manipulated invoices can be stored | Use a strict allowlist and validate the actual file signature and structure; do not trust the extension or MIME type alone |
| High | No file-size or request-size limit | Large uploads may exhaust memory, storage, bandwidth, or processing capacity | Configure request limits and enforce an application-level maximum file size |
| High | No authentication or authorization is shown | An unauthorized caller may be able to upload files | Require authentication and verify that the caller has permission to upload invoices |
| Medium | The physical server path is returned | The response exposes filesystem and deployment details | Return an opaque file identifier or a controlled logical URL |
| Medium | No malware quarantine or scanning | Malicious documents may be stored and later distributed | Store new uploads in a private quarantine area, scan them, and move them only after validation succeeds |
| Medium | No collision or concurrency handling | Simultaneous uploads with the same name can overwrite one another or produce unpredictable results | Use generated identifiers and atomic creation or move operations |
| Medium | No cleanup strategy after a failed copy | A failed request may leave an empty, truncated, or partially written file | Write to a temporary private file and delete it on failure; publish it atomically only after processing succeeds |

## Path traversal risk

The main path traversal weakness is:

```csharp
var fullPath = Path.Combine(targetFolder, file.FileName);
```

`file.FileName` comes from the multipart request and must be treated as untrusted. A crafted filename can contain parent-directory components, rooted paths, alternative separators, or platform-specific path syntax. Depending on the operating system and path form, the resulting path may point outside the intended invoice directory.

Examples of hostile input include:

```text
../../appsettings.json
../../wwwroot/index.html
C:\some\absolute\path.txt
```

Using `Path.GetFileName(file.FileName)` can remove directory components when preserving a display name, but it is not a complete storage strategy. The stored file should receive a unique server-generated name. If any user-derived path is ever constructed, the application should obtain its canonical path with `Path.GetFullPath` and verify that it remains beneath the canonical target directory.

## Missing validation

The endpoint does not verify:

- Whether `file` is `null`.
- Whether the upload is empty.
- Whether the file exceeds an allowed size.
- Whether the filename is too long or contains invalid or reserved characters.
- Whether the extension is allowed.
- Whether the actual file signature and structure match the expected format.
- Whether `file.ContentType` corresponds to the content. This value is supplied by the client and is not proof of the file type.
- Whether the document contains scripts, malicious macros, exploits, or malware.
- Whether the uploaded document is a valid invoice.
- Whether the caller is authenticated and authorized.
- Whether the destination filename already exists.
- Whether the target directory exists and is writable.

Checking only the extension or `ContentType` would remain insufficient because both can be forged. A file named `.pdf` may contain completely different data.

## File upload and content manipulation risks

Writing unvalidated uploads directly under `wwwroot` may allow an attacker to:

- Host malware or prohibited content through the application.
- Upload HTML or SVG content that executes JavaScript when opened.
- Replace an existing public asset or document.
- Fill the server's disk with oversized or repeated uploads.
- Supply a malformed document that exploits a later parser or viewer.
- Upload a forged or manipulated invoice.
- Leave partially written files when a copy operation fails.

A safer flow would receive the file in a non-public quarantine location, validate its size and actual format, scan it for malicious content, and then atomically move the accepted file to private permanent storage.

## Exception handling review

The endpoint contains no local exception handling, and the snippet does not show centralized exception middleware. File operations may fail because of:

- A missing target directory.
- Insufficient filesystem permissions.
- Invalid or excessively long paths.
- An existing file or concurrent write.
- I/O errors or exhausted disk space.
- A disconnected client or cancelled request.
- A failure while copying the stream.

Because `FileMode.Create` creates or truncates the destination before the copy completes, a failure may leave an empty or partial file.

Expected validation failures should produce controlled HTTP responses, such as:

- `400 Bad Request` for a missing or invalid upload.
- `413 Payload Too Large` for a file above the configured limit.
- `415 Unsupported Media Type` for an unsupported format.

Unexpected failures should be handled by centralized exception middleware and returned as a generic `500` Problem Details response. Physical paths, stack traces, and internal exception details must not be exposed to the client. The request's `CancellationToken` should also be passed to `CopyToAsync`, and temporary files should be removed when processing fails.

## Logging review

The endpoint performs no visible security or operational logging. A secure implementation should use structured logging to record:

- A generated upload or file identifier.
- The authenticated user identifier.
- The validated file type and file size.
- The correlation identifier.
- Whether the upload succeeded, was rejected, or failed scanning.
- A sanitized reason for rejection or failure.

Logs should not include:

- File contents or sensitive invoice data.
- Full physical filesystem paths.
- Unsanitized client-supplied filenames.
- Authentication tokens or secrets.
- Detailed exceptions in responses sent to clients.

Detailed exception information may be retained in protected server logs, subject to the application's data-retention and access-control policies.

## Minimum safe design

A safe replacement should:

1. Authenticate the caller and verify upload permission.
2. Reject missing, empty, oversized, or unsupported files.
3. Validate the real file signature and structure rather than trusting the filename or MIME type.
4. Generate a unique server-side storage name.
5. Store uploads outside `wwwroot` in private storage.
6. Prevent overwriting existing files.
7. Quarantine and scan uploads before making them available.
8. Clean up temporary or partially written files after failure.
9. Use centralized exception handling with safe Problem Details responses.
10. Record structured security events without logging sensitive content or internal paths.
11. Return only an opaque file identifier or authorized logical URL.

## Conclusion

The supplied endpoint should not be used in production in its current form. The most urgent corrections are eliminating client-controlled storage paths, moving uploads outside the web root, enforcing size and content validation, preventing overwrites, and avoiding disclosure of the physical path.
