import React, { useState } from "react";
import axios from "axios";
import {
  ChakraProvider,
  Box,
  Heading,
  Input,
  Button,
  VStack,
  Text,
  useToast,
} from "@chakra-ui/react";

const App = () => {
  const [apiKey, setApiKey] = useState(""); // Armazena a chave de API
  const [videos, setVideos] = useState([]); // Armazena os vídeos retornados
  const [error, setError] = useState(null); // Armazena erros, se existirem
  const toast = useToast(); // Exibe mensagens toast

  // Função para autenticar (configurar o cabeçalho global)
  const authenticate = () => {
    if (apiKey.trim() === "") {
      toast({
        title: "Erro",
        description: "Por favor, insira uma chave de API válida.",
        status: "error",
        duration: 3000,
        isClosable: true,
      });
      return;
    }

    axios.defaults.headers.common["Authorization"] = apiKey.trim();
    toast({
      title: "Autenticado",
      description: "Chave de API configurada com sucesso!",
      status: "success",
      duration: 3000,
      isClosable: true,
    });
  };

  // Função para buscar vídeos
  const fetchVideos = async () => {
    console.log(axios.defaults.headers.common["Authorization"]);
    try {
      setError(null);
      const response = await axios.get("https://localhost:7001/api/YTBSearch/ListVideos");
      setVideos(response.data);
    } catch (err) {
      setError(err.response?.data || "Erro ao buscar vídeos");
      toast({
        title: "Erro",
        description: err.response?.data || "Erro ao buscar vídeos.",
        status: "error",
        duration: 3000,
        isClosable: true,
      });
    }
  };

  return (
    <ChakraProvider>
      <Box p={5}>
        <Heading mb={4} textAlign="center">
          Busca de Vídeos do YouTube
        </Heading>
        <VStack spacing={4} align="stretch">
          {/* Campo para inserir a chave de API */}
          <Input
            placeholder="Insira sua chave de API do YouTube"
            value={apiKey}
            onChange={(e) => setApiKey(e.target.value)}
          />
          {/* Botão para autenticar */}
          <Button colorScheme="green" onClick={authenticate}>
            Autenticar
          </Button>
          {/* Botão para buscar vídeos */}
          <Button colorScheme="blue" onClick={fetchVideos} isDisabled={!apiKey.trim()}>
            Listar Vídeos
          </Button>
        </VStack>

        {/* Exibição de erros */}
        {error && <Text color="red.500" mt={4}>{error}</Text>}

        {/* Lista de vídeos */}
        <Box mt={6}>
          {videos.map((video) => (
            <Box key={video.id} p={4} borderWidth={1} borderRadius="md" mb={4}>
              <Heading size="md">{video.title}</Heading>
              <Text>{video.description}</Text>
            </Box>
          ))}
        </Box>
      </Box>
    </ChakraProvider>
  );
};

export default App;
