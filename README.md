# Ecosystem-Simulation
Simulation project for CS 4632

## Project Status

**Implemented so far**
- Scalable simulation grid
- Prey agents that move, consume resources, and die
- Stochastic drought events which reduce cell moisture and density
- Visualization of simulation with customizable parameters

**What's next**
- Improved prey migration patterns
- Predator agents & hunting behavior
- Reproduction for animal agents
- More complex drought events
- Data collection and export
- Expanded simulation customization

**Changes from original proposal**
- Lotka-Volterra equations will not be used as they did not fit well with an agent based simulation

## Instillation Instructions
- Install EcosystemSimulation.zip
- Extract zip file

## Usage
- Run EcosystemSimulation.exe within the extracted files from EcosystemSimulation.zip
- Input simulation speed (simulation steps per second)
- Input grid size (number of rows and columns)
- Input drought chance (chance per step that a drought will occur, from 0 - 1)

## Architecture Overview
**Main Components**
- SimulationController: Manages overall simulation function on a grid
- EnvironmentCell: Individual cells with moisture and density values
- AnimalAgent: Abstract class with shared functions of predator and prey
- Prey: Prey migration & consumption
- UserInputs: Handles user input for simulation
- Drought: Handles cell changes & stochastic droughts

**How they map to the UML design**
- Environment was split into SimulationController, EnvironmentCell, and Drought
- Species = AnimalAgent
- The function of resources is handled within Drought & EnvironmentCell

**Architecture Changes**
- The environment structure was changed to fit a cell based grid
- Resources were changed to be values held within the cells and updated within the Drought class to simplify the calculation of changes within the environment

