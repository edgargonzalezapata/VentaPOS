# VentaPOS

Sistema de Punto de Venta (POS) con gestión de suscripciones y planes.

## Características

- Gestión de usuarios y roles
- Sistema de suscripciones y planes
- Gestión de inventario
- Punto de venta
- Gestión de clientes
- Reportes y estadísticas

## Requisitos

- .NET 7.0 o superior
- SQL Server 2019 o superior
- Visual Studio 2022 o superior

## Instalación

1. Clonar el repositorio
```bash
git clone https://github.com/edgargonzalezapata/VentaPOS.git
```

2. Restaurar los paquetes NuGet
```bash
dotnet restore
```

3. Actualizar la base de datos
```bash
dotnet ef database update
```

4. Ejecutar la aplicación
```bash
dotnet run
```

## Configuración

1. Actualizar la cadena de conexión en `appsettings.json`
2. Configurar los datos de la empresa en el panel de administración
3. Crear usuarios y asignar roles

## Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles. 