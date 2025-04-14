// Importamos React para poder usar JSX
import React from "react";

// Importamos algunos componentes visuales de Material-UI
import { Divider, List, ListItem, ListItemText } from "@material-ui/core";

// Importamos el componente Link para navegación sin recargar la página
import { Link } from 'react-router-dom';

// Creamos un componente llamado MenuIzquierda, que recibe una propiedad llamada "classes" para aplicar estilos
export const MenuIzquierda = ({ classes }) => (
  
  // Contenedor principal del menú
  <div className={classes.list}>

    {/* Primer grupo de opciones del menú */}
    <List>
      {/* Opción que lleva al perfil del usuario */}
      <ListItem component={Link} button to="/auth/perfil">
        {/* Icono con Material Icons */}
        <i className="material-icons">account_box</i>
        {/* Texto del ítem del menú con estilos personalizados */}
        <ListItemText classes={{ primary: classes.ListItemText }} primary="Perfil" />
      </ListItem>
    </List>

    {/* Línea divisora entre secciones */}
    <Divider />

    {/* Segundo grupo de opciones del menú */}
    <List>
      {/* Opción para crear un nuevo curso */}
      <ListItem component={Link} button to="/curso/nuevo">
        <i className="material-icons">add_box</i>
        <ListItemText classes={{ primary: classes.listItemText }} primary="Nuevo Cursos" />
      </ListItem>

      {/* Opción para ver la lista de cursos */}
      <ListItem component={Link} button to="/curso/lista">
        <i className="material-icons">menu_book</i>
        <ListItemText classes={{ primary: classes.listItemText }} primary="Lista Cursos" />
      </ListItem>
    </List>

    <Divider />

    {/* Tercer grupo de opciones del menú */}
    <List>
      {/* Opción para crear un nuevo instructor */}
      <ListItem component={Link} button to="/instructor/nuevo">
        <i className="material-icons">person_add</i>
        <ListItemText classes={{ primary: classes.listItemText }} primary="Nuevo Instructor" />
      </ListItem>

      {/* Opción para ver la lista de instructores (NO navega, solo muestra el ítem por ahora) */}
      <ListItem>
        <i className="material-icons">people</i>
        <ListItemText classes={{ primary: classes.listItemText }} primary="Lista Instructor" />
      </ListItem>
    </List>
  </div>
);
