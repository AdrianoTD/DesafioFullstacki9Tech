import React from "react";
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


const Favorites = ({ videos, favorites, toggleFavorite, setSelectedVideoId, setIsModalOpen, selectedVideoId, setCurrentView, openDeleteModal, isModalOpen, isDeleteModalOpen, cancelDelete, confirmDelete }) => {
    const favoriteVideos = videos.filter((video) => favorites[video.id]);
    debugger;

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
    
  
    return (
    <Box maxW="100%" width="100%" mx="auto">
      <Box>
        {videos.length === 0 ? (
          <Box textAlign="center" color="gray.300">
            Nenhum vídeo favoritado!
          </Box>
        ) : (
            <Box mt={2} overflow="hidden">
                {videos.length > 1 ? (
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
                            onClick={() => window.open(`https://www.youtube.com/watch?v=${video.videoId}`, "_blank")} // Lógica para abrir
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
                            p={2}
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
                                <Box mb={2}>
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
                                bg="gray.200" // Fundo semitransparente
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
                <Modal isOpen={isModalOpen} onClose={() => () => {setSelectedVideoId(null); setIsModalOpen(false)}} size="4xl" isCentered>
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
        )}
      </Box>
      </Box>
    );
  };
  
  export default Favorites;