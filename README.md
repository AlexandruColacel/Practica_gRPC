# Desarrollo de Aplicaciones Distribuidas con gRPC 🚀🍔
Este repositorio contiene la implementación de un proyecto simula un sistema distribuido para la gestión online de pedidos del restaurante "Distributed Systems and Fun", desarrollado en C# (.NET) utilizando el framework de comunicación gRPC. Además, incluye la extensión del Proyecto Final, implementando un mecanismo de gestión concurrente de ciclomotores para los repartidores.

## 📌 Descripción del Proyecto
El objetivo principal del sistema es gestionar la concurrencia y la comunicación entre diferentes actores en un entorno distribuido. Se aplican conceptos teóricos fundamentales como:

  * **`RPC (Remote Procedure Calls)`:** Comunicación eficiente y fuertemente tipada entre cliente y servidor usando HTTP/2 y Protocol Buffers (protobuf).

  * **`Cerrojos (Locks) y Sección Crítica`:** Uso de lock para garantizar la exclusión mutua al acceder a recursos compartidos (la cola de pedidos).

  * **`Semáforos (SemaphoreSlim)`:** Coordinación entre procesos productores (clientes/restaurante) y consumidores (repartidores).

  * **`Colas Concurrentes (ConcurrentQueue)`:** Estructura de datos Thread-Safe para almacenar los pedidos en curso.

  * **`Gestión de Recursos Compartidos (Pools)`:** Control de una flota limitada de ciclomotores utilizando SemaphoreSlim y ConcurrentQueue para evitar condiciones de carrera entre múltiples repartidores.

## 🏗️ Arquitectura y Roles
El sistema se divide en 3 aplicaciones de consola independientes:


**Restaurante (Servidor):** Expone el servicio gRPC (RestauranteService).

  * Mantiene el estado global de la aplicación.

  * Gestiona una cola concurrente de pedidos con un límite de capacidad.

  * Avisa a los repartidores cuando hay pedidos listos usando semáforos.
  
  * Gestiona una cola concurrente de ciclomotores (limitados a 2) y controla su asignación y devolución mediante un semáforo independiente.
    

**Cliente (Productor):** Aplicación interactiva que permite a los usuarios hacer nuevos pedidos indicando los platos y la distancia.

  * Permite consultar el estado y el tiempo estimado de un pedido mediante su ID.
    

**Repartidor (Consumidor):**  Aplicación que se queda a la espera (WaitAsync) hasta que el Restaurante libera un pedido.

  * Una vez asignado, extrae el pedido de la cola, actualiza su estado a "En reparto" y muestra la distancia a recorrer.
  
  * Una vez que obtiene un pedido, solicita un ciclomotor. Si no hay motos disponibles, se queda a la espera.

  * Simula el tiempo de viaje según la distancia del pedido (Task.Delay) y, al terminar, devuelve la moto al restaurante para que otro repartidor pueda usarla.

## 🛠️ Tecnologías Utilizadas
Lenguaje: C# 13 / .NET 9.0

  * **Framework RPC:** gRPC (Grpc.AspNetCore, Grpc.Net.Client)

  * **Serialización de datos:** Protocol Buffers (.proto)

  * **Contenedores:** Docker / Docker Compose (Configurados para el despliegue)

## ⚙️ Requisitos Previos
Para ejecutar este proyecto en local necesitas tener instalado:

  * .NET SDK [(Versión compatible con tu proyecto, ej. .NET 8 o 9)](https://dotnet.microsoft.com/download/dotnet/9.0).

  * (Opcional) Docker Desktop si deseas levantar los servicios mediante los archivos docker-compose.yml y Dockerfile suministrados.
