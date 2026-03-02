using Grpc.Net.Client;
using Restaurante.Protos;

namespace Cliente.Services;


public class RestauranteClientService
{
    private readonly string _serverAddress;
    private GrpcChannel? _channel;
    private RestauranteService.RestauranteServiceClient? _client;

    public RestauranteClientService(string serverAddress = "http://localhost:5000")
    {
        _serverAddress = serverAddress;
    }

    public async Task ConnectAsync()
    {
        // If using unencrypted HTTP/2 (http://) enable the switch (only for development)
        if (_serverAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
        // Prepare channel options.
        var channelOptions = new GrpcChannelOptions();
        //En el fichero proporcionado podéis comprobar que un apartado para conectar si usamos https (no requerido en esta práctica).

        _channel = GrpcChannel.ForAddress(_serverAddress, channelOptions);
        _client = new RestauranteService.RestauranteServiceClient(_channel);
        Console.WriteLine($"Conectado al servidor en {_serverAddress}");

    }

    public async Task DisconnectAsync()
    {
        if (_channel != null)
            {
                await _channel.ShutdownAsync();
                Console.WriteLine("Desconectado del servidor");
            }

    }

    private void EnsureConnected()
    {
        if (_client == null)
        {
            throw new InvalidOperationException("No está conectado al servidor. Llame a ConnectAsync() primero.");
        }
    }

    public async Task<PedidoResponse> HacerPedidoAsync(List<(string nombre, int cantidad)> platos, int distancia)
    {
        EnsureConnected(); // No eliminar
        PedidoRequest MiPedido = new PedidoRequest {Distancia = distancia};
        
        for (int i = 0; i < platos.Count; i++)
        {
            PlatoPedido ThisPlatoPedido = new PlatoPedido{Nombre = platos[i].nombre, Cantidad = platos[i].cantidad};
            MiPedido.Platos.Add(ThisPlatoPedido);
        }

        try{
            var respuesta = await _client!.HacerPedidoAsync(MiPedido);
            Console.WriteLine($"Pedido realizado. ID: {respuesta.IdPedido}, Mensaje: {respuesta.Mensaje}");
            return respuesta;
        
        }
        catch (Exception ex){
            Console.WriteLine($"Error al realizar el pedido: {ex.Message}");
            throw;

        }
        
    }

    public async Task<EstadoResponse> ConsultarEstadoPedidoAsync(string idPedido)
    {
        EnsureConnected();
        ConsultaRequest MiConsulta = new ConsultaRequest {IdPedido = idPedido}; 
        try{
            var respuesta = await _client!.ConsultarEstadoPedidoAsync(MiConsulta);
            Console.WriteLine($"Estado: {respuesta.Estado}, Tiempo estimado; {respuesta.TiempoEstimado}");
            return respuesta;

        }
        catch (Exception ex){
            Console.WriteLine($"Error durante la consulta: {ex.Message}");
            throw;
        }
        
    }
}