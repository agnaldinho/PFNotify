using System.Collections.Generic;

public class RelatorioPronto
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Query { get; set; }

    public List<Parametro> Parametros { get; set; } = new List<Parametro>();
    public List<Campo> Campos { get; set; } = new List<Campo>();

    public bool PermiteExcel { get; set; }
    public bool Agrupado { get; set; }
}
public class Parametro
{
    public string Nome { get; set; }
    public string Tipo { get; set; }
    public string Label { get; set; }
}

public class Campo
{
    public string Nome { get; set; }
    public string Titulo { get; set; }
    public string Tipo { get; set; }
}