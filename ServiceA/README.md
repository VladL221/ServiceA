# Service A - Graph Management API

This service provides a REST API for graph management operations and communicates with Service B via WebSocket.

## Design Patterns & SOLID Principles

### SOLID Principles Implementation
1. **Single Responsibility Principle**
   - `GraphService`: Handles graph operations logic
   - `WebSocketClientService`: Manages WebSocket communication
   - `JsonMessageStrategy`: Handles message formatting

2. **Open/Closed Principle**
   - Message strategy pattern allows adding new message formats without modifying existing code
   - New graph operations can be added by extending the service without changing existing code

3. **Interface Segregation Principle**
   - `IWebSocketClient`: Defines only necessary WebSocket operations
   - `IMessageStrategy`: Focused on message formatting only
   - `IGraphService`: Specific to graph operations

4. **Dependency Inversion Principle**
   - High-level modules depend on abstractions
   - All dependencies are injected through constructors

### Design Patterns
1. **Strategy Pattern**
   - `IMessageStrategy` interface
   - `JsonMessageStrategy` implementation
   - Allows for different message formatting strategies

2. **Singleton Pattern**
   - `WebSocketClientService` registered as singleton
   - Ensures single WebSocket connection throughout application lifecycle

## Project Structure
```
ServiceA/
├── Controllers/
│   └── GraphController.cs
├── Interfaces/
│   ├── IGraphService.cs
│   ├── IMessageStrategy.cs
│   └── IWebSocketClient.cs
├── Models/
│   ├── Edge.cs
│   ├── Graph.cs
│   └── Node.cs
└── Services/
    ├── GraphService.cs
    ├── JsonMessageStrategy.cs
    └── WebSocketClientService.cs
```

## How to Run

1. Configure ports in `launchSettings.json`:
```json
{
    "profiles": {
        "http": {
            "applicationUrl": "http://localhost:5001"
        }
    }
}
```

2. Start the service:
```bash
dotnet run
```

## API Testing with Postman

### Create Graph
```http
POST http://localhost:5001/api/graph
Content-Type: application/json

{
  "node": {
    "label": "Root Node"
  },
  "edges": [
    {
      "sourceNodeId": "00000000-0000-0000-0000-000000000000",
      "targetNodeId": "00000000-0000-0000-0000-000000000001",
      "label": "Edge 1"
    },
    {
      "sourceNodeId": "00000000-0000-0000-0000-000000000000",
      "targetNodeId": "00000000-0000-0000-0000-000000000002",
      "label": "Edge 2"
    }
  ]
}
```

### Get Graph
```http
GET http://localhost:5001/api/graph/{id}
```

### Update Graph
```http
PUT http://localhost:5001/api/graph/{id}
Content-Type: application/json

{
  "node": {
    "label": "Updated Root Node"
  },
  "edges": [
    {
      "sourceNodeId": "00000000-0000-0000-0000-000000000000",
      "targetNodeId": "00000000-0000-0000-0000-000000000001",
      "label": "Updated Edge 1"
    },
    {
      "sourceNodeId": "00000000-0000-0000-0000-000000000000",
      "targetNodeId": "00000000-0000-0000-0000-000000000002",
      "label": "Updated Edge 2"
    }
  ]
}
```

### Delete Graph
```http
DELETE http://localhost:5001/api/graph/{id}
```

## Dependencies
- .NET 8.0
- Service B running on port 5002

## Important Notes
- Ensure Service B is running before starting Service A
- All graph operations require exactly 2 edges
- IDs are automatically generated for new graphs