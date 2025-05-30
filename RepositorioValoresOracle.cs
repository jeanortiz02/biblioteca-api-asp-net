using System;
using BibliotecaAPI.Entidades;

namespace BibliotecaAPI;

public class RepositorioValoresOracle : IRepositorioValores
{

    private List<Valor> _valores;

    public RepositorioValoresOracle()
    {
        _valores = new List<Valor>
        {
            new Valor { Id = 1, Nombre = "Jean"},
            new Valor { Id = 2, Nombre = "Jean 2"}
        };
    }
    public IEnumerable<Valor> ObtenerValores()
    {
        return _valores;
    }

    public void InsertarValor(Valor valor)
    {
        _valores.Add(valor);
    }
}
