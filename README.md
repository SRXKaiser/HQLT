# Sistema de Facturación HQLT

**Versión:** 1.0  
**Tecnologías:** C#, Windows Forms, MySQL, .NET Framework 4.7.2+, iTextSharp, MySql.Data

## Descripción

HQLT es un sistema de facturación de escritorio desarrollado en C# con Windows Forms, diseñado para facilitar la gestión administrativa y comercial de pequeñas y medianas empresas. Permite registrar clientes, productos, usuarios, generar facturas electrónicas, emitir reportes en PDF y ejecutar consultas SQL avanzadas con control de seguridad.

Su arquitectura modular, interfaz amigable y compatibilidad con bases de datos locales como MySQL lo convierten en una solución robusta y accesible para entornos empresariales sin necesidad de conexión permanente a internet.

---

## Características Principales

- Inicio de sesión con control de roles (Administrador, Usuario, Invitado)
- Gestión completa de clientes y productos
- Emisión de facturas con cálculo de impuestos
- Generación de reportes por periodo, cliente, producto o ventas
- Exportación en formato PDF (iTextSharp)
- Módulo de consultas SQL con historial y seguridad
- Configuración editable de la empresa emisora

---

## Requisitos del Sistema

- Sistema Operativo: Windows 10 o superior (64 bits)
- .NET Framework 4.7.2 o superior
- Resolución mínima de pantalla: 1366x768 px
- MySQL Server instalado y configurado (o SQLite, según versión)
- Permisos de lectura/escritura en la carpeta del sistema

---

## Instalación

### Opción 1: Ejecutable directo

1. Descargar el archivo `HQLT.zip` desde la sección [Releases](https://github.com/SRX/HQLT/releases).
2. Extraer el contenido en una carpeta local.
3. Ejecutar el archivo `HQLT.exe`.
4. Registrar un nuevo usuario administrador al iniciar por primera vez.

### Opción 2: Desde código fuente

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/SRXKaiser/HQLT
