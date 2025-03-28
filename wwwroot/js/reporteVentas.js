// Función para mostrar detalles de venta
function mostrarDetalles(ventaId) {
    // Mostrar el modal con spinner de carga
    $('#detallesModal').modal('show');
    
    // Cargar los detalles mediante AJAX
    fetch(`/api/VentasApi/Detalles/${ventaId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Error al cargar los detalles');
            }
            return response.json();
        })
        .then(data => {
            // Construir el contenido HTML con los detalles
            let contenido = `
                <div class="mb-4">
                    <div class="row">
                        <div class="col-md-6">
                            <h6 class="font-weight-bold">Información de Venta</h6>
                            <p><strong>ID Venta:</strong> ${data.ventaID}</p>
                            <p><strong>Fecha:</strong> ${new Date(data.fecha).toLocaleString()}</p>
                            <p><strong>Cliente:</strong> ${data.cliente?.nombre || 'Cliente no registrado'}</p>
                        </div>
                        <div class="col-md-6">
                            <h6 class="font-weight-bold">Resumen</h6>
                            <p><strong>Subtotal:</strong> $${(data.total - (data.impuestos || 0) + (data.descuento || 0)).toLocaleString('es-CL')}</p>
                            <p><strong>Impuestos:</strong> $${(data.impuestos || 0).toLocaleString('es-CL')}</p>
                            <p><strong>Descuento:</strong> $${(data.descuento || 0).toLocaleString('es-CL')}</p>
                            <p><strong>Total:</strong> $${data.total.toLocaleString('es-CL')}</p>
                        </div>
                    </div>
                </div>
                
                <h6 class="font-weight-bold">Productos</h6>
                <div class="table-responsive">
                    <table class="table table-bordered table-striped">
                        <thead class="table-dark">
                            <tr>
                                <th>Producto</th>
                                <th class="text-center">Cantidad</th>
                                <th class="text-end">Precio</th>
                                <th class="text-end">Subtotal</th>
                            </tr>
                        </thead>
                        <tbody>`;
            
            data.productosVendidos.forEach(p => {
                contenido += `
                    <tr>
                        <td>${p.nombre}</td>
                        <td class="text-center">${p.cantidad}</td>
                        <td class="text-end">$${p.precio.toLocaleString('es-CL')}</td>
                        <td class="text-end">$${p.subtotal.toLocaleString('es-CL')}</td>
                    </tr>`;
            });
            
            contenido += `
                        </tbody>
                        <tfoot>
                            <tr class="table-secondary">
                                <td colspan="3" class="text-end fw-bold">Total:</td>
                                <td class="text-end fw-bold">$${data.total.toLocaleString('es-CL')}</td>
                            </tr>
                        </tfoot>
                    </table>
                </div>`;
            
            document.getElementById('detallesVentaContenido').innerHTML = contenido;
        })
        .catch(error => {
            console.error('Error:', error);
            document.getElementById('detallesVentaContenido').innerHTML = `
                <div class="alert alert-danger">
                    <i class="bi bi-exclamation-triangle-fill me-2"></i>
                    Error al cargar los detalles de la venta. Por favor, intente nuevamente.
                </div>`;
        });
}

// Función para imprimir los detalles
function imprimirDetalles() {
    const contenido = document.getElementById('detallesVentaContenido').innerHTML;
    const ventanaImpresion = window.open('', '_blank');
    ventanaImpresion.document.write(`
        <!DOCTYPE html>
        <html>
        <head>
            <title>Detalles de Venta</title>
            <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
            <style>
                body { padding: 20px; }
                @media print {
                    .no-print { display: none; }
                }
            </style>
        </head>
        <body>
            <div class="container">
                <div class="row mb-4">
                    <div class="col-12 text-center">
                        <h3>Detalles de Venta</h3>
                    </div>
                </div>
                ${contenido}
                <div class="row mt-4 no-print">
                    <div class="col-12 text-center">
                        <button onclick="window.print()" class="btn btn-primary">Imprimir</button>
                        <button onclick="window.close()" class="btn btn-secondary ms-2">Cerrar</button>
                    </div>
                </div>
            </div>
        </body>
        </html>
    `);
    ventanaImpresion.document.close();
}

// Función para exportar a Excel
function exportTableToExcel(tableID, filename = 'reporte_ventas') {
    var downloadLink;
    var dataType = 'application/vnd.ms-excel';
    var tableSelect = document.getElementById(tableID);
    var tableHTML = tableSelect.outerHTML.replace(/ /g, '%20');
    
    filename = filename + '_' + new Date().toISOString().slice(0,10) + '.xls';
    
    downloadLink = document.createElement("a");
    document.body.appendChild(downloadLink);
    
    if(navigator.msSaveOrOpenBlob){
        var blob = new Blob(['\ufeff', tableHTML], {
            type: dataType
        });
        navigator.msSaveOrOpenBlob(blob, filename);
    } else {
        downloadLink.href = 'data:' + dataType + ', ' + tableHTML;
        downloadLink.download = filename;
        downloadLink.click();
    }
}

// Función para imprimir
function printReport() {
    window.print();
}

// Función para confirmar eliminación
function confirmarEliminar(ventaId) {
    // Guardar el ID de la venta en una variable global
    window.ventaIdAEliminar = ventaId;
    
    // Mostrar el modal de confirmación
    $('#eliminarModal').modal('show');
}

// Función para eliminar la venta mediante AJAX
function eliminarVenta() {
    const ventaId = window.ventaIdAEliminar;
    
    // Mostrar indicador de carga en el botón
    const botonEliminar = document.querySelector('#eliminarModal .btn-danger');
    const textoOriginal = botonEliminar.innerHTML;
    botonEliminar.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Eliminando...';
    botonEliminar.disabled = true;
    
    // Obtener el token CSRF
    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const token = tokenElement ? tokenElement.value : '';
    
    // Realizar la petición AJAX para eliminar
    fetch('/Ventas/Delete', {
        method: 'POST',
        headers: {
            'RequestVerificationToken': token,
            'X-Requested-With': 'XMLHttpRequest',
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: 'id=' + ventaId
    })
    .then(async response => {
        const contentType = response.headers.get('content-type');
        let data;
        
        if (contentType && contentType.includes('application/json')) {
            data = await response.json();
        } else {
            const text = await response.text();
            try {
                data = JSON.parse(text);
            } catch {
                throw new Error(`Respuesta inválida del servidor: ${text.substring(0, 100)}`);
            }
        }

        if (!response.ok) {
            throw new Error(data.message || `Error HTTP ${response.status}`);
        }
        return data;
    })
    .then(data => {
        // Verificar si la operación fue exitosa
        if (data.success) {
            // Cerrar el modal
            $('#eliminarModal').modal('hide');
            
            // Eliminar la fila de la tabla
            const filaVenta = document.querySelector(`tr[data-venta-id="${ventaId}"]`);
            if (filaVenta) {
                filaVenta.remove();
            } else {
                // Si no se puede encontrar la fila específica, recargar la página
                window.location.reload();
            }
            
            // Mostrar mensaje de éxito
            mostrarNotificacion('Venta eliminada correctamente', 'success');
            
            // Actualizar los totales y contadores
            actualizarEstadisticas();
        } else {
            throw new Error(data.message || 'Error al eliminar la venta');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        // Restaurar el botón
        botonEliminar.innerHTML = textoOriginal;
        botonEliminar.disabled = false;
        
        // Mostrar mensaje de error
        mostrarNotificacion('Error al eliminar la venta. Por favor, inténtelo de nuevo.', 'danger');
        
        // Cerrar el modal después de mostrar el error
        setTimeout(() => {
            $('#eliminarModal').modal('hide');
        }, 2000);
    });
}

// Función para actualizar estadísticas después de eliminar
function actualizarEstadisticas() {
    // Si hay pocas ventas, es más fácil recargar la página
    if (ventasData.length <= 5) {
        window.location.reload();
        return;
    }
    
    // Actualizar contadores y totales
    const ventaEliminada = ventasData.find(v => v.ventaID === window.ventaIdAEliminar);
    if (!ventaEliminada) return;
    
    // Eliminar la venta de los datos
    ventasData = ventasData.filter(v => v.ventaID !== window.ventaIdAEliminar);
    
    // Actualizar totales
    const totalVentas = ventasData.reduce((sum, v) => sum + v.total, 0);
    const cantidadVentas = ventasData.length;
    const productosVendidos = ventasData.reduce((sum, v) => sum + v.productosVendidos.length, 0);
    const promedioVenta = cantidadVentas > 0 ? totalVentas / cantidadVentas : 0;
    
    // Actualizar los elementos en la página
    document.querySelector('.card-body .text-primary.text-uppercase').nextElementSibling.textContent = 
        '$' + totalVentas.toLocaleString('es-CL');
    
    document.querySelector('.card-body .text-success.text-uppercase').nextElementSibling.textContent = 
        cantidadVentas.toString();
    
    document.querySelector('.card-body .text-info.text-uppercase').nextElementSibling.textContent = 
        productosVendidos.toString();
    
    document.querySelector('.card-body .text-warning.text-uppercase').nextElementSibling.textContent = 
        '$' + promedioVenta.toLocaleString('es-CL', {maximumFractionDigits: 0});
    
    // Actualizar totales en la tabla
    const tfoot = document.querySelector('#reportTable tfoot');
    if (tfoot) {
        const cells = tfoot.querySelectorAll('td');
        if (cells.length >= 8) {
            const subtotal = ventasData.reduce((sum, v) => sum + (v.total - (v.impuestos || 0) + (v.descuento || 0)), 0);
            const impuestos = ventasData.reduce((sum, v) => sum + (v.impuestos || 0), 0);
            const descuento = ventasData.reduce((sum, v) => sum + (v.descuento || 0), 0);
            
            cells[4].textContent = '$' + subtotal.toLocaleString('es-CL');
            cells[5].textContent = '$' + impuestos.toLocaleString('es-CL');
            cells[6].textContent = '$' + descuento.toLocaleString('es-CL');
            cells[7].textContent = '$' + totalVentas.toLocaleString('es-CL');
        }
    }
}

// Función para mostrar notificaciones
function mostrarNotificacion(mensaje, tipo) {
    // Crear elemento de notificación
    const notificacion = document.createElement('div');
    notificacion.className = `alert alert-${tipo} alert-dismissible fade show`;
    notificacion.setAttribute('role', 'alert');
    notificacion.innerHTML = `
        ${mensaje}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    // Agregar al contenedor de notificaciones o al cuerpo del documento
    const contenedorNotificaciones = document.getElementById('notificaciones') || document.querySelector('.container-fluid');
    contenedorNotificaciones.prepend(notificacion);
    
    // Auto-cerrar después de 5 segundos
    setTimeout(() => {
        notificacion.classList.remove('show');
        setTimeout(() => notificacion.remove(), 300);
    }, 5000);
}

// Inicializar gráficos cuando el documento esté listo
document.addEventListener('DOMContentLoaded', function() {
    try {
        // Agrupar ventas por día
        const ventasPorDia = {};
        ventasData.forEach(v => {
            const fecha = v.fecha.split('T')[0];
            if (!ventasPorDia[fecha]) {
                ventasPorDia[fecha] = 0;
            }
            ventasPorDia[fecha] += v.total;
        });
        
        // Preparar datos para Chart.js
        const labels = Object.keys(ventasPorDia).sort();
        const data = labels.map(fecha => ventasPorDia[fecha]);
        
        // Crear gráfico de línea para ventas por día
        const ctxLine = document.getElementById('ventasPorDiaChart').getContext('2d');
        new Chart(ctxLine, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Ventas por Día',
                    data: data,
                    backgroundColor: 'rgba(78, 115, 223, 0.05)',
                    borderColor: 'rgba(78, 115, 223, 1)',
                    pointRadius: 3,
                    pointBackgroundColor: 'rgba(78, 115, 223, 1)',
                    pointBorderColor: 'rgba(78, 115, 223, 1)',
                    pointHoverRadius: 5,
                    pointHoverBackgroundColor: 'rgba(78, 115, 223, 1)',
                    pointHoverBorderColor: 'rgba(78, 115, 223, 1)',
                    pointHitRadius: 10,
                    pointBorderWidth: 2,
                    tension: 0.3
                }]
            },
            options: {
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return '$' + value.toLocaleString('es-CL', { maximumFractionDigits: 0 });
                            }
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
        
        // Productos más vendidos
        const productosCantidad = {};
        ventasData.forEach(v => {
            v.productosVendidos.forEach(p => {
                if (!productosCantidad[p.nombre]) {
                    productosCantidad[p.nombre] = 0;
                }
                productosCantidad[p.nombre] += p.cantidad;
            });
        });
        
        // Ordenar y tomar los 5 más vendidos
        const productosOrdenados = Object.entries(productosCantidad)
            .sort((a, b) => b[1] - a[1])
            .slice(0, 5);
        
        const labelsPie = productosOrdenados.map(p => p[0]);
        const dataPie = productosOrdenados.map(p => p[1]);
        
        // Crear gráfico de pie para productos más vendidos
        const ctxPie = document.getElementById('productosMasVendidosChart').getContext('2d');
        new Chart(ctxPie, {
            type: 'doughnut',
            data: {
                labels: labelsPie,
                datasets: [{
                    data: dataPie,
                    backgroundColor: [
                        '#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b'
                    ],
                    hoverBackgroundColor: [
                        '#2e59d9', '#17a673', '#2c9faf', '#dda20a', '#be2617'
                    ],
                    hoverBorderColor: "rgba(234, 236, 244, 1)",
                }]
            },
            options: {
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'right'
                    }
                },
                cutout: '70%'
            }
        });
    } catch (error) {
        console.error("Error al inicializar gráficos:", error);
    }
});

// Agregar esta función al final del archivo
function filtrarVentasHoy() {
    // Obtener la fecha actual en formato YYYY-MM-DD
    const hoy = new Date().toISOString().split('T')[0];
    
    // Filtrar las ventas del día actual
    const ventasHoy = ventasData.filter(v => v.fecha.split('T')[0] === hoy);
    
    // Actualizar la tabla
    const tbody = document.querySelector('#reportTable tbody');
    tbody.innerHTML = '';
    
    if (ventasHoy.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="8" class="text-center">No hay ventas registradas hoy</td>
            </tr>`;
        return;
    }
    
    // Actualizar la tabla con las ventas de hoy
    ventasHoy.forEach(venta => {
        const fecha = new Date(venta.fecha).toLocaleString();
        const subtotal = venta.total - (venta.impuestos || 0) + (venta.descuento || 0);
        
        tbody.innerHTML += `
            <tr data-venta-id="${venta.ventaID}">
                <td>${venta.ventaID}</td>
                <td>${fecha}</td>
                <td>${venta.cliente?.nombre || 'Cliente no registrado'}</td>
                <td>${venta.productosVendidos.length}</td>
                <td class="text-end">$${subtotal.toLocaleString('es-CL')}</td>
                <td class="text-end">$${(venta.impuestos || 0).toLocaleString('es-CL')}</td>
                <td class="text-end">$${(venta.descuento || 0).toLocaleString('es-CL')}</td>
                <td class="text-end">$${venta.total.toLocaleString('es-CL')}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-info" onclick="mostrarDetalles(${venta.ventaID})">
                        <i class="fas fa-eye"></i>
                    </button>
                    <button class="btn btn-sm btn-danger" onclick="confirmarEliminar(${venta.ventaID})">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>`;
    });
    
    // Actualizar totales en el pie de la tabla
    const totalVentas = ventasHoy.reduce((sum, v) => sum + v.total, 0);
    const subtotalVentas = ventasHoy.reduce((sum, v) => sum + (v.total - (v.impuestos || 0) + (v.descuento || 0)), 0);
    const totalImpuestos = ventasHoy.reduce((sum, v) => sum + (v.impuestos || 0), 0);
    const totalDescuentos = ventasHoy.reduce((sum, v) => sum + (v.descuento || 0), 0);
    
    const tfoot = document.querySelector('#reportTable tfoot');
    if (tfoot) {
        const cells = tfoot.querySelectorAll('td');
        if (cells.length >= 8) {
            cells[4].textContent = '$' + subtotalVentas.toLocaleString('es-CL');
            cells[5].textContent = '$' + totalImpuestos.toLocaleString('es-CL');
            cells[6].textContent = '$' + totalDescuentos.toLocaleString('es-CL');
            cells[7].textContent = '$' + totalVentas.toLocaleString('es-CL');
        }
    }
    
    // Actualizar título de la página
    const titulo = document.querySelector('h1.h3') || document.querySelector('h1');
    if (titulo) {
        titulo.textContent = 'Ventas de Hoy';
    }
    
    // Mostrar notificación
    mostrarNotificacion(`Mostrando ${ventasHoy.length} ventas del día de hoy`, 'info');
}

// Agregar el event listener cuando el documento esté listo
document.addEventListener('DOMContentLoaded', function() {
    // ... código existente ...
    
    // Agregar el event listener para el botón de ventas de hoy
    const ventasHoyBtn = document.getElementById('ventasHoyBtn');
    if (ventasHoyBtn) {
        ventasHoyBtn.addEventListener('click', function(e) {
            e.preventDefault();
            filtrarVentasHoy();
        });
    }
}); 