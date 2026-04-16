# Sistema de Gestión de Inventario y Usuarios 

> **Aplicación Web con ASP.NET Core Razor Pages**

Esta plataforma es una solución integral para el control de inventarios y la administración de usuarios. Utiliza el modelo de desarrollo basado en páginas de **ASP.NET Core (Razor Pages)**, lo que permite una separación clara entre la lógica del servidor y una interfaz de usuario dinámica.

## Funcionalidades Clave

### Seguridad y Acceso
* **Sistema de Login:** Gestión de sesiones y autenticación segura de usuarios.
* **Control de Acceso basado en Roles (RBAC):** * **Administrador:** Acceso total al panel de gestión, control de cuentas y configuración.
    * **Usuario:** Permisos restringidos para la consulta y gestión operativa de productos.

### Gestión de Inventario (CRUD)
* **Mantenimiento de Productos:** Interfaz fluida para el alta, edición, baja y control de stock.
* **Administración de Usuarios:** Panel centralizado para gestionar los perfiles con acceso al sistema.
* **Frontend Dinámico:** Uso de scripts internos de JavaScript para mejorar la interactividad de los formularios Razor.

## Tecnologías Utilizadas
* **Backend:** C# con .NET Core.
* **Arquitectura:** ASP.NET Core Razor Pages.
* **Frontend:** HTML, CSS, JavaScript y sintaxis Razor.
* **Base de Datos:** SQL Server.

## Estructura del Proyecto
* **Pages:** Contiene los archivos `.cshtml` (interfaz visual) y sus archivos asociados `.cshtml.cs` (lógica y modelo de página).
* **Models:** Clases que definen la estructura de los datos (Productos, Usuarios, etc.).
* **Data:** Contexto de la base de datos y configuración de la persistencia de datos.
