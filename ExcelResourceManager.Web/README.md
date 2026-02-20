# Excel Resource Manager - Web Application

## ASP.NET Core MVC Application for HR Management

### Overview
This is a web-based HR management system for Ecuador and Paraguay operations, featuring automatic conflict detection, holiday tracking, and Excel report generation.

### Technology Stack
- **ASP.NET Core 8.0** - MVC web framework
- **LiteDB** - Embedded NoSQL database
- **Bootstrap 5** - Responsive UI framework
- **Serilog** - Logging
- **ClosedXML** - Excel generation

### Running the Application

1. **Prerequisites**:
   - .NET 8.0 SDK installed
   - Any modern web browser

2. **Start the application**:
```bash
cd ExcelResourceManager.Web
dotnet run
```

3. **Access the application**:
   - Open browser to `https://localhost:5001` or `http://localhost:5000`
   - The application will automatically create and seed test data on first run

### Features

#### Dashboard
- **KPI Cards**: Real-time metrics for employees, clients, and conflicts
- **Upcoming Vacations**: View vacations scheduled for the next 30 days
- **Quick Actions**: Navigate to common tasks

#### Modules
- **Empleados**: Manage employee records
- **Clientes**: Manage client information
- **Vacaciones**: Request and track vacations with conflict detection
- **Conflictos**: View and resolve scheduling conflicts
- **Reportes**: Generate Excel reports with ClosedXML

### Architecture

**100% Code Reuse from Desktop Version:**
- `ExcelResourceManager.Core` - Business logic, models, services
- `ExcelResourceManager.Data` - LiteDB repositories
- `ExcelResourceManager.Reports` - Excel generation

**New Web Layer:**
- `ExcelResourceManager.Web` - ASP.NET Core MVC
  - Controllers for each module
  - Razor views with Bootstrap
  - Dependency injection
  - Serilog logging

### Configuration

Edit `appsettings.json` to configure:
- **ConnectionStrings**: Database paths for Test and Production modes
- **App:DefaultMode**: "Test" or "Production"
- **Serilog**: Logging configuration

### Test Data

On first run, the application seeds 84 test records:
- 20 employees across Guayaquil, Quito, and Asunción
- 15 vacations (with intentional conflicts for testing)
- 10 trips
- 10 support shifts
- 31 holidays for 2026 (Ecuador and Paraguay)

### Benefits Over Desktop Version

✅ **No Threading Issues** - Web requests are isolated
✅ **Cross-platform** - Works on Windows, Linux, Mac  
✅ **Browser-based** - Access from anywhere
✅ **Simple Deployment** - Just copy files
✅ **Better Debugging** - Full Visual Studio support
✅ **Scalable** - Easy to add features

### Development

- **Debug**: Press F5 in Visual Studio
- **Hot Reload**: Edit Razor views while running
- **Logs**: Check `Logs/log-{date}.txt`

### Production Deployment

1. Publish the application:
```bash
dotnet publish -c Release
```

2. Copy `bin/Release/net8.0/publish/` to server

3. Configure IIS, Kestrel, or nginx as reverse proxy

4. Update `appsettings.json` with production settings

---

**No more threading exceptions! Stable, mature, and production-ready.**
