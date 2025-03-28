# Configuración de Tailwind CSS para VentaPOS

## Instalación 

Para utilizar Tailwind CSS en producción, se debe instalar y configurar como se indica a continuación:

1. Instalar Node.js y npm si no los tienes instalados: https://nodejs.org/

2. Instalar las dependencias necesarias:

```bash
npm install -D tailwindcss postcss autoprefixer
```

3. Inicializar tailwind (ya se ha creado el archivo tailwind.config.js en la raíz del proyecto):

```bash
npx tailwindcss init -p
```

## Compilación de CSS

Para compilar los estilos de Tailwind CSS y generar el archivo final:

```bash
npx tailwindcss -i ./tailwind-input.css -o ./wwwroot/css/tailwind.css
```

Para la compilación en modo desarrollo (con observación de cambios):

```bash
npx tailwindcss -i ./tailwind-input.css -o ./wwwroot/css/tailwind.css --watch
```

Para la compilación en modo producción (minificado):

```bash
npx tailwindcss -i ./tailwind-input.css -o ./wwwroot/css/tailwind.css --minify
```

## Automatización con scripts de npm

Puedes añadir estos scripts a tu package.json:

```json
"scripts": {
  "tailwind:dev": "tailwindcss -i ./tailwind-input.css -o ./wwwroot/css/tailwind.css --watch",
  "tailwind:build": "tailwindcss -i ./tailwind-input.css -o ./wwwroot/css/tailwind.css --minify"
}
```

Luego, puedes ejecutarlos con:

```bash
# Para desarrollo
npm run tailwind:dev

# Para producción
npm run tailwind:build
```

## Integración con ASP.NET Core

El proyecto ya está configurado para cargar el archivo CSS generado desde:

```html
<link rel="stylesheet" href="~/css/tailwind.css" asp-append-version="true" />
```

Se han eliminado todas las referencias al CDN de Tailwind (cdn.tailwindcss.com) que no deben usarse en producción. 