# Overview
Open-source, high performance .NET game services for Fiesta Online. Built using modern architecture and practices, Jedi offers players a refreshed and revamped gameplay experience, while also improving on the core aspects of the game.

## Copyright notice for new files
We prefer all new files created in the Jedi codebase to begin with the following copyright notice:
```
// <copyright file="Program.cs" company="Jedi Developers">
// Copyright (c) Jedi Developers. All rights reserved.
//  
// This software is licensed under the MIT license. Read the LICENSE.txt file in the 
// repository for more information.
// </copyright>
```

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
