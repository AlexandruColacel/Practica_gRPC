using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Restaurante.Protos;
using System.Collections.Concurrent;

namespace Restaurante.Services;

public class RestauranteService : Protos.RestauranteService.RestauranteServiceBase, IDisposable
{
    private readonly ILogger<RestauranteService> _logger;
    private readonly ConcurrentQueue<Pedido> _ordersQueue;

    private readonly ConcurrentQueue<int> _motoretaQueue; //Cola para esperar las motoretas
    private const int MaxQueueCapacity = 10;

    private const int MaxMotoreta = 2; //Maximo de motoretas

    private readonly SemaphoreSlim _readyOrdersSemaphore;

    private readonly SemaphoreSlim _readyMotoretaSemaphore; //semaforo motoretas
    private readonly object _takeLock;
    private int _orderID;

    public RestauranteService(ILogger<RestauranteService> logger)
    {
       _logger = logger;
       _ordersQueue = new ConcurrentQueue<Pedido>();
       _readyOrdersSemaphore = new SemaphoreSlim(0); 
       _takeLock = new object();
       _orderID = 1;
       //Inicializo las nuevas variables que he declaro en el constructor
       _motoretaQueue = new ConcurrentQueue<int>(new int[]  {1,2}); //id's de las motororetas a usae
       _readyMotoretaSemaphore = new SemaphoreSlim(MaxMotoreta); //Declaro a 2 el semaforo porque son el numero de motos que puedo coger
    }
    
    public override async Task<PedidoResponse> HacerPedido(PedidoRequest request, ServerCallContext context)
    {
        lock (_takeLock) 
        {
            if (_ordersQueue.Count < MaxQueueCapacity)
            {
                var platosLista = request.Platos.ToList();
                var nuevoPedido = new Pedido(_orderID, platosLista, request.Distancia);

                _ordersQueue.Enqueue(nuevoPedido);
                
                _logger.LogInformation($"Recibido pedido ID: {_orderID}");

                var response = new PedidoResponse
                {
                    IdPedido = nuevoPedido.Id,
                    Mensaje = "Pedido recibido exitosamente"
                };

                _orderID++;

                _readyOrdersSemaphore.Release();

                return Task.FromResult(response).Result;
            }
        }
        
        throw new RpcException(new Status(StatusCode.Internal, "Error al procesar el pedido porque la cola está llena"));
    }

    public override Task<EstadoResponse> ConsultarEstadoPedido(ConsultaRequest request, ServerCallContext context)
    {
        var pedido = _ordersQueue.FirstOrDefault(p => p.Id == request.IdPedido);

        if (pedido != null)
        {
            return Task.FromResult(new EstadoResponse
            {
                Estado = pedido.Estado,
                TiempoEstimado = pedido.TiempoEstimado,
                Mensaje = "Estado consultado correctamente"
            });
        }
        
        throw new RpcException(new Status(StatusCode.NotFound, "Pedido no encontrado"));
    }

    public override Task<PedidoResponse> ConsultarPlatosPedido(ConsultaRequest request, ServerCallContext context)
    {
        var pedido = _ordersQueue.FirstOrDefault(p => p.Id == request.IdPedido);

        if (pedido != null)
        {
            // Concatenamos los nombres de los platos para devolverlos en el mensaje
            string listaPlatos = string.Join(", ", pedido.Platos.Select(p => $"{p.Cantidad}x {p.Nombre}"));

            return Task.FromResult(new PedidoResponse
            {
                IdPedido = pedido.Id,
                Mensaje = $"Platos: {listaPlatos}"
            });
        }

        throw new RpcException(new Status(StatusCode.NotFound, "Pedido no encontrado"));
    }

public override async Task<PedidoResponse> TomarPedidoParaRepartir(Empty request, ServerCallContext context)
    {
        await _readyOrdersSemaphore.WaitAsync();

        lock (_takeLock) 
        {
            if (_ordersQueue.TryDequeue(out Pedido? pedido))
            {
                pedido.ActualizarEstado("En reparto"); 
                _logger.LogInformation($"Pedido {pedido.Id} tomado para reparto.");

                return new PedidoResponse
                {
                    IdPedido = pedido.Id,
                    Distancia = pedido.TiempoEstimado,
                    Mensaje = "Pedido asignado para reparto"
                    
                };
            }
        }
       
        throw new RpcException(new Status(StatusCode.Unavailable, "No hay pedidos disponibles"));
    }

    public override async Task<idMotoreta> CogerMotoreta(Empty request, ServerCallContext context)
    {
        await _readyMotoretaSemaphore.WaitAsync(); //para poder usar semafora wait semfro
        int idMotoretaScada;
        bool hayMotitoChula = _motoretaQueue.TryDequeue(out idMotoretaScada);//AQUI VEO SI PUEDO SACAR UNA MOTORETA Y SI SI EN TRYDEQUEUE OUT IDMotoretaScada lo mete 
        
        
        if(hayMotitoChula)
        {
            
            return new idMotoreta
            {
                Id = idMotoretaScada
            };
        }

        throw new RpcException(new Status(StatusCode.Internal, "INCONSISTENTEEE SINVERGUENZAAAAA"));

        
    }

    public override async Task<Empty> DejarMotoreta(idMotoreta request,ServerCallContext context)
    {
        _logger.LogInformation("Motoreta devuelta");
        _motoretaQueue.Enqueue(request.Id); //EL ID DE LA MOTORETE ESTA EN IDMOTORETA REQUEST PUES DEL REQUEST SACAMOS EL ID
        _readyMotoretaSemaphore.Release();
        return new Empty(); 

    }
    public void Dispose()
    { 
       _readyOrdersSemaphore?.Dispose();
    }
    



}