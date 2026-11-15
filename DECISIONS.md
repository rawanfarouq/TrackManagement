# Development Decisions & AI Usage

## 1. What did AI generate, and what did I write or modify myself?

AI assistance was used to understand the assignment requirements, plan the project structure, and generate the initial .NET CLI commands for setting up the solution using Clean Architecture.

I chose **React and SQL Server** based on my previous experience with these technologies and the 72-hour submission deadline, allowing me to focus on implementation rather than learning a new stack.

I executed the setup commands, configured the solution and project references, installed the necessary development tools, and verified that the backend projects built successfully.

For the Domain layer, AI helped generate the initial entity classes (`Artist`, `Track`, `DSP`, and `TrackDistribution`) and status enums. I reviewed their properties, relationships, and navigation properties, including the purpose of ISRC and `ICollection<T>`, to understand how the entities relate to each other.

I also initialized the Git repository and created commits to track development progress.

This section will be updated throughout implementation to distinguish AI-generated code from my own modifications.

## 2. What security issues did I identify in AI-generated code, and how did I handle them?

To be completed after implementing and reviewing authentication, input validation, database constraints, and sensitive configuration.

## 3. What did AI get wrong that I had to fix, and why?

To be completed when an actual issue in AI-generated code is identified, investigated, and corrected during development.