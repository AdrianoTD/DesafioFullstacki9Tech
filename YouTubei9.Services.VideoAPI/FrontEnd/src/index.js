import React from "react";
import ReactDOM from "react-dom";
import { ChakraProvider } from "@chakra-ui/react"; // Estilização com Chakra UI
import App from "./App"; // Componente principal
import "./index.css"; // Arquivo CSS opcional para estilização global

ReactDOM.render(
  <React.StrictMode>
    <ChakraProvider>
      <App />
    </ChakraProvider>
  </React.StrictMode>,
  document.getElementById("root") // Elemento <div id="root"> do index.html
);