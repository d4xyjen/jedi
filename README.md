## Welcome to the official Jedi repository
Open-source, high performance .NET game services project for Fiesta Online. Built with modern architecture and practices in mind, Jedi offers players a refreshing and revamped gameplay experience, while also improving heavily on the core aspects of the game.

![GitHub](https://img.shields.io/github/license/dakaraic/jedi?style=flat-square)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/dakaraic/jedi?style=flat-square)
![GitHub Release Date](https://img.shields.io/github/release-date/dakaraic/jedi?style=flat-square)
![GitHub contributors](https://img.shields.io/github/contributors/dakaraic/jedi?style=flat-square)
![GitHub issues](https://img.shields.io/github/issues/dakaraic/jedi?style=flat-square)
![GitHub pull requests](https://img.shields.io/github/issues-pr/dakaraic/jedi?style=flat-square)

## Structure
Jedi contains many game services, each handling a different set of business logic. The table below provides a brief description of what each service does and where its codebase is located.

| Service | Description |
| ---: | :--- |
| [Sienna](https://github.com/dakaraic/jedi/tree/main/src/Sienna) | Responsible for the orchestration of all game services. |
| [Authorization](https://github.com/dakaraic/jedi/tree/main/src/Authorization) | Authorizes game accounts and guards access to available [game regions](https://github.com/dakaraic/jedi/tree/main/src/Region). |
| [Region](https://github.com/dakaraic/jedi/tree/main/src/Region) | Contains and manages partitioned [game](https://github.com/dakaraic/jedi/tree/main/src/Game) instances and its own cluster of [gateways](https://github.com/dakaraic/jedi/tree/main/src/Gateway). |
| [Gateway](https://github.com/dakaraic/jedi/tree/main/src/Gateway) | Single replicable gateway that facilitates communication between players and the services in the game region. |
| [Game](https://github.com/dakaraic/jedi/tree/main/src/Game) | Serves game data and handles all game logic. |
| [Persistence](https://github.com/dakaraic/jedi/tree/main/src/Persistence) | Persists game data in a database. |
| [Backup](https://github.com/dakaraic/jedi/tree/main/src/Backup) | Creates frequent backups of game data. |

## Contributions
Jedi is open to contributions, but it's recommended that you first create an issue (if it's not already created) or let us know in the Discord server what you plan on working on so that we can mitigate conflicts.

Get started by cloning the repository:
```
git clone https://github.com/dakaraic/jedi.git
cd jedi
```
  
**Pull Requests**  

When making changes to the codebase, be sure to first create a branch that you'll be working on. Once you're done with your changes, open up a pull request to have your changes merged into the main branch.

Note: You'll need a code review and approval before your changes are merged into the main branch. You can request reviews [in our Discord server](https://discordapp.com/channels/966409323817361520/966435732380086302) 
  
  
**Conventional Commits**  

We use conventional commits to help us automate releases for the services. If your commit messages follow any other format, **they will fail the required check**. If you're not familiar with conventional commits, here's a [page where you can learn more about them and all of their benefits](https://www.conventionalcommits.org/en/v1.0.0/).

**Copyright and license information for new files**  

We prefer all new files created in the Jedi codebase to begin with the following copyright notice:
```
// <copyright file="$safeitemrootname$.cs" company="Jedi Developers">
// Copyright (c) Jedi Developers. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
```

### Need help?
If you need help making changes to the codebase, don't hesitate to reach out in [our help channel on Discord](https://discord.com/channels/966409323817361520/966445867517227042).
