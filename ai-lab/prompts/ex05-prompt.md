Context:  The Product business logic is divided into command and query handlers under Commands/Products and Queries/Products but it contains legacy backend logic that needs refactoring to improve long-term readability, safety, and performance.





Task: Analyze and refactor the product-related handlers to improve readability, maintainability, and performance without changing their behavior and explain the main problems found in the current implementation.



Requirements:

-Simplify unnecessary nesting or duplicated logic.

-Avoid N+1 database queries.

-Keep database operations asynchronous. Do not use .Result or .Wait().

-Preserve the existing business rules, exception patterns, routes, and API contracts.

-Verify that all existing tests continue to pass.



Constraints:

-Make only necessary changes and don't modify any unrelated functionality.