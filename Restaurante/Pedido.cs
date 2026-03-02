using Restaurante.Protos;

namespace Restaurante;

public class Pedido
{
    public string Id { get; private set; }
    public List<PlatoPedido> Platos { get; private set; }
    public string Estado { get; private set; }
    public int TiempoEstimado { get; private set; }

    public Pedido(int _numPedidos,List<PlatoPedido> platos, int distancia)
    {
        Id = _numPedidos.ToString();
        Platos = platos;
        Estado = "Pendiente";
        TiempoEstimado = distancia;
    }


    public void ActualizarEstado(string nuevoEstado)
    {
        Estado = nuevoEstado;        
    }

    public override string ToString()
    {
        return $"Pedido {Id} - Estado: {Estado} - Tiempo estimado: {TiempoEstimado} minutos";
    }
}
