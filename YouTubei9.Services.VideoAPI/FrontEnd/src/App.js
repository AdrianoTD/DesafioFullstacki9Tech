import React, { useState } from "react";
import { Global } from "@emotion/react";
import { useEffect } from "react";
import axios from "axios";
import {
  ChakraProvider,
  Box,
  Heading,
  Flex,
  Input,
  Select,
  Icon,
  Button,
  VStack,
  Text,
  Link,
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  useDisclosure,
  useToast,
} from "@chakra-ui/react";
import { FaStar, FaRegStar, FaTrash, FaPlay } from "react-icons/fa";
import { FiExternalLink } from "react-icons/fi";
import Slider from "react-slick";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

import Favorites from "./components/Favorites";

const App = () => {
  const [currentView, setCurrentView] = useState("home");
  const [apiKey, setApiKey] = useState("");
  const [videos, setVideos] = useState([]);
  const [error, setError] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [searchFilter, setSearchFilter] = useState("title");
  const [isLoading, setIsLoading] = useState(false);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [selectedVideoId, setSelectedVideoId] = useState(null);

  const [favorites, setFavorites] = useState(() => {
    const storedFavorites = localStorage.getItem("favorites");
    return storedFavorites ? JSON.parse(storedFavorites) : {};
  });
  
  const baseUrl = "https://localhost:7001/api/";

  const { isOpen, onOpen, onClose } = useDisclosure();
  const isHomeView = currentView === "home";

  useEffect(() => {
    localStorage.setItem("favorites", JSON.stringify(favorites));
  }, [favorites]);

  useEffect(() => {
    const fetchVideosOnLoad = async() => {
      const savedApiKey = localStorage.getItem("apiKey");
      const isAuthenticated = localStorage.getItem("isAuthenticated");
      const isAuthorized = localStorage.getItem("isAuthorized");

      if (savedApiKey && isAuthorized) {
        setApiKey(savedApiKey);
        axios.defaults.headers.common["Authorization"] = savedApiKey;
        setIsAuthenticated(isAuthenticated);
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

  const GlobalStyle = () => (
    <Global
      styles={`
        body {
          margin: 0;
          padding: 0;
          background-color: #121212;
        }
        * {
          box-sizing: border-box;
        }
        .slick-slide > div {
          padding: 0 0.1px;
        }

        .slick-list {
          margin: 0 -10px;
        }

        .slick-track {
          display: flex !important;
          gap: 10px;
        }
      `}
    />
  );

  const carouselSettings = {
    dots: false,
    infinite: true,
    speed: 500,
    slidesToShow: 3,
    slidesToScroll: 1,
    draggable: true,
    swipeToSlide: true,
    centerMode: false,
    centerPadding: "15px",
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
          centerMode: true,
        },
      },
      {
        breakpoint: 480,
        settings: {
          slidesToShow: 1,
          slidesToScroll: 1,
          centerMode: true,
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
        `${baseUrl}YTBSearch/ValidateApiKey`,
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
        localStorage.setItem("isAuthenticated", true);
        localStorage.setItem("isAuthorized", true);
        axios.defaults.headers.common["IsAuthorized"] = true;

        await fetchVideos();

      }
    }

    catch (err) {
      const errorMessage = err.response?.data || "Ocorreu um erro inesperado. Tente novamente mais tarde!";
      toast({
        title: "Erro na Autenticação",
        description: errorMessage || "Erro ao validar a chave de API.",
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

  const handleListVideos = async () => {
    if (searchTerm.trim() === "") {
      await fetchVideos();
    } else {
      await fetchVideosByFilter();
    }
  };

  const fetchVideos = async () => {
    try {
      setError(null);
      const response = await axios.get(`${baseUrl}YTBSearch/ListVideos`,
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

  const fetchVideosByFilter = async () => {
    try {
      setError(null);

      const filterMap = {
        title: "Title",
        duration: "Duration",
        author: "Author",
      };
  
      const mappedFilter = filterMap[searchFilter];

      const response = await axios.get(`${baseUrl}YTBSearch/ListVideosByFilter`, {
        headers: {
          Authorization: apiKey.trim(),
        },
        params: {
          filter: mappedFilter,
          search: searchTerm.trim(),
        },
      });
      setVideos(response.data);
      toast({
        title: "Busca realizada",
        description: "Os vídeos filtrados foram carregados com sucesso.",
        status: "success",
        duration: 3000,
        isClosable: true,
      });
    } catch (err) {
      setError(err.response?.data || "Erro ao buscar vídeos filtrados.");
      toast({
        title: "Erro na busca",
        description: err.response?.data || "Erro ao buscar vídeos filtrados.",
        status: "error",
        duration: 3000,
        isClosable: true,
      });
    }
  };

  const deleteVideo = async (videoId) => {
    try{
      setError(null);
      const response = await axios.get(`${baseUrl}YTBSearch/DeleteVideo`, {
        params: { id: videoId },
      });
      toast({
        title: "Vídeo Deletado",
        description: `O vídeo com ID ${videoId} foi deletado com sucesso.`,
        status: "success",
        duration: 3000,
        isClosable: true,
      });

      setVideos((prevVideos) => prevVideos.filter((video) => video.id !== videoId));

    } catch (err) {
      setError(err.response?.data || "Não foi possível excluir este vídeo")
      toast({
        title: "Erro",
        description: err.response?.data || "Erro ao excluir vídeo.",
        status: "error",
        duration: 3000,
        isClosable: true,
      });
    }
  };

  const openDeleteModal = (videoId) => {
    setSelectedVideoId(videoId);
    setIsDeleteModalOpen(true);
  };

  const confirmDelete = async () => {
    if (selectedVideoId) {
      await deleteVideo(selectedVideoId);
      setIsDeleteModalOpen(false);
      setSelectedVideoId(null);
    }
  };
  
  const cancelDelete = () => {
    setIsDeleteModalOpen(false);
    setSelectedVideoId(null);
  };

  const toggleFavorite = (videoId) => {
    setFavorites((prev) => ({
      ...prev,
      [videoId]: !prev[videoId],
    }));
  };


  return (
    <ChakraProvider>
      <GlobalStyle />
      <Box p={{ base: 4, md: 8 }} maxW="100%" width="100%" mx="auto">
        {isHomeView ? (
          <Heading mb={4} textAlign="center" color="whiteAlpha.900">
            Busca de Vídeos do{" "}
            <Text as="span" color="red.500">
              YouTube
            </Text>
          </Heading>
        ) : (
          <Heading mb={4} textAlign="center" color="whiteAlpha.900">
            Favoritos do{" "}
            <Text as="span" color="red.500">
              YouTube
            </Text>
          </Heading>
        )}
        <VStack spacing={4} align="stretch" maxW="100%" mx="auto">
          {!isAuthenticated ? (
            <>
              <Input
                placeholder="Insira sua chave de API do YouTube"
                color="gray.200"
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

              <Box textAlign="center" mt={2}>
                <Link
                  href="https://developers.google.com/youtube/v3/getting-started"
                  color="red.500"
                  _hover={{ textDecoration: "underline", color: "red.700" }}
                  isExternal
                >
                  Como conseguir uma chave de API para o YouTube?
                </Link>
              </Box>
            </>
          ) : (
            <>
              <Flex 
              direction={{ base: "column", md: "row" }}
              gap={4} 
              align="center"
              justify="center"
              width="100%">

                <Select
                  value={searchFilter}
                  onChange={(e) => setSearchFilter(e.target.value)}
                  size="sm"
                  maxW={{ base: "100%", md: "150px" }}
                  bg="gray.800"
                  color="gray.200"
                >
                  <option style={{ backgroundColor: "#2d2d2d", color: "white" }} value="title">Título</option>
                  <option style={{ backgroundColor: "#2d2d2d", color: "white" }} value="duration">Duração</option>
                  <option style={{ backgroundColor: "#2d2d2d", color: "white" }} value="author">Nome do Canal</option>
                </Select>
                <Input
                  placeholder="Busque vídeos pelo título, duração ou autor"
                  color="gray.200"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  size="sm"
                  flex="1"
                  width="100%"
                />
              </Flex>

              
                <Flex justifyContent="center" gap={4} mb={4}>
                  <Button
                    colorScheme="red" 
                    onClick={handleListVideos} 
                    size="md" 
                    minW="150px" 
                    width={{ base: "100%", sm: "300px", md: "400px" }} 
                    alignSelf="center"
                    isLoading={isLoading}>
                  
                    Listar Vídeos
                  </Button>
                  {isHomeView ? (
                    <Button 
                      colorScheme="blue" 
                      onClick={() => setCurrentView("Favorites")}
                      size="md" 
                      minW="150px" 
                      width={{ base: "100%", sm: "300px", md: "400px" }} 
                      alignSelf="center">
                        
                      Visualizar Favoritos
                    </Button>
                    ) : (
                      <Button 
                      colorScheme="blue" 
                      onClick={() => setCurrentView("home")}
                      size="md" 
                      minW="150px" 
                      width={{ base: "100%", sm: "300px", md: "400px" }} 
                      alignSelf="center">
                        
                      Voltar para Home
                    </Button>
                  )}
                </Flex>
              
            </>
          )}
        </VStack>

        {error && <Text color="red.500" mt={4} textAlign="center">{error}</Text>}

        <Box mt={6} overflow="hidden">
          {currentView === "home" ? (
            videos.length > 1 ? (
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
                    borderColor="gray.700"
                    textAlign="center"
                    bg= "#FFFFFF1A"
                    mx={{ base: "5px", md: "15px" }}
                    width={{ base: "90%", sm: "300px", md: "260px" }}
                    overflow="hidden"
                    position="relative"
                  >
                    <Box
                      display="flex"
                      flexDirection="column"
                      justifyContent="space-between"
                      h={{ base: "auto", md: "350px" }}
                    >
                      {/* Exibe a Thumbnail no topo, caso exista */}
                      {defaultThumb?.url && (
                        <Box mb={4}>
                          <img
                            src={defaultThumb.url}
                            alt={`Thumbnail de ${video.videoTitle}`}
                            style={{
                              width: "100%",
                              height: "250px",
                              objectFit: "cover",
                              borderRadius: "8px",
                            }}
                          />
                          {/* Botão de Play */}
                          <Box
                            position="absolute"
                            top="40%"
                            left="50%"
                            transform="translate(-50%, -50%)"
                            bg="whiteAlpha.800"
                            borderRadius="full"
                            w="80px"
                            h="80px"
                            display="flex"
                            alignItems="center"
                            justifyContent="center"
                            boxShadow="lg"
                            cursor="pointer"
                            _hover={{
                              bg: "whiteAlpha.900",
                              transform: "translate(-50%, -50%) scale(1.1)",
                              transition: "all 0.2s ease",
                            }}
                            onClick={() => {setSelectedVideoId(video.videoId); setIsModalOpen(true)} }
                          >
                            <Icon as={FaPlay} color="black" w={8} h={8} />
                          </Box>
                        </Box>
                      )}

                      <Box
                        position="absolute"
                        top="10px"
                        right="10px"
                        bg="gray.200"
                        borderRadius="md"
                        p="5px"
                        cursor="pointer"
                        onClick={() => toggleFavorite(video.id)}
                        display="flex"
                        alignItems="center"
                        justifyContent="center"
                        boxShadow="md"
                        w="32px"
                        h="32px"
                      >
                        <Icon
                          as={favorites[video.id] ? FaStar : FaRegStar}
                          color={favorites[video.id] ? "yellow.400" : "gray.600"}
                          w={6}
                          h={6}
                        />
                      </Box>

                      {/* Exibe o Título do Vídeo */}
                      <Heading size="md" color="gray.200" mb={2} noOfLines={1}>
                        {video.videoTitle}
                      </Heading>

                      <Text fontSize="xs" color="gray.400" mb={3} noOfLines={1}>
                        {video.channelTitle}
                      </Text>

                      {/* Exibe a Duração do Vídeo */}
                      <Text fontSize="sm" color="gray.200" mb={2} noOfLines={2}>
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
                      {/* Ícone de Abrir */}
                      <Box
                        bg="rgba(255, 255, 255, 0.8)"
                        borderRadius="md"
                        p="5px"
                        cursor="pointer"
                        boxShadow="md"
                        color="orange.400"
                        transition="all 0.2s ease"
                        _hover={{
                          bg: "green.500",
                          color: "white",
                        }}
                        onClick={() => window.open(`https://www.youtube.com/watch?v=${video.videoId}`, "_blank")}
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
                      onClick={() => openDeleteModal(video.id)}
                    >
                      <Icon as={FaTrash} w={5} h={5}/>
                    </Box>
                  </Box>
                );
              })}
            </Slider>
            ) : (
              videos.map((video) => {
                const defaultThumb = video.thumbnails?.find(
                  (thumb) => thumb.thumbType === 2
                );
          
                return (
                  <Box
                    key={video.id}
                    p={4}
                    borderWidth={1}
                    borderRadius="md"
                    borderColor="gray.700"
                    textAlign="center"
                    bg= "#FFFFFF1A"
                    mx="auto"
                    width={{ base: "90%", sm: "80%", md: "60%", lg: "50%" }}
                    maxW="600px"
                    overflow="hidden"
                    position="relative"
                  >
                    <Box
                      display="flex"
                      flexDirection="column"
                      justifyContent="space-between"
                      h={{ base: "auto", md: "400px" }}
                    >
                      {/* Exibe a Thumbnail no topo, caso exista */}
                      {defaultThumb?.url && (
                        <Box mb={4}>
                          <img
                            src={defaultThumb.url}
                            alt={`Thumbnail de ${video.videoTitle}`}
                            style={{
                              width: "100%",
                              height: "250px",
                              objectFit: "cover",
                              borderRadius: "8px",
                            }}
                          />
                          {/* Botão de Play */}
                          <Box
                            position="absolute"
                            top="35%"
                            left="50%"
                            transform="translate(-50%, -50%)"
                            bg="whiteAlpha.800"
                            borderRadius="full"
                            w="80px"
                            h="80px"
                            display="flex"
                            alignItems="center"
                            justifyContent="center"
                            boxShadow="lg"
                            cursor="pointer"
                            _hover={{
                              bg: "whiteAlpha.900",
                              transform: "translate(-50%, -50%) scale(1.1)",
                              transition: "all 0.2s ease",
                            }}
                            onClick={() => {setSelectedVideoId(video.videoId); setIsModalOpen(true)}}
                          >
                            <Icon as={FaPlay} color="black" w={8} h={8} />
                          </Box>
                        </Box>
                      )}

                      <Box
                        position="absolute"
                        top="10px"
                        right="10px"
                        bg="gray.200"
                        borderRadius="md"
                        p="5px"
                        cursor="pointer"
                        onClick={() => toggleFavorite(video.id)}
                        display="flex"
                        alignItems="center"
                        justifyContent="center"
                        boxShadow="md"
                        w="32px"
                        h="32px"
                      >
                        <Icon
                          as={favorites[video.id] ? FaStar : FaRegStar}
                          color={favorites[video.id] ? "yellow.400" : "gray.600"}
                          w={6}
                          h={6}
                        />
                      </Box>

                      {/* Exibe o Título do Vídeo */}
                      <Heading size="md" color="gray.200" mb={2} noOfLines={1}>
                        {video.videoTitle}
                      </Heading>

                      <Text fontSize="xs" color="gray.400" mb={3} noOfLines={1}>
                        {video.channelTitle}
                      </Text>

                      {/* Exibe a Duração do Vídeo */}
                      <Text fontSize="sm" color="gray.200" mb={2} noOfLines={2}>
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
                      
                      {/* Ícone de Abrir */}
                      <Box
                        bg="rgba(255, 255, 255, 0.8)"
                        borderRadius="md"
                        p="5px"
                        cursor="pointer"
                        boxShadow="md"
                        color="orange.400"
                        transition="all 0.2s ease"
                        _hover={{
                          bg: "green.500",
                          color: "white",
                        }}
                        onClick={() => window.open(`https://www.youtube.com/watch?v=${video.videoId}`, "_blank")}
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
                      onClick={() => openDeleteModal(video.id)}
                    >
                      <Icon as={FaTrash} w={5} h={5}/>
                    </Box>
                  </Box>
                );
              })
            )
          ) : (
            <Favorites
              videos={videos.filter((video) => favorites[video.id])}
              favorites={favorites}
              toggleFavorite={toggleFavorite}
              setSelectedVideoId={setSelectedVideoId}
              selectedVideoId={selectedVideoId}
              setIsModalOpen={setIsModalOpen}
              setCurrentView={setCurrentView}
              openDeleteModal={openDeleteModal}
              isModalOpen={isModalOpen}
              isDeleteModalOpen={isDeleteModalOpen}
              cancelDelete={cancelDelete}
              confirmDelete={confirmDelete}
            />
          )}
          <Modal isOpen={isDeleteModalOpen} onClose={cancelDelete} isCentered>
            <ModalOverlay />
            <ModalContent bg="gray.800" color="white">
              <ModalHeader>Confirmação de Exclusão</ModalHeader>
              <ModalCloseButton />
              <ModalBody>
                Tem certeza que deseja excluir este vídeo? Esta ação não poderá ser desfeita.
              </ModalBody>
              <ModalFooter>
                <Button
                  mr={2} 
                  bg="red.500" 
                  color="white" 
                  _hover={{
                    bg: "white", 
                    color: "red.500",
                  }}
                  onClick={confirmDelete}>
                  Sim
                </Button>
                <Button  
                  bg="white" 
                  color="black" 
                  _hover={{
                    bg: "black", 
                    color: "white",
                  }}
                  onClick={cancelDelete}>
                  Não
                </Button>
              </ModalFooter>
            </ModalContent>
          </Modal>
          <Modal isOpen={isModalOpen} onClose={() => {setSelectedVideoId(null); setIsModalOpen(false)}} size="4xl" isCentered>
            <ModalOverlay />
            <ModalContent>
              <ModalCloseButton />
              <ModalBody p={0}>
                <Box position="relative" pb="56.25%" h="0"> {/* Proporção 16:9 */}
                  <iframe
                    src={`https://www.youtube.com/embed/${selectedVideoId}`}
                    title="YouTube Video Player"
                    style={{
                      position: "absolute",
                      top: 0,
                      left: 0,
                      width: "100%",
                      height: "100%",
                      border: "none",
                    }}
                    allow="autoplay; encrypted-media"
                    allowFullScreen
                  ></iframe>
                </Box>
              </ModalBody>
            </ModalContent>
          </Modal>
        </Box>
      </Box>
    </ChakraProvider>
  );
};

export default App;
