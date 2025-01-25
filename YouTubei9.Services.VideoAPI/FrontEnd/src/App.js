import React, { useState } from "react";
import { useEffect } from "react";
import axios from "axios";
import {
  ChakraProvider,
  Box,
  Heading,
  Input,
  Icon,
  Button,
  VStack,
  Text,
  useToast,
} from "@chakra-ui/react";
import { FaStar, FaRegStar, FaSave, FaTrash } from "react-icons/fa";
import { FiExternalLink } from "react-icons/fi";
import Slider from "react-slick";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

const App = () => {
  const [apiKey, setApiKey] = useState("");
  const [videos, setVideos] = useState([]);
  const [error, setError] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [favorites, setFavorites] = useState({});

  useEffect(() => {
    const fetchVideosOnLoad = async() => {
      const savedApiKey = localStorage.getItem("apiKey");
      const isAuthorized = localStorage.getItem("isAuthorized");

      if (savedApiKey && isAuthorized) {
        setApiKey(savedApiKey);
        axios.defaults.headers.common["Authorization"] = savedApiKey;
        axios.defaults.headers.common["IsAuthorized"] = isAuthorized;

        try {
          await fetchVideos();
        } catch (error) {
          console.error("Erro ao listar vídeos no carregamento:", error);
        }
      }
    };

    fetchVideosOnLoad();
  }, []);

  const toast = useToast();

  const carouselSettings = {
    dots: true,
    infinite: true,
    speed: 500,
    slidesToShow: 3,
    slidesToScroll: 1,
    draggable: true,
    swipeToSlide: true,
    centerMode: false,
    responsive: [
      {
        breakpoint: 1024,
        settings: {
          slidesToShow: 2,
          slidesToScroll: 1,
        },
      },
      {
        breakpoint: 768,
        settings: {
          slidesToShow: 1,
          slidesToScroll: 1,
        },
      },
      {
        breakpoint: 480, // Smartphones pequenos
        settings: {
          slidesToShow: 1, // Apenas 1 slide visível
          slidesToScroll: 1,
          centerMode: true, // Centraliza os slides
        },
      },
    ],
  };

  const authenticate = async () => {
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

    setIsLoading(true);
    try {
      if (apiKey.trim() === "") {
        setError("Por favor, insira uma chave de API válida.");
        return;
      }

      const response = await axios.post(
        "https://localhost:7001/api/YTBSearch/ValidateApiKey",
        {},
        {
          headers: {
            Authorization: apiKey.trim(),
          },
        }
      );
  
      if (response.data === true) {
        setError(null);
        toast({
          title: "Autenticado",
          description: "Chave de API válida. Autenticado com sucesso!",
          status: "success",
          duration: 3000,
          isClosable: true,
        });
        setIsAuthenticated(true);
        localStorage.setItem("apiKey", apiKey.trim());
        localStorage.setItem("isAuthorized", true);
        axios.defaults.headers.common["IsAuthorized"] = true;

        await fetchVideos();

      } else {
        throw new Error("Chave de API inválida.");
      }
    }

    catch (err) {
      setError(err.message || "Erro ao validar a chave de API.");
      toast({
        title: "Erro na Autenticação",
        description: err.message || "Erro ao validar a chave de API.",
        status: "error",
        duration: 3000,
        isClosable: true,
      });
      setIsAuthenticated(false);
    }

    finally {
      setIsLoading(false);
    }
  };


  const fetchVideos = async () => {
    try {
      setError(null);
      const response = await axios.get("https://localhost:7001/api/YTBSearch/ListVideos",
        {
          headers: {
            Authorization: apiKey.trim(),
          },
          params: {
            search: searchTerm,
          },
        }
      );
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

  const toggleFavorite = (videoId) => {
    setFavorites((prev) => ({
      ...prev,
      [videoId]: !prev[videoId],
    }));
  };

  return (
    <ChakraProvider>
      <Box p={{ base: 4, md: 8 }} maxW="100%" mx="auto">
        <Heading mb={4} textAlign="center">
          Busca de Vídeos do YouTube
        </Heading>
        <VStack spacing={4} align="stretch" maxW="100%" mx="auto">
          {!isAuthenticated ? (
            <>
              <Input
                placeholder="Insira sua chave de API do YouTube"
                value={apiKey}
                onChange={(e) => setApiKey(e.target.value)}
                size="sm"
              />
              <Button colorScheme="green" 
                onClick={authenticate} 
                size="md" 
                minW="200px" 
                width={{ base: "100%", sm: "300px", md: "400px" }} 
                alignSelf="center" 
                isLoading={isLoading}>

                Autenticar

              </Button>
            </>
          ) : (
            <>
              <Input
                placeholder="Busque vídeos pelo título"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                size="sm"
              />
              <Button
                colorScheme="blue" 
                onClick={fetchVideos} 
                size="md" 
                minW="200px" 
                width={{ base: "100%", sm: "300px", md: "400px" }} 
                alignSelf="center"
                isLoading={isLoading}>
              
                Listar Vídeos
              </Button>
            </>
          )}
        </VStack>

        {error && <Text color="red.500" mt={4} textAlign="center">{error}</Text>}

        <Box mt={6} overflow="hidden">
          <Slider {...carouselSettings}>
            {videos.map((video) => {
              const defaultThumb = video.thumbnails?.find(
                (thumb) => thumb.thumbType === 2
              );

              return (
                <Box
                  key={video.id}
                  p={4}
                  borderWidth={1}
                  borderRadius="md"
                  textAlign="center"
                  bg="gray.100"
                  mx={{ base: "5px", md: "15px" }}
                  width={{ base: "90%", sm: "300px", md: "260px" }}
                  overflow="hidden"
                  position="relative"
                >
                  <Box
                    display="flex" // Habilita layout flex
                    flexDirection="column" // Organiza os itens em coluna
                    justifyContent="space-between" // Espaçamento automático entre os itens
                    h={{ base: "auto", md: "350px" }} // Altura ajustável para diferentes tamanhos de tela
                  >
                    {/* Exibe a Thumbnail no topo, caso exista */}
                    {defaultThumb?.url && (
                      <Box mb={4}>
                        <img
                          src={defaultThumb.url}
                          alt={`Thumbnail de ${video.videoTitle}`}
                          style={{
                            width: "100%",
                            height: "150px", // Define uma altura fixa
                            objectFit: "cover", // Garante que a imagem se ajuste sem distorção
                            borderRadius: "8px", // Arredonda as bordas da imagem
                          }}
                        />
                      </Box>
                    )}

                    <Box
                      position="absolute"
                      top="10px"
                      right="10px"
                      bg="rgba(255, 255, 255, 1)" // Fundo semitransparente
                      borderRadius="md"
                      p="5px"
                      cursor="pointer"
                      onClick={() => toggleFavorite(video.id)} // Alternar favorito
                      display="flex" // Centraliza o ícone dentro do quadrado
                      alignItems="center"
                      justifyContent="center"
                      boxShadow="md"
                      w="32px" // Largura do quadrado
                      h="32px" // Altura do quadrado
                    >
                      <Icon
                        as={favorites[video.id] ? FaStar : FaRegStar}
                        color={favorites[video.id] ? "yellow.400" : "gray.400"}
                        w={6}
                        h={6}
                      />
                    </Box>

                    {/* Exibe o Título do Vídeo */}
                    <Heading size="md" mb={2} noOfLines={1}>
                      {video.videoTitle}
                    </Heading>

                    {/* Exibe a Duração do Vídeo */}
                    <Text fontSize="sm" color="gray.600" mb={2} noOfLines={2}>
                      {video.duration}
                    </Text>
                  </Box>

                  <Box
                    position="absolute"
                    bottom="10px"
                    left="10px"
                    display="flex"
                    gap="10px"
                    flexWrap="wrap"
                  >
                    {/* Ícone de Salvar */}
                    <Box
                      bg="rgba(255, 255, 255, 0.8)"
                      borderRadius="md"
                      p="5px"
                      cursor="pointer"
                      boxShadow="md"
                      color="blue.500"
                      transition="all 0.2s ease"
                      _hover={{
                        bg: "blue.500",
                        color: "white",
                      }}
                      onClick={() => console.log(`Salvar vídeo ${video.id}`)} // Lógica para salvar
                    >
                      <Icon as={FaSave} w={5} h={5} />
                    </Box>

                    {/* Ícone de Abrir */}
                    <Box
                      bg="rgba(255, 255, 255, 0.8)"
                      borderRadius="md"
                      p="5px"
                      cursor="pointer"
                      boxShadow="md"
                      color="green.500"
                      transition="all 0.2s ease"
                      _hover={{
                        bg: "green.500",
                        color: "white",
                      }}
                      onClick={() => console.log(`Abrir vídeo ${video.id}`)} // Lógica para abrir
                    >
                      <Icon as={FiExternalLink} w={5} h={5} />
                    </Box>
                  </Box>

                  {/* Ícone de Lixeira */}
                  <Box
                    position="absolute"
                    bottom="10px"
                    right="10px"
                    bg="rgba(255, 255, 255, 0.8)"
                    borderRadius="md"
                    p="5px"
                    cursor="pointer"
                    boxShadow="md"
                    color="red.500"
                    transition="all 0.2s ease"
                    _hover={{
                      bg: "red.500",
                      color: "white",
                    }}
                    onClick={() => console.log(`Deletar vídeo ${video.id}`)}
                  >
                    <Icon as={FaTrash} w={5} h={5}/>
                  </Box>
                </Box>
              );
            })}
          </Slider>
        </Box>
      </Box>
    </ChakraProvider>
  );
};

export default App;
