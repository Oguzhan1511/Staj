namespace kitap.Models;

public class Book
{
    public int Id { get; set;}
    public string KitapAdi { get; set;} = string.Empty;
    public string KitapYazari { get; set;} = string.Empty;
    public string YayinEvi { get; set;} = string.Empty;
    public int YayinYili { get; set; } 

}
