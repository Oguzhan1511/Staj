namespace kitap.Models;

public class Book
{
    
    public int Id { get; set;}
    public string Title { get; set;} = string.Empty;
    public string Author { get; set;} = string.Empty;
    public string Publisher { get; set;} = string.Empty;
    public int PublisherYear { get; set; } 

}
