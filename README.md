# Ecosystem-Simulation
Simulation project for CS 4632

## Project Overview
This project simulates ecosystem dynamics in an ecosystem experiencing drought. This simulation combines Lotka-Volterra predator and prey interactions with resources, carrying capacities, and stochastic droughts to simulate the impact droughts have on species interactions.

## Project Status

**Implemented Features**
- Lotka-Volterra Based Simulation
- Predator & Prey interaction
- Resources which are consumed by Prey
- Stochastic Drought events which affect resources, prey, and predators
- Logistic growth for Prey and Resources with carrying capacities
- Data collection for populations, drought level, and performance

**Changes from original proposal**
- Simulation is no longer agent-based, shifting towards an approach that is more closely tied to mathematical models

## Instillation Instructions
- Install EcosystemSimulation.zip
- Extract zip file

## Usage
- Run EcosystemSimulation.exe within the extracted files from EcosystemSimulation.zip
- Input initial population sizes for Prey, Predators, and Resources
- Input parameters for reproduction, predation, growth, and death rates
- Input name for run
- Input seed for randomization
- Click Start to begin simulation

## Architecture Overview
**Main Components**
- SimulationController: Manages all simulation logic, data collection and exports
- UserInputs: Handles the users inputs and creates new simulations using user input

**Architecture Changes**
- Individual scripts for Prey, Predator, and Resources have been removed in the process of shifting away from the agent based approach
- All simulation functions are now within SimulationController
