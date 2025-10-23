# Ecosystem-Simulation
Simulation project for CS 4632

## Simulation Description

**Overview**
	This project is intended to simulate an ecosystem under drought, and the changes in populations  and dynamics that result from them. This simulation models the population dynamics of an ecosystem with 3 entities: prey, predators, and resources. The simulation is primarily based on the Lotka-Volterra model for predator-prey interactions, with modifications to include a carrying capacity and logistic growth for prey, resource entities that interact with prey, and stochastic droughts that modify the simulation and impact the ecosystem.
	
**Mathematical Models**
	The primary model used in this simulation is Lotka-Volterra, which simulates predator-prey population dynamics. The prey and resources utilize logistic growth with a carrying capacity. In addition, stochastic droughts randomly occur which impacts several parameters in the simulation. The birth rates, growth rate for resources, death rates, and carrying capacities are all dynamically altered by the drought level.
	
**Ecological Phenomenon**
	This simulation is intended to study the ways in which stochastic environmental stressors impact an ecosystem and the species within. For this simulation, the environmental stressor is droughts, which have far reaching effects across the environment, impacting resources, predators, and prey. 
	
**Research Questions**
- How do environmental pressures affect the survivability of predator and prey populations?
- How do droughts impact the long term stability of an ecosystem?
- How are predator-prey dynamics altered under conditions of limited resources?


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
