// App.js

import React, { useState, useEffect } from "react";
import axios from "axios";
import {
  ChakraProvider,
  Box,
  Button,
  Input,
  Heading,
  Stack,
  Text,
  Simplerid,
  useToast,
} from "@chakra-ui/react";

const API_BASE_URL = "http://localhost:5000/api/YTBSearchController"; // URL do seu backend

function App() {
  const [videos, setVideos] = useState([]); // Armazena os vídeos listados
  const [apiKey, setApiKey] = useState(""); // Armazena a chave de API
  const [filter, setFilter] = useState(""); // Filtro para pesquisa
  const [search, setSearch] = useState(""); // Texto de busca
  const toast = useToast();

  // Função para listar todos os vídeos
  const fetchVideos = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/ListVideos`, {
        headers: { Authorization: apiKey },
      });
      setVideos(response.data);
    } catch (error) {
      handleError(error);
    }
  };

  // Função para filtrar vídeos
  const fetchFilteredVideos = async () => {
    try {
      const response = await axios.get(
        `${API_BASE_URL}/GetVideosFilteredFromDatabase/${filter}/${search}`,
        { headers: { Authorization: apiKey } }
      );
      setVideos(JSON.parse(response.data));
    } catch (error) {
      handleError(error);
    }
  };

  // Função para lidar com erros
  const handleError = (error) => {
    const message = error.response?.data || "Erro ao se comunicar com o servidor!";
    toast({
      title: "Erro",
      description: message,
      status: "error",
      duration: 4000,
      isClosable: true,
    });
  };

  return (
    <ChakraProvider>
      <Box p={5} maxWidth="800px" mx="auto">
        <Heading mb={4} textAlign="center">Gestão de Vídeos do YouTube</Heading>

        {/* Input para a chave da API */}
        <Stack spacing={3} mb={5}>
          <Input
            placeholder="Insira sua chave de API do YouTube"
            value={apiKey}
            onChange={(e) => setApiKey(e.target.value)}
          />

          <Button colorScheme="blue" onClick={fetchVideos} isDisabled={!apiKey}>
            Listar Todos os Vídeos
          </Button>
        </Stack>

        {/* Filtros de busca */}
        <Stack spacing={3} mb={5} direction="row">
          <Input
            placeholder="Filtro (Ex: Título, Categoria)"
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
          />
          <Input
            placeholder="Termo de busca"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
          <Button colorScheme="green" onClick={fetchFilteredVideos} isDisabled={!apiKey}>
            Buscar
          </Button>
        </Stack>

        {/* Listagem dos vídeos */}
        <SimpleGrid columns={[1, 2]} spacing={5}>
          {videos.map((video) => (
            <Box key={video.id} p={4} borderWidth="1px" borderRadius="md" shadow="sm">
              <Text fontWeight="bold">{video.title}</Text>
              <Text>{video.description}</Text>
            </Box>
          ))}
        </SimpleGrid>

        {videos.length === 0 && <Text textAlign="center">Nenhum vídeo encontrado.</Text>}
      </Box>
    </ChakraProvider>
  );
}

export default App;