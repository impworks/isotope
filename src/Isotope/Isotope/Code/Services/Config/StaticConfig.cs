namespace Isotope.Code.Services.Config;

/// <summary>
/// Global configuration properties defined in appsettings.json.
/// </summary>
public class StaticConfig
{
    public DatabaseConfig Database { get; set; }
    public DebugConfig Debug { get; set; }
    public WebServerConfig WebServer { get; set; }
    public DemoModeConfig DemoMode { get; set; }
}

/// <summary>
/// Connection string properties.
/// </summary>
public class DatabaseConfig
{
    public string ConnectionString { get; set; }
}

/// <summary>
/// Properties related to debugging.
/// </summary>
public class DebugConfig
{
    public bool DetailedExceptions { get; set; }
    public int? Latency { get; set; }
}

/// <summary>
/// Webserver properties.
/// </summary>
public class WebServerConfig
{
    public bool RequireHttps { get; set; }
    public long? MaxUploadSize { get; set; }
}

/// <summary>
/// Demo mode configuration options.
/// </summary>
public class DemoModeConfig
{
    public bool Enabled { get; set; }
    public bool ClearOnStartup { get; set; }
    public bool SeedSampleData { get; set; }
    public string YandexMetrikaId { get; set; }
}