using System;
using BibliotecaAPI.Entidades;

namespace BibliotecaAPI;

public interface IRepositorioValores
{
    IEnumerable<Valor> ObtenerValores();
    void InsertarValor(Valor valor);
}
