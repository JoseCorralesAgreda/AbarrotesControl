namespace AbarrotesControl.Core.DTOs;

public class ResumenAlertasDto
{
    public int Total { get; set; }
    public int SinStock { get; set; }
    public int StockBajo { get; set; }
    public int PorVencer { get; set; }
    public int Vencidos { get; set; }
    public List<AlertaStockDto> Items { get; set; } = [];
}
