# Elevator Simulation System

A comprehensive C# console application that simulates the movement of elevators within a large building, designed to optimize passenger transportation efficiently while adhering to Object-Oriented Programming (OOP) principles and Clean Architecture.

## 🏗️ Architecture Overview

This project follows Clean Architecture principles with the following layers:

- **Common.Dry**: Shared interfaces and base classes for data access patterns
- **Common.Messaging**: Event-driven messaging infrastructure
- **Elevator.Domain**: Core business logic and entities
- **Elevator.Infrastructure**: Implementation of domain services
- **Elevator.Console**: User interface and application entry point
- **Elevator.Tests**: Comprehensive unit tests

## 🚀 Features

### Core Functionality
- **Real-Time Elevator Status**: Display current floor, direction, movement state, and passenger count
- **Interactive Elevator Control**: Call elevators to specific floors with direction and passenger count
- **Multiple Floors Support**: 20-story building simulation with configurable floor capacities
- **Multiple Elevators Support**: 4 different elevator types with varying characteristics
- **Efficient Elevator Dispatching**: Smart algorithm to send the nearest available elevator
- **Passenger Limit Handling**: Weight/passenger capacity management for each elevator
- **Different Elevator Types**: Support for Standard, High-Speed, Glass, Freight, Service, and Express elevators

### Advanced Features
- **Real-Time Operation**: Immediate responses to user interactions
- **Event-Driven Architecture**: Domain events for elevator calls and arrivals
- **Maintenance Mode**: Set elevators under maintenance when needed
- **Statistics Tracking**: Monitor total trips and distance traveled
- **Door State Management**: Realistic door opening/closing simulation
- **Direction Optimization**: Smart routing based on current elevator direction

## 🎯 SOLID Principles Implementation

### Single Responsibility Principle (SRP)
- Each class has a single, well-defined responsibility
- Elevator control, floor management, and passenger handling are separated

### Open/Closed Principle (OCP)
- System can be extended without modifying existing code
- New elevator types can be added easily
- Additional control strategies can be implemented

### Liskov Substitution Principle (LSP)
- All elevator implementations can be used interchangeably
- Base elevator functionality is preserved across types

### Interface Segregation Principle (ISP)
- Interfaces are specific to client needs
- Separate interfaces for elevator control, floor events, and passenger interactions

### Dependency Inversion Principle (DIP)
- High-level modules depend on abstractions
- Easy to swap different elevator implementations

## 🧪 Unit Testing

Comprehensive unit tests covering:
- Elevator service operations
- Entity behavior and state changes
- Floor management operations
- Event handling and messaging
- Edge cases and error conditions

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Building the Solution
```bash
# Navigate to the project directory
cd ElevatorTwo

# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the console application
dotnet run --project src/Elevator.Console/Elevator.Console.csproj
```

### Running the Application
1. Build the solution
2. Run the console application
3. Use the interactive menu to:
   - Call elevators to specific floors
   - View elevator status
   - Update people waiting on floors
   - Process simulation steps
   - Set maintenance modes

## 🔄 Continuous Integration

This project uses GitHub Actions for automated building and testing:

### Workflows

#### Build and Test (`build-test.yml`)
- **Trigger**: Push to `main`/`master` branch or pull requests
- **Actions**: 
  - Restore NuGet dependencies
  - Build the solution in Release configuration
  - Run all unit tests
  - Provide build status feedback

#### CI/CD Pipeline (`ci-cd.yml`)
- **Trigger**: Push to `main`/`master` branch or pull requests
- **Features**:
  - Multi-platform testing (Ubuntu & Windows)
  - Dependency caching for faster builds
  - Code quality analysis
  - Comprehensive test reporting
  - Workflow summary with detailed results

### Local Testing
You can run the same commands locally that the CI pipeline uses:

```bash
# Restore dependencies
dotnet restore ElevatorSystem.sln

# Build solution
dotnet build ElevatorSystem.sln --configuration Release

# Run tests
dotnet test ElevatorSystem.sln --configuration Release
```

## 📋 Usage Examples

### Calling an Elevator
1. Select option 1: "Call Elevator"
2. Enter floor number (1-20)
3. Enter direction (U for Up, D for Down)
4. Enter number of people waiting
5. System will assign the nearest available elevator

### Viewing Status
- Option 2: Detailed elevator status
- Option 3: Update people waiting on floors
- Option 4: Process simulation step
- Option 5: Set maintenance mode

## 🏢 Building Configuration

### Floors
- 20 floors (1-20)
- Ground floor (1) has higher capacity (200 people)
- Upper floors have standard capacity (100 people)
- Floors 16-20 are restricted access

### Elevators
- **E1**: Standard elevator (8 passengers, 1.0 speed)
- **E2**: High-speed elevator (10 passengers, 2.0 speed)
- **E3**: Glass elevator (6 passengers, 1.2 speed)
- **E4**: Freight elevator (12 passengers, 0.8 speed)

## 🔧 Configuration

The system can be easily configured by modifying:
- Number of floors
- Elevator specifications
- Floor capacities
- Elevator speeds and door operation times
- Maintenance schedules

## 📊 Performance Features

- **Efficient Routing**: Smart algorithm to minimize wait times
- **Direction Optimization**: Prioritizes elevators heading in the same direction
- **Capacity Management**: Prevents overloading and optimizes passenger distribution
- **Real-Time Updates**: Immediate status updates and event notifications

## 🛡️ Error Handling

- Comprehensive input validation
- Graceful error handling with meaningful messages
- Exception logging and monitoring
- Fallback mechanisms for edge cases

## 🔮 Future Enhancements

- **Real-Time Monitoring**: Web dashboard for building management
- **Predictive Analytics**: AI-powered elevator optimization
- **Integration APIs**: Connect to building management systems
- **Mobile App**: Smartphone elevator calling
- **Energy Optimization**: Smart scheduling for energy efficiency

## 📝 Code Quality

- **C# Coding Standards**: Follows Microsoft and industry best practices
- **Comprehensive Documentation**: XML documentation for all public APIs
- **Clean Code Principles**: Readable, maintainable, and self-documenting code
- **Performance Optimization**: Efficient algorithms and data structures
- **Security Considerations**: Input validation and safe operations

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add/update tests
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🏆 Challenge Requirements Met

✅ **Real-Time Elevator Status**: Complete status display with floor, direction, movement, and passengers  
✅ **Interactive Control**: Full user interaction for calling elevators and updating floor information  
✅ **Multiple Floors & Elevators**: 20 floors and 4 different elevator types  
✅ **Efficient Dispatching**: Smart algorithm for nearest available elevator selection  
✅ **Passenger Limits**: Weight/passenger capacity management  
✅ **Different Elevator Types**: Support for 6 elevator types with extensible architecture  
✅ **Real-Time Operation**: Immediate responses and real-time status updates  
✅ **Clean Architecture**: Proper separation of concerns and dependency injection  
✅ **SOLID Principles**: All five principles implemented correctly  
✅ **Unit Tests**: Comprehensive test coverage for all components  
✅ **Error Handling**: Robust error handling and validation  
✅ **User Feedback**: Clear system output and user guidance  

## 🎯 Extra Credit Features

- **Multiple Elevator Types**: 6 different elevator types with varying characteristics
- **Advanced Routing Algorithm**: Smart elevator selection based on multiple criteria
- **Statistics Tracking**: Monitor elevator performance and usage
- **Maintenance Mode**: Set elevators under maintenance when needed
- **Event-Driven Architecture**: Domain events for system monitoring
- **Extensible Design**: Easy to add new elevator types and features
