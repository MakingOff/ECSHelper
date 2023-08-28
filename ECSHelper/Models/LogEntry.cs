using System;

namespace ECSHelper.Models; 

public class LogEntry {
    private DateTime Date { get; set; }
    private string Content { get; set; }
    private bool IsSeparator { get; set; }

    public string Display => IsSeparator ? "" : $"{Date:dd/MM/yyyy HH:mm:ss} - {Content}";

    public LogEntry(string content) {
        Date = DateTime.Now;
        Content = content;
    }

    public LogEntry(bool isSeparator = true) {
        IsSeparator = true;
    }
}