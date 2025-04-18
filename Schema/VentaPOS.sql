USE [VentaPOS]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 09-04-2025 11:10:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categorias]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categorias](
	[CategoriaID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Descripcion] [text] NULL,
	[EmpresaRut] [varchar](20) NOT NULL,
 CONSTRAINT [PK__Categori__F353C1C53DC7F524] PRIMARY KEY CLUSTERED 
(
	[CategoriaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clientes]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clientes](
	[ClienteID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Apellidos] [varchar](100) NULL,
	[Email] [varchar](100) NULL,
	[Telefono] [varchar](20) NULL,
	[UltimaCompra] [datetime] NULL,
	[EmpresaRut] [varchar](20) NOT NULL,
	[RutCliente] [varchar](10) NULL,
 CONSTRAINT [PK__Clientes__71ABD0A71131B070] PRIMARY KEY CLUSTERED 
(
	[ClienteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfiguracionEmpresa]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfiguracionEmpresa](
	[ConfiguracionID] [int] IDENTITY(1,1) NOT NULL,
	[EmpresaRut] [varchar](20) NOT NULL,
	[Clave] [varchar](50) NOT NULL,
	[Valor] [varchar](500) NOT NULL,
	[Descripcion] [varchar](200) NULL,
 CONSTRAINT [PK__Configur__9B95E0563E7C30A7] PRIMARY KEY CLUSTERED 
(
	[ConfiguracionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetallesVenta]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetallesVenta](
	[DetalleID] [int] IDENTITY(1,1) NOT NULL,
	[VentaID] [int] NOT NULL,
	[ProductoID] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[PrecioUnitario] [decimal](10, 2) NOT NULL,
	[Descuento] [decimal](10, 2) NULL,
	[Impuesto] [decimal](10, 2) NULL,
	[Subtotal] [decimal](10, 2) NOT NULL,
 CONSTRAINT [PK__Detalles__6E19D6FA3E480FBF] PRIMARY KEY CLUSTERED 
(
	[DetalleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Empresas]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Empresas](
	[Rut] [varchar](20) NOT NULL,
	[NombreEmpresa] [varchar](100) NOT NULL,
	[Direccion] [varchar](200) NULL,
	[Telefono] [varchar](20) NULL,
	[FechaRegistro] [datetime] NULL,
	[Activo] [bit] NULL,
	[Contraseña] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_Empresas] PRIMARY KEY CLUSTERED 
(
	[Rut] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Inventario]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Inventario](
	[InventarioID] [int] IDENTITY(1,1) NOT NULL,
	[ProductoID] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[Ubicacion] [varchar](100) NULL,
	[FechaActualizacion] [datetime] NULL,
	[EmpresaRut] [varchar](20) NOT NULL,
 CONSTRAINT [PK__Inventar__FB8A24B78F5A075C] PRIMARY KEY CLUSTERED 
(
	[InventarioID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Mesas]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Mesas](
	[MesaID] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [int] NOT NULL,
	[Capacidad] [int] NULL,
	[Estado] [varchar](20) NOT NULL,
	[Ubicacion] [varchar](100) NULL,
	[EmpresaRut] [varchar](20) NOT NULL,
 CONSTRAINT [PK_Mesas] PRIMARY KEY CLUSTERED 
(
	[MesaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Planes]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Planes](
	[PlanID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[MaxUsuarios] [int] NULL,
	[Precio] [int] NULL,
 CONSTRAINT [PK__Planes__755C22D75E7A22C7] PRIMARY KEY CLUSTERED 
(
	[PlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Productos]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Productos](
	[ProductoID] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [varchar](50) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Descripcion] [text] NULL,
	[Precio] [decimal](10, 2) NOT NULL,
	[Stock] [int] NOT NULL,
	[CategoriaID] [int] NOT NULL,
	[UltimaActualizacion] [datetime] NULL,
	[EmpresaRut] [varchar](20) NOT NULL,
 CONSTRAINT [PK__Producto__A430AE832C367D1B] PRIMARY KEY CLUSTERED 
(
	[ProductoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RolID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Descripcion] [varchar](200) NULL,
	[EmpresaRut] [varchar](20) NOT NULL,
 CONSTRAINT [PK__Roles__F92302D15CC78A2C] PRIMARY KEY CLUSTERED 
(
	[RolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Suscripciones]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suscripciones](
	[SuscripcionID] [int] IDENTITY(1,1) NOT NULL,
	[EmpresaRut] [varchar](20) NOT NULL,
	[PlanID] [int] NOT NULL,
	[FechaInicio] [datetime] NOT NULL,
	[FechaFin] [datetime] NOT NULL,
	[FormaPago] [varchar](50) NULL,
	[Pagado] [bit] NULL,
	[Activa] [bit] NOT NULL,
	[MaxUsuarios] [int] NOT NULL,
	[NumeroComprobante] [nvarchar](50) NULL,
 CONSTRAINT [PK__Suscripc__814D768B4ED74F72] PRIMARY KEY CLUSTERED 
(
	[SuscripcionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UsuarioRoles]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsuarioRoles](
	[UsuarioID] [int] NOT NULL,
	[RolID] [int] NOT NULL,
 CONSTRAINT [PK__UsuarioR__24AFD7B554E7B9DF] PRIMARY KEY CLUSTERED 
(
	[UsuarioID] ASC,
	[RolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[UsuarioID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Correo] [varchar](100) NOT NULL,
	[Telefono] [varchar](20) NULL,
	[Contraseña] [varbinary](max) NOT NULL,
	[FechaRegistro] [datetime] NOT NULL,
	[Activo] [bit] NOT NULL,
	[EmpresaRut] [varchar](20) NOT NULL,
 CONSTRAINT [PK__Usuarios__2B3DE798FCB1AA72] PRIMARY KEY CLUSTERED 
(
	[UsuarioID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UsuariosSuscripciones]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsuariosSuscripciones](
	[UsuarioID] [int] NOT NULL,
	[SuscripcionID] [int] NOT NULL,
 CONSTRAINT [PK__Usuarios__832930F0783A500D] PRIMARY KEY CLUSTERED 
(
	[UsuarioID] ASC,
	[SuscripcionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ventas]    Script Date: 09-04-2025 11:10:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ventas](
	[VentaID] [int] IDENTITY(1,1) NOT NULL,
	[UsuarioID] [int] NOT NULL,
	[ClienteID] [int] NULL,
	[NumeroFactura] [varchar](50) NULL,
	[FechaVenta] [datetime] NULL,
	[MetodoPago] [varchar](50) NULL,
	[Subtotal] [decimal](10, 2) NOT NULL,
	[Impuestos] [decimal](10, 2) NULL,
	[Descuento] [decimal](10, 2) NULL,
	[Total] [decimal](10, 2) NOT NULL,
	[Comentarios] [varchar](500) NULL,
	[EmpresaRut] [varchar](20) NOT NULL,
	[Estado] [varchar](50) NULL,
	[TipoVenta] [varchar](50) NULL,
	[MesaID] [int] NULL,
	[Propina] [decimal](10, 2) NULL,
	[MedioPago] [varchar](50) NULL,
 CONSTRAINT [PK__Ventas__5B41514CCDF474A8] PRIMARY KEY CLUSTERED 
(
	[VentaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DetallesVenta] ADD  DEFAULT ((0.0)) FOR [Descuento]
GO
ALTER TABLE [dbo].[DetallesVenta] ADD  DEFAULT ((0.0)) FOR [Impuesto]
GO
ALTER TABLE [dbo].[Empresas] ADD  DEFAULT (getdate()) FOR [FechaRegistro]
GO
ALTER TABLE [dbo].[Empresas] ADD  DEFAULT (CONVERT([bit],(1))) FOR [Activo]
GO
ALTER TABLE [dbo].[Inventario] ADD  DEFAULT (getdate()) FOR [FechaActualizacion]
GO
ALTER TABLE [dbo].[Mesas] ADD  DEFAULT ('Disponible') FOR [Estado]
GO
ALTER TABLE [dbo].[Productos] ADD  DEFAULT (getdate()) FOR [UltimaActualizacion]
GO
ALTER TABLE [dbo].[Suscripciones] ADD  DEFAULT (CONVERT([bit],(1))) FOR [Pagado]
GO
ALTER TABLE [dbo].[Suscripciones] ADD  DEFAULT (CONVERT([bit],(1))) FOR [Activa]
GO
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT (getdate()) FOR [FechaRegistro]
GO
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT (CONVERT([bit],(1))) FOR [Activo]
GO
ALTER TABLE [dbo].[Ventas] ADD  CONSTRAINT [DF__Ventas__FechaVen__5CD6CB2B]  DEFAULT (getdate()) FOR [FechaVenta]
GO
ALTER TABLE [dbo].[Ventas] ADD  CONSTRAINT [DF__Ventas__Impuesto__5DCAEF64]  DEFAULT ((0.0)) FOR [Impuestos]
GO
ALTER TABLE [dbo].[Ventas] ADD  CONSTRAINT [DF__Ventas__Descuent__5EBF139D]  DEFAULT ((0.0)) FOR [Descuento]
GO
ALTER TABLE [dbo].[Categorias]  WITH CHECK ADD  CONSTRAINT [FK_Categorias_Empresas] FOREIGN KEY([EmpresaRut])
REFERENCES [dbo].[Empresas] ([Rut])
GO
ALTER TABLE [dbo].[Categorias] CHECK CONSTRAINT [FK_Categorias_Empresas]
GO
ALTER TABLE [dbo].[Clientes]  WITH CHECK ADD  CONSTRAINT [FK_Clientes_Empresas] FOREIGN KEY([EmpresaRut])
REFERENCES [dbo].[Empresas] ([Rut])
GO
ALTER TABLE [dbo].[Clientes] CHECK CONSTRAINT [FK_Clientes_Empresas]
GO
ALTER TABLE [dbo].[ConfiguracionEmpresa]  WITH CHECK ADD  CONSTRAINT [FK_ConfiguracionEmpresa_Empresas] FOREIGN KEY([EmpresaRut])
REFERENCES [dbo].[Empresas] ([Rut])
GO
ALTER TABLE [dbo].[ConfiguracionEmpresa] CHECK CONSTRAINT [FK_ConfiguracionEmpresa_Empresas]
GO
ALTER TABLE [dbo].[DetallesVenta]  WITH CHECK ADD  CONSTRAINT [FK_DetallesVenta_Productos] FOREIGN KEY([ProductoID])
REFERENCES [dbo].[Productos] ([ProductoID])
GO
ALTER TABLE [dbo].[DetallesVenta] CHECK CONSTRAINT [FK_DetallesVenta_Productos]
GO
ALTER TABLE [dbo].[DetallesVenta]  WITH CHECK ADD  CONSTRAINT [FK_DetallesVenta_Ventas] FOREIGN KEY([VentaID])
REFERENCES [dbo].[Ventas] ([VentaID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DetallesVenta] CHECK CONSTRAINT [FK_DetallesVenta_Ventas]
GO
ALTER TABLE [dbo].[Inventario]  WITH CHECK ADD  CONSTRAINT [FK_Inventario_Empresas] FOREIGN KEY([EmpresaRut])
REFERENCES [dbo].[Empresas] ([Rut])
GO
ALTER TABLE [dbo].[Inventario] CHECK CONSTRAINT [FK_Inventario_Empresas]
GO
ALTER TABLE [dbo].[Inventario]  WITH CHECK ADD  CONSTRAINT [FK_Inventario_Productos] FOREIGN KEY([ProductoID])
REFERENCES [dbo].[Productos] ([ProductoID])
GO
ALTER TABLE [dbo].[Inventario] CHECK CONSTRAINT [FK_Inventario_Productos]
GO
ALTER TABLE [dbo].[Mesas]  WITH CHECK ADD  CONSTRAINT [FK_Mesas_Empresas] FOREIGN KEY([EmpresaRut])
REFERENCES [dbo].[Empresas] ([Rut])
GO
ALTER TABLE [dbo].[Mesas] CHECK CONSTRAINT [FK_Mesas_Empresas]
GO
ALTER TABLE [dbo].[Productos]  WITH CHECK ADD  CONSTRAINT [FK_Productos_Categorias] FOREIGN KEY([CategoriaID])
REFERENCES [dbo].[Categorias] ([CategoriaID])
GO
ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [FK_Productos_Categorias]
GO
ALTER TABLE [dbo].[Productos]  WITH CHECK ADD  CONSTRAINT [FK_Productos_Empresas] FOREIGN KEY([EmpresaRut])
REFERENCES [dbo].[Empresas] ([Rut])
GO
ALTER TABLE [dbo].[Productos] CHECK CONSTRAINT [FK_Productos_Empresas]
GO
ALTER TABLE [dbo].[Roles]  WITH CHECK ADD  CONSTRAINT [FK_Roles_Empresas] FOREIGN KEY([EmpresaRut])
REFERENCES [dbo].[Empresas] ([Rut])
GO
ALTER TABLE [dbo].[Roles] CHECK CONSTRAINT [FK_Roles_Empresas]
GO
ALTER TABLE [dbo].[Suscripciones]  WITH CHECK ADD  CONSTRAINT [FK_Suscripciones_Empresas] FOREIGN KEY([EmpresaRut])
REFERENCES [dbo].[Empresas] ([Rut])
GO
ALTER TABLE [dbo].[Suscripciones] CHECK CONSTRAINT [FK_Suscripciones_Empresas]
GO
ALTER TABLE [dbo].[Suscripciones]  WITH CHECK ADD  CONSTRAINT [FK_Suscripciones_Planes] FOREIGN KEY([PlanID])
REFERENCES [dbo].[Planes] ([PlanID])
GO
ALTER TABLE [dbo].[Suscripciones] CHECK CONSTRAINT [FK_Suscripciones_Planes]
GO
ALTER TABLE [dbo].[UsuarioRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsuarioRoles_Roles] FOREIGN KEY([RolID])
REFERENCES [dbo].[Roles] ([RolID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsuarioRoles] CHECK CONSTRAINT [FK_UsuarioRoles_Roles]
GO
ALTER TABLE [dbo].[UsuarioRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsuarioRoles_Usuarios] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuarios] ([UsuarioID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsuarioRoles] CHECK CONSTRAINT [FK_UsuarioRoles_Usuarios]
GO
ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_Usuarios_Empresas] FOREIGN KEY([EmpresaRut])
REFERENCES [dbo].[Empresas] ([Rut])
GO
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_Empresas]
GO
ALTER TABLE [dbo].[UsuariosSuscripciones]  WITH CHECK ADD  CONSTRAINT [FK_UsuariosSuscripciones_Suscripciones] FOREIGN KEY([SuscripcionID])
REFERENCES [dbo].[Suscripciones] ([SuscripcionID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsuariosSuscripciones] CHECK CONSTRAINT [FK_UsuariosSuscripciones_Suscripciones]
GO
ALTER TABLE [dbo].[UsuariosSuscripciones]  WITH CHECK ADD  CONSTRAINT [FK_UsuariosSuscripciones_Usuarios] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuarios] ([UsuarioID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsuariosSuscripciones] CHECK CONSTRAINT [FK_UsuariosSuscripciones_Usuarios]
GO
ALTER TABLE [dbo].[Ventas]  WITH CHECK ADD  CONSTRAINT [FK_Ventas_Clientes] FOREIGN KEY([ClienteID])
REFERENCES [dbo].[Clientes] ([ClienteID])
GO
ALTER TABLE [dbo].[Ventas] CHECK CONSTRAINT [FK_Ventas_Clientes]
GO
ALTER TABLE [dbo].[Ventas]  WITH CHECK ADD  CONSTRAINT [FK_Ventas_Empresas] FOREIGN KEY([EmpresaRut])
REFERENCES [dbo].[Empresas] ([Rut])
GO
ALTER TABLE [dbo].[Ventas] CHECK CONSTRAINT [FK_Ventas_Empresas]
GO
ALTER TABLE [dbo].[Ventas]  WITH CHECK ADD  CONSTRAINT [FK_Ventas_Mesas] FOREIGN KEY([MesaID])
REFERENCES [dbo].[Mesas] ([MesaID])
GO
ALTER TABLE [dbo].[Ventas] CHECK CONSTRAINT [FK_Ventas_Mesas]
GO
ALTER TABLE [dbo].[Ventas]  WITH CHECK ADD  CONSTRAINT [FK_Ventas_Usuarios] FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuarios] ([UsuarioID])
GO
ALTER TABLE [dbo].[Ventas] CHECK CONSTRAINT [FK_Ventas_Usuarios]
GO
