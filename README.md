# Sistema de Gestión de Inventario y Usuarios 

> **Aplicación Web con ASP.NET Core Razor Pages**

Esta plataforma es una solución integral para el control de inventarios y la administración avanzada de usuarios. Utiliza el modelo de desarrollo basado en páginas de **ASP.NET Core (Razor Pages)**, lo que permite una arquitectura robusta, segura y escalable.

## Funcionalidades Principales

### Seguridad y Gestión de Acceso
* **Autenticación y Sesiones:** Sistema de login seguro para el control de acceso.
* **Control de Acceso basado en Roles (RBAC):**
    * **Administrador:** Acceso total. Gestión de cuentas (borrado, cambio de contraseñas, activación/desactivación de usuarios).
    * **Usuario:** Permisos para ver todos los productos, pero con restricción de edición y borrado solo a sus propios registros.
* **Recuperación de Contraseña:** Flujo automatizado mediante envío de correo electrónico con enlace único para restablecimiento de credenciales en la base de datos.

### Gestión de Inventario Avanzada
* **Mantenimiento (CRUD):** Operaciones completas de Crear, Editar, Detalle y Borrar productos.
* **Identificador Único Personalizado:** Implementación de un **Código de Producto** manual y único (sustituyendo visualmente al ID numérico) con validación de duplicados.
* **Paginación y Filtrado:** Sistema de búsqueda optimizado directamente en la base de datos que permite filtrar por cualquier campo y navegación por páginas.

### Integración de Datos
* **Importación Masiva vía CSV:** Módulo para cargar productos mediante archivos `.csv`. 
    * **Lógica de Sincronización:** Si el producto ya existe (basado en el código único), se actualiza automáticamente (Upsert).
    * **Control de Errores:** Reporte detallado post-importación que indica: productos añadidos, modificados, errores específicos y códigos duplicados.

## Tecnologías Utilizadas
* **Backend:** C# con .NET Core (ASP.NET Core Razor Pages).
* **Frontend:** HTML5, CSS3, JavaScript y sintaxis Razor para vistas dinámicas.
* **Base de Datos:** SQL Server con filtrado y persistencia optimizada.
* **Servicios:** SMTP para el envío de correos automáticos de recuperación.

## Estructura del Proyecto
* **Pages:** Interfaces `.cshtml` y lógica de servidor `.cshtml.cs`.
* **Models:** Clases que definen las entidades de negocio (Producto con atributo de código único, Usuario, Roles).
* **Data:** Capa de persistencia y configuración del contexto de la base de datos.
