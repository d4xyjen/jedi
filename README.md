<p align="center">
  <img src="https://img.shields.io/github/license/dakaraic/jedi?style=flat-square" alt="GitHub"/>
  <img src="https://img.shields.io/github/v/release/dakaraic/jedi?style=flat-square" alt="GitHub release (latest by date)"/>
  <img src="https://img.shields.io/github/release-date/dakaraic/jedi?style=flat-square" alt="GitHub Release Date"/>
  <img src="https://img.shields.io/github/contributors/dakaraic/jedi?style=flat-square" alt="GitHub contributors"/>
  <img src="https://img.shields.io/github/issues/dakaraic/jedi?style=flat-square" alt="GitHub issues"/>
  <img src="https://img.shields.io/github/issues-pr/dakaraic/jedi?style=flat-square" alt="GitHub pull requests"/>
</p>

<br/>

## Overview
Welcome to Jedi's official repository, home to source code, documentation, and releases.  
<br/>
Jedi is a .NET game service project for Fiesta Online aimed at improving the core aspects of the game while also allowing developers to easily expand its capabilities.  
<br/>
Here are some useful links to help you get started:
- [Join the Discord community](https://discord.gg/528mphj6Fd)
- [Download builds](https://github.com/dakaraic/jedi/releases)
- [Track current issues](https://github.com/users/dakaraic/projects/1/views/1)
- [View official documentation](https://github.com/dakaraic/jedi/wiki)


<br/>

## Code Structure
Jedi contains many game services, each handling a different set of business logic. The table below provides a brief description of what each service does and where its codebase is located.

| Codebase | Description |
| :--- | :--- |
| [Authorization](https://github.com/dakaraic/jedi/tree/main/src/Authorization) | Authorizes accounts and guards access to available worlds. |
| [Common](https://github.com/dakaraic/jedi/tree/main/src/Common) | Library of common code shared amongst the services. |
| [Datastore](https://github.com/dakaraic/jedi/tree/main/src/Datastore) | Stores, retrieves, and backs up game data. |
| [Game](https://github.com/dakaraic/jedi/tree/main/src/Game) | Runs zone-partitioned instances of the game. |
| [World](https://github.com/dakaraic/jedi/tree/main/src/World) | Orchestrates communication with zone services. |
| [Zone](https://github.com/dakaraic/jedi/tree/main/src/Zone) | Load balancer for communication with game services. |

<br/>

## Contributing
Jedi is open to contributions, but it's recommended that you first create an issue (if it's not already created).
You can view a list of current issues on the project page [here](https://github.com/users/dakaraic/projects/1/views/1).

Get started by cloning the repository:
```
git clone https://github.com/dakaraic/jedi.git
cd jedi
```  

<br/>

**Pull Requests**  

All changes in the repository happen through pull requests, where your changes are reviewed and merged into the main branch. We actively invite you to submit your pull requests [here](https://github.com/dakaraic/jedi/pulls).

After creating your pull request, add it to our project page's "Please Review" page [here](https://github.com/users/dakaraic/projects/1/views/3).
You can also request reviews in the [#reviews](https://discordapp.com/channels/966409323817361520/966435732380086302) channel in our Discord server.

<br/>

**Conventional Commits**  

We use conventional commits to help us automate releases for the services. Please make sure your commit messages are standardized. Here's a page where you can learn more about the [conventional commits](https://www.conventionalcommits.org/en/v1.0.0/) standard and its benefits.

<br/>

**Copyright and license information for new files**  

We prefer that all new files created in the Jedi codebase begin with the following copyright notice:
```
// <copyright file="$safeitemrootname$.cs" company="Jedi Contributors">
// Copyright (c) Jedi Contributors. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE file in the 
// repository for more information.
// </copyright>
```

<br/>

If you need help making changes to the codebase, don't hesitate to reach out in the [#help](https://discord.com/channels/966409323817361520/966445867517227042) channel in our Discord server.
