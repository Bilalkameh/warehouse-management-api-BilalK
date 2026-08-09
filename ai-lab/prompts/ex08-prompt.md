Context: The following AI-generated file-upload endpoint may contain serious security flaws.



Task: Audit this code:

[HttpPost("upload-invoice")] public async Task<IActionResult> UploadInvoice(IFormFile file) 
{ // WARNING: This AI snippet contains multiple severe security gaps. 
// Analyze for path traversal risks, content manipulation, validation gaps, and unsafe logging. 
var targetFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "invoices"); 
var fullPath = Path.Combine(targetFolder, file.FileName); 
using (var stream = new FileStream(fullPath, FileMode.Create)) { await file.CopyToAsync(stream); } return Ok(new { path = fullPath }); }


Requirements:

-Identify security flaws.

-Identify missing validation.

-Review the exception handling.

-Identify file-upload risks.

-Identify path traversal risks.

-Review the logging.

