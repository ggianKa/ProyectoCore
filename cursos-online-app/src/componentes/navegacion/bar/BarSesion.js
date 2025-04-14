// Importación de React y del hook useState
import React, { useState } from "react";

// Importación de componentes de Material UI
import {
  Button,
  IconButton,
  makeStyles,
  Toolbar,
  Typography,
  Avatar,
  Drawer,
  List,
  ListItem,
  ListItemText,
} from "@material-ui/core";

// Imagen temporal del usuario
import FotoUsuarioTemp from "../../../logo.svg";

// Hook personalizado para acceder al contexto global de la app
import { useStateValue } from "../../../contexto/store";
import { MenuIzquierda } from "./menuIzquierda";
import { MenuDerecha } from "./menuDerecha";
import { withRouter } from "react-router-dom";

// Definición de estilos personalizados con makeStyles
const useStyles = makeStyles((theme) => ({
  // Sección visible solo en pantallas grandes (escritorio)
  seccionDesktop: {
    display: "none",
    [theme.breakpoints.up("md")]: {
      display: "flex",
    },
  },
  // Sección visible solo en pantallas pequeñas (móvil)
  seccionMobile: {
    display: "flex",
    [theme.breakpoints.up("md")]: {
      display: "none",
    },
  },
  // Utilizado para empujar los elementos a la derecha (flex-grow)
  grow: {
    flexGrow: 1,
  },
  // Tamaño del avatar
  avatarSize: {
    width: 40,
    height: 40,
  },
  // Estilo del contenido dentro del Drawer
  list: {
    width: 250,
  },
  // Estilo de texto de cada ítem en el Drawer
  listItemText: {
    fontSize: "14px",
    fontWeight: 600,
    paddingLeft: "15px",
    color: "#212121",
  },
}));

// Componente principal que representa la barra de sesión del usuario
const BarSesion = (props) => {
  const classes = useStyles(); // Aplicar estilos
  const [{ sesionUsuario }, dispatch] = useStateValue(); // Obtener estado global del usuario

  // Estado para controlar si el Drawer está abierto
  const [abrirMenuIzquierda, setAbrirMenuIzquierda] = useState(false);

  // Estado para controlar si el Drawer está abierto
  const [abrirMenuDerecha, setAbrirMenuDerecha] = useState(false);

  // Función para cerrar el Drawer Izquierdo
  const cerrarMenuIzquierda = () => {
    setAbrirMenuIzquierda(false);
  };

  // Función para abrir el Drawer Izquierdo
  const abrirMenuIzquierdaAction = () => {
    setAbrirMenuIzquierda(true);
  };

  // Función para cerrar el Drawer Derecho
  const cerrarMenuDerecha = () => {
    setAbrirMenuDerecha(false);
  };

  // Función para abrir el Drawer Derecho
  const abrirMenuDerechaAction = () => {
    setAbrirMenuDerecha(true);
  }

  const salirSesionApp = () => {
    localStorage.removeItem('token_seguridad');
    props.history.push('/auth/login');
  }

  

  return (
    <React.Fragment>
      {/* Drawer (menú lateral izquierdo) */}
      <Drawer 
        open={abrirMenuIzquierda}
        onClose={cerrarMenuIzquierda}
        anchor="left"
      >
        {/* Contenido del Drawer */}
        <div
          className={classes.list}
          onKeyDown={cerrarMenuIzquierda}
          onClick={cerrarMenuIzquierda}
        >
          <MenuIzquierda classes={classes}/>
        </div>
      </Drawer>

      {/* Drawer (menú lateral derecho) */}
      <Drawer 
        open={abrirMenuDerecha}
        onClose={cerrarMenuDerecha}
        anchor="right"
      >
        {/* Contenido del Drawer */}
        <div
          role="button"
          onClick={cerrarMenuDerecha}
          onKeyDown={cerrarMenuDerecha}
        >
          <MenuDerecha 
          classes={classes} 
          salirSesion={salirSesionApp}
          usuario = { sesionUsuario ?  sesionUsuario.usuario : null}
          />
        </div>
      </Drawer>

      {/* Barra superior (Toolbar) */}
      <Toolbar>
        {/* Botón de menú para abrir el Drawer */}
        <IconButton color="inherit" onClick={abrirMenuIzquierdaAction}>
          <i className="material-icons">menu</i>
        </IconButton>

        {/* Título de la aplicación */}
        <Typography variant="h6">Cursos Online</Typography>

        {/* Espacio flexible para empujar elementos a la derecha */}
        <div className={classes.grow}></div>

        {/* Sección visible solo en escritorio */}
        <div className={classes.seccionDesktop}>
          <Button color="inherit">Salir</Button>
          <Button color="inherit">
            {/* Mostrar nombre del usuario si está logueado */}
            {sesionUsuario ? sesionUsuario.usuario.nombreCompleto : ""}
          </Button>
          {/* Avatar del usuario (imagen temporal) */}
          <Avatar src={FotoUsuarioTemp}></Avatar>
        </div>

        {/* Sección visible solo en móvil */}
        <div className={classes.seccionMobile}>
          <IconButton color="inherit" onClick={abrirMenuDerechaAction}>
            <i className="material-icons">more_vert</i>
          </IconButton>
        </div>
      </Toolbar>
    </React.Fragment>
  );
};

export default withRouter(BarSesion);
