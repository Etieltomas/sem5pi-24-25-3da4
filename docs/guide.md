# Guide for All

# 1. Context

This document is intended to be used by everyone who uses this project to ensure that the best practices are taken into account.
This contains guidelines not only for code implementation but also for documentation.

# 2. Guide

## 2.1. Commits

### 2.1.1. Commit Structure

When performing a commit, the commit message should follow the following structure:

"#<span style="color: #eabe26">**[Issue Number in GitHub]**</span> - <span style="color: #47b969">**[Commit message]**</span>"

Where:
- <span style="color: #eabe26">**[Issue Number in GitHub]**</span> is the GitHub issue ID that the commit is related to.
- <span style="color: #47b969">**[Commit message]**</span> is the message of the commit.

The "[]" should not be included in the commit message or Issue Number of the GitHub.

#### Important: All commits made must be associated with an Issue on GitHub!

---

### 2.1.2. Times to commit and push

#### <span style="color: #3a88d0">**Commit**</span>
- A basic operation in version control systems like Git is a commit. It stands for a location on the project timeline 
where you can document file modifications.
- When you commit modifications, you first choose which ones to include, write a note explaining the changes, and save 
the changes to the version control repository.
- A clear and intelligible history of the modifications made over time is maintained via the accompanying descriptive 
messages that are frequently attached to commits.

#### <span style="color: #3a88d0">**Push**</span>
- A feature unique to Git and other distributed version control systems is pushing. Commits produced in your local 
repository are pushed to a remote repository (like Bitbucket, GitHub, or GitLab) via this method.
- A push updates the remote repository with the modifications that have been logged in your local commits since the 
previous push.
- In order to keep the local repository and the remote repository in sync and to guarantee that all project 
participants have access to the most recent changes, pushing is essential.

Commits must appear in a balanced way as the project progresses (neither rarely nor too often). Despite
Since there are no rules about this, it is important to maintain good documentation for the project that 
is developed by several people.

Pushes should only occur when the code is ready and reviewed without any errors that could jeopardize the
project compilation. This must therefore be working and tested.

## 2.2. Documentation

### 2.2.1. To be taken into consideration

#### <span style="color: #3a88d0">**When making and reviewing the documentation, it should be assured that:**</span>

- The code is well documented.
- The test cases cover all the use cases, in the different scenarios.
- SOLID principles are followed.
- GRASP principles are followed.
- BASE principles are followed.
- Each use case is accompanied by a read me file with the software engineering processes.
- There is a general glossary for the project with technical terms used.
- A read me containing FURPS+, with non-functional requirements, is created.
- Use Case Diagram must be presented in the project.

## 2.3. Code

### 2.3.1. To be taken into consideration

#### <span style="color: #3a88d0">**When making and reviewing the code, it should be assured that:**</span>

- It is necessary to abide by all of the documentation guidelines listed above.
- The documentation and the code make sense together.
- The commits were made according to the commit struct and in the correct order.
- Tested code is well-written. This implies that tests for the various scenarios should be conducted for each approach.
- Comments should be included to complex code.
- The code has a clean, organized format.