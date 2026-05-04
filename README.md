# Sistema de Gestión de Inventario y Usuarios 

![Responsive Design](https://img.shields.io/badge/Responsive-Design-2ea44f?style=for-the-badge&logo=airplay)

> **Aplicación Web con ASP.NET Core Razor Pages**
Esta plataforma es una solución integral para el control de inventarios y la administración avanzada de usuarios. Utiliza el modelo de desarrollo basado en páginas de **ASP.NET Core (Razor Pages)**, lo que permite una arquitectura robusta, segura y escalable.

## 📋 Funcionalidades Principales

### 🔐 Seguridad y Gestión de Acceso
* **Autenticación y Sesiones:** Sistema de login seguro para el control de acceso.
* **Control de Acceso basado en Roles:**
    * **Administrador:** Acceso total. Gestión de cuentas (borrado, cambio de contraseñas, activación/desactivación de usuarios).
    * **Usuario:** Permisos para ver todos los productos, pero con restricción de edición y borrado solo a sus propios registros.
* **Recuperación de Contraseña:** Flujo automatizado mediante envío de correo electrónico con enlace único para restablecimiento de credenciales en la base de datos.

### 📦 Gestión de Inventario Avanzada
* **Mantenimiento (CRUD):** Operaciones completas de Crear, Editar, Detalle y Borrar productos.
* **Identificador Único Personalizado:** Implementación de un **Código de Producto** manual y único con validación de duplicados.
* **Paginación y Filtrado:** Sistema de búsqueda optimizado directamente en la base de datos que permite filtrar por cualquier campo y navegación por páginas.

### 📤 Integración de Datos
* **Importación Masiva vía CSV:** Módulo para cargar productos mediante archivos `.csv`. 
    * **Lógica de Sincronización:** Si el producto ya existe (basado en el código único), se actualiza automáticamente.
    * **Control de Errores:** Reporte detallado post-importación que indica: productos añadidos, modificados, errores específicos y códigos duplicados.

## 💻 Tecnologías Utilizadas
* **Backend:** C# con .NET Core (ASP.NET Core Razor Pages).
* **Frontend:** HTML, CSS, JavaScript y sintaxis Razor para vistas dinámicas.
* **Base de Datos:** SQL Server con filtrado y persistencia optimizada.
* **Servicios:** SMTP para el envío de correos automáticos de recuperación.

## 📁 Estructura del Proyecto
* **Pages:** Interfaces `.cshtml` y lógica de servidor `.cshtml.cs`.
* **Models:** Clases que definen las entidades de negocio (Producto con atributo de código único, Usuario, Roles).
* **Data:** Capa de persistencia y configuración del contexto de la base de datos.

## 📸 Vistas de la Aplicación

> [!TIP]
> Cada sección de la plataforma ha sido diseñada para ser 100% funcional y responsive. Las siguientes capturas muestran los puntos clave, pero el sistema integra una experiencia de navegación mucho más completa.

| **Inicio de Sesión / Login** | **Recuperación de Contraseña** |
|:---:|:---:|
| <img alt="imgLogin2" src="https://github.com/user-attachments/assets/40c4c152-e240-4fef-81b6-ed92c5d24786" /> | <img alt="imgContraseña2" src="https://github.com/user-attachments/assets/9411d057-eb25-488c-a1c2-ebd49831e558" /> |

| **Listado y Gestión de Productos** | **Gestión de Usuarios (Admin)** |
|:---:|:---:|
| <img width="1903" height="1068" alt="imgInicio" src="https://github.com/user-attachments/assets/b23d209e-eb47-4146-a7f0-1697108622fc" /> | <img alt="imgControlUsuarios" src="https://github.com/user-attachments/assets/e725bb84-82b4-4837-a411-3ba2abfe3f53" /> |

| **Historial de Actividad (Admin)** | **Importación de Productos** |
|:---:|:---:|
| <img alt="imgHistorial" src="https://github.com/user-attachments/assets/58fd367c-9944-4ea1-a55f-484940132eb0" /> | <img alt="imgImportar" src="https://github.com/user-attachments/assets/27b5dd6a-dd92-4b6c-a473-b9c52c3266b6" /> |

| **Formulario de Alta de Producto** | |
|:---:|:---:|
| <img alt="imgAñadirProducto" src="https://github.com/user-attachments/assets/e78674f3-6365-4369-9bcf-ab43bb92aa17" /> | |
