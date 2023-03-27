namespace CryptoFinderLib.Models;

public class Information
{
    /// <summary>
    /// Ip Адрес
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// Страна
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// Локация
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Зип код
    /// </summary>
    public string? ZipCode { get; set; }
    
    /// <summary>
    /// Язык ПК
    /// </summary>
    public string? Language { get; set; }
    
    /// <summary>
    /// Операционная система
    /// </summary>
    public string? OperationSystem { get; set; }
    
    /// <summary>
    /// Дата лога
    /// </summary>
    public string? LogDate { get; set; }
}