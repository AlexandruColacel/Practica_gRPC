using System.Threading.Channels;
using Grpc.Net.Client;
using Repartidor.Protos;
using Google.Protobuf.WellKnownTypes; // Necesario para 'Empty'

namespace Repartidor;

class Program
{
    static async Task Main(string[] args)
    {
       //To be completed
       // Reemplaza las dos siguientes líneas con la implementación propuesta en el guión, ya que
       // solo se han creados estas dos variables para proporcionar el proyecto base libre de errores

       string urlRestaurante = "http://host.docker.internal:5000"; 
       //Configurar para permitir HTTP/2 sin TLS
         var httpHandler = new HttpClientHandler();
        httpHandler.ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        var channel = GrpcChannel.ForAddress(   urlRestaurante, 
                                                new GrpcChannelOptions 
                                                { 
                                                    HttpHandler = httpHandler 
                                                }
                                            );
        var client = new RestauranteService.RestauranteServiceClient(channel);


        Console.WriteLine("Cliente gRPC del Repartidor");
        Console.WriteLine("1. Tomar pedido para repartir");
        Console.WriteLine("2. Salir");

        while (true)
        {
            Console.Write("\nElige una opción (1-2): ");
            var opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    await TomarPedidoParaRepartir(client);
                    break;
                case "2":
                    return;
                default:
                    Console.WriteLine("Opción no válida");
                    break;
            }
        }
    }

    static async Task TomarPedidoParaRepartir(RestauranteService.RestauranteServiceClient client)
    {
        try{
           
            Console.WriteLine("Solicitando pedido al restaurante...");

            // 1. Llamar al método remoto. 
            var response = await client.TomarPedidoParaRepartirAsync(new Empty());

           

            // 2. Mostrar en pantalla el ID del pedido recibido 
            Console.WriteLine($"¡Pedido Recibido! ID: {response.IdPedido} y ahora voy a pillar una moto ");

            var motito = await client.CogerMotoretaAsync(new Empty());

            Console.WriteLine($"Moto pillada!!!! IDmotito: {motito.Id} y ahora voy a repartir");

            await Task.Delay(response.Distancia * 1000); //espero el tiempo 1seg por cada km de distancia


            var motodeja = await client.DejarMotoretaAsync(motito);

            Console.WriteLine($"Moto con el id:{motito.Id} dejado");

            // 3. Mostrar la distancia 
            Console.WriteLine($"Distancia a recorrer: {response.Distancia} km");



        }






        catch(Exception ex){
            Console.WriteLine($"Error al intentar tomar un pedido: {ex.Message}");
        
            // Esperar un poco antes de reintentar si hubo error
            await Task.Delay(5000);
        }
        
    }
}