namespace DaikinNet;

public class Config
{
    public bool ReportWithSnakeCase { get; set; }
    public List<ReportEntry> ReportFields { get; set; }
    
    public class ReportEntry
    {
        public string SourceField { get; set; }
        public string ReportField { get; set; }
    }
}
