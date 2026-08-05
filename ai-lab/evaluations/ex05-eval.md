The AI-generated refactor built successfully and all existing tests passed without regressions.

It correctly identified duplicated cache invalidation logic and unnecessary EF Core tracking in read-only queries.
It added a small cache invalidation helper and used AsNoTracking() where appropriate.
There was no N+1 query problem because the supplier data was already loaded using Include.
I did not need to correct any code.