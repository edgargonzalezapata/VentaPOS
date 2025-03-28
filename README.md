# VentaPOS

![.NET](https://img.shields.io/badge/.NET%207.0-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white)
![Tailwind](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge&logo=tailwind-css&logoColor=white)

Sistema de Punto de Venta (POS) moderno y eficiente con gestión de suscripciones y planes. Diseñado para pequeñas y medianas empresas que necesitan una solución completa para gestionar sus ventas, inventario y clientes.

## 🚀 Características

### Gestión de Usuarios y Roles
- Sistema de autenticación y autorización robusto
- Roles personalizables con permisos granulares
- Gestión de perfiles de usuario

### Sistema de Suscripciones y Planes
- Planes personalizables con diferentes niveles de acceso
- Gestión de pagos y renovaciones
- Historial de suscripciones
- Notificaciones de vencimiento

### Gestión de Inventario
- Control de stock en tiempo real
- Alertas de bajo stock
- Gestión de proveedores
- Historial de movimientos
- Categorización de productos

### Punto de Venta
- Interfaz intuitiva y rápida
- Múltiples formas de pago
- Descuentos y promociones
- Impresión de tickets
- Modo offline

### Gestión de Clientes
- Base de datos de clientes
- Historial de compras
- Sistema de fidelización
- Gestión de créditos

### Reportes y Estadísticas
- Dashboard interactivo
- Reportes personalizables
- Exportación en múltiples formatos
- Análisis de ventas
- Tendencias y proyecciones

## 📋 Requisitos

### Software
- .NET 7.0 o superior
- SQL Server 2019 o superior
- Visual Studio 2022 o superior

### Hardware Recomendado
- Procesador: 2.0 GHz o superior
- RAM: 4GB mínimo, 8GB recomendado
- Espacio en disco: 500MB para la aplicación

## 🔧 Instalación

1. **Clonar el repositorio**
```bash
git clone https://github.com/edgargonzalezapata/VentaPOS.git
cd VentaPOS
```

2. **Restaurar los paquetes NuGet**
```bash
dotnet restore
```

3. **Configurar la base de datos**
- Actualizar la cadena de conexión en `appsettings.json`
- Ejecutar las migraciones:
```bash
dotnet ef database update
```

4. **Ejecutar la aplicación**
```bash
dotnet run
```

## ⚙️ Configuración

### Configuración Inicial
1. Actualizar la cadena de conexión en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=VentaPOS;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

2. Configurar los datos de la empresa:
   - Acceder al panel de administración
   - Ir a "Configuración > Empresa"
   - Completar la información requerida

### Configuración de Usuarios
1. Crear el usuario administrador inicial
2. Configurar roles y permisos
3. Crear usuarios adicionales según sea necesario

## 🔒 Seguridad

- Autenticación basada en JWT
- Encriptación de datos sensibles
- Protección contra CSRF
- Validación de entrada de datos
- Registro de auditoría

## 🤝 Contribuir

1. Fork el proyecto
2. Crear una rama para tu característica (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## 📝 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

## 📞 Soporte

- Documentación: [Wiki](https://github.com/edgargonzalezapata/VentaPOS/wiki)
- Problemas: [Issues](https://github.com/edgargonzalezapata/VentaPOS/issues)
- Email: soporte@ventapos.com

## ✨ Agradecimientos

- [Microsoft .NET](https://dotnet.microsoft.com/)
- [Bootstrap](https://getbootstrap.com/)
- [Tailwind CSS](https://tailwindcss.com/)
- [Font Awesome](https://fontawesome.com/) 