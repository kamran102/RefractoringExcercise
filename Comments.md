Test does not actually invoke the SUT.
The tests do not cover all of the operations the function can do.
Product application service does not follow Single responsiblity principle as it tries to manipulate the input into various forms for the output.
The single method should be split into seperate operations that are specific to what is needed - Interface segregation principle.
The input to the function is an interface, but then it uses the concrete implementations to determine the course of the function. This should be a check for the interface to ensure the open-closed prinicple is adhered to.
The return type is int for the applcation id, but -1 for failures.  It's not verbose enough to determine what the issue with the operation is, it would equate to "Computer says No", without an understanding of why and as such would require as investigation to determine the route cause.  Need to log or return the errors encountered in the operation.
Each new entry type would require an addition to the function as it currently stands, and over time it would get quite messy without clear indiciation of which services are invoked without looking at the underlying code.

Plan of action
Overall: Tests to verify current functionality. Refractor the code into individual service opertions.

Actions:
1. Additional tests to ensure all test scenarios of the current code are covered.
2. Introduce new methods on ProductApplicationService to cover existing scenarios.
3. Extend the test coverage to ensure new functionality is fully testable.
4. Refractor the existing function to utilise the new functionality but mark the method as obselete to ensure it's usage is phased out.
5. Introduce interfaces for the various products as this would allow the items to be customised in the future.

What has Been Done:
Actions 1-4 were completed.  4 is partially implemented, tests need to be seperated out.

What is left to do:
5 will need to be done in the future.
Additionaly checks on how the current functionality is utilised so that the return type of (2) needs to be amended.